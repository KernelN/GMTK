using UnityEngine;
using UnityEngine.Serialization;

namespace PlanetMover.Gameplay.Props.Circuit
{
    public class CircuitPartModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitPartController controller;
        [SerializeField] float activationDelay;
        [SerializeField] Animator animator;
        [FormerlySerializedAs("animationParam")] [SerializeField] string activateAnimaParam;
        [Tooltip("If false, will use parameter as boolean")]
        [SerializeField] bool isTrigger = true;
        [SerializeField] bool activateTriggerOnlyOnComplete;
        [Tooltip("If true, will send percentage animation float (0-1). " +
                 "\n Take note, not all circuit parts support this " +
                 "(if this one doesn't, it will jump from 0 to 1 as it activates)")]
        [SerializeField] bool usePercent = false;
        [SerializeField] string percentAnimParam;
        [Header("OPTIONALS")]
        [SerializeField, Min(0)] float minActivationTime;
        [Header("DEPRECATED")]
        [SerializeField] GameObject VFX;
        //[Header("Runtime Values")]
        float lastActivationPercent = 0;
        float timer = 0;
        float activationTimer = 0;

        //Unity Events
        private void Start()
        {
            if (controller == null)
            {
                controller = GetComponent<CircuitPartController>();
            }
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            controller.Activated += OnControllerActivated;
        }
        private void Update()
        {
            float dt = Time.deltaTime;
            
            if (activationTimer > 0)
            {
                activationTimer -= dt;
                
                if (activationTimer <= 0)
                    TryActivate();
                
                return;
            }
            
            if (timer > 0)
            {
                timer -= dt;
                if (timer <= 0)
                    Activate();
            }
        }
        void LateUpdate()
        {
            if (!usePercent) return;
            if(Mathf.Approximately(lastActivationPercent, controller.percentage))
                return;
            
            lastActivationPercent = controller.percentage;
            animator.SetFloat(percentAnimParam, controller.percentage);
        }
        
        //Methods
        void TryActivate()
        {
            if (activationDelay > 0)
            {
                timer = activationDelay;
            }
            else
            {
                Activate();
            }
        }
        void Activate()
        {
            if (animator)
            {
                if (isTrigger)
                {
                    if(!activateTriggerOnlyOnComplete || controller.isActive)
                        animator.SetTrigger(activateAnimaParam);
                }
                else
                    animator.SetBool(activateAnimaParam, controller.active);

                if (usePercent)
                    animator.SetFloat(percentAnimParam, controller.active ? 1 : 0);
            }

            if (VFX)
            {
                GameObject vfx = Instantiate(VFX);
                vfx.transform.position = transform.position;
            }
        }

        //Event Receivers
        void OnControllerActivated(CircuitPartController controller)
        {
            if (controller.active)
            {
                if (minActivationTime > 0)
                    activationTimer = minActivationTime;
                else 
                    TryActivate();
            }
            else
            {
                if (minActivationTime > 0)
                    activationTimer = 0;

                TryActivate();
            }
        }
    }
}