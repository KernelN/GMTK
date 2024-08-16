using System;
using UnityEngine;

namespace PlanetMover.Objects
{
    [RequireComponent(typeof(Collider))]
    public class ObjectScaler : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Collider collider;
        [SerializeField, Min(0)] float minScale;
        [SerializeField, Min(0)] float maxScale;
        //[Header("Runtime Values")]
        Vector3 normalizedSize;
        
        public float Scale { get; private set; }

        //Unity Events
        void Awake()
        {
            if(!collider)
                collider = GetComponent<Collider>();
            
            GetScale();
        }

        //Methods
        public void GetScale()
        {
            if(!collider)
                collider = GetComponent<Collider>();

            // Vector3 size = collider.bounds.size;
            // Scale = (size.x + size.y + size.z) / 3f;
            Scale = collider.bounds.size.magnitude;
            normalizedSize = collider.bounds.size.normalized;
        }
        public void SetScale(float scale)
        {
            if(scale < minScale || scale > maxScale) return;
            transform.localScale = normalizedSize * scale;
        }
    }
}