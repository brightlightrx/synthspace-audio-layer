/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using UnityEngine;
using UnityEditor;
using Core;
using Songspace.Container;

namespace Songspace.AudioDataNodes {

	[CanEditMultipleObjects]
	[CustomEditor(typeof( AudioDataNode ), true)]
	public class AudioDataNodeEditor : Editor<AudioDataNode> {

		public override void OnInspectorGUI () {
			serializedObject.Update();
			DrawScriptField(); //Draw the script field
			DrawContent(); //Draw your custom content
			serializedObject.ApplyModifiedProperties();
		}

		public void DrawScriptField() {
			EditorGUI.BeginDisabledGroup(true);
			SerializedProperty prop = serializedObject.FindProperty("m_Script");
			EditorGUILayout.PropertyField( prop, true, new GUILayoutOption[0] );
			EditorGUI.EndDisabledGroup();
		}

		public virtual void DrawContent() {
			DrawPropertiesExcluding( serializedObject, "m_Script" );
			if( GUILayout.Button( "Auto-Setup" ) ) {
				AutoSetup();
			}
		}
		
		//**************************************************************************

		public virtual void AutoSetup() { 
			//by default, just fill in all the empty slots
			for ( int i = 0; i < Target.Ins.Count; i++ ) {
				if( Target.Ins[i] == null ) {
					CreateMappedIn( i, "In " + i, -1f, 1f, -1f, 1f, 0f );
				}
			}

			for ( int i = 0; i < Target.Slots.Count; i++ ) {
				if( Target.Slots[i] == null ) {
					//CreateSlot<SampleContainerSlot>( i, "Slot " + i );
				}
			}
			
			for ( int i = 0; i < Target.Switches.Count; i++ ) {
				if( Target.Switches[i] == null ) {
					CreateSimpleData( i, "Switch " + i );
				}
			}
			
			for ( int i = 0; i < Target.Outs.Count; i++ ) {
				if( Target.Outs[i] == null ) {
					CreateOut( i, "Out " + i );
				}
			}
		}

		public AudioDataInlet CreateIn(int id, string inName, bool createGameObject = true) {
			AudioDataNode node = (AudioDataNode)target;
			Undo.RecordObject( node, "Create In" );
			AudioDataInlet adi = null;
			if( createGameObject ) {
				GameObject go = new GameObject( "IN: " + inName );
				adi = Undo.AddComponent<AudioDataInlet>( go );
				go.transform.parent = node.transform;
				Undo.RegisterCreatedObjectUndo( go, "Create In" );
			} else {
				adi = Undo.AddComponent<AudioDataInlet>( node.gameObject );
			}
			adi.Node = node;
			adi.Description = "IN: " + inName;
			while( node.Ins.Count <= id ) {
				node.Ins.Add( null );
			}
			node.Ins[id] = adi;
			return adi;
		}
		
		public AudioDataInletMapped CreateMappedIn(int id, string inName, bool createGameObject = true) {
			AudioDataNode node = (AudioDataNode)target;
			Undo.RecordObject( node, "Create In" );
			AudioDataInletMapped adim = null;
			if( createGameObject ) {
				GameObject go = new GameObject( "IN: " + inName );
				adim = Undo.AddComponent<AudioDataInletMapped>( go );
				go.transform.parent = node.transform;
				Undo.RegisterCreatedObjectUndo( go, "Create In" );
			} else {
				adim = Undo.AddComponent<AudioDataInletMapped>( node.gameObject );
			}
			adim.Node = node;
			adim.Description = "IN: " + inName;
			while( node.Ins.Count <= id ) {
				node.Ins.Add( null );
			}
			node.Ins[id] = adim;
			EditorUtility.SetDirty( node );
			return adim;
		}

		public AudioDataInletMapped CreateMappedIn( int id, string inName, float fromMin, float fromMax, float toMin,
			float toMax, float mappedDefaultValue ) {
			AudioDataInletMapped adim = CreateMappedIn( id, inName );
			adim.SetMapping( fromMin, fromMax, toMin, toMax );
			adim.SetValueFromMapped( mappedDefaultValue );
			return adim;
		}

		public void CreateOut( int id, string inName, bool createGameObject = true ) {
			AudioDataNode node = (AudioDataNode) target;
			Undo.RecordObject( node, "Create Out" );
			AudioData ado = null;
			if( createGameObject ) {
				GameObject go = new GameObject( "OUT: " + inName );
				ado = Undo.AddComponent<AudioData>( go );
				go.transform.parent = node.transform;
				Undo.RegisterCreatedObjectUndo( go, "Create Out" );
			} else {
				ado = Undo.AddComponent<AudioData>( node.gameObject );
			}

			ado.Node = node;
			ado.Description = "OUT: " + inName;
			while( node.Outs.Count <= id ) {
				node.Outs.Add( null );
			}

			node.Outs[id] = ado;
		}

		public void CreateSlot<T>( int id, string slotName, bool createGameObject = true ) where T : ContainerSlot {
			AudioDataNode node = (AudioDataNode)target;
			Undo.RecordObject( node, "Create Slot" );
			T scs = null;
			if( createGameObject ) {
				GameObject go = new GameObject( "SLOT: " + slotName );
				scs = Undo.AddComponent<T>( go );
				go.transform.parent = node.transform;
				Undo.RegisterCreatedObjectUndo( go, "Create Slot" );
			} else {
				scs = Undo.AddComponent<T>( node.gameObject );
			}
			scs.Description = "SLOT: " + slotName;
			while( node.Slots.Count <= id ) {
				node.Slots.Add( null );
				Debug.Log(node.Slots.Count  );
			}
			node.Slots[id] = scs;
			Debug.Log( node.Slots[id] );

		}

		public void CreateSimpleData( int id, string dataName, float minValue, float maxValue, float startValue,
			SimpleData.WrapModeOptions wrapMode, bool createGameObject = true ) {
			CreateSimpleData( id, dataName, createGameObject );
			Target.Switches[id].Value = startValue;
			SerializedObject so = new SerializedObject( Target.Switches[id] );
			so.Update();
			so.FindProperty( "m_MinValue" ).floatValue = minValue;
			so.FindProperty( "m_MaxValue" ).floatValue = maxValue;
			so.FindProperty( "m_Value" ).floatValue = startValue;
			so.FindProperty( "m_WrapMode" ).enumValueIndex = (int) wrapMode;
			so.ApplyModifiedProperties();
		}
		
		public void CreateSimpleData( int id, string dataName, bool createGameObject = true ) {
			AudioDataNode node = (AudioDataNode)target;
			Undo.RecordObject( node, "Create SimpleData" );
			SimpleData sd = null;
			if( createGameObject ) {
				GameObject go = new GameObject( "DATA: " + dataName );
				sd = Undo.AddComponent<SimpleData>( go );
				go.transform.parent = node.transform;
				Undo.RegisterCreatedObjectUndo( go, "Create SimpleData" );
			} else {
				sd = Undo.AddComponent<SimpleData>( node.gameObject );
			}
			sd.Description = "DATA: " + dataName;
			while( node.Switches.Count <= id ) {
				node.Switches.Add( null );
			}
			node.Switches[id] = sd;
		}

	}
}