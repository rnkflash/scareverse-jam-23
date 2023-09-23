using System;
using UnityEngine;

namespace _Content.Scripts.Objects
{
    [RequireComponent(typeof(AudioSource))]
    public class Drawer: MonoBehaviour
    {
        public enum DrawerDirection { Forward, Backward, Left, Right, Up, Down }
        [SerializeField] private DrawerDirection drawerOpenDirection = DrawerDirection.Forward;
        [SerializeField] private float drawerOpenSpeed = 3f;
        [SerializeField] private float drawerCloseSpeed = 3f;
        [SerializeField] private float slideDistance = 0.5f;
        [SerializeField] private Transform drawerTransform;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;

        private Vector3 closedPosition;
        private Vector3 openPosition;
        private bool isOpening;
        private bool isClosing;
        private bool open;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            closedPosition = drawerTransform.position;
            openPosition = closedPosition + GetDirection(drawerOpenDirection) * slideDistance;
        }

        public void Use()
        {
            if (isOpening || isClosing) return;

            if (open)
            {
                isClosing = true;
                audioSource.PlayOneShot(closeSound);
            }
            else
            {
                isOpening = true;
                audioSource.PlayOneShot(openSound);
            }

            open = !open;
        }
        
        private void Update()
        {
            if (drawerTransform == null) return;

            if (isOpening)
            {
                drawerTransform.position = Vector3.MoveTowards(drawerTransform.position, openPosition, Time.deltaTime * drawerOpenSpeed);
                if (Vector3.Distance(drawerTransform.position, openPosition) < 0.01f)
                {
                    drawerTransform.position = openPosition;
                    isOpening = false;
                }
            }
            else if (isClosing)
            {
                drawerTransform.position = Vector3.MoveTowards(drawerTransform.position, closedPosition, Time.deltaTime * drawerCloseSpeed);
                if (Vector3.Distance(drawerTransform.position, closedPosition) < 0.01f)
                {
                    drawerTransform.position = closedPosition;
                    isClosing = false;
                }
            }
        }
        
        private Vector3 GetDirection(DrawerDirection dir)
        {
            switch (dir)
            {
                case DrawerDirection.Forward: return drawerTransform.forward;
                case DrawerDirection.Backward: return -drawerTransform.forward;
                case DrawerDirection.Left: return -drawerTransform.right;
                case DrawerDirection.Right: return drawerTransform.right;
                case DrawerDirection.Up: return drawerTransform.up;
                case DrawerDirection.Down: return -drawerTransform.up;
                default: return Vector3.forward;
            }
        }
        
    }
}