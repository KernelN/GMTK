using Cinemachine;
using UnityEngine;

namespace PlanetMover.Gameplay.Cam
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class CMPlayerGetter : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CinemachineVirtualCameraBase cam;
        //[Header("Runtime Values")]

        //Unity Events
        void Awake()
        {
            if(!cam)
                cam = GetComponent<CinemachineVirtualCameraBase>();
            
            if(cam.Follow) return; //camera already has a follow object

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (!player)
            {
                Debug.LogError("No player found!!!");
                return;
            }
            
            cam.Follow = player.transform;
        }

        //Methods
    }
}
