/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using System.Collections.Generic;
using Songspace.Container;

namespace Interaction.Audio {
	public class PatternContainerSlot : ContainerSlotGeneric<List<float>> {

		//[SerializeField, Color(true)] private GameObject m_NewContainerPrefab;
		
		//protected new SampleContainer m_CurrentContainer;
		public new PatternContainer CurrentContainer => m_CurrentContainer as PatternContainer;

		// public virtual PatternContainer CreateContainer() {
		// 	GameObject containerGameObject = Instantiate( m_NewContainerPrefab, transform, true );
		// 	PatternContainer container = containerGameObject.GetComponent<PatternContainer>();
		// 	containerGameObject.transform.localScale = Vector3.one;
		// 	SetCurrentContainer( container );
		// 	return container;
		// }

		public void SetCurrentContainer( PatternContainer container ) {
			base.SetCurrentContainer( container );
		}
	}
}