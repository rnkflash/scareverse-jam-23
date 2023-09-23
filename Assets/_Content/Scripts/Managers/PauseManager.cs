using System;
using _Content.Scripts.Input;
using _Content.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Content.Scripts.Managers
{
    public class PauseManager: MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private InputManager inputManager;
        private bool isPaused;

        private void Start()
        {
            pauseMenuUI.SetActive(false);
        }

        private void Update()
        {
            if (inputManager.IsPauseTriggered())
            {
                Pause(!isPaused);
            }
        }

        private void Pause(bool on)
        {
            pauseMenuUI.SetActive(on);
            isPaused = on;
            inputManager.EnablePlayerInput(!on);
            inputManager.SetCursorState(!on);
            Time.timeScale = on ? 0.0f : 1.0f;
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!isPaused)
                inputManager.SetCursorState(true);
        }

        public void OnUIButton(PauseMenu.Button button)
        {
            switch (button)
            {
                case PauseMenu.Button.Resume:
                    Pause(false);
                    
                    break;
                case PauseMenu.Button.BackToMenu:
                    Pause(false);
                    SceneManager.LoadScene("menu");
                    break;
                case PauseMenu.Button.QuitGame:
                    Application.Quit();
                    break;
            }
        }
    }
}