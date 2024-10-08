using System;
using UnityEngine;

namespace PlanetMover.Gameplay.Player.Interaction
{
    public class MoveInteractor : InteractionMode
    {
        [Header("Set Values")]
        [SerializeField] Transform point;
        [SerializeField, Min(0)] float maxMoveSpeed;
        //[Header("Runtime Values")]
        Movable target;
        bool isGrabbingTarget;

        //Methods
        public override void SetTarget(Transform target)
        {
            if (isGrabbingTarget) return;
            if(this.target && target == this.target.transform) return;

            if (target == null)
            {
                this.target = null;
                return;
            }
            if (!target.TryGetComponent<Movable>(out var t))
            {
                this.target = null;
                return;
            }
            
            this.target = t;
        }
        public override void Interact(bool reverse = false)
        {
            if(!target) return;
            
            //If is grabbing and action is grab, do nothing
            if(isGrabbingTarget != reverse) return;

            isGrabbingTarget = !reverse;
            
            //Grab target
            if (isGrabbingTarget)
            {
                point.position = target.transform.position;
                target.SetTarget(point, maxMoveSpeed);
                target.targetAcquired += b => isOn?.Invoke(b);
            }
            //Drop target
            else
            {
                target.targetAcquired -= b => isOn?.Invoke(b);
                target.RemoveTarget();
            }
            
            isOn?.Invoke(isGrabbingTarget);
        }
        public override void ReleaseInteract() { }
    }
}
