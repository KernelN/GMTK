using System;
using UnityEngine;

namespace PlanetMover.Gameplay.Objects
{
    [RequireComponent(typeof(Collider))]
    public class ObjectScale : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Collider collider;
        [SerializeField, Min(0)] float minScale;
        [SerializeField, Min(0)] float maxScale;
        //[Header("Runtime Values")]
        float proportion;

        public float CurrentScale { get; private set; } = -1;
        public float MinScale => minScale;
        public float MaxScale => maxScale;
        
        const float _v3Magnitude = 1.73205081f;

        //Unity Events
        void Awake()
        {
            if(!collider)
                collider = GetComponent<Collider>();
            
            GetScale();
        }
        void OnEnable()
        {
            GetScale();
        }

        //Methods
        public void GetScale()
        {
            if(!collider)
                collider = GetComponent<Collider>();

            CurrentScale = collider.bounds.size.magnitude / _v3Magnitude;
            proportion = (transform.localScale.magnitude/_v3Magnitude) / CurrentScale;
        }
        public void SetScale(float scale)
        {
            scale = Mathf.Clamp(scale, minScale, maxScale);
            Vector3 normalizedSize = transform.localScale.normalized;
            transform.localScale = normalizedSize * _v3Magnitude * (proportion * scale);
            CurrentScale = scale;
        }
        public void Scale(float scale)
        {
            float temp = CurrentScale + scale;
            
            temp = Mathf.Clamp(temp, minScale, maxScale);
            
            Vector3 normalizedSize = transform.localScale.normalized;
            transform.localScale = normalizedSize * _v3Magnitude * (proportion * temp);
            CurrentScale = temp;
        }
    }
}