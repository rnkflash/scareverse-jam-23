using System;
using System.Collections.Generic;
using _Content.Scripts.ScriptableObjects;
using _Content.Scripts.Util;

namespace _Content.Scripts.Managers
{
    public class PlayerInventory: Singleton<PlayerInventory>
    {
        public Dictionary<Item, int> items;

        public Action onUpdate;

        protected override void Created()
        {
            items = new Dictionary<Item, int>();
        }

        public void AddItem(Item item, int count = 1)
        {
            if (items.ContainsKey(item))
                items[item] += count;
            else
                items[item] = count;
            
            onUpdate?.Invoke();
        }
        
        public void RemoveItem(Item item, int count = 1)
        {
            if (!items.ContainsKey(item)) return;
            
            items[item] -= count;
            
            if (items[item] <= 0)
                items.Remove(item);
            
            onUpdate?.Invoke();
        }
        
        public void EquipItem(Item item)
        {
            RemoveItem(item);
        }

        public void UseItem(Item item)
        {
            RemoveItem(item);
        }
    }
}