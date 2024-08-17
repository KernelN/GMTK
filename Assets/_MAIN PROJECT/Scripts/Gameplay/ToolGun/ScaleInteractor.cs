using System;
using PlanetMover.Gameplay.Objects;
using PlanetMover.Gameplay.Player.Interaction;
using UnityEngine;

namespace TimeDistortion.Gameplay.Player.Interaction
{
    public class ScaleInteractor : InteractionMode
    {
        [Header("Set Values")]
        [SerializeField, Min(.01f)] float strengthScalation;
        [SerializeField, Min(.01f)] float baseStrength = 1f;
        [Header("Runtime Values")]
        [SerializeField] ObjectScale target;
        [SerializeField] float currentStrength = 1f;
        
        //Unity Events

        //Methods
        public override void SetTarget(Transform target)
        {
            if(this.target && target == this.target.transform) return;
            
            if (target == null)
            {
                ReleaseInteract();
                return;
            }
            if (!target.TryGetComponent<ObjectScale>(out var t))
            {
                ReleaseInteract();
                return;
            }
            
            this.target = t;
        }
        public override void Interact(bool reverse = false)
        {
            if(!target) return;
            
            float dt = Time.deltaTime;
            target.Scale((reverse ? -currentStrength : currentStrength) * dt);
            currentStrength += currentStrength * strengthScalation * dt;
        }
        public override void ReleaseInteract()
        {                 
            target = null;

            if(currentStrength > baseStrength)
                currentStrength = baseStrength;
        }
    }
}
