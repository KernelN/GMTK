using System;
using UnityEngine;

namespace PlanetMover.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movable : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Rigidbody rb;
        [SerializeField, Min(0)] float speedMod = 1f;
        [Header("Runtime Values")]
        [SerializeField] Transform target;
        bool wasKinematic;
        float maxSpeed;
        
        float SqrMaxSpeed => maxSpeed * maxSpeed;
        
        //Unity Events
        void Start()
        {
            if(!rb) rb = GetComponent<Rigidbody>();
        }
        void FixedUpdate()
        {
            if(!target) return;
            
            Vector3 vel = target.position - transform.position;
            if (vel.sqrMagnitude > SqrMaxSpeed)
                vel = vel.normalized * maxSpeed;
            
            rb.velocity = vel * speedMod;
        }
        
        //Methods
        public void SetTarget(Transform target, float maxSpeed)
        {
            this.target = target;
            this.maxSpeed = maxSpeed;
            
            wasKinematic = rb.isKinematic;
            rb.isKinematic = false;
            rb.useGravity = false;
        }
        public void RemoveTarget()
        {
            target = null;

            rb.useGravity = true;
            rb.isKinematic = wasKinematic;
        }
    }
}