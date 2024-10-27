using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AICore3lb
{
    public abstract class BaseAI_SpeechToText : BaseAI
    {
        [AICoreHeader("Recording Settings")]
        [Tooltip("8000 | 16000 | 22050 | 44100")]

        public bool forceInputDevice;
        [AICoreReadOnly]
        public string defaultMicrophone;
        [AICoreShowIf("forceInputDevice")]
        public string inputDevice;
        [AICoreShowIf("forceInputDevice")]
        [AICoreReadOnly]
        public List<string> inputSources;
        public int sampleRate = 16000; //44100 //16000 //11025 //8000
        [Range(1, 30)]
        public float maxRecordingTime = 5;
        public bool startListeningToggles;
        [AICoreHeader("Silence Detection")]
        public bool hasSilenceDetection;
        [Range(.01f, .001f)]
        [Tooltip("To remove this make the required silence duration over recording time")]
        [AICoreShowIf("hasSilenceDetection")]
        public float silenceThreshold = 0.02f; // Adjust this value based on your needs
        [AICoreShowIf("hasSilenceDetection")]
        private float silenceLoopCheckRate = .2f;
        [AICoreShowIf("hasSilenceDetection")]
        public float requiredSilenceDuration = 1.5f;
        [AICoreShowIf("hasSilenceDetection")]
        public float stopAfterSilence = 1f;
        AudioSource audioSource;
        private AudioClip recording;
        private bool isListening = false;
        [AICoreHeader("Debug and Events")]
        [Tooltip("This is For Testing")]
        public bool echoTestForMic;
        public UnityEvent startedListening;
        public UnityEvent stopListening;


        public virtual void Start()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            defaultMicrophone = Microphone.devices[0];
            inputSources = new List<string>();
            foreach (string device in Microphone.devices)
            {
                inputSources.Add(device);
            }
#endif
        }
        [AICoreButton]
        public virtual void _StartListening()
        {
            if (startListeningToggles)
            {
                if(isListening)
                {
                    _StopRecordingAndProcess();
                    return;
                }
            }
            StartedListening();
            if (echoTestForMic)
            {
                audioSource = GetComponent<AudioSource>();
            }
            StartRecording();
        }

        public void _ProcessAudioClip(AudioClip myClip)
        {
            StartCoroutine(PostAudioFileCoroutineData(myClip));

        }

        protected void StartedListening()
        {
            startedListening.Invoke();
        }

        public virtual void _ChangeMic(string chg)
        {
            inputDevice = chg;
        }

        void StartRecording()
        {
            if (isListening)
            {
                return;
            }
            if (showDebugs)
            {
                Debug.LogError("Started Recording");
            }
            //int sampleRate = 44100;
            int seconds = Mathf.RoundToInt(maxRecordingTime);
            recording = null;
#if !UNITY_WEBGL || UNITY_EDITOR
            if (forceInputDevice)
            {
                recording = Microphone.Start(inputDevice, true, seconds, sampleRate);
            }
            else
            {
                recording = Microphone.Start(null, true, seconds, sampleRate);
            }
            if (showDebugs)
            {
                Debug.LogError("Starting Coroutine WaitForMicrophoneToStart");
            }
            StartCoroutine(WaitForMicrophoneToStart());
#endif
        }

        IEnumerator WaitForMicrophoneToStart()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            string whatInput = null;
            if (forceInputDevice)
            {
                whatInput = inputDevice;
            }
            while (!(Microphone.GetPosition(whatInput) > 0))
            {
                if (showDebugs)
                {
                    Debug.LogError("Running WaitForMicrophoneToStart");
                }
                yield return new WaitForEndOfFrame();
            }
            if (showDebugs)
            {
                Debug.LogError("Done WaitForMicrophoneToStart");
            }
            isListening = true;
            if(hasSilenceDetection)
            {
                StartCoroutine(CheckForSilence());
            }
#endif
        }

        [AICoreButton]
        public virtual void _StopRecordingAndProcess()
        {
            if (!isListening)
            {
                Debug.Log("Cannot process was not listening");
                return;
            }
            isListening = false;
            _CancelListening();
            if (showDebugs)
            {
                Debug.LogError("Stopped Recording Processing");
            }
            if (recording != null)
            {
                if (echoTestForMic)
                {
                    if (audioSource == null)
                    {
                        audioSource = gameObject.AddComponent<AudioSource>();
                    }
                    if (showDebugs)
                    {
                        Debug.LogError("Echoing");
                    }
                    audioSource.clip = recording;
                    audioSource.Play();
                    return;
                }
                //STOPPED TRANSCRIPTBING FOR Now!
                StartCoroutine(PostAudioFileCoroutineData(recording));
                AIStarted("Processing");
            }
            else
            {
                Debug.LogError("Recording has Failed");
            }

        }

        public virtual void _CancelListening()
        {
            isListening = false;
#if !UNITY_WEBGL || UNITY_EDITOR
            if (hasSilenceDetection)
            {
                StopCoroutine(CheckForSilence());
            }
            if (forceInputDevice)
            {
                Microphone.End(inputDevice);
            }
            else
            {
                Microphone.End(null);
            }
#endif
            stopListening.Invoke();
        }
#if !UNITY_WEBGL || UNITY_EDITOR
        private IEnumerator CheckForSilence()
        {
            float accumulatedSilenceTime = 0f; // Accumulated silence time in seconds
            float elapsedRecordingTime = 0f; // Track the total elapsed recording time
            string whatInput = null;
            if (forceInputDevice)
            {
                whatInput = inputDevice;
            }
            while (isListening)
            {
                yield return new WaitForSeconds(silenceLoopCheckRate); // Check periodically, adjust as needed
                elapsedRecordingTime += silenceLoopCheckRate; // Increment the elapsed recording time by 1 second

                // Check if maxRecordingTime has been reached
                if (elapsedRecordingTime >= maxRecordingTime)
                {
                    _StopRecordingAndProcess();
                    yield break; // Exit the coroutine
                }
                int currentPosition = Microphone.GetPosition(whatInput);
                if (currentPosition < recording.frequency) continue; // Ensure we have at least 1 second of audio

                float[] data = new float[recording.frequency]; // Array size based on 1 second of audio
                recording.GetData(data, currentPosition - data.Length); // Get last 1 second of audio

                float averageVolume = 0f;
                foreach (var sample in data)
                {
                    averageVolume += Mathf.Abs(sample);
                }
                averageVolume /= data.Length;

                if (showDebugs)
                {
                    Debug.LogError($"Silence Detection {averageVolume} / {silenceThreshold}");
                }

                if (averageVolume < silenceThreshold)
                {
                    accumulatedSilenceTime += silenceLoopCheckRate; // We've had 1 more second of silence
                    if (accumulatedSilenceTime >= requiredSilenceDuration)
                    {
                        yield return new WaitForSeconds(stopAfterSilence); // Wait for a specified period after silence is detected
                        _StopRecordingAndProcess();
                        yield break; // Exit the coroutine
                    }
                }
                else
                {
                    accumulatedSilenceTime = 0f; // Reset the counter if sound is detected
                }
            }
        }
#endif
        public virtual IEnumerator PostAudioFileCoroutineData(AudioClip myClip)
        {
            Debug.LogError("This must be implemented");
            yield return null;
        }

    }


    //This is a duplicate just in case someone only wants the AI module save utility
    public static class AISaveWavUtil
    {
        public static AudioClip TrimSilenceFromEnd(AudioClip clip, float threshold = 0.001f)
        {
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            int endSample = samples.Length - 1;
            while (endSample > 0 && Mathf.Abs(samples[endSample]) <= threshold)
            {
                endSample--;
            }

            endSample = Mathf.Max(endSample, 1); // Ensure there is at least one sample left

            float[] newSamples = samples.Take(endSample + 1).ToArray();
            AudioClip newClip = AudioClip.Create(clip.name + "_trimmed", newSamples.Length / clip.channels, clip.channels, clip.frequency, false);
            newClip.SetData(newSamples, 0);

            return newClip;
        }
        const int HEADER_SIZE = 44;

        public static bool Save(string filename, AudioClip clip)
        {
            if (!clip)
            {
                Debug.LogError("No AudioClip found!");
                return false;
            }

            var filepath = Path.Combine(Path.GetTempPath(), filename);
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            using (var fileStream = new FileStream(filepath, FileMode.Create))
            using (var writer = new BinaryWriter(fileStream))
            {
                var wavData = AudioClipToByteArray(clip);
                writer.Write(wavData, 0, wavData.Length);
            }

            return true;
        }

        public static byte[] AudioClipToByteArray(AudioClip clip)
        {
            var samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            var byteArray = new byte[samples.Length * 2];
            var rescaleFactor = 32767; // to convert float to Int16
            for (int i = 0; i < samples.Length; i++)
            {
                var intData = (short)(samples[i] * rescaleFactor);
                byte[] byteArr = System.BitConverter.GetBytes(intData);

                byteArray[i * 2] = byteArr[0];
                byteArray[i * 2 + 1] = byteArr[1];
            }

            return AddWavHeader(byteArray, clip.channels, clip.frequency);
        }

        static byte[] AddWavHeader(byte[] audioData, int channels, int sampleRate)
        {
            byte[] header = new byte[HEADER_SIZE];

            var fileLength = audioData.Length + HEADER_SIZE - 8;
            var audioFormat = 1;
            var byteRate = sampleRate * channels * 2;
            var blockAlign = (short)(channels * 2);

            // ChunkID
            header[0] = (byte)'R';
            header[1] = (byte)'I';
            header[2] = (byte)'F';
            header[3] = (byte)'F';
            // ChunkSize
            System.BitConverter.GetBytes(fileLength).CopyTo(header, 4);
            // Format
            header[8] = (byte)'W';
            header[9] = (byte)'A';
            header[10] = (byte)'V';
            header[11] = (byte)'E';
            // Subchunk1ID
            header[12] = (byte)'f';
            header[13] = (byte)'m';
            header[14] = (byte)'t';
            header[15] = (byte)' ';
            // Subchunk1Size
            System.BitConverter.GetBytes(16).CopyTo(header, 16);
            // AudioFormat
            System.BitConverter.GetBytes((short)audioFormat).CopyTo(header, 20);
            // NumChannels
            System.BitConverter.GetBytes((short)channels).CopyTo(header, 22);
            // SampleRate
            System.BitConverter.GetBytes(sampleRate).CopyTo(header, 24);
            // ByteRate
            System.BitConverter.GetBytes(byteRate).CopyTo(header, 28);
            // BlockAlign
            System.BitConverter.GetBytes(blockAlign).CopyTo(header, 32);
            // BitsPerSample
            System.BitConverter.GetBytes((short)16).CopyTo(header, 34);
            // Subchunk2ID
            header[36] = (byte)'d';
            header[37] = (byte)'a';
            header[38] = (byte)'t';
            header[39] = (byte)'a';
            // Subchunk2Size
            System.BitConverter.GetBytes(audioData.Length).CopyTo(header, 40);

            byte[] wavData = new byte[header.Length + audioData.Length];
            System.Buffer.BlockCopy(header, 0, wavData, 0, header.Length);
            System.Buffer.BlockCopy(audioData, 0, wavData, header.Length, audioData.Length);

            return wavData;
        }
    }
}
