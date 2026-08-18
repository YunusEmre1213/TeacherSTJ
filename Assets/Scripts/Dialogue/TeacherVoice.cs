using System;
using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.Dialogue
{

    [RequireComponent(typeof(AudioSource))]
    public class TeacherVoice : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

       
        public void Play(AudioClip clip, Action onFinished = null)
        {
            if (clip == null)
            {
                onFinished?.Invoke();
                return;
            }
            StartCoroutine(PlayRoutine(clip, onFinished));
        }

        private IEnumerator PlayRoutine(AudioClip clip, Action onFinished)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
            yield return new WaitForSeconds(clip.length);
            onFinished?.Invoke();
        }

        public IEnumerator PlayAndWaitRoutine(AudioClip clip)
        {
            if (clip == null) yield break;
            _audioSource.clip = clip;
            _audioSource.Play();
            yield return new WaitForSeconds(clip.length);
        }
    }
}