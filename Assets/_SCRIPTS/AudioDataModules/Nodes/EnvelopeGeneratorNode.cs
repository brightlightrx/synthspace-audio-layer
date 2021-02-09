// Synthspace Envelope Generator (ADSR)
// based on NAudio Envelope Generator (https://github.com/naudio/NAudio),
// which in turn was based on work by Nigel Redmon, EarLevel Engineering: earlevel.com
// http://www.earlevel.com/main/2013/06/03/envelope-generators-adsr-code/

using System;
using UnityEngine;

namespace Songspace.AudioDataNodes {

	public class EnvelopeGeneratorNode : AudioDataNode {

		[SerializeField] private float m_TriggerGateValue = 0.95f;
		private AudioDataInlet m_GateCvIn; // => Ins[0];
		private bool m_Triggering = false;
		
		[SerializeField] private float m_AttackRate = 0.1f;
		[SerializeField] private float m_DecayRate = 0.3f;
		[SerializeField] private float m_SustainLevel = 0.8f;
		[SerializeField] private float m_ReleaseRate = 5f;

		private AudioDataInlet m_AttackRateCvIn; // => Ins[1];
		private AudioDataInlet m_DecayRateCvIn; // => Ins[2];
		private AudioDataInlet m_SustainLevelCvIn; // => Ins[3];
		private AudioDataInlet m_ReleaseRateCvIn; // => Ins[4];

		private EnvelopeState m_State;
		private float m_Output;

		private float m_CurrentAttackRate;
		private float m_CurrentDecayRate;
		private float m_CurrentReleaseRate;
		private float m_AttackCoef;
		private float m_DecayCoef;
		private float m_ReleaseCoef;
		private float m_CurrentSustainLevel;
		private float m_TargetRatioAttack;
		private float m_TargetRatioDecayRelease;
		private float m_AttackBase;
		private float m_DecayBase;
		private float m_ReleaseBase;

		public AudioData CVOut => Outs[0];

		protected override void Start() {
			base.Start();

			m_GateCvIn = Ins[0];
			
			m_AttackRateCvIn = Ins[1];
			m_DecayRateCvIn = Ins[2];
			m_SustainLevelCvIn = Ins[3];
			m_ReleaseRateCvIn = Ins[4];
			
			ResetState();
			SetTargetRatioAttack( 0.3f );
			SetTargetRatioDecayRelease( 0.0001f );

			AttackRate = m_AttackRate = m_AttackRateCvIn.GetSample( 0 ) * SampleRate;
			DecayRate = m_DecayRate = m_DecayRateCvIn.GetSample( 0 ) * SampleRate;
			ReleaseRate = m_ReleaseRate = m_ReleaseRateCvIn.GetSample( 0 ) * SampleRate;
			SustainLevel = m_SustainLevel = m_SustainLevelCvIn.GetSample( 0 );

		}

		public enum EnvelopeState {
			Idle = 0,
			Attack,
			Decay,
			Sustain,
			Release
		};

		/// <summary>
		/// Attack Rate (seconds * SamplesPerSecond)
		/// </summary>
		public float AttackRate {
			get => m_CurrentAttackRate;
			set {
				m_CurrentAttackRate = value;
				m_AttackCoef = CalculateCoefficient( value, m_TargetRatioAttack );
				m_AttackBase = ( 1.0f + m_TargetRatioAttack ) * ( 1.0f - m_AttackCoef );
			}
		}

		/// <summary>
		/// Decay Rate (seconds * SamplesPerSecond)
		/// </summary>
		public float DecayRate {
			get => m_CurrentDecayRate;
			set {
				m_CurrentDecayRate = value;
				m_DecayCoef = CalculateCoefficient( value, m_TargetRatioDecayRelease );
				m_DecayBase = ( m_CurrentSustainLevel - m_TargetRatioDecayRelease ) * ( 1.0f - m_DecayCoef );
			}
		}

		/// <summary>
		/// Release Rate (seconds * SamplesPerSecond)
		/// </summary>
		public float ReleaseRate {
			get => m_CurrentReleaseRate;
			set {
				m_CurrentReleaseRate = value;
				m_ReleaseCoef = CalculateCoefficient( value, m_TargetRatioDecayRelease );
				m_ReleaseBase = -m_TargetRatioDecayRelease * ( 1.0f - m_ReleaseCoef );
			}
		}

		private static float CalculateCoefficient( float rate, float targetRatio ) {
			return (float) Math.Exp( -Math.Log( ( 1.0f + targetRatio ) / targetRatio ) / rate );
		}

		/// <summary>
		/// Sustain Level (1 = 100%)
		/// </summary>
		public float SustainLevel {
			get => m_CurrentSustainLevel;
			set {
				m_CurrentSustainLevel = value;
				m_DecayBase = ( m_CurrentSustainLevel - m_TargetRatioDecayRelease ) * ( 1.0f - m_DecayCoef );
			}
		}

		/// <summary>
		/// Sets the attack curve
		/// </summary>
		void SetTargetRatioAttack( float targetRatio ) {
			if ( targetRatio < 0.000000001f )
				targetRatio = 0.000000001f;  // -180 dB
			m_TargetRatioAttack = targetRatio;
			m_AttackBase = ( 1.0f + m_TargetRatioAttack ) * ( 1.0f - m_AttackCoef );
		}

		/// <summary>
		/// Sets the decay release curve
		/// </summary>
		void SetTargetRatioDecayRelease( float targetRatio ) {
			if ( targetRatio < 0.000000001f )
				targetRatio = 0.000000001f;  // -180 dB
			m_TargetRatioDecayRelease = targetRatio;
			m_DecayBase = ( m_CurrentSustainLevel - m_TargetRatioDecayRelease ) * ( 1.0f - m_DecayCoef );
			m_ReleaseBase = -m_TargetRatioDecayRelease * ( 1.0f - m_ReleaseCoef );
		}


		public float Process() {
			switch ( m_State ) {
				case EnvelopeState.Idle:
					break;
				case EnvelopeState.Attack:
					m_Output = m_AttackBase + m_Output * m_AttackCoef;
					if ( m_Output >= 1.0f ) {
						m_Output = 1.0f;
						m_State = EnvelopeState.Decay;
					}
					break;

				case EnvelopeState.Decay:
					m_Output = m_DecayBase + m_Output * m_DecayCoef;
					if ( m_Output <= m_CurrentSustainLevel ) {
						m_Output = m_CurrentSustainLevel;
						m_State = EnvelopeState.Sustain;
					}
					break;

				case EnvelopeState.Sustain:
					break;
				
				case EnvelopeState.Release:
					m_Output = m_ReleaseBase + m_Output * m_ReleaseCoef;
					if ( m_Output <= 0.0 ) {
						m_Output = 0.0f;
						m_State = EnvelopeState.Idle;
					}

					break;
			}

			return m_Output;
		}

		/// <summary>
		/// Trigger the gate
		/// </summary>
		/// <param name="gate">If true, enter attack phase, if false enter release phase (unless already idle)</param>
		public void Gate( bool gate ) {
			if ( gate )
				m_State = EnvelopeState.Attack;
			else if ( m_State != EnvelopeState.Idle )
				m_State = EnvelopeState.Release;
		}

		/// <summary>
		/// Reset to idle state
		/// </summary>
		public void ResetState() {
			m_State = EnvelopeState.Idle;
			m_Output = 0.0f;
		}


		/// <summary>
		/// Process Data
		/// </summary>
		/// <returns></returns>
		public override void ProcessData() {

			for ( int i = 0; i < BufferSize; i++ ) {

				bool t = m_GateCvIn.GetSample( i ) >= m_TriggerGateValue;
				if ( t != m_Triggering ) {
					m_Triggering = t;
					Gate( m_Triggering );
				}

				float aR = m_AttackRateCvIn.GetSample( i );
				if ( Math.Abs( aR - m_AttackRate ) > 0.0001f ) { //changed
					m_AttackRate = aR;
					AttackRate = m_AttackRate * SampleRate;
				}

				float dR = m_DecayRateCvIn.GetSample( i );
				if ( Math.Abs( dR - m_DecayRate ) > 0.0001f ) { //changed
					m_DecayRate = dR;
					DecayRate = m_DecayRate * SampleRate;
				}

				float sL = m_SustainLevelCvIn.GetSample( i );
				if( Mathf.Abs( sL - m_SustainLevel ) > 0.0001f ) { //changed
					m_SustainLevel = sL;
					SustainLevel = m_SustainLevel;
				}

				float rR = m_ReleaseRateCvIn.GetSample( i );
				if ( Math.Abs( rR - m_ReleaseRate ) > 0.0001f ) { //changed
					m_ReleaseRate = rR;
					ReleaseRate = m_ReleaseRate * SampleRate;
				}

				CVOut.Data[i] = Mathf.Lerp( -1f, 1f, Process() );
			}

			base.ProcessData();
		}
	}
}
