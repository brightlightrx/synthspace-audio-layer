/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using UnityEngine;

namespace Songspace.AudioDataNodes {
    public class SimpleData : MonoBehaviour {

        public enum WrapModeOptions {
            Wrap,
            Clamp
        }
        
        public string Description = "";
        [SerializeField] private float m_Value = 0f;
        [SerializeField] private float m_MinValue = 0f;
        [SerializeField] private float m_MaxValue = 1f;

        private float m_InitialValue;
        
        [SerializeField] private WrapModeOptions m_WrapMode = WrapModeOptions.Wrap;


        public delegate void ValueChangeHandler (float value);
        public event ValueChangeHandler onValueChange;
        
        public float Value {
            get => m_Value;
            set {
                if( m_WrapMode == WrapModeOptions.Wrap ) {
                    m_Value = value;
                    if( m_Value > m_MaxValue ) {
                        m_Value = m_MinValue;
                    }

                    if( m_Value < m_MinValue ) {
                        m_Value = m_MaxValue;
                    }
                } else if( m_WrapMode == WrapModeOptions.Clamp ) {
                    m_Value = Mathf.Clamp( value, m_MinValue, m_MaxValue );
                }

                onValueChange?.Invoke( m_Value );
            }
        }

        public float InitialValue => m_InitialValue;
        
        public float NormalizedInitialValue => Mathf.InverseLerp( m_MinValue, m_MaxValue, m_InitialValue );

        public float MinValue => m_MinValue;
        public float MaxValue => m_MaxValue;

        private void OnValidate() {
            m_InitialValue = m_Value;
        }

        public virtual void NextValue() {
            Value += 1f;
        }

        public virtual void PrevValue() {
            Value -= 1f;
        }

        public virtual void ShiftValue( float offset ) {
            Value += offset; 
        }

        /// <summary>
        /// Set the value without any safty-checks and without notifying anyone. Only do this if you know what you're doing!
        /// </summary>
        public float PureValue {
            get => m_Value;
            set => m_Value = value;
        }

        public float NormalizedValue => Mathf.InverseLerp( m_MinValue, m_MaxValue, m_Value );
    }
   
}