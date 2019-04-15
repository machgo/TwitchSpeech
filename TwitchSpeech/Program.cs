using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TwitchSpeech
{
    class Program
    {
        private static void Recognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
        {
            var result = e.Result;
            // Checks result.
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"We recognized: {result.Text}");
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }
            }
        }

        static async Task Main()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-i http://127.0.0.1:56792/ -c:v none -acodec pcm_s16le -ac 1 -ar 16000 -f wav -",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };

            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Start();
            process.BeginErrorReadLine();
            var a = new MyWavStream(process.StandardOutput.BaseStream);

            byte channels = 1;
            byte bitsPerSample = 16;
            uint samplesPerSecond = 16000;
            var audioFormat = AudioStreamFormat.GetWaveFormatPCM(samplesPerSecond, bitsPerSample, channels);

            var audioConfig = AudioConfig.FromStreamInput(a, audioFormat);
            var speechConfig = SpeechConfig.FromSubscription("x", "westeurope");

            var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            recognizer.Recognized += Recognizer_Recognized;
            await recognizer.StartContinuousRecognitionAsync();

            process.WaitForExit(10000000);
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                //Console.WriteLine(e.Data);
                // do nothing
            }
        }
    }
}
