/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using UnityEngine;

namespace Songspace.AudioDataNodes {
    public class AudioDataInletMapped : AudioDataInlet {
        [Header("Mapped From")]
        public float MinFrom = -1f;
        public float MaxFrom = 1f;
        [Header("Mapped To")]
        public float MinTo = -1f;
        public float MaxTo = 1f;

        public override float NormalizedValue => Mathf.InverseLerp( MinFrom, MaxFrom, Value );
        public override float NormalizedInitialValue => Mathf.InverseLerp( MinFrom, MaxFrom, InitialValue );
        
        public override float GetSample( int id ) {
            if ( !IsConnected ) {
                return DisplayValue = Mathf.Lerp( MinTo, MaxTo, Mathf.InverseLerp( MinFrom, MaxFrom, Value ) );
            }

            return DisplayValue = Mathf.Lerp( MinTo, MaxTo, Mathf.InverseLerp( MinFrom, MaxFrom, ConnectedFrom.GetSample( id ) ) );
        }

        public void SetMapping( float minFrom, float maxFrom, float minTo, float maxTo) {
            MinFrom = minFrom;
            MaxFrom = maxFrom;
            MinTo = minTo;
            MaxTo = maxTo;
        }

        public void SetValueFromMapped( float mappedValue ) {
            Value = Mathf.Lerp( MinFrom, MaxFrom, Mathf.InverseLerp( MinTo, MaxTo, mappedValue ) );
        }

        public override void DriveFromSource( float val ) {
            base.DriveFromSource( val );
            SetValueFromMapped( val );
        }
    }
}