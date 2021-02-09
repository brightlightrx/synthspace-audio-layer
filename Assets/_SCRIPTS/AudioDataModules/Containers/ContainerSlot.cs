/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using System.Collections.Generic;
using UnityEngine;

namespace Songspace.Container {
    public class ContainerSlot : MonoBehaviour {
        public delegate void ContainerEventHandler();
        public event ContainerEventHandler onNewCurrentContainer;
        public event ContainerEventHandler onNewContainerAdded;
        public event ContainerEventHandler onNewContainerRequested;
        
        [SerializeField] protected Container m_CurrentContainer;
        private readonly Queue<Container> m_ContainerQueue = new Queue<Container>();
        
        [SerializeField] protected bool m_IsNew = false;

        public string Description;
        
        public Container CurrentContainer => m_CurrentContainer;

        public virtual void AddContainer( Container container ) {
            m_ContainerQueue.Enqueue( container );
            onNewContainerAdded?.Invoke();
        }

        public void SetCurrentContainer( Container container ) {
            m_CurrentContainer = container;
            Debug.Log( "New Container: " + ( container == null ? "NULL" : container.ToString() ) );
            onNewCurrentContainer?.Invoke();
        }

        void Update() {
            if ( m_IsNew ) {
                m_IsNew = false;
                SetCurrentContainer( m_CurrentContainer );
            }
        }
        
        public virtual void CreateContainer() { 
            onNewContainerRequested?.Invoke(); //Use ContainerDataHandler to listen for this and spawn one!
        }
    }
}