using UnityEngine;
using UnityEngine.Events;

namespace _Content.Scripts.Interaction
{
    public class InteractableObject: MonoBehaviour
    {
        [SerializeField] private UnityEvent<Interactor> onInteract;

        public void Interact(Interactor interactor)
        {
            onInteract?.Invoke(interactor);
        }
    }
}