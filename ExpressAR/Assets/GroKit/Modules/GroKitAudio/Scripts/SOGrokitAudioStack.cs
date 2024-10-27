
using System.Collections.Generic;
using UnityEngine;

namespace Core3lb
{
    [CreateAssetMenu(fileName = "GroKitAudioStack", menuName = "GroKit-Core/GroKitAudioStack", order = 1)]
    public class SOGroKitAudioStack : ScriptableObject
    {
        [Space(10)]
        public bool isLooper= false;
        public AudioSource defaultTemplate;
        public AudioClipData[] clipStack;

        public List<AudioClipData> GetWeightedList()
        {
            List<AudioClipData> weightedList = new List<AudioClipData>();

            foreach (var clipData in clipStack)
            {
                for (int i = 0; i < clipData.randomWeights; i++)
                {
                    weightedList.Add(clipData);
                }
            }

            return weightedList;
        }

        public AudioClipData PickOne()
        {
            List<AudioClipData> weightedList = GetWeightedList();
            if (weightedList.Count == 0)
            {
                return null;
            }

            int index = UnityEngine.Random.Range(0, weightedList.Count);
            return weightedList[index];
        }
    }

    [System.Serializable]
    public class AudioClipData
    {
        public float delay = 0.0f;
        public AudioClip[] myClips;
        public AudioSource template;
        public ClipAlterations alterations;
        public bool shouldFollow;
        [Range(0,100)]
        public int randomWeights;
    }

    [System.Serializable]
    public class ClipAlterations
    {
        public bool alterPitch;
        public Vector2 randomPitch = Vector2.one;
        public bool alterVolume;
        public Vector2 randomVolume = Vector2.one;
    }




}
