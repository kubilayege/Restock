using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class Path
    {
        [SerializeField] private Transform[] points;

        public Vector3[] GetPath()
        {
            var path = new Vector3[points.Length];
            var i = 0;
            foreach (var point in points)
            {
                path[i++] = point.position;
            }

            return path;
        }
        
        public Vector3[] GetReversePath()
        {
            var path = new Vector3[points.Length];
            var i = points.Length-1;
            foreach (var point in points)
            {
                path[i--] = point.position;
            }

            return path;
        }
        
        public float GetLenght()
        {
            var lastPointPos = points[0].position;
            var distance = 0f;
            foreach (var point in points)
            {
                var position = point.position;
                distance = (position - lastPointPos).magnitude;
                lastPointPos = position;
            }

            // Debug.Log(distance);
            return distance;
        }

    }
}