using _Content.Scripts.Managers;
using UnityEngine;

namespace _Content.Scripts.Util
{
    public class PlayMusic: MonoBehaviour
    {
        [SerializeField] private AudioClip music;
        
        private void Start()
        {
            SoundManager.Instance.ChangeTrack(music);
        }
    }
}