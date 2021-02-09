/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */
using UnityEngine;

namespace Songspace.AudioDataNodes {

	public class MixerNode : AudioDataNode {

		private int m_NumIns;

		public AudioDataInlet GetOverallVolume() { return Ins[0]; }
		public AudioDataInlet GetIn( int id ) { return Ins[1 + id * 3]; }
		public AudioDataInlet GetVolume( int id ) { return Ins[1 + id * 3 + 1]; }
		public AudioDataInlet GetPan( int id ) { return Ins[1 + id * 3 + 2]; }

		[SerializeField] private bool NormalizeForNumberOfChannels = true;

		protected override void Start() {
			base.Start();
			m_NumIns = Mathf.RoundToInt( ( Ins.Count - 1 ) * 0.333333333333f );
		}


		private float GetAdjustedIn( int inId, int channel, int sampleId ) {
			if ( channel == 0 ) {
				return GetIn( inId ).GetSample( sampleId )
				       * GetVolume( inId ).GetSample( sampleId )
				       * Mathf.Lerp( 1f, 0f, Mathf.InverseLerp( 0f, 1f, GetPan( inId ).GetSample( sampleId ) ) );
			} else if ( channel == 1 ) {
				return GetIn( inId ).GetSample( sampleId )
				       * GetVolume( inId ).GetSample( sampleId )
					   * Mathf.Lerp( 0f, 1f, Mathf.InverseLerp( -1f, 0f, GetPan( inId ).GetSample( sampleId ) ) );
			}

			return GetIn( inId ).GetSample( sampleId ) * GetVolume( inId ).GetSample( sampleId );
			//TODO: rewrite so you can pan across any number of channels?
		}

		public override void ProcessData() {
			float f = NormalizeForNumberOfChannels ? 1f / (float)m_NumIns : 1f ;

			//Clear Output
			for ( int i = 0; i < Outs.Count; i++ ) {
				for ( int j = 0; j < BufferSize; j++ ) {
					Outs[i].Data[j] = 0f;
				}
			}
			
			//Mix Channels
			for( int s = 0; s < BufferSize; s++ ) { //go through all samples
				for( int c = 0; c < Outs.Count; c++ ) { //go through all output channels
					for( int i = 0; i < m_NumIns; i++ ) { //go through all input channels
						Outs[c].Data[s] += GetAdjustedIn( i, c, s ) * f;
					}

					Outs[c].Data[s] = Mathf.Clamp( Outs[c].Data[s] * Mathf.Clamp( GetOverallVolume().GetSample( s ), 0f, 2f), -1f, 1f );
				}
			}

			
			
			
		}
	}
}
