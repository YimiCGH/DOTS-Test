using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class DataDisplay : MonoBehaviour
    {
        

        private static DataDisplay _ins;

        public static DataDisplay Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = FindObjectOfType<DataDisplay>();
                }

                return _ins;
            }
        }

        private Dictionary<string, DisplayItem> _items;

        public void SetValue(string itemName,string value)
        {
            if (_items.TryGetValue(itemName,out var item))
            {
                item.Value.text = value;
            }
        }

        public void Start()
        {
            _items = new Dictionary<string, DisplayItem>();
            var items = GetComponentsInChildren<DisplayItem>();
            foreach (var item in items)
            {
                _items.Add(item.gameObject.name,item);
            }
        }
    }
}

