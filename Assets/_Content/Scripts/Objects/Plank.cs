using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Content.Scripts.Objects
{
    [RequireComponent(typeof(AudioSource))]
    public class Plank : MonoBehaviour
    {
        [SerializeField] private AudioClip[] removeSounds;
        [SerializeField] private GameObject model;

        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Use()
        {
            PlaySound();
            model.SetActive(false);
        }

        public void Appear()
        {
            if (model.activeSelf) return;
            PlaySound();
            model.SetActive(true);
        }

        private void PlaySound()
        {
            if (removeSounds.Length > 0)
                audioSource.PlayOneShot(removeSounds[Random.Range(0, removeSounds.Length)]);
        }
    }
}
