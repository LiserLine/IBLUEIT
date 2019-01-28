using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ibit.WindGame
{
    public class RestartLevel : MonoBehaviour
    {
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
