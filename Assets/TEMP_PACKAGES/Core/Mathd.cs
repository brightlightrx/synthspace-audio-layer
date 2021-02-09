using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core {
    public class Mathd {
        public static double Repeat(double t, double length) {
            return Clamp(t - Math.Floor(t / length) * length, 0.0f, length);
        }
    
        public static double Clamp(double value, double min, double max) {
            if (value < min)
                value = min;
            else if ( value > max)
                value = max;
            return value;
        }

        public static int RoundToInt( double value ) {
            return (int) Math.Round( value );
        }
        
        public static double InverseLerp(double a, double b, double value) {
            if (a != b) {
                double preClamp = (value - a) / (b - a);
                if (preClamp < 0.0d) return 0.0d;
                if (preClamp > 1.0d) return 1d;
                return preClamp;
            }
            return 0.0d;
        }
        
    }
}