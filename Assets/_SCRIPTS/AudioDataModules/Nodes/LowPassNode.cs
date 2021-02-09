/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */
using UnityEngine;

namespace Songspace.AudioDataNodes {
	public class LowPassNode : AudioDataNode {
        private float m_In1 = 0f;
		private float m_In2 = 0f;
		private float m_In3 = 0f;
		private float m_In4 = 0f;

		private float m_Out1 = 0f;
		private float m_Out2 = 0f;
		private float m_Out3 = 0f;
		private float m_Out4 = 0f;

	    public AudioDataInlet AudioIn => Ins[0];
	    public AudioDataInlet CutOff => Ins[1]; 
		public AudioDataInlet Resonance => Ins[2];
		public AudioData AudioOut => Outs[0];

		private float Saturate( float x ) { return Mathf.Clamp( x, -1f, 1f ); }

		public override void ProcessData() {
			for ( int i = 0; i < BufferSize; i++ ) {
				AudioOut.Data[i] = Filter( AudioIn.GetSample( i ), CutOff.GetSample( i ), Resonance.GetSample( i ) );
			}
		}

		private float Filter( float input, float cutoff, float resonance ) {
			float f = cutoff * 1.16f;
			float fb = resonance * ( 1f - 0.15f * f * f );
			input -= m_Out4 * fb;
			input *= 0.35013f * ( f * f ) * ( f * f );
			m_Out1 = Saturate( input + 0.3f * m_In1 + ( 1f - f ) * m_Out1 );
			m_In1 = input;
			m_Out2 = Saturate( m_Out1 + 0.3f * m_In2 + ( 1f - f ) * m_Out2 );
			m_In2 = m_Out1;
			m_Out3 = Saturate( m_Out2 + 0.3f * m_In3 + ( 1f - f ) * m_Out3 );
			m_In3 = m_Out2;
			m_Out4 = Saturate( m_Out3 + 0.3f * m_In4 + ( 1f - f ) * m_Out4 );
			m_In4 = m_Out3;
			return m_Out4;
		}
    }
}
