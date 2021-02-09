/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

//#define DEBUGX

using System.Collections.Generic;
using UnityEngine;
#if DEBUGX
using System.Diagnostics;
#endif

namespace Songspace.AudioDataNodes {
    public class AudioOutputNode : AudioDataNode {

        #region static
        
        //Keep a list of all OUTs (for future use?)
        private static readonly List<AudioOutputNode> OutputNodes = new List<AudioOutputNode>();

        private static void Register( AudioOutputNode node ) {
            if( !OutputNodes.Contains( node ) ) {
                OutputNodes.Add( node );
            }
        }

        private static void Deregister( AudioOutputNode node ) {
            OutputNodes.Remove( node );
        }

        
        //In principle nodes only trigger when they're connected, but for some nodes it make sense to always trigger them.
        private static readonly List<AudioDataNode> AlwaysTriggerTheseNodes = new List<AudioDataNode>();

        public static void RegisterToAlwaysTrigger( AudioDataNode node ) {
            if( !AlwaysTriggerTheseNodes.Contains( node ) ) {
                AlwaysTriggerTheseNodes.Add( node );
            }
        }

        public static void DeregisterFromAlwaysTrigger( AudioDataNode node ) {
            AlwaysTriggerTheseNodes.Remove( node );
        }
        
        
        #endregion
        
        [SerializeField] private bool m_Muted;
        [SerializeField] private double m_CurrentDataTime;
        public double CurrentDataTime => m_CurrentDataTime;

        private int m_OutChannels;
        
        [SerializeField] private long m_ElapsedMilliseconds;
        [SerializeField] private long m_ElapsedTicks;

        private void OnEnable() {
            Register( this );
        }

        private void OnDisable() {
            Deregister( this );
        }

        protected override void Start() {
            base.Start();
            m_OutChannels = Outs.Count;
        }

        public bool Muted => m_Muted;

        public void SetMute( bool on ) { m_Muted = on; }

        protected virtual void OnAudioFilterRead( float[] data, int channels ) {

            #if DEBUGX
            var stopwatch = Stopwatch.StartNew();
            #endif            

            //ADVANCE TIME BY BUFFERSIZE
            AdvanceLocalTime( BufferSize ); //Advance time
            
            //MUTED?
            if( m_Muted ) return; //*****

            m_CurrentDataTime = GlobalDataTime;
                
            //PREPARE DATA - this causes all connected nodes to process their data
            PrepareData( m_CurrentDataTime, BufferSize );
            
            //Now everything should be in place. Go through the inputs and construct the final output data
            int inChannels = Ins.Count - 1;
            for ( int i = 0; i < BufferSize; i++ ) {
                for ( int c = 0; c < channels; c++ ) {
                    //Write it into the data array - this plays it!
                    data[i * channels + c] = Ins[Mathf.Clamp( c, 0, inChannels )].GetSample( i );
                    
                    //Also write it into OUTs (mostly for debugging, could delete this eventually...)
                    if( c == 0 && m_OutChannels >= 1) Outs[0].Data[i] = data[i * channels + c]; //output L channel
                    if( c == 1 && m_OutChannels >= 2) Outs[1].Data[i] = data[i * channels + c]; //output R channel
                }
            }

            //ONLY ON MAIN OUT: TRIGGER everything that's not connected, but subscribed to be triggered anyway
            if( OutputNodes.Count > 0 && OutputNodes[0] == this ) { //only on main out
                foreach( AudioDataNode node in AlwaysTriggerTheseNodes ) {
                    if(node.m_DataTime < m_CurrentDataTime) node.PrepareData( m_CurrentDataTime, BufferSize );
                }
            }
            
            #if DEBUGX
            stopwatch.Stop();
            m_ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            m_ElapsedTicks = stopwatch.ElapsedTicks;
            #endif

        }
        
        #if DEBUGX
        private void OnGUI() {
            GUILayout.Label( "ms: " + m_ElapsedMilliseconds.ToString() );
            GUILayout.Label( "ticks: " + m_ElapsedTicks.ToString() );
        }
        #endif
    }
}