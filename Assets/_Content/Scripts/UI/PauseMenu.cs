using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _Content.Scripts.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public enum Button
        {
            Resume, BackToMenu, QuitGame
        }
        
        public UnityEvent<Button> onButtonPress;

        private void OnEnable()
        {
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
        }

        public void OnButtonPress(int buttonEnum)
        {
            onButtonPress?.Invoke((Button)buttonEnum);
        }
    }
}
