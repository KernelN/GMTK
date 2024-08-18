using System;
using UnityEngine;

namespace PlanetMover.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movable : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Rigidbody rb;
        [SerializeField, Min(0)] float maxSpeed;
        [SerializeField, Min(0)] float minSpeed;
        [Header("Runtime Values")]
        [SerializeField] Transform target;
        
        float SqrMaxSpeed => maxSpeed * maxSpeed;
        float SqrMinSpeed => minSpeed * minSpeed;
        
        //Unity Events
        void FixedUpdate()
        {
            if(!target) return;
            Vector3 vel = target.position - transform.position;
            if (vel.sqrMagnitude > SqrMaxSpeed)
                vel = vel.normalized * maxSpeed;
            else if (vel.sqrMagnitude < SqrMinSpeed)
                vel = vel.normalized * minSpeed;
            rb.velocity = vel;
        }
        
        //Methods
        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}