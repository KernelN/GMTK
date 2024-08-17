using UnityEngine;

namespace Universal.Singletons
{
    public class MonoBehaviourSingletonInScene<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        public static T Get()
        {
            return instance;
        }

        internal virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}