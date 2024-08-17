using UnityEngine;
using UnityEngine.InputSystem;
using Universal.Singletons;

namespace PlanetMover.Gameplay
{
    public class GameplayManager : MonoBehaviourSingletonInScene<GameplayManager>
    {
        //[Header("Set Values")]
        //[Header("Runtime Values")]

        public CustomInputActions inputActions;

        //Unity Events
        void Start()
        {
            //Lock & hide mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            inputActions = new CustomInputActions();
            inputActions.Enable();
        }

        //Methods
    }
}
