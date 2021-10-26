using System;
using MonoObjects;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Level/Pool")]
    public class LevelPool : ScriptableObject
    {
        public Level[] levels;
        public int Length => levels.Length;

        public Level GetLevel(int index)
        {
            return levels[index];
        }
    }
}