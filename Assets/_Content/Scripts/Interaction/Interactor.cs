using System;
using _Content.Scripts.Input;
using _Content.Scripts.UI;
using UnityEngine;

namespace _Content.Scripts.Interaction
{
    public class Interactor: MonoBehaviour
    {
        [SerializeField] private float rayDistance = 2;
        [SerializeField] private Camera camera;
        [SerializeField] private CrossHair crossHair;

        private InteractableObject currentInteractableObject;
        private InputManager input;

        private void Start()
        {
            input = GetComponent<InputManager>();
        }

        public Camera GetCamera()
        {
            return camera;
        }

        void Update()
        {
            if (Physics.Raycast(camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), camera.transform.forward, out RaycastHit hit, rayDistance))
            {
                Debug.DrawRay(camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), camera.transform.forward, Color.green);
                var interactableObject = hit.collider.GetComponent<InteractableObject>();;
                if (interactableObject != null)
                {
                    currentInteractableObject = interactableObject;
                    HighlightCrosshair(true);
                }
                else
                    ClearItem();
            }
            else
                ClearItem();
            
            if (input.IsUseTriggered())
                currentInteractableObject?.Interact(this);
        }
        
        private void ClearItem()
        {
            currentInteractableObject = null;    
            HighlightCrosshair(false);
        }

        void HighlightCrosshair(bool on)
        {
            crossHair?.TurnRed(on);
        }
        
    }
}