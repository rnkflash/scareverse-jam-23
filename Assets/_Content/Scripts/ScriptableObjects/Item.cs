using UnityEngine;

namespace _Content.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items")]
    public class Item: ScriptableObject
    {
        public string name;
        public Sprite icon;
        public AudioClip pickUpSound;
    }
}