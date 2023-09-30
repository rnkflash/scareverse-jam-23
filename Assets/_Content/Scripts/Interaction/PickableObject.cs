using System;
using _Content.Scripts.Managers;
using _Content.Scripts.ScriptableObjects;
using DG.Tweening;
using UnityEngine;

namespace _Content.Scripts.Interaction
{
    public class PickableObject : MonoBehaviour
    {
        public Item item;

        public void PickUp(Interactor interactor)
        {
            if (interactor == null) return;
            
            var collider = GetComponent<Collider>();
            if (collider != null)
                collider.enabled = false;

            var rigidBody = GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.useGravity = false;
            }

            transform.SetParent(interactor.transform);
            var camera = interactor.GetCamera();
            var localPoint = interactor.transform.InverseTransformPoint(camera.transform.position);
            
            transform.DOLocalMove(localPoint + Vector3.down * 0.7f, 0.25f).OnComplete(() =>
            {
                Destroy(gameObject);    
            });

            if (item != null)
            {
                SoundManager.Instance.PlaySound(item.pickUpSound);
            }
        }
    }
}
