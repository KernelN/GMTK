using UnityEngine;

namespace PlanetMover.Gameplay.Props.Circuit
{
    public class CircuitPartController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] internal bool canDeactivate = true;
        [Header("Runtime Values")]
        [SerializeField] internal bool active;

        public System.Action<CircuitPartController> Activated;
        
        public float percentage { get; internal set; }
        public bool isActive => active;

        //Unity Events

        //Methods
        
        //Interface implementations
        public void ActivateRejuven(bool shouldActivate)
        {
            enabled = shouldActivate;
        }
        public void Deactivate()
        {
            active = false;
            Activated?.Invoke(this);
        }
        internal void TrySetActive(bool shouldActivate)
        {
            //If is active & can't deactivate, exit
            if(active && !canDeactivate) return;
            
            //If it's already in the state it should be, exit
            if(active == shouldActivate) return;
            
            //Set the state
            active = shouldActivate;
            Activated?.Invoke(this);
        }
    }
}