/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

namespace Songspace.AudioDataNodes {
    public class AmplifierNode : AudioDataNode {
        public override void ProcessData() {
            for( int i = 0; i < BufferSize; i++ ) {
                Outs[0].Data[i] = Ins[0].GetSample( i ) * Ins[1].GetSample( i ) * ( Ins[2].IsConnected ? Ins[2].GetSample( i ) : 1f );
            }
        }
    }
}