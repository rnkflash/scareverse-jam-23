using System;
using System.Collections.Generic;
using _Content.Scripts.Managers;
using _Content.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace _Content.Scripts.UI
{
    public class InventoryDebug : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemTemplate;
        [SerializeField] private Transform panel;

        private TMP_FontAsset font;
        private float fontSize;
        private bool wrap;
        private float textWidth;
        private float textHeight;

        private void Awake()
        {
            font = itemTemplate.font;
            fontSize = itemTemplate.fontSize;
            wrap = itemTemplate.enableWordWrapping;
            
            RectTransform rt = itemTemplate.GetComponent<RectTransform>();
            textWidth = rt.sizeDelta.x;
            textHeight = rt.sizeDelta.y;

            ClearItems();

            PlayerInventory.Instance.onUpdate += UpdateItems;
        }

        private void OnDestroy()
        {
            PlayerInventory.Instance.onUpdate -= UpdateItems;
        }

        private void UpdateItems()
        {
            var inventory = PlayerInventory.Instance.items;

            ClearItems();
            
            foreach(KeyValuePair<Item, int> entry in inventory)
            {
                AddItem(entry.Key, entry.Value);
            }
            
        }

        private void ClearItems()
        {
            foreach (Transform child in panel) {
                Destroy(child.gameObject);
            }
        }

        private void AddItem(Item item, int count)
        {
            var gameObject = new GameObject();
            var tmpText = gameObject.AddComponent<TextMeshProUGUI>();
            tmpText.font = font;
            tmpText.fontSize = fontSize;
            tmpText.enableWordWrapping = wrap;
            tmpText.text = $"{item.name} x {count}";
            
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(textWidth, textHeight);
            
            gameObject.transform.SetParent(panel);
        }
    }
}
