using UnityEngine;
using Universal.Singletons;

namespace PlanetMover
{
    public class SuperMusicPlayer : MonoBehaviourSingleton<SuperMusicPlayer>
    {
        //[Header("Set Values")]
        [SerializeField] AudioSource audioSource;
        //[Header("Runtime Values")]

        //Unity Events
        void Start()
        {
            audioSource.Play();
        }

        //Methods
    }
}
