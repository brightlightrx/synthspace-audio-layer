/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using System;
using UnityEngine;

namespace Songspace.AudioDataNodes {
    public class OscillatorNode : AudioDataNode {
        public enum OscillatorType {
            Sin,
            Square,
            Triangle,
            Saw,
            Pulse,
            WhiteNoise,  // float between 1 und -1
            DigitalNoise, // 1 or 0
            Curve
        }

        [SerializeField] private OscillatorType m_OscType = OscillatorType.Sin;

        public AudioDataInlet FrequencyCVIn => Ins[0];
        public AudioDataInlet PulseWidthCVIn => Ins[1];
        public AudioDataInlet IntensityCVIn => Ins[2];

        private float m_EffectiveIntensity = 1f;
        
        public float GetIntensitySample( int id ) {
            if( Ins.Count >= 3 ) return IntensityCVIn.GetSample( id );
            return 1f;
        }
        
        public AudioDataInlet OffsetCVIn => Ins[3];

        public float GetOffsetSample( int id ) {
            if( Ins.Count >= 4 ) return OffsetCVIn.GetSample( id );
            return 0f;
        }

        public AudioData AudioOut => Outs[0];

        [SerializeField] private float m_CurrentFrequency = 440f;
        [SerializeField] private double m_Phase = 0d; //Test

        public AnimationCurve Curve;

        private System.Random m_Random;


        protected override void Start() {
            base.Start();
            m_Random = new System.Random();
            m_TimeLoopLengthInSeconds = 1f / 440f;
        }

        public void SetWaveform( int w ) { m_OscType = (OscillatorType) w; }

        public override void ProcessData() {

            double sampleTime = m_Time;

            for ( int i = 0; i < BufferSize; i++ ) {
                switch ( m_OscType ) {
                    case OscillatorType.Sin:
                        AudioOut.Data[i] = CreateSine( sampleTime, m_CurrentFrequency ) * m_EffectiveIntensity + GetOffsetSample( i );
                        //if ( Mathf.Abs( AudioOut.Data[i] ) < 0.01f ) m_EffectiveIntensity = GetIntensitySample( i );
                        break;
                    case OscillatorType.Square:
                        AudioOut.Data[i] = CreateSquareWithPW( sampleTime, m_TimeLoopLengthInSeconds,
                            Mathf.Clamp( PulseWidthCVIn.GetSample( i ), 0.001f, 0.999f ) ) * GetIntensitySample( i ) + GetOffsetSample( i );
                        break;
                    case OscillatorType.Triangle:
                        AudioOut.Data[i] = CreateTriangle( sampleTime, m_CurrentFrequency ) * m_EffectiveIntensity + GetOffsetSample( i );
                        //if ( Mathf.Abs( AudioOut.Data[i] ) < 0.01f ) m_EffectiveIntensity = GetIntensitySample( i );
                        break;
                    case OscillatorType.Saw:
                        AudioOut.Data[i] = CreateSaw( sampleTime, m_CurrentFrequency ) * m_EffectiveIntensity + GetOffsetSample( i );
                        //if ( Mathf.Abs( AudioOut.Data[i] ) < 0.01f ) m_EffectiveIntensity = GetIntensitySample( i );
                        break;
                    case OscillatorType.Pulse:
                        AudioOut.Data[i] = CreatePulse( sampleTime, m_CurrentFrequency ) * m_EffectiveIntensity + GetOffsetSample( i );
                        break;
                    case OscillatorType.WhiteNoise:
                        AudioOut.Data[i] = CreateWhiteNoise() * GetIntensitySample( i ) + GetOffsetSample( i );
                        break;
                    case OscillatorType.DigitalNoise:
                        AudioOut.Data[i] = CreateDigitalNoise() * GetIntensitySample( i ) + GetOffsetSample( i );
                        break;
                    case OscillatorType.Curve:
                        AudioOut.Data[i] = CreateCurve( sampleTime, m_TimeLoopLengthInSeconds ) * m_EffectiveIntensity + GetOffsetSample( i );
                        break;
                }

                sampleTime += AudioDataNode.SingleSampleDuration;

                float nextFrequencyValue = FrequencyCVIn.GetSample( i );
                if( Math.Abs( nextFrequencyValue - m_CurrentFrequency ) > 0.0001f || sampleTime > m_TimeLoopLengthInSeconds ) {
                    //unwind sampleTime
                    while( sampleTime > m_TimeLoopLengthInSeconds ) sampleTime -= m_TimeLoopLengthInSeconds;
                    //calculate how far along our wavelength we are
                    double t = InverseLerp( 0d, m_TimeLoopLengthInSeconds, sampleTime );
                    //clamp new frequency
                    m_CurrentFrequency = Mathf.Clamp( nextFrequencyValue, 0.000001f, 100000f );
                    m_EffectiveIntensity = GetIntensitySample( i );
                    //calculate new wavelength
                    m_TimeLoopLengthInSeconds = 1d / m_CurrentFrequency;
                    //put time where it needs to be
                    sampleTime = Lerp( 0d, m_TimeLoopLengthInSeconds, t );
                }
            }

            m_Time = sampleTime;
        }

        private double InverseLerp( double a, double b, double value ) {
            if ( a != b ) return Clamp01( ( value - a ) / ( b - a ) );
            return 0.0f;
        }

        private double Lerp( double a, double b, double t ) { return a + ( b - a ) * Clamp01( t ); }

        private double Clamp01( double value ) {
            if ( value < 0.0 ) return 0.0f;
            if ( value > 1.0 ) return 1f;
            return value;
        }

        private float CreateCurve( double time, double cycleLength ) {
            while ( time > cycleLength + m_Phase ) time -= cycleLength + m_Phase;
            return Curve.Evaluate( Mathf.InverseLerp( 0f, (float) cycleLength, (float) time + (float) m_Phase ) );
        }

        public float CreateSine( double time, float frequency ) {
            return (float) Math.Sin( 2d * Math.PI * time * (double) frequency + m_Phase );
        }

        public float CreateSquare( double time, float frequency ) {
            return (float) Math.Sign( Math.Sin( 2d * Math.PI * time * (double) frequency + m_Phase ) );
        }

        public float CreateSquareWithPW( double time, double cycleLength, float pw ) {
            while ( time > cycleLength + m_Phase ) time -= cycleLength + m_Phase;
            return ( InverseLerp( 0d, cycleLength, time + m_Phase ) > pw ? 1f : -1f );
        }

        public float CreateTriangle( double time, float frequency ) {
            float t = (float) ( time * (double) frequency + m_Phase );
            return 1f - 4f * Mathf.Abs( Mathf.Round( t - 0.25f ) - ( t - 0.25f ) );
        }

        public float CreateSaw( double time, float frequency ) {
            float t = (float) ( time * (double) frequency + m_Phase );
            return 2f * ( t - (float) Mathf.Floor( t + 0.5f ) );
        }

        public float CreatePulse( double time, float frequency ) {
            return ( Mathf.Abs( Mathf.Sin( 2f * Mathf.PI * (float) time * frequency + (float) m_Phase ) ) <
                     1.0f - 10E-3f )
                ? ( 0f )
                : ( 1f );
        }

        public float CreateWhiteNoise() {
            return (float) ( m_Random.NextDouble() * 2d - 1d );
        }

        public float CreateDigitalNoise() {
            return (float) ( m_Random.Next( 2 ) );
        }
    }
}