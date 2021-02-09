using System;
using System.Runtime.InteropServices;
using FaustUtilities_flanger;
using Songspace.AudioDataNodes;
using UnityEngine;

public class FlangerNode : AudioDataNode {
    
    public float[] parameters = new float[9];
    
    //0 = bypass
    //1 = invert flange sum
    //2 = ?
    //3 = speed
    //4 = depth
    //5 = feedback
    //6 = flange delay
    //7 = delay offset
    //8 = flanger ouput level
    
    private Faust_Context m_Ctx;

    /* @brief Returns true if the plugin is instantiated (the plugin is instantiated when play mode button is pressed)*/
    public bool IsInstantiated() {
        return ( m_Ctx != null );
    }
    
    /* @brief Gets a parameter value from the plugin
    * @param param Use the parameter number available in the parameter inspector (tooltip)
    * @return The parameter value */
    public float GetParameter( int param ) {
        if( IsInstantiated() ) {
            // if the the plugin is instantiated, the parameter value is changed directly in the plugin
            return m_Ctx.getParameterValue( param );
        }
        else { // if not, the value is stored in parameters[]
            return parameters[param];
        }
    }

    /* @brief Sets a parameter value in the plugin
    * @param param Use the parameter number available in the parameter inspector (tooltip)
    * @param x New parameter value */
    public void SetParameter( int param, float x ) {
        if( IsInstantiated() ) {
            m_Ctx.setParameterValue( param, x );
            parameters[param] = x;
        }
        else {
            parameters[param] = x;
        }
    }

    /* @brief Instantiates the plugin and the interface between the plugin and Unity
    * @brief And sets the new parameter values changed while in pause mode */
    private void Awake() {
	    Debug.Log( $"************* Flanger: START" );
	    m_Ctx = new Faust_Context( BufferSize );
        Debug.Log( $"************* Flanger: Faust_Context: {m_Ctx}" );
        m_Ctx.context_init( SampleRate );
        for (int i = 0; i < parameters.Length; i++) {
            SetParameter(i, parameters[i]);
        }

        Debug.Log( "Awake Done" );
    }
    
    /* @brief Gets the min value of a parameter
    * @param Use the parameter number available in the parameter inspector (tooltip) */
    public float GetParameterMin(int param) {
        return m_Ctx.getParamMin(param);
    }

    /* @brief Gets the max value of a parameter
    * @param Use the parameter number available in the parameter inspector (tooltip) */
    public float GetParameterMax(int param) {
        return m_Ctx.getParamMax(param);
    }
    
    public override void ProcessData() {
        //base.ProcessData();
        //Array.Copy( Ins[0].GetSamples(), Outs[0].Data, BufferSize );

        if( Switches[0].Value != parameters[3] ) { //SPEED
            SetParameter( 3, Switches[0].Value );
        }
        if( Switches[1].Value != parameters[4] ) { //DEPTH
            SetParameter( 4, Switches[1].Value );
        }
        
        m_Ctx.process( Ins[0].GetSamples(), Outs[0].Data, BufferSize, 1 );
    }
}

namespace FaustUtilities_flanger {
	/* @brief This class is the interface between the native plugin and the Unity environment
	*/
	public class Faust_Context {

		private IntPtr _context;
//UNITY_EDITOR_OSX || UNITY_EDITOR_WIN || 
#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_WSA_10_0
		const string _dllName = "libFaustPlugin_flanger";
#elif UNITY_IOS
        const string _dllName = "__Internal";
#elif UNITY_EDITOR || UNITY_ANDROID || UNITY_STANDALONE_LINUX
        const string _dllName = "libFaustPlugin_flanger.so";
#else
        Debug.LogError("Architecture not supported by the plugin");
#endif

		// Imports all c++ function to intialize and process the dsp. The methods need to be private
		[DllImport( _dllName )]
		private static extern IntPtr Faust_contextNew( int buffersize );

		[DllImport( _dllName )]
		private static extern void Faust_contextInit( IntPtr ctx, int samplerate );

		[DllImport( _dllName )]
		private static extern void Faust_process( IntPtr ctx, [In] float[] inbuffer, [Out] float[] outbuffer,
			int numframes, int channels );

		[DllImport( _dllName )]
		private static extern void Faust_delete( IntPtr ctx );

		[DllImport( _dllName )]
		private static extern int Faust_getSampleRate( IntPtr ctx );

		[DllImport( _dllName )]
		private static extern int Faust_getNumInputChannels( IntPtr ctx );

		[DllImport( _dllName )]
		private static extern int Faust_getNumOutputChannels( IntPtr ctx );

		[DllImport( _dllName )]
		private static extern void Faust_setParameterValue( IntPtr ctx, int param, float value );

		[DllImport( _dllName )]
		private static extern float Faust_getParameterValue( IntPtr ctx, int param );

		[DllImport( _dllName )]
		private static extern float Faust_getParamMin( IntPtr ctx, int param );

		[DllImport( _dllName )]
		private static extern float Faust_getParamMax( IntPtr ctx, int param );

		public Faust_Context( int buffersize ) {
			_context = Faust_contextNew( buffersize );
		}

		~Faust_Context() {
			Faust_delete( _context );
		}

		public void context_init( int samplerate ) {
			Faust_contextInit( _context, samplerate );
		}

		public int getSampleRate() {
			return Faust_getSampleRate( _context );
		}

		public int getNumInputChannels() {
			return Faust_getNumInputChannels( _context );
		}

		public int getNumOutputChannels() {
			return Faust_getNumOutputChannels( _context );
		}

		public void process( float[] buffer, int numframes, int channels ) {
			Faust_process( _context, buffer, buffer, numframes, channels );
		}

		public void process( float[] inBuffer, float[] outBuffer, int numframes, int channels ) {
			Faust_process( _context, inBuffer, outBuffer, numframes, channels );
		}

		public void setParameterValue( int param, float value ) {
			Faust_setParameterValue( _context, param, value );
		}

		public float getParameterValue( int param ) {
			return Faust_getParameterValue( _context, param );
		}

		public float getParamMin( int param ) {
			return Faust_getParamMin( _context, param );
		}

		public float getParamMax( int param ) {
			return Faust_getParamMax( _context, param );
		}

	}

}