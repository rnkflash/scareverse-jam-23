using UnityEngine;
using UnityEngine.Events;

namespace _Content.Scripts.Interaction
{
    public class InteractableObject: MonoBehaviour
    {
        [SerializeField] private UnityEvent onInteract;

        public void Interact()
        {
            onInteract?.Invoke();
        }
    }
}