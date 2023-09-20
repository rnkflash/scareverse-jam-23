using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Content.Scripts.UI
{
    public class StaminaBar : MonoBehaviour
    {
        [SerializeField] private float speed = 0.2f;
        [SerializeField] private float fadingDuration = 0.5f;
        [SerializeField] private Image fill;
        private float target = 1.0f;
        private float current;
        private CanvasGroup canvasGroup;

        enum State
        {
            Still,
            FadingIn,
            FadingOut
        }
        
        private State fadingState = State.Still;

        void Start()
        {
            current = target;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            DOTween.Kill(canvasGroup);
        }

        void Update()
        {
            if (current != target)
            {
                if (current > target)
                {
                    current -= speed * Time.deltaTime;
                    if (current <= target)
                        current = target;
                }
                else
                {
                    current += speed * Time.deltaTime;
                    if (current >= target)
                        current = target;
                }
                

                var temp = fill.transform.localScale;
                temp.x = current;
                fill.transform.localScale = temp;

                FadeInOut(State.FadingIn);
            }
            else
            {
                FadeInOut(State.FadingOut);
            }
        }

        public void SetTarget(float _target)
        {
            target = _target;
        }

        private void FadeInOut(State desiredState)
        {
            if (fadingState == desiredState) return;
            fadingState = desiredState;
            if (fadingState == State.Still) return;
            canvasGroup.DOFade(fadingState == State.FadingIn ? 1.0f : 0.0f, fadingDuration).OnComplete(() =>
            {
                fadingState = State.Still;
            });
        }
    }
}
