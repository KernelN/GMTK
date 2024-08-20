using UnityEngine;

namespace PlanetMover.Gameplay.Props.Circuit
{
    public class DoorCircuitTarget : MonoBehaviour
    {
        enum Axis { X = 0, Y = 1, Z = 2 }
        
        [Header("Set Values")]
        [SerializeField] CircuitManager manager;
        [SerializeField] BoxCollider coll;
        [SerializeField] Vector3 openedPos;
        [SerializeField] Vector3 closedPos;
        [SerializeField] Axis axis = Axis.Y;
        [SerializeField] bool openToPositive = true;
        [SerializeField] float speed;
        [SerializeField] bool openOnComplete = true;
        [Header("Runtime Values")]
        [SerializeField] Vector3 targetPos;
        Vector3 startPos;
        float timeToTarget;
        float timer;
        bool onPos;
        bool isBeingForced;
        [Header("DEBUG")]
        [SerializeField] bool setPositions;

        public System.Action DoorsClosed;

        //Unity Events
        private void Awake()
        {
            manager.CircuitCompleted += OnCircuitCompleted;
            manager.ForcedActive += OnCircuitCompleted;
            
            //Init with all doors closed
            targetPos = openOnComplete ? closedPos : openedPos;
            transform.localPosition = targetPos;
            onPos = true;
        }
        void Update()
        {
            if(onPos) return;
            
            timer += Time.deltaTime;
            if (timer >= timeToTarget) onPos = true;
            
            Move();
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Vector3 parentPos = transform.parent.position;
            
            //Draw closed door
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(parentPos + closedPos, coll.bounds.size);

            //Draw opened door
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(parentPos + openedPos, coll.bounds.size);

            if(!setPositions) return;
            setPositions = false;
            SetPositions();
        }
#endif

        //Methods
        public void ForceOpen()
        {
            OnCircuitCompleted(openOnComplete); //Open door
            isBeingForced = true; //Set as forced
        }
        void SetPositions()
        {
            closedPos = transform.localPosition;
            
            int sign = openToPositive ? 1 : -1;
            switch (axis)
            {
                case Axis.X:
                    //openedPos = closedPos + coll.bounds.size.x * Vector3.right * sign;
                    openedPos = closedPos + coll.size.x * Vector3.right * sign;
                    break;
                case Axis.Y:
                    //openedPos = closedPos + coll.bounds.size.y * Vector3.up * sign;
                    openedPos = closedPos + coll.size.y * Vector3.up * sign;
                    break;
                case Axis.Z:
                    //openedPos = closedPos + coll.bounds.size.z * Vector3.forward * sign;
                    openedPos = closedPos + coll.size.z * Vector3.forward * sign;
                    break;
            }
        }
        void Move()
        {
            if(onPos)
            {
                transform.localPosition = targetPos;
                
                if(targetPos == closedPos)
                    DoorsClosed?.Invoke();

                if (isBeingForced)
                {
                    isBeingForced = false;
                    
                    OnCircuitCompleted(manager.IsComplete);
                }
                return;
            }
            
            float t = timer / timeToTarget;
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
        }

        //Event Receiver
        void OnCircuitCompleted(bool isCircuitComplete)
        {
            if(isBeingForced) return; //only check complete if not forced
            
            if (isCircuitComplete)
            {
                targetPos = openOnComplete ? openedPos : closedPos;
            }
            else
            {
                targetPos = openOnComplete ? closedPos : openedPos;
            }

            startPos = transform.localPosition;
            timeToTarget = Vector3.Distance(startPos, targetPos) / speed;
            timer = 0;
            onPos = false;
        }
    }
}