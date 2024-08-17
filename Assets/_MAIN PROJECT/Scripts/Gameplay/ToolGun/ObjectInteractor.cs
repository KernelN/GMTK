using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlanetMover.Gameplay.Player.Interaction
{
    public abstract class InteractionMode : MonoBehaviour
    {
        public abstract void SetTarget(Transform target);
        public abstract void Interact(bool reverse = false);
        public abstract void ReleaseInteract();
    }
    
    public class ObjectInteractor : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField, Min(0.1f)] float range;
        [SerializeField] LayerMask interactableLayer;
        [SerializeField] InteractionMode[] modes;
        [Header("Runtime Values")]
        [SerializeField] InteractionMode currentMode;
        Transform forwardRef;
        [Header("DEBUG")]
        [SerializeField] bool drawGizmos;
        bool isFiring = false;
        bool wasFiring = false;
        bool isFiringReverseInteraction = false;

        //Unity Events
        void Start()
        {
            currentMode = modes[0];
            
            CustomInputActions inputs = GameplayManager.Get().inputActions;
            inputs.Player.Fire.performed += context =>
            {
                isFiringReverseInteraction = false;
                isFiring = true;
            };
            inputs.Player.Fire.canceled += context =>
            {
                isFiringReverseInteraction = false;
                isFiring = false;
            };
            inputs.Player.ReverseFire.performed += context =>
            {
                isFiringReverseInteraction = true;
                isFiring = true;
            };
            inputs.Player.ReverseFire.canceled += context =>
            {
                isFiringReverseInteraction = false;
                isFiring = false;
            };
            
            
            forwardRef = Camera.main.transform;
        }
        void LateUpdate()
        {
            if(!currentMode) return;
            if (isFiring)
            {
                currentMode.SetTarget(GetTarget());
                currentMode.Interact(isFiringReverseInteraction);

                wasFiring = true;
            }
            else if(wasFiring)
            {
                currentMode.ReleaseInteract();

                wasFiring = false;
            }
        }

        void OnDrawGizmos()
        {
            if(!drawGizmos) return;
            Gizmos.color = Color.green;
            if(forwardRef)
                Gizmos.DrawRay(transform.position, forwardRef.forward * range);
            else
                Gizmos.DrawRay(transform.position, transform.forward * range);
        }

        //Methods
        Transform GetTarget()
        {
            if(!forwardRef) return null;
            Ray ray = new Ray(transform.position, forwardRef.forward);
            if(!Physics.Raycast(ray, out var hit, range, interactableLayer))
                return null;
            
            return hit.transform;
        }
    }
}
