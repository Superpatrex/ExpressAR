using System;
using System.IO;
using UnityEngine;

namespace AICore3lb
{

    public class TTS_SaveToFile : AICoreBehaviour
    {
        [AICoreRequired]
        public BaseAI_TextToSpeech targetTTS;

        [TextArea]
        public string whatToSay;

        [AICoreButton]
        public void ShowSaveFolder()
        {
            string filepath = Path.Combine(Application.persistentDataPath, $"waveFile.wav");
            string folderPath = Path.GetDirectoryName(filepath);
            Application.OpenURL("file://" + folderPath);
        }

        public void Start()
        {
            if (targetTTS == null)
            {
                Debug.LogError("No TTS Target Set", gameObject);
                return;
            }
            targetTTS.audioPlayStarted?.AddListener(SaveAudioToFile);
        }


        [AICoreButton]
        public void SpeakAndSaveAudio()
        {
            targetTTS._Speak(whatToSay);
        }

        [AICoreButton]
        public void _SpeakAndSaveAudio()
        {
            targetTTS._Speak(whatToSay);
        }


        private void SaveAudioToFile(AudioClip clipToSave)
        {
            if (clipToSave == null)
            {
                Debug.LogError("Got a null clip cannot save");
                return;
            }
            string[] words = whatToSay.Split(' ');
            string firstThreeWords = string.Join("_", words.Length >= 4 ? words[..4] : words);
            // Get the current day and time
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            // Create the filename
            string fileName = $"{firstThreeWords.Replace(" ", "")}_{timestamp}.wav";
            SaveClipToWaveInPersistent(clipToSave, fileName);
        }

        public static void SaveClipToWaveInPersistent(AudioClip clip, string fileName = "SavedAudio")
        {

            string filepath = Path.Combine(Application.persistentDataPath, fileName);

            var samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            using (var fileStream = new FileStream(filepath, FileMode.Create))
            {
                for (int i = 0; i < 44; i++)
                    fileStream.WriteByte(0);

                Int16[] intData = new Int16[samples.Length];
                Byte[] bytesData = new Byte[samples.Length * 2];
                float rescaleFactor = 32767;

                for (int i = 0; i < samples.Length; i++)
                {
                    intData[i] = (short)(samples[i] * rescaleFactor);
                    BitConverter.GetBytes(intData[i]).CopyTo(bytesData, i * 2);
                }
                fileStream.Write(bytesData, 0, bytesData.Length);

                fileStream.Seek(0, SeekOrigin.Begin);

                Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
                fileStream.Write(riff, 0, 4);

                Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
                fileStream.Write(chunkSize, 0, 4);

                Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
                fileStream.Write(wave, 0, 4);

                Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
                fileStream.Write(fmt, 0, 4);

                Byte[] subChunk1 = BitConverter.GetBytes(16);
                fileStream.Write(subChunk1, 0, 4);

                UInt16 one = 1;

                Byte[] audioFormat = BitConverter.GetBytes(one);
                fileStream.Write(audioFormat, 0, 2);

                Byte[] numChannels = BitConverter.GetBytes(clip.channels);
                fileStream.Write(numChannels, 0, 2);

                Byte[] sampleRate = BitConverter.GetBytes(clip.frequency);
                fileStream.Write(sampleRate, 0, 4);

                Byte[] byteRate = BitConverter.GetBytes(clip.frequency * clip.channels * 2);
                fileStream.Write(byteRate, 0, 4);

                UInt16 blockAlign = (ushort)(clip.channels * 2);
                fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

                UInt16 bps = 16;
                Byte[] bitsPerSample = BitConverter.GetBytes(bps);
                fileStream.Write(bitsPerSample, 0, 2);

                Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
                fileStream.Write(datastring, 0, 4);

                Byte[] subChunk2 = BitConverter.GetBytes(clip.samples * clip.channels * 2);
                fileStream.Write(subChunk2, 0, 4);
            }
            Debug.LogError($"File Saved to {Application.persistentDataPath} + Name: {fileName}");
        }

        private static byte[] ConvertToWav(AudioClip clip)
        {
            var samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            int sampleCount = samples.Length;
            int byteCount = sampleCount * sizeof(short);
            byte[] wav = new byte[44 + byteCount];

            int sampleRate = clip.frequency;
            int channels = clip.channels;
            int bitsPerSample = 16;

            int subchunk2Size = sampleCount * channels * bitsPerSample / 8;
            int chunkSize = 36 + subchunk2Size;

            using (MemoryStream stream = new MemoryStream(wav))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(chunkSize);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16);
                    writer.Write((short)1);
                    writer.Write((short)channels);
                    writer.Write(sampleRate);
                    writer.Write(sampleRate * channels * bitsPerSample / 8);
                    writer.Write((short)(channels * bitsPerSample / 8));
                    writer.Write((short)bitsPerSample);

                    writer.Write("data".ToCharArray());
                    writer.Write(subchunk2Size);

                    for (int i = 0; i < samples.Length; i++)
                    {
                        writer.Write((short)(samples[i] * short.MaxValue));
                    }
                }
            }
            return wav;
        }
    }
}