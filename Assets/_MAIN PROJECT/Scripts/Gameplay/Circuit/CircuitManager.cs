using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetMover.Gameplay.Props.Circuit
{
    public class CircuitManager : MonoBehaviour
    {
        [Serializable] enum ActivationModes { AllActive, AnyActive }

        [Header("Set Values")]
        [SerializeField] List<CircuitPartController> circuitParts;
        [SerializeField] ActivationModes mode;
        [Tooltip("If time <= 0, it'll never be locked active")] 
        [SerializeField] float timeToLockActive = -1;
        [SerializeField] bool canDeactivate = true;
        [Header("Runtime Values")]
        [SerializeField] List<CircuitPartController> activeCircuitParts;
        [SerializeField] float lockActiveTimer;
        [SerializeField] bool isComplete;
        [SerializeField] bool activeLocked;
        bool isDisabled;

        public Action<CircuitManager> CircuitLocked;
        public Action<float> CircuitPerUpdated;
        public Action<bool> CircuitCompleted;
        public Action<bool> ForcedActive;
        
        public float Percent { get; private set; }
        
        public bool IsComplete => isComplete;
        public bool canBeLocked => timeToLockActive >= 0;

        //Unity Events
        private void Awake()
        {
            for (int i = 0; i < circuitParts.Count; i++)
                circuitParts[i].Activated += OnCircuitPartActivated;
        }
        void Update()
        {
            if(isDisabled) return;
            if (ShouldLockActive())
                LockActivation();
        }

        //Methods
        public void ForceActive()
        {
            ForceActive(false);
        }
        public void ForceActive(bool forceLocked, bool canLock = true)
        {
            isComplete = true;
            
            if (forceLocked)
            {
                activeLocked = true;
                CircuitLocked?.Invoke(this);
            }
            
            ForcedActive?.Invoke(isComplete);

            if (!canLock)
                activeLocked = true; //it's set as locked, but no one knows about it (no event called)
        }
        public void ForcePercent(float percent)
        {
            if(percent > 1) percent = 1;
            
            Percent = percent;
            
            if(percent < 1) return;
            if(isComplete) return;
            
            isComplete = true;
            CircuitCompleted?.Invoke(isComplete);
        }
        /// <summary>
        /// Forces all the circuits to be checked
        /// </summary>
        /// <param name="sendEvent">call the CompleteActions on state change?</param>
        public void ForceCheck(bool sendEvent = true)
        {
            for (int i = 0; i < circuitParts.Count; i++)
                CheckPartActivation(circuitParts[i], sendEvent);
        }
        public void Deactivate()
        {
            isComplete = false;
            CircuitCompleted?.Invoke(isComplete);
        }
        public void Enable()
        {
            isDisabled = false;
        }
        public void Disable()
        {
            isDisabled = true;
        }
        bool ShouldLockActive()
        {
            if (!canBeLocked) return false; //It can't be locked active, so exit
            if (!isComplete)
            {
                lockActiveTimer = 0;
                return false; 
            } //It's not active yet, so reset timer & exit
            if(activeLocked) return false; //It's already locked, so exit

            lockActiveTimer += Time.deltaTime;
            return lockActiveTimer >= timeToLockActive;
        }
        void LockActivation()
        {
            activeLocked = true;
            CircuitLocked?.Invoke(this);
        }
        void CheckPartActivation(CircuitPartController part, bool sendEvent = true)
        {
            if(isDisabled) return;
            if(activeLocked) return;
            if(isComplete && !canDeactivate) return;

            //If list update failed, exit
            if(!UpdatePartList(part)) return;
            
            bool wasComplete = isComplete;
            switch (mode)
            {
                case ActivationModes.AllActive:
                    isComplete = activeCircuitParts.Count >= circuitParts.Count;
                    Percent = activeCircuitParts.Count / (float)circuitParts.Count;
                    break;
                case ActivationModes.AnyActive:
                    isComplete = activeCircuitParts.Count > 0;
                    Percent = isComplete ? 1 : 0;
                    break;
            }
            
            if(!sendEvent) return;
            
            CircuitPerUpdated?.Invoke(Percent);

            if (wasComplete == isComplete) return;
            CircuitCompleted?.Invoke(isComplete);
        }
        bool UpdatePartList(CircuitPartController part)
        {
            bool partWasInList = activeCircuitParts.Contains(part);
                    
            //If part is active, but it was already in list, exit
            if (part.active)
            {
                if (partWasInList) return false;
                activeCircuitParts.Add(part);
            }
            else if (partWasInList)
            {
                activeCircuitParts.Remove(part);
            }

            return true;
        }

        //Event Receivers
        void OnCircuitPartActivated(CircuitPartController part)
        {
            if(!enabled) return;
            
            CheckPartActivation(part);
        }
    }
}