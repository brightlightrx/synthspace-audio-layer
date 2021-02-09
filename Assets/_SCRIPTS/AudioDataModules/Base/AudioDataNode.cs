/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using System;
using System.Collections.Generic;
using Songspace.Container;
using UnityEngine;
using UnityEngine.Serialization;

namespace Songspace.AudioDataNodes {

	public class AudioDataNode : MonoBehaviour {

		#region Static

		private static bool s_Initialized = false;
		public static int SampleRate = 44100;
		public static double SingleSampleDuration = 2.267573696145125e-5d;
		public static int BufferSize = -1;
		public static int NumBuffers = 2;
		public static double GlobalDataTime => AudioSettings.dspTime;

		private static List<AudioDataNode> AllNodes = new List<AudioDataNode>();

		private static AudioDataNode FindNode( int runtimeId ) {
			foreach( AudioDataNode node in AllNodes ) {
				if( node.UniqueRuntimeId == runtimeId ) return node;
			}

			return null;
		}
		

		#endregion
		
		[SerializeField] private int m_UniqueRuntimeId = int.MinValue;

		public int UniqueRuntimeId => m_UniqueRuntimeId;

		public List<AudioDataInlet> Ins = new List<AudioDataInlet>();
		public List<SimpleData> Switches = new List<SimpleData>();
		public List<ContainerSlot> Slots = new List<ContainerSlot>();
		public List<AudioData> Outs = new List<AudioData>();

		[FormerlySerializedAs( "m_WaveLengthInSeconds" )] [SerializeField] protected double m_TimeLoopLengthInSeconds = 200.0f; //used to determine when to reset time

		[SerializeField] protected double m_Time = 0d; //current time of this Module (may be localized/decoupled for each module) 
		public double m_DataTime = -1d; //last time data was generated/processed
		

		void Awake() { 
			AudioSettings.GetDSPBufferSize( out BufferSize, out NumBuffers );
			//m_UniqueRuntimeId = UniqueIdManager.CreateUniqueRuntimeId();
			AllNodes.Remove( this );
			AllNodes.Add( this );
		}

		protected virtual void Start() {
			if( !s_Initialized ) {
				SampleRate = AudioSettings.outputSampleRate;
				SingleSampleDuration = 1d / (double) SampleRate;
				s_Initialized = true;
				Debug.Log( $"SampleRate: {SampleRate}, BufferSize: {BufferSize}"  );
			}
			m_Time = 0d; // AudioSettings.dspTime;
		}

		private void OnDestroy() {
			AllNodes.Remove( this );
		}

		public void Synchronize( AudioDataNode synchronizeTo ) {
			m_Time = synchronizeTo.m_Time;
		}
		
		/// <summary>
		/// Prepare Data
		/// </summary>
		/// <param name="dataTime">reference time</param>
		/// <param name="num">number of samples needed</param>
		public void PrepareData( double dataTime, int num ) {
			if( Math.Abs( m_DataTime - dataTime ) > 0.000001f ) { //to prevent infinite loops
				m_DataTime = dataTime; //IMPORTANT that this happens BEFORE In.GetData, otherwise the check above doesn't help and we can crash due to infinite loop
				for ( int i = 0; i < Ins.Count; i++ ) {
					Ins[i].GetData( dataTime, num );
				}
				for ( int i = 0; i < Outs.Count; i++ ) {
					Outs[i].SetDataLength( num );
				}
				ProcessData();
			}
		}

		public bool AdvanceLocalTime( int num ) {
			m_Time += SingleSampleDuration * (double) num;

			if ( m_Time > m_TimeLoopLengthInSeconds ) {
				m_Time -= m_TimeLoopLengthInSeconds;
				return true;
			}

			return false;
		}
		

		public virtual void ProcessData() { //override this
			AdvanceLocalTime( BufferSize );	
		}

		public double GetLocalTime() {
			return m_Time;
		}
		
	}
}
