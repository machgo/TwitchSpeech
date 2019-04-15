using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TwitchSpeech
{
    class MyWavStream : PullAudioInputStreamCallback
    {
        private BinaryReader _reader;

        public MyWavStream(Stream stream)
        {
            _reader = new BinaryReader(stream);
        }

        public override int Read(byte[] dataBuffer, uint size)
        {
            return _reader.Read(dataBuffer, 0, (int)size);
        }
    }
}
