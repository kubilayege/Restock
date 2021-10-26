using MonoObjects;
using UnityEngine;

namespace Core.MoveCommands
{
    public enum Direction
    {
        Forward,
        Backward,
        Right,
        Left
    }
    public class CheckCommand
    {
        public int RepeatCount;
        public Direction Direction;
        
        public CheckCommand(Direction direction, int repeat)
        {
            RepeatCount = repeat;
            Direction = direction;
        }
    }
}