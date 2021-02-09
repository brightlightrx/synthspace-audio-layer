/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */
using UnityEngine;

namespace Songspace.AudioDataNodes {
    //NEW SEQUENCER
    public class SequencerXNode : AudioDataNode {

        [SerializeField] private AudioDataInlet m_ClockCvIn; // => Ins[0];
        [SerializeField] private AudioDataInlet m_ResetCvIn; // => Ins[1];
        //Switches 0-7: Step Toggles
        //Switches 8-15: Freq CH-1
        //Switches 16-23: Freq CH-2
        [SerializeField] private SimpleData m_StepNumber; // => Switches[24];
        [SerializeField] private SimpleData m_GateTime1; // => Switches[25]; //-1 to 1
        [SerializeField] private SimpleData m_GateTime2; // => Switches[26]; //-1 to 1
        
        [SerializeField] private AudioData m_Freq1Out; // => Outs[0];
        [SerializeField] private AudioData m_Freq2Out; // => Outs[1];
        [SerializeField] private AudioData m_Gate1Out; // => Outs[2];
        [SerializeField] private AudioData m_Gate2Out; // => Outs[3];

        //RUNTIME
        private bool m_Initialized;
        private bool m_Gate;
        private float m_PrevVal;
        private float m_PrevResetVal;
        
        [SerializeField] private int m_CurrentStep;
        [SerializeField] private int m_ActiveStep; //the last step that was ON
        
        private float m_Chan1FreqValue; //Current frequency of the active step for channel 1
        private float m_Chan2FreqValue; //Current frequency of the active step for channel 2
        
        public int CurrentStep => m_CurrentStep;

        private bool m_ClockConnected;
        private float m_CurrentSwitchValue;

       
        protected void OnValidate() {
            //For performance reasons these are no longer properties
            m_ClockCvIn = Ins[0];
            m_ResetCvIn = Ins[1];

            m_StepNumber = Switches[24];
            m_GateTime1 = Switches[25]; //-1 to 1
            m_GateTime2 = Switches[26]; //-1 to 1
            
            m_Freq1Out = Outs[0];
            m_Freq2Out = Outs[1];
            m_Gate1Out = Outs[2];
            m_Gate2Out = Outs[3];
        }

        private void OnEnable() {
            AudioOutputNode.RegisterToAlwaysTrigger( this );
        }

        private void OnDisable() {
            AudioOutputNode.DeregisterFromAlwaysTrigger( this );
        }

        public override void ProcessData() {
            //Clock connected or disconnected?
            var clockConnected = m_ClockCvIn.IsConnected;
            if( clockConnected != m_ClockConnected ) {
                m_Initialized = false;
                m_CurrentStep = m_ActiveStep = 0;
                m_PrevVal = 0f;
                m_ClockConnected = clockConnected;
            }
            
            BeatX beatX = BeatX.I;
            var gateTime1Value = m_GateTime1.Value * 2f - 1f;
            var gateTime2Value = m_GateTime2.Value * 2f - 1f;

            if( !clockConnected && beatX == null ) {
                return; //NO CLOCK, do nothing
            }
            
            for( int i = 0; i < BufferSize; i++ ) {
                //Reset Signal
                float reset = m_ResetCvIn.GetSample( i );
                if( reset > 0.98f ) {
                    m_CurrentStep = -1;
                }
                
                //Get clock signal from connected CLOCK or BEATX
                float clock = m_ClockConnected ? m_ClockCvIn.GetSample( i ) : beatX.ClockData.GetSample( i );
                if( !m_Initialized ) {
                    m_PrevVal = clock;
                    m_Initialized = true;
                }
                if( clock < m_PrevVal ) { //past 1, new beat
                    m_CurrentStep++;
                    if( m_CurrentStep >= m_StepNumber.Value ) m_CurrentStep = 0;
                    else if( m_CurrentStep < 0 ) m_CurrentStep = Mathf.RoundToInt( m_StepNumber.Value - 1f );
                    m_PrevVal = clock;

                    m_CurrentSwitchValue = Switches[m_CurrentStep].Value;

                    if( m_CurrentSwitchValue > 0.5f ) {
                        m_ActiveStep = m_CurrentStep;
                        m_Chan1FreqValue = Switches[m_ActiveStep + 8].PureValue;
                        m_Chan2FreqValue = Switches[m_ActiveStep + 16].PureValue;
                    }
                }

                //FREQUENCY OUT - using ActiveStep, so frequency stays at the value of the last step that was on!
                m_Freq1Out.Data[i] = m_Chan1FreqValue; // Switches[m_ActiveStep + 8].Value;
                m_Freq2Out.Data[i] = m_Chan2FreqValue; // Switches[m_ActiveStep + 16].Value;
                
                //GATE OUT
                if( m_CurrentSwitchValue < 0.5f ) { //Step is off?
                    m_Gate1Out.Data[i] = -1f;
                    m_Gate2Out.Data[i] = -1f;
                }
                else { //Step is on
                    m_Gate1Out.Data[i] = clock < gateTime1Value ? 1f : -1f;
                    m_Gate2Out.Data[i] = clock < gateTime2Value ? 1f : -1f;
                }

                m_PrevVal = clock;
            }

        }
    }
}