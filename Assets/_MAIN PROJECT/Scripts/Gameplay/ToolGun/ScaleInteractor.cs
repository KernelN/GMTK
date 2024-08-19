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
        [SerializeField] Scalable target;
        [SerializeField] float currentStrength = 1f;
        bool isScaling;
        
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
            if (!target.TryGetComponent<Scalable>(out var t))
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
            
            if(isScaling) return;

            isScaling = true;
            isOn?.Invoke(true);
        }
        public override void ReleaseInteract()
        {                 
            target = null;

            if(currentStrength > baseStrength)
                currentStrength = baseStrength;
            
            if(!isScaling) return;
            
            isScaling = false;
            isOn?.Invoke(false);
        }
    }
}
