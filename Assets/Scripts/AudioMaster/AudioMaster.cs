using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AudioMaster : MonoBehaviour {

	static AudioMaster instance;

	Dictionary<AudioSource, Coroutine> playingAudios = new Dictionary<AudioSource, Coroutine>();

	public static AudioMaster Instance {
		get {
			if (instance == null) {
				instance = new GameObject("Audio Master").AddComponent<AudioMaster>();
			}
			return instance;
		}
	}

	public AudioSource Play(SoundHolder holder, float fadeInDuration = 0, Transform follow = null, bool stopOnFollowNull = true) {
		if  (holder == null) {
			Debug.Log("No holder");
			return null;
		}
		AudioClip clip = holder.clip;
		int random = -1;
		if (holder.additionalClips != null && holder.additionalClips.Length > 0) {
			random = Random.Range(-1, holder.additionalClips.Length);
			if (random >= 0)
				clip = holder.additionalClips[random];
		}

		if (clip == null) {
			if (random == -1)
				Debug.Log("No clip in main clip slot");
			else
				Debug.Log($"No clip in additional clip slot { random + 1 }");
			return null;
		}
		AudioSource audioSource = new GameObject("Sound - " + clip.name).AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.transform.SetParent(transform);
		audioSource.outputAudioMixerGroup = holder.mixerGroup;
		audioSource.loop = holder.loop;
		audioSource.volume = Random.Range(holder.minVolume, holder.maxVolume);
		audioSource.pitch = Random.Range(holder.minPitch, holder.maxPitch);
		audioSource.Play();
		playingAudios.Add(audioSource, StartCoroutine(SoundRoutine(audioSource, fadeInDuration, follow, stopOnFollowNull)));
		return audioSource;
	}

	public void Stop(AudioSource source) {
		_ = Stop(source, 0);
	}

	public async Task Stop(AudioSource audioSource, float duration) {
		if (!playingAudios.ContainsKey(audioSource)) return;
		float a = 1;
		float volume = audioSource.volume;
		while (a > 0) {
			a -= Time.deltaTime / duration;
			audioSource.volume = volume * a;
			await Awaiters.NextFrame;
		}
		audioSource.Stop();
		StopCoroutine(playingAudios[audioSource]);
		playingAudios.Remove(audioSource);
		Destroy(audioSource.gameObject);
	}

	IEnumerator SoundRoutine(AudioSource audioSource, float fadeInDuration, Transform target, bool stopOnFollowNull) {
		bool follow = target != null;
		float targetVolume = audioSource.volume;
		audioSource.volume = fadeInDuration > 0 ? 0 : targetVolume;
		while (audioSource.isPlaying) {
			if (follow) {
				if (target == null) {
					if (stopOnFollowNull) {
						Stop(audioSource);
						yield break;
					}
				}
				else 
				 audioSource.transform.position = target.position;
			}
			if (audioSource.volume < targetVolume) {
				audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, targetVolume/fadeInDuration * Time.deltaTime);
			}
			
			yield return null;

		}
		if (!playingAudios.ContainsKey(audioSource)) yield break;
		playingAudios.Remove(audioSource);
		Destroy(audioSource.gameObject);
	}
}
