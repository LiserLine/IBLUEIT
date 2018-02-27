using Ibit.Core.Audio;
using UnityEngine;

namespace Ibit.Core.Game
{
    public partial class GameManager
    {
        public delegate void GamePauseHandler();

        public delegate void GameUnPauseHandler();

        public static event GamePauseHandler OnGamePause;

        public static event GameUnPauseHandler OnGameResume;

        public static bool GameIsPaused { get; private set; }

        public static void PauseGame()
        {
            if (GameIsPaused)
                return;

            SoundManager.Instance?.PlaySound("GamePause");

            Time.timeScale = 0f;
            GameIsPaused = true;
            OnGamePause?.Invoke();
        }

        public static void ResumeGame()
        {
            if (!GameIsPaused)
                return;

            SoundManager.Instance.PlaySound("GameUnPause");

            Time.timeScale = 1f;
            GameIsPaused = false;
            OnGameResume?.Invoke();
        }
    }
}