using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Core3lb
{
    [DefaultExecutionOrder(-10)]
	[CoreClassName("GroKit AudioManager")]
	public class GroKitAudioManager : Singleton<GroKitAudioManager>
	{
		public int preloadAmount = 20;
        public bool isPersistent = false;
        [CoreHeader("Audio Defaults")]
        [CoreRequired]
        public AudioMixerGroup defaultAudioMixerForPooled;
        [CoreRequired]
        public AudioMixer masterMixer;
		[CoreRequired]
		public AudioSource defaultTemplate3D; //This the template for everything without one
        public AudioMixerSnapshot defaultSnapshot;
		static List<GameObject> components;
        [CoreEmphasize]
        public LoopedSourcesCrossFader ApplicationPlayList;
        [CoreHeader("Debugs")]
		[SerializeField] bool debugAudioEditor;
        public AudioClip testClip;
        public static bool showDebugs;
		public static bool goToTemplates;

		public List<AudioPooler> lastPooled;
        public List<LoopedSource> loopedSources;

		public void Start()
		{
			if (isPersistent)
			{
				DontDestroyOnLoad(gameObject);
            }
			if(defaultTemplate3D == null)
			{
				Debug.LogError("Default Template Not Found! / Creating");
				defaultTemplate3D = gameObject.AddComponent<AudioSource>();
                defaultTemplate3D.playOnAwake = false;
                defaultTemplate3D.volume = 1;
                defaultTemplate3D.pitch = 1;
                defaultTemplate3D.spatialBlend = 1;
                defaultTemplate3D.outputAudioMixerGroup = defaultAudioMixerForPooled;
            }
			if (debugAudioEditor)
			{
				showDebugs = true;
			}
			if (!Application.isEditor)
			{
				showDebugs = false;
			}
			lastPooled = new List<AudioPooler>();
			GenerateAudioSources();
		}

		public void _ChangeTrack(int chg)
		{
            if (!instance.ApplicationPlayList)
            {
				return;    
            }
            instance.ApplicationPlayList._PlayTrack(chg);
        }

		[CoreButton]
		public void ToggleDebugs()
		{
			showDebugs = !showDebugs;
			if (showDebugs)
			{
				Debug.LogError("-- Audio Debugging Enabled");
			}
			else
			{
				Debug.LogError("-- Audio Debugging Disabled");
			}
		}

		[CoreButton]
		public void GetPlayingLoopedSources()
		{
			loopedSources = new List<LoopedSource>();
            var sources = FindObjectsOfType<LoopedSource>();
			foreach (var item in sources)
			{
				if(item.mySource.isPlaying)
				{
                    loopedSources.Add(item);
                }
			}
		}

        public void GenerateAudioSources()
		{
			GameObject go = new();
			go.name = "SoundInstance";
			go.transform.parent = transform;
			AudioSource audio = go.AddComponent<AudioSource>();
			audio.playOnAwake = false;
			audio.volume = 1;
			audio.pitch = 1;
			audio.spatialBlend = 1;
			audio.outputAudioMixerGroup = defaultAudioMixerForPooled;
			AudioPooler holder = go.AddComponent<AudioPooler>();
			holder.mySource = audio;
			holder.overrideMixer = defaultAudioMixerForPooled;
			AudioPool.Preload(go, preloadAmount);
			components = new List<GameObject>(AudioPool.inactive);
			foreach (var item in components)
			{
				item.transform.parent = transform;
			}
        }

		[CoreButton]
		public void TestAudio()
		{
			PlaySFX2D(gameObject, testClip, 1, 1);
		}

		public void _Play2DAudio(AudioClip whatClip)
		{
            PlaySFX2D(gameObject, whatClip, 1, 1);
        }

		public static List<GameObject> audioInstances
		{
			get
			{
				return components;
			}
		}
		/// <summary>
		/// Play3D Audio at Position
		/// </summary>
		/// <param name="audioClip">The clip you want the player to hear</param>
		/// <param name="position">The position where the clip will be played</param>
		/// <param name="volume">The clip volume. (Will use default volume if not set or under 0)</param>
		/// <param name="is3D"></param>
		/// <param name="randomization">Use this to add some randomness when you use the same clip multiples times</param>
		/// <param name="audioSource">The AudioSource template if you want to make it by yourself. (Volume and is3D will be overwritten)</param>
		public void PlayAudio3D(GameObject whoDidit, AudioClip audioClip, Transform whereTo, bool follow, float pitch = 1, float volume = 1, AudioMixerGroup mixer = null, float min = 1, float max = 500, bool spatalize = false, AudioRolloffMode rollMode = AudioRolloffMode.Linear)
		{
			AudioPooler holder = AudioPool.Spawn().GetComponent<AudioPooler>();
			holder.transform.position = whereTo.position;
			holder.transform.rotation = whereTo.rotation;
			if (follow)
			{
				holder.followTarget = whereTo;
			}
			else
			{
				holder.followTarget = null;
			}
			//holder.mySource.CopyFrom(defaultTemplate3D);
            holder.mySource.playOnAwake = false;
			holder.mySource.loop = false;
			holder.mySource.clip = audioClip;
			holder.mySource.volume = volume;
			holder.mySource.pitch = pitch;
			holder.mySource.spatialize = spatalize;
			holder.mySource.spatialBlend = 1;
			holder.mySource.minDistance = min;
			holder.mySource.maxDistance = max;
			holder.mySource.rolloffMode = AudioRolloffMode.Linear;
			if (mixer != null)
			{
				holder.mySource.outputAudioMixerGroup = mixer;
			}
			else
			{
				holder.mySource.outputAudioMixerGroup = defaultAudioMixerForPooled;
			}
			holder.mySource.Play();
			DebugRun(holder, whoDidit);
			holder.disableWhenDone = true;
		}

		public void PlaySFX2D(GameObject whoDidit, AudioClip myClip, float pitch = 1, float volume = 1, AudioMixerGroup mixer = null)
		{
			AudioPooler holder = AudioPool.Spawn().GetComponent<AudioPooler>();
			holder.mySource.loop = false;
			holder.mySource.clip = myClip;
			holder.mySource.volume = volume;
			holder.mySource.pitch = pitch;
			holder.mySource.spatialBlend = 0;
			if (mixer != null)
			{
				holder.mySource.outputAudioMixerGroup = mixer;
			}
			else
			{
				holder.mySource.outputAudioMixerGroup = defaultAudioMixerForPooled;
			}
			holder.mySource.Play();
			DebugRun(holder, whoDidit);
			holder.disableWhenDone = true;
		}

  //      /// <summary>
  //      /// Plays from Template but with an Audio/Volume Shift
  //      /// </summary>
  //      /// <param name="audioClip"></param>
  //      /// <param name="position"></param>
  //      /// <param name="audioSource"></param>
  //      /// <param name="pitch"></param>
        /// <param name="volume"></param>
        public void PlayAudio3DTemplate(GameObject whoDidit, AudioClip audioClip, Transform whereTo,bool follow, AudioSource audioSource, float pitch = 1, float volume = 1, AudioMixerGroup overrideMixerGroup = null)
		{
			AudioPooler holder = AudioPool.Spawn().GetComponent<AudioPooler>();
            holder.transform.position = whereTo.position;
            holder.transform.rotation = whereTo.rotation;
            if (follow)
            {
                holder.followTarget = whereTo;
            }
            else
            {
                holder.followTarget = null;
            }
			if(audioSource == null)
			{
                holder.mySource.CopyFrom(defaultTemplate3D);
            }
			else
			{
                holder.mySource.CopyFrom(audioSource);
            }
            holder.mySource.playOnAwake = false;
			holder.mySource.loop = false;
			holder.mySource.volume = volume;
            holder.mySource.pitch = pitch;
            holder.mySource.clip = audioClip;
            holder.sourceTemplate = audioSource;


			//This was considered too much just use a source template 
			//holder.mySource.bypassEffects = audioSource.bypassEffects;
			//holder.mySource.bypassListenerEffects = audioSource.bypassListenerEffects;
			//holder.mySource.bypassReverbZones = audioSource.bypassReverbZones;
			//holder.mySource.priority = audioSource.priority;

			//holder.mySource.panStereo = audioSource.panStereo;
			//holder.mySource.reverbZoneMix = audioSource.reverbZoneMix;
			//holder.mySource.dopplerLevel = audioSource.dopplerLevel;
			//holder.mySource.spread = audioSource.spread;
			//holder.mySource.rolloffMode = audioSource.rolloffMode;
			//holder.mySource.minDistance = audioSource.minDistance;
			//holder.mySource.maxDistance = audioSource.maxDistance;
			//holder.mySource.spatialBlend = audioSource.spatialBlend;

			//holder.mySource.spatialize = audioSource.spatialize;
			//holder.mySource.spatializePostEffects = audioSource.spatializePostEffects;


			if (overrideMixerGroup != null)
            {
                holder.mySource.outputAudioMixerGroup = overrideMixerGroup;
            }
            else
            {
                holder.mySource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            }
            holder.mySource.Play();
            holder.disableWhenDone = true;
            DebugRun(holder, whoDidit);
        }

		public void DebugRun(AudioPooler myComponent, GameObject caller)
		{
			if (showDebugs)
			{
				myComponent.name = myComponent.mySource.clip.name;
				myComponent.lastCaller = caller;
                lastPooled.Add(myComponent);
				if(lastPooled.Count >50)
				{
                    lastPooled.RemoveAt(0);
				}
                if (myComponent.mySource.outputAudioMixerGroup)
				{
                    Debug.LogError($"AUDIO  Caller: {caller.name} |  Clip: {myComponent.mySource.clip.name} | Mixer: {myComponent.mySource.outputAudioMixerGroup.name} ".ToColor(StringExtensions.ColorType.Orange), caller);
					if(myComponent.sourceTemplate)
					{
                        Debug.LogError($"AUDIO  Source Template: {caller.name} |  Clip: {myComponent.mySource.clip.name} | Mixer: {myComponent.mySource.outputAudioMixerGroup.name} | Template Name: {myComponent.sourceTemplate.name} ".ToColor(StringExtensions.ColorType.Orange), myComponent.sourceTemplate);
                    }
                }
				else
				{
                    Debug.LogError($"AUDIO  Caller: {caller.name} |  Clip: {myComponent.mySource.clip.name} | Mixer: NO MIXER ".ToColor(StringExtensions.ColorType.Orange), caller);
                    if (myComponent.sourceTemplate)
                    {
                        Debug.LogError($"AUDIO  Source Template: {caller.name} |  Clip: {myComponent.mySource.clip.name} | Mixer: NO MIXER | Template Name: {myComponent.sourceTemplate.name} ".ToColor(StringExtensions.ColorType.Orange));
                    }
                }

				//Debug.LogError($"--- AUDIO  Clip: {myComponent.mySource.clip.name} | Mixer: {myComponent.mySource.outputAudioMixerGroup.name} ", myComponent.gameObject);
			}
		}
	}

	public static class AudioPool
	{
		static bool inited;
		// The structure containing our inactive objects.
		// Why a Stack and not a List? Because we'll never need to
		// pluck an object from the start or middle of the array.
		// We'll always just grab the last one, which eliminates
		// any need to shuffle the objects around in memory.
		public static Stack<GameObject> inactive;

		// The prefab that we are pooling
		static GameObject prefab;

		// Constructor
		public static void Preload(GameObject startPrefab, int initialQty)
		{
			prefab = startPrefab;
			GameObject obj;
			inactive = new Stack<GameObject>(initialQty);
			for (int i = 0; i < initialQty; i++)
			{
				obj = (GameObject)GameObject.Instantiate(prefab, GroKitAudioManager.instance.transform);
				obj.SetActive(false);
				inactive.Push(obj);
			}
			inited = true;
		}

		// Spawn an object from our pool
		public static GameObject Spawn(Vector3 pos = default(Vector3))
		{
			if (!inited)
			{
				Debug.LogError("Pool was not started Missing UnityAudioManager");
				return null;
			}
			GameObject obj;
			if (inactive.Count == 0)
			{
				obj = (GameObject)GameObject.Instantiate(prefab);
				obj.name = prefab.name + "-OverPool";
			}
			else
			{
				obj = inactive.Pop();

				if (obj == null)
				{
					return Spawn(pos);
				}
			}

			obj.transform.position = pos;
			//obj.transform.rotation = rot;
			obj.SetActive(true);
			return obj;

		}

		// Return an object to the inactive pool.
		public static void Despawn(GameObject obj)
		{
			obj.SetActive(false);
			// Since Stack doesn't have a Capacity member, we can't control
			// the growth factor if it does have to expand an internal array.
			// On the other hand, it might simply be using a linked list 
			// internally.  But then, why does it allow us to specify a size
			// in the constructor? Maybe it's a placebo? Stack is weird.
			inactive.Push(obj);
		}
	}
}