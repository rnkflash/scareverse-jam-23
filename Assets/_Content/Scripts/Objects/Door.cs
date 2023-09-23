using System;
using UnityEngine;

namespace _Content.Scripts.Objects
{
    [RequireComponent(typeof(AudioSource))]
    public class Door: MonoBehaviour
    {
        [SerializeField] private bool doorOpenRight = true;
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float openSpeed = 3f;
        [SerializeField] private float closeSpeed = 3f;
        [SerializeField] private Transform doorTransform;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;
        
        private Quaternion closedRotation;
        private Quaternion openRotation;
        private bool isOpening;
        private bool isClosing;
        private bool doorOpen;
        private AudioSource audioSource;
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            closedRotation = doorTransform.rotation;
            openRotation = Quaternion.Euler(doorTransform.eulerAngles + new Vector3(0, doorOpenRight ? -openAngle : openAngle, 0));
        }

        private void Update()
        {
            if (doorTransform == null) return;
            
            if (isOpening)
            {
                doorTransform.rotation = Quaternion.Slerp(doorTransform.rotation, openRotation, Time.deltaTime * openSpeed);
                if (Quaternion.Angle(doorTransform.rotation, openRotation) < 0.1f)
                {
                    doorTransform.rotation = openRotation;
                    isOpening = false;
                }
            }
            else if (isClosing)
            {
                doorTransform.rotation = Quaternion.Slerp(doorTransform.rotation, closedRotation, Time.deltaTime * closeSpeed);
                if (Quaternion.Angle(doorTransform.rotation, closedRotation) < 0.1f)
                {
                    doorTransform.rotation = closedRotation;
                    isClosing = false;
                }
            }
        }

        public void Use()
        {
            //if (isOpening || isClosing) return;

            if (doorOpen)
            {
                isClosing = true;
                isOpening = false;
                audioSource.PlayOneShot(closeSound);
            }
            else
            {
                isClosing = false;
                isOpening = true;
                audioSource.PlayOneShot(openSound);
            }

            doorOpen = !doorOpen;
        }
    }
}