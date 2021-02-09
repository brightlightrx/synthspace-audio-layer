/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using System.IO;
using UnityEngine;

namespace Songspace.Container {
	public class Container : MonoBehaviour {

		//public Tag Tag;
		public string Filepath = "";
		public string Filename => Path.GetFileNameWithoutExtension( Filepath );
		public string Label;
		public Color Color = Color.white;

		public delegate void ContentEventHandler();
		public event ContentEventHandler onContentLoaded;
		public event ContentEventHandler onContentChanged;

		protected bool m_ContentChanged;
		
		public virtual void CheckCargo() { }

		public void CheckPath( string path ) {
			if( !Directory.Exists( path ) ) {
				Directory.CreateDirectory( path );
			}
		}

		public virtual string GetLabel() {
			if( !string.IsNullOrEmpty( Label ) ) return Label;
			return Filename;
		}
		
		public virtual string SaveToFile( string filePath ) {
			Filepath = filePath;
			return SaveToFile();
		}
		public virtual string SaveToFile() { return null; }

		public virtual bool LoadFromFile( string filepath ) { return false; }

		public void SetContentChanged() {
			m_ContentChanged = true;
		}
		
		public void CheckIfContentChanged() {
			if( m_ContentChanged ) InvokeOnContentChanged();
		}
		
		protected void InvokeOnContentLoaded() {
			onContentLoaded?.Invoke();
		}

		public void InvokeOnContentChanged() {
			m_ContentChanged = false;
			onContentChanged?.Invoke();
		}
	}
}