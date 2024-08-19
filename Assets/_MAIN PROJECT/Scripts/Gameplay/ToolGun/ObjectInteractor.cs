using UnityEngine;
using UnityEngine.VFX;

namespace PlanetMover.Gameplay.Player.Interaction
{
    public abstract class InteractionMode : MonoBehaviour
    {
        public System.Action<bool> isOn;
        
        public abstract void SetTarget(Transform target);
        public abstract void Interact(bool reverse = false);
        public abstract void ReleaseInteract();
    }
    
    public class ObjectInteractor : MonoBehaviour
    {
        [Header("Shoot Values")]
        [SerializeField, Min(0.1f)] float range;
        [SerializeField] LayerMask interactableLayer;
        
        [Space] //Modes
        [SerializeField] InteractionMode[] modes;

        [Header("VFX")]
        [SerializeField] VisualEffect ray;
        [SerializeField] float rayOnDelay;
        [SerializeField] ParticleSystem rayCharge;
        [SerializeField] float chargeOffDelay;
        [SerializeField] ParticleSystem rayHit;
        
        [Header("Runtime Values")]
        [SerializeField] InteractionMode currentMode;
        int modeIndex;
        Transform forwardRef;
        bool isFiring = false;
        bool wasFiring = false;
        bool isFiringReverseInteraction = false;
        bool modeIsOn = false;
        
        [Header("DEBUG")]
        [SerializeField] bool drawGizmos;

        //Unity Events
        void Start()
        {
            currentMode = modes[0];

            for (int i = 0; i < modes.Length; i++)
                modes[i].isOn += b =>
                {
                    modeIsOn = b;
                    
                    if (b) StartVFX();
                    else if(!isFiring) StopVFX();
                };
            
            //Set inputs
            CustomInputActions inputs = GameplayManager.Get().inputActions;
            inputs.Player.Fire.performed += context => StartRay(false);
            inputs.Player.Fire.canceled += context => StopRay();
            inputs.Player.ReverseFire.performed += context => StartRay(true);
            inputs.Player.ReverseFire.canceled += context => StopRay();
            inputs.Player.SwapWeapon.performed += context =>
            {
                modeIndex++;
                if(modeIndex >= modes.Length)
                    modeIndex = 0;
                currentMode = modes[modeIndex];
            };
            
            forwardRef = Camera.main.transform;
        }
        void LateUpdate()
        {
            if(!currentMode) return;
            
            if(modeIsOn || isFiring || wasFiring)
                UpdateVFXRay();
            
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
            if (!Physics.Raycast(ray, out var hit, range, interactableLayer))
                return null;
            
            return hit.transform;
        }
        void UpdateVFXRay()
        {
            if(!forwardRef) return;
            Ray ray = new Ray(transform.position, forwardRef.forward);
            if (Physics.Raycast(ray, out var hit, range))
            {
                rayHit.transform.position = hit.point;
                
                if(rayHit.isStopped)
                    rayHit.Play();
            }
            else
            {
                rayHit.transform.position = ray.origin + ray.direction * range;
                
                if(rayHit.isPlaying)
                    rayHit.Stop();
            }
        }
        void StartRay(bool reverse)
        {
            isFiringReverseInteraction = reverse;
            isFiring = true;
            
            StartVFX();
        }
        void StopRay()
        {
            isFiring = false;
            
            if(!modeIsOn)
                StopVFX();
        }
        void StartVFX()
        {
            rayCharge.Play();
            ray.Play();
        }
        void StopVFX()
        {
            ray.Stop();
            rayHit.Stop();
            Universal.LambdaInvoker.Invoke(this, 
                () => { rayCharge.Stop(); }, chargeOffDelay);
        }
    }
}
