using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlanetMover.UI
{
    public class SceneChanger : MonoBehaviour
    {
        public void ChangeScene(int sceneIndex)
        {
            ResumeGame();
            SceneManager.LoadScene(sceneIndex);
        }
        public void ExitGame()
        {
            Application.Quit();
        }
        public void PauseGame()
        {
            Time.timeScale = 0;
        }
        public void ResumeGame()
        {
            Time.timeScale = 1;
        }
    }
}
