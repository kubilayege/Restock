using System;
using UnityEngine;

namespace MonoObjects
{
    public class ItemHandle : MonoBehaviour
    {
        public Item item;

        public void Init(Item _item)
        {
            gameObject.layer = 7;
            item = _item;
        }

        private void Awake()
        {
        }
    }
}