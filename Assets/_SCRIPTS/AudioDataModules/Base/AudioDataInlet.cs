/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using UnityEngine;

namespace Songspace.AudioDataNodes {

	public class AudioDataInlet : MonoBehaviour {

		public string Description = "";
		public AudioDataNode Node;

		[Header( "Value and Source" )] 
		[SerializeField] private AudioData m_ConnectedFrom;
		[SerializeField] private float m_Value;
		[SerializeField] private float m_InitialValue; //store the initial value - resetting the knob goes back to this value
		private float m_PrevValue = 0f;
		public float DisplayValue = 0f; //

		private bool m_ValueDrivenFromSource; //driving directly, make visualization update to our value!
		
		protected float[] m_ValueData = new float[512];

		protected float m_DataTime = 0f;
		private float m_NumFactor = 1f;

		[SerializeField] private bool m_IsConnected;
		
		public virtual float NormalizedValue => Value;
		public virtual float NormalizedInitialValue => m_InitialValue;
		public bool ValueDrivenFromSource => m_ValueDrivenFromSource;
		public bool IsConnected => m_IsConnected;

		public AudioData ConnectedFrom {
			get => m_ConnectedFrom;
			set {
				m_IsConnected = value != null;
				m_ConnectedFrom = value;
			}
		}

		public float Value {
			get => m_Value;
			set => m_Value = value;
		}

		public float InitialValue => m_InitialValue;
		
		private void OnValidate() {
			m_IsConnected = m_ConnectedFrom != null;
			m_InitialValue = Value;
		}

		private void Reset() {
			if( Node == null ) Node = GetComponent<AudioDataNode>();
			if( Node == null ) Node = GetComponentInParent<AudioDataNode>();
		}

		private void Start() {
			m_InitialValue = Value;
			m_PrevValue = Value;
			ConnectedFrom = m_ConnectedFrom;
			if( Node == null ) Debug.LogError( "AudioDataNode not set! (Assign Node on the AudioDataInlet!)", gameObject );
		}
		
		/// <summary>
		/// Prepare the Buffer Array
		/// </summary>
		/// <param name="time">reference time</param>
		/// <param name="num">number of samples needed</param>
		public virtual void GetData( double time, int num ) {
			if ( !m_IsConnected ) { //no actual data coming in? Fill ValueData Array from single Value
				GetDefaultData( num );
			} else { //Data coming in from outside, make sure that that's ready 
				ConnectedFrom.GetData( time, num );
			}
		}

		protected void GetDefaultData( int num ) {
			if( m_ValueData.Length != num ) {
				m_ValueData = new float[num];
				m_NumFactor = 1f / num;
			}

			if( m_PrevValue != m_Value ) {
				for( int i = 0; i < num; i++ ) { //Lerp from PrevValue
					m_ValueData[i] = Mathf.Lerp( m_PrevValue, m_Value, m_NumFactor * i );
				}
			}

			m_PrevValue = Value;
		}

		/// <summary>
		/// Get a single sample
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual float GetSample( int id ) {
			if ( !m_IsConnected ) return DisplayValue = m_ValueData[id];
			return DisplayValue = ConnectedFrom.GetSample( id );
		}

		public virtual float GetSample( int id, int channel ) {
			if ( !m_IsConnected ) return DisplayValue = m_ValueData[id];
			return DisplayValue = ConnectedFrom.GetSample( id, channel );
		}

		public virtual float[] GetSamples() {
			if( !m_IsConnected ) return m_ValueData;
			return ConnectedFrom.Data; //GetSamples();
		}

		/// <summary>
		/// Set a value and make sure it propagates up to the visualization
		/// </summary>
		public virtual void DriveFromSource( float val ) {
			if( !m_IsConnected ) {
				m_ValueDrivenFromSource = true;
				m_Value = val;
			}
		}

		public void StopDrivingFromSource() {
			m_ValueDrivenFromSource = false;
		}
	}
}
