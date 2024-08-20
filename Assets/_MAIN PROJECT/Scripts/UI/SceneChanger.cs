using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlanetMover.UI
{
    public class SceneChanger : MonoBehaviour
    {
        public void ChangeScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
