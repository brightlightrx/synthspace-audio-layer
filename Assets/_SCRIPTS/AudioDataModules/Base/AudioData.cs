/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using UnityEngine;

namespace Songspace.AudioDataNodes {
    public class AudioData : MonoBehaviour {

        public AudioDataNode Node;

        public int DataSampleCount = 512;
        public float[] Data = new float[512];
        
        public string Description = "";

        public object OutId => Node.Outs.IndexOf( this );

        private void Reset() { Node = GetComponent<AudioDataNode>(); }

        private void Start() {
            if ( Node == null ) {
                Debug.LogError( "AudioDataNode missing on OutConnection", gameObject );
            }

            Data = new float[AudioDataNode.BufferSize];
            DataSampleCount = AudioDataNode.BufferSize;
        }

        public void SetDataLength( int num ) {
            if ( DataSampleCount != num ) {
                DataSampleCount = num;
                Data = new float[num];
            }
        }

        public virtual void GetData( double time, int num ) {
            if ( num != DataSampleCount ) {
                Data = new float[num];
                DataSampleCount = num;
            }

            Node.PrepareData( time, num );
        }

        public virtual float GetSample( int id ) { return Data[id]; }

        public virtual float GetSample( int id, int channel ) { return Data[id]; }

        public virtual float[] GetSamples() { return Data; }
    }
}
