using System;
using Core;
using UnityEngine;
/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

namespace Songspace.AudioDataNodes {
    public class ClockNode : AudioDataNode {
        
        private AudioDataInlet BPM; // => Ins[0];
        private AudioDataInlet RestartIn; // => Ins[1]
        private SimpleData Restart; // => Switches[0];
        private SimpleData SignatureHigh; // => Switches[1];
        private SimpleData SignatureLow; // => Switches[2];
        public AudioData ClockData; // => Outs[0];
        private AudioData BeatId; // => Outs[1];

        [SerializeField] private double m_BPM;
        private double m_BeatsPerSecond = 1.666666666666667f;
        private double m_OneDividedBySixty = 0.0166666666666667;
        private double m_BeatStart = 0d;
        private double m_SecPerBeat = 1d;
        private double m_SamplesPerBeat = 1d;
        private double m_PrevBeat = 0d;
        private double m_NextBeat = 0d;

        public int SignatureHi => Mathf.RoundToInt( SignatureHigh.Value );
        public int SignatureLo => Mathf.RoundToInt( SignatureLow.Value );
        public int CurrentBeatId = 0;
        public int BeatType = 0;

        public double BeatStart => m_BeatStart; 
        public double SecPerBeat => m_SecPerBeat;
        public double NextBeat => m_NextBeat;
        public double Time => m_Time;


        private bool m_DidRestartViaIn;
        
        protected virtual void OnEnable() {
            m_OneDividedBySixty = 1d / 60d;
            m_Time = 0f;
        }

        protected override void Start() {
            base.Start();
            BPM = Ins[0];
            RestartIn = Ins[1];
            Restart = Switches[0];
            SignatureHigh = Switches[1];
            SignatureLow = Switches[2];
            ClockData = Outs[0];
            BeatId = Outs[1];
        }

        public void SetBpm( double bpm ) {
            m_BPM = bpm;
            
            m_BeatsPerSecond = m_BPM * m_OneDividedBySixty; // / 60d;
            m_SecPerBeat = 1d / m_BeatsPerSecond;
            
            // calculate backwards from fraction of current
            double pos = Mathd.InverseLerp( m_PrevBeat, m_NextBeat, m_Time );
            m_PrevBeat = m_BeatStart = m_Time - pos * m_SecPerBeat;
            m_NextBeat = m_PrevBeat + m_SecPerBeat;
            
            //Debug.LogFormat( "prev {0} next {1} current {2}", m_PrevBeat, m_NextBeat, m_Time  );
            
            //CurrentBeatId = 0;
        }


        public void CheckBPM( float bpm ) {
            if( Math.Abs( bpm - m_BPM ) > 0.01f ) {
                SetBpm( bpm );
            }
        }
        
        public override void ProcessData() {
            for( int i = 0; i < BufferSize; i++ ) {
                m_Time += SingleSampleDuration;

                float restartSample = RestartIn.GetSample( i );
                if( m_DidRestartViaIn && restartSample < 0.79f ) m_DidRestartViaIn = false;
                if( restartSample > 0.8f && !m_DidRestartViaIn ) {
                    ResetNow();
                    m_DidRestartViaIn = true;
                }
                CheckBPM( BPM.GetSample( i ));
                
                if( m_Time >= m_NextBeat ) {
                    m_PrevBeat = m_NextBeat;
                    m_NextBeat += m_SecPerBeat;
                    if( ++CurrentBeatId >= SignatureHi ) CurrentBeatId = 0;
                    BeatType = CurrentBeatId == 0 ? 2 : 1;
                }

                float clock = (float) Mathd.InverseLerp( m_PrevBeat, m_NextBeat, m_Time );
                ClockData.Data[i] = Mathf.Lerp( -1f, 1f, clock );
                BeatId.Data[i] = CurrentBeatId + clock;
            }
            
            //base.ProcessData();
        }

        public void ResetNow() { //RESET
            m_Time = 0f;
            CurrentBeatId = 0;
            m_BeatStart = 0f;
            m_PrevBeat = 0d;
            m_NextBeat = 0d;
            SetBpm( m_BPM );
        }
        

        private bool m_Restarting;
        private void Update() {
            if( !m_Restarting && Restart.Value > 0.9f ) {
                m_Restarting = true;
                ResetNow();
            } else if( m_Restarting ) {
                m_Restarting = false;
            }
        }
    }
}