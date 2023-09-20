using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Content.Scripts.Menu
{
    public class Menu : MonoBehaviour
    {
        public Image image;
        private bool pressAnyKey;

        private void Start()
        {
            var tempColor = image.color;
            tempColor.a = 1f;
            image.color = tempColor;
            StartCoroutine(WaitAndStart());
        }

        private IEnumerator WaitAndStart()
        {
            yield return new WaitForSeconds(1f);
            pressAnyKey = true;
            FadeOut();
        }

        private void Update()
        {
            if (!pressAnyKey) return;
            if (UnityEngine.Input.anyKey)
            {
                pressAnyKey = false;
                DOTween.Kill(image);
                FadeIn();
            }
        }

        private void FadeIn()
        {
            
            image.DOFade(1.0f, 2.0f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                SceneManager.LoadScene("probuilder_level");
            });
        }

        private void FadeOut()
        {
            image.DOFade(0.0f, 10.0f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                
            });
        }
    
    }
}
