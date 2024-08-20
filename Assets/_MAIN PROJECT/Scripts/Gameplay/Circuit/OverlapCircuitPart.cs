using UnityEngine;

namespace PlanetMover.Gameplay.Props.Circuit
{
    public class OverlapCircuitPart : CircuitPartController
    {
        [Header("--CHILD--")]
        [Header("Set Values")]
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset;
        [SerializeField, Min(0)] Vector3 radius;
        [SerializeField] bool shouldOverlap = true;
        [Header("OPTIONALS")]
        [Tooltip("If null, will use target position instead of closest point in collider")]
        [SerializeField] Collider targetColl;
        [SerializeField, Min(0)] float minOverlapTime;
        [SerializeField] Quaternion rotation = Quaternion.identity;
        [SerializeField, Tooltip("Replaces rotation")] bool useTransformRot = false;
        [SerializeField] bool usePlayer = false;
        [Header("OPTIONALS - SPHERE")]
        [SerializeField] float sphereRadius;
        [SerializeField] bool sphereOverlap = false;
        [Header("Runtime Values")]
        [SerializeField] bool isOverlapping;
        [SerializeField] float timer;
        [Header("DEBUG")] 
        [SerializeField] bool useGizmos = true;
        [SerializeField] Color gizmoColor = Color.Lerp(Color.yellow, Color.clear, .9f);

        float sqrSphereRadius { get { return sphereRadius * sphereRadius; } }
        Vector3 checkPos { get { return transform.position + offset; } }

        //Unity Events
        void Start()
        {
            if (usePlayer)
            {
                target = GameObject.FindWithTag("Player").transform;
                targetColl = target.GetComponent<Collider>();
            }
        }
        void Update()
        {
            //Check if target is overlapping with object
            CheckOverlap();
            
            //Check if target SHOULD be overlapping with object
            if(isOverlapping != shouldOverlap) return;
            
            //If target is where it should, count down and activate at the end
            UpdateTimer();
        }
        void OnDrawGizmos()
        {
            if(!useGizmos) return;
            
            Gizmos.color = gizmoColor;

            Vector3 pos = transform.position + offset;
            if (sphereOverlap)
                Gizmos.DrawSphere(pos, sphereRadius);
            else
            {
                Quaternion rot = useTransformRot ? transform.rotation : rotation;
                if(rot == Quaternion.identity)
                    Gizmos.DrawCube(pos, radius * 2);
                else
                {
                    //Swap matrix
                    Gizmos.matrix = Matrix4x4.TRS(pos, rot, Vector3.one);
                    
                    Gizmos.DrawCube(Vector3.zero, radius * 2);
                    
                    //Restore matrix
                    Gizmos.matrix = Matrix4x4.identity;
                }
            }
        }

        //Methods
        void UpdateTimer()
        {
            if(timer < 0) return;
            
            timer -= Time.deltaTime;
            
            if(timer > 0) return;
            
            CheckActivation(true);
        }
        void CheckActivation(bool shouldActivate)
        {
            bool wasActivated = active;
            
            active = shouldActivate;
            
            if(wasActivated == active) return;

            Activated?.Invoke(this);
        }
        void CheckOverlap()
        {
            if(active && !canDeactivate) return;
            
            bool wasOverlapping = isOverlapping;
            
            //If collider is set, use closest point to check position, else use target position
            if(targetColl)
                isOverlapping = IsOverlapping(targetColl.ClosestPointOnBounds(checkPos));
            else 
                isOverlapping = IsOverlapping(target.position);

            if(wasOverlapping == isOverlapping) return;

            //If isn't in the right state, try to deactivate
            if (isOverlapping != shouldOverlap)
            {
                CheckActivation(false);
                return;
            }

            if(minOverlapTime > 0)
                timer = minOverlapTime;
            else
                CheckActivation(true);
        }
        bool IsOverlapping(Vector3 point)
        {
            if (sphereOverlap)
            {
                float sqrTargetDist = (point - checkPos).sqrMagnitude;

                //Is distance to target less than distance to bounds?
                return sqrTargetDist < sqrSphereRadius;
            }

            Vector3 dist = point - checkPos;
            Vector3 rad = (useTransformRot ? transform.rotation : rotation) * radius;

            if (Mathf.Abs(dist.x) > Mathf.Abs(rad.x)) return false; //if dist X > radius X, no overlap
            if (Mathf.Abs(dist.y) > Mathf.Abs(rad.y)) return false; //if dist Y > radius Y, no overlap

            return Mathf.Abs(dist.z) <= Mathf.Abs(rad.z); //if dist Z <= radius Z, is overlapping
        }
    }
}