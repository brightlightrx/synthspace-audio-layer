/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using UnityEngine;

namespace Songspace.AudioDataNodes {
	public class SimpleDataMapped : SimpleData {
		[Header("Mapped From")]
		public float MinFrom = -1f;
		public float MaxFrom = 1f;
		[Header("Mapped To")]
		public float MinTo = -1f;
		public float MaxTo = 1f;

		public float MappedValue => Mathf.Lerp( MinTo, MaxTo, Mathf.InverseLerp( MinFrom, MaxFrom, Value ) );
		
		public void SetMapping( float minFrom, float maxFrom, float minTo, float maxTo) {
			MinFrom = minFrom;
			MaxFrom = maxFrom;
			MinTo = minTo;
			MaxTo = maxTo;
		}
		
	}
}