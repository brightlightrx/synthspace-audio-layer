/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using UnityEditor;

namespace Songspace.AudioDataNodes {

	[CanEditMultipleObjects]
	[CustomEditor(typeof( LowPassNode ), true)]
	public class LowPassNodeEditor : AudioDataNodeEditor {
		public override void AutoSetup() {
			LowPassNode node = (LowPassNode) target;
			if( node.Ins.Count < 3 || node.Ins[0] == null ) {
				node.Ins.Clear();
				CreateIn( 0, "Audio In" );
				CreateMappedIn( 1, "CutOff", 0f, 1f, 0f, 1f, 0.5f );
				CreateMappedIn( 2, "Resonance", 0f, 1f, 0f, 4f, 0f );
			}

			if( node.Outs.Count < 1 || node.Outs[0] == null ) {
				node.Outs.Clear();
				CreateOut( 0, "Audio Out" );
			}
		}
	}
}