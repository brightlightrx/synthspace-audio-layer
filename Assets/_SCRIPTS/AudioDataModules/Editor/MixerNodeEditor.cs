/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Graphs.AnimationBlendTree;

namespace Songspace.AudioDataNodes {
    [CanEditMultipleObjects]
    [CustomEditor( typeof( MixerNode ), true )]
    public class MixerNodeEditor : AudioDataNodeEditor {
        public override void DrawContent() {
            MixerNode node = (MixerNode) target;
            base.DrawContent();
            if( GUILayout.Button( "Add Input Channel" ) ) {
                AddInputChannel();
            }
            if( GUILayout.Button( "Add Output Channel" ) ) {
                CreateOut( node.Outs.Count, "Out " + node.Outs.Count );
            }
        }

        public override void AutoSetup() {
            MixerNode node = (MixerNode) target;
            if( node.Ins.Count < 4 || node.Ins[0] == null ) {
                node.Ins.Clear();
                CreateMappedIn( 0, "Overall Volume", -1f, 1f, 0f, 2f, 1f );
                AddInputChannel();
            }

            if( node.Outs.Count < 2 || node.Outs[0] == null ) {
                node.Outs.Clear();
                CreateOut( 0, "Out L" );
                CreateOut( 1, "Out R" );
            }
        }

        private void AddInputChannel() {
            MixerNode node = (MixerNode) target;
            int channelId = (node.Ins.Count - 1) / 3;
            CreateIn( node.Ins.Count, "Audio In " + channelId );
            CreateMappedIn( node.Ins.Count, "Volume " + channelId, -1f, 1f, 0f, 1f, 1f );
            CreateMappedIn( node.Ins.Count, "Pan " + channelId, -1f, 1f, -1f, 1f, 0f );
        }

    }
}