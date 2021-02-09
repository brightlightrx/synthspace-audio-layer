/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using UnityEditor;

namespace Songspace.AudioDataNodes {

	[CanEditMultipleObjects]
	[CustomEditor(typeof( OscillatorNode ), true)]
	public class OscillatorNodeEditor : AudioDataNodeEditor {

		public override void AutoSetup() {
			OscillatorNode node = (OscillatorNode)target;
			if( node.Ins.Count < 1 || node.Ins[0] == null ) {
				CreateMappedIn( 0, "Frequency", -1f, 1f, 40f, 2000f, 440f );
			}
			if( node.Ins.Count < 2 || node.Ins[1] == null ) {
				CreateMappedIn( 1, "PulseWidth", -1f, 1f, 0f, 1f, 0.5f );
			}
			if( node.Ins.Count < 3 || node.Ins[2] == null ) {
				CreateMappedIn( 2, "Intensity", -1f, 1f, -1f, 1f, 1f );
			}
			if( node.Ins.Count < 4 || node.Ins[3] == null ) {
				CreateIn( 3, "Offset" );
			}
			if( node.Outs.Count < 1 || node.Outs[0] == null ) {
				CreateOut( 0, "Out" );
			}
		}
	}
}