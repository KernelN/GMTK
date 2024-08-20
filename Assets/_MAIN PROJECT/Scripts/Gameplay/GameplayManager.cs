using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Universal.Singletons;

namespace PlanetMover.Gameplay
{
    public class GameplayManager : MonoBehaviourSingletonInScene<GameplayManager>
    {
        [Header("Set Values")]
        [SerializeField] List<Objects.Scalable> objectsToStore;
        [SerializeField] GridKeeper freightBox;
        [SerializeField] GameObject pauseMenu;
        [SerializeField] int victorySceneIndex = 1;
        //[Header("Runtime Values")]
        

        public CustomInputActions inputActions;

        //Unity Events
        void Start()
        {
            //Lock & hide mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
         
            //Set inputs
            inputActions = new CustomInputActions();
            inputActions.Player.Pause.performed += context =>
            {
                if(Time.timeScale == 0) Resume();
                else Pause();
            };
            inputActions.Enable();

            freightBox.OnObjectStored += () =>
            {
                if (objectsToStore.Count == 0) return;

                if(freightBox.StoredObjectCount >= objectsToStore.Count)
                    SceneManager.LoadScene(victorySceneIndex);
            };
        }

        //Methods
        void Pause(bool useUI = true)
        {
            Time.timeScale = 0;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if(useUI) pauseMenu.SetActive(true);
        }
        void Resume(bool useUI = true)
        {
            Time.timeScale = 1;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if(useUI) pauseMenu.SetActive(false);
        }
    }
}
