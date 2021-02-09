/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Graphs;

namespace Songspace.AudioDataNodes {

	[CanEditMultipleObjects]
	[CustomEditor(typeof( AudioOutputNode ), true)]
	public class AudioOutputNodeEditor : AudioDataNodeEditor {
		public override void DrawContent() {
			AudioOutputNode node = (AudioOutputNode) target;
			base.DrawContent();
			if( GUILayout.Button( "Add In Channel" ) ) {
				CreateIn( node.Ins.Count, "Audio " + node.Ins.Count );
			}
		}

		public override void AutoSetup() {
			AudioOutputNode node = (AudioOutputNode) target;
			if( node.GetComponent<AudioSource>() == null ) {
				Undo.AddComponent<AudioSource>( node.gameObject );
			}
			if( node.Ins.Count < 2 || node.Ins[0] == null ) {
				node.Ins.Clear();
				CreateIn( 0, "Audio L" );
				CreateIn( 1, "Audio R" );
			}

			if( node.Outs.Count < 1 || node.Outs[0] == null ) {
				CreateOut( 0, "Out" );
			}
		}
	}
}