using System.Collections;
using _Content.Scripts.Util;
using UnityEngine;
using UnityEngine.Audio;

namespace _Content.Scripts.Managers
{
    public class SoundManager: Singleton<SoundManager>
    {
        private AudioSource musicAudioSource;
        private AudioSource soundAudioSource;

        private IEnumerator musicCoroutine;

        protected override void Created()
        {
            var audioMixer = Resources.Load<AudioMixer>("AudioMixer");
            
            musicAudioSource = gameObject.AddComponent<AudioSource>();
            musicAudioSource.loop = true;
            musicAudioSource.playOnAwake = false;
            musicAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
            
            soundAudioSource = gameObject.AddComponent<AudioSource>();
            soundAudioSource.loop = false;
            soundAudioSource.playOnAwake = false;
            soundAudioSource.spatialBlend = 0f;
            soundAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        }
        
        public void PlaySound(AudioClip[] sound)
        {
            PlaySound(sound[Random.Range(0, sound.Length)]);
        }

        public void PlaySound(AudioClip sound)
        {
            soundAudioSource.PlayOneShot(sound);
        }

        public void PlayMusic(AudioClip music)
        {
	        musicAudioSource.clip = music;
	        musicAudioSource.loop = true;
	        musicAudioSource.Play();
        }

        public void ChangeTrack(AudioClip clip)
        {
	        if (clip == musicAudioSource.clip) return;
	        
	        if (musicCoroutine != null)
		        StopCoroutine(musicCoroutine);
	        
	        musicCoroutine = SwitchTrack(clip);
	        StartCoroutine(musicCoroutine);
        }

        public void StopMusic()
        {
	        if (musicCoroutine != null)
				StopCoroutine(musicCoroutine);
	        
	        musicCoroutine = SwitchTrack(null);
	        StartCoroutine(musicCoroutine);
        }

        private IEnumerator SwitchTrack(AudioClip nextTrack, float durationIn = .25f, float durationOut = .25f)
        {
	        var elapsedTime = 0f;
	        var fadeStartTime = durationIn;
	        if (fadeStartTime < 0f)
		        fadeStartTime = 0f;
	        while (elapsedTime < fadeStartTime)
	        {
		        elapsedTime += Time.deltaTime;
		        yield return null;
	        }

	        if (musicAudioSource.clip != null)
		        while (musicAudioSource.volume > 0f && elapsedTime < musicAudioSource.clip.length)
		        {
			        musicAudioSource.volume -= Time.deltaTime * 1f / durationIn;
			        yield return null;
		        }

	        if (nextTrack != null)
	        {
		        PlayMusic(nextTrack);
		        elapsedTime = 0f;
		        while (elapsedTime < durationOut)
		        {
			        elapsedTime += Time.deltaTime;
			        musicAudioSource.volume = Mathf.Lerp(0f, 1f, elapsedTime / durationOut);
			        yield return null;
		        }    
	        }

	        musicCoroutine = null;
        }

    }
}