/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using System.Collections.Generic;
using Songspace.AudioDataNodes;
using UnityEngine;

public class DebugUI : MonoBehaviour {
	
	[SerializeField] private Texture2D m_Logo;
	[SerializeField] private List<AudioDataInlet> m_AudioDataInlets = new List<AudioDataInlet>();

	private void OnGUI() {

		if( m_Logo != null ) GUI.DrawTexture( new Rect( Screen.width - m_Logo.width - 16f, 16f, m_Logo.width, m_Logo.height ), m_Logo );
		
		GUILayout.BeginVertical(  );
		GUILayout.Space( 16f );
		foreach( AudioDataInlet inlet in m_AudioDataInlets ) {
			GUILayout.BeginHorizontal(  );
			GUILayout.Space( 16f );
			GUILayout.Label( inlet.Node.name + "." + inlet.Description, GUILayout.Width( 160f ) );
			inlet.Value = GUILayout.HorizontalSlider( inlet.Value, -1f, 1f, GUILayout.Width( 200f ) );
			GUILayout.Label( inlet.DisplayValue.ToString("0.00"), GUILayout.Width( 50f ) );
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}
}
