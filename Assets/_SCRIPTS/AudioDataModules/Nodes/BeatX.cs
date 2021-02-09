/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using System.Collections.Generic;
using Songspace.AudioDataNodes;
using UnityEngine;

public class BeatX : ClockNode {

	//STATIC
	#region Singleton 
	
	private static List<BeatX> BeatXes = new List<BeatX>();

	private static void Register( BeatX beatX ) {
		BeatXes.Remove( beatX );
		BeatXes.Add( beatX );
	}

	private static void DeRegister( BeatX beatX ) {
		BeatXes.Remove( beatX );
	}
	
	public static bool Exists => s_Instance != null;

	private static BeatX s_Instance = null;
	public static BeatX I => Instance;

	private static bool NotFoundWarningShown;
	
	public static BeatX Instance { 
		get {
			if( s_Instance == null ) {
				if( BeatXes.Count > 0 ) BeatXes.RemoveAll( ( b ) => b == null );
				if( BeatXes.Count > 0 ) s_Instance = BeatXes[0];
				if( s_Instance == null && !NotFoundWarningShown ) {
					Debug.LogWarning( "Could not locate a BeatX object." );
					NotFoundWarningShown = true;
				}
			}
			return s_Instance;
		}
	}
	
	#endregion

	protected override void OnEnable() {
		base.OnEnable();
		Register( this );
	}

	private void OnDisable() {
		DeRegister( this );
	}

}
