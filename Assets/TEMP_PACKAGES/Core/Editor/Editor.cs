using UnityEditor;
using UnityEngine;

namespace Core {
	public class Editor<T> : Editor where T : Object {

		private T m_Target;
	
		public T Target {
			get {
				if( m_Target == null ) m_Target = target as T;
				return m_Target;
			}
		} 
	}
}