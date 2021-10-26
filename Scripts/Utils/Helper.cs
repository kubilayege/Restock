using System.Collections;
using MonoObjects;
using UnityEngine;

namespace Utils
{
    public static class Helper
    {
        public static bool TryGetWorldPos(Vector2 mousePoint,out Vector3 position , LayerMask layerMask = default)
        {
            Ray ray =Camera.main.ScreenPointToRay(mousePoint);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
            {
                position = hit.point;
                return true;
            }

            position = Vector3.zero;
            return false;
        }
        
        public static bool GetObjectOfType<T>(Vector2 mousePoint, LayerMask layerMask, out T objectRef)
        {
            Ray ray =Camera.main.ScreenPointToRay(mousePoint);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
            {
                objectRef = hit.collider.gameObject.GetComponent<T>(); 
                return true;
            }

            objectRef = default(T);
            return false;
        }
        
        public static bool TryGetObjectOfType<T>(Vector2 mousePoint, LayerMask layerMask, out T objectRef) where T : MonoBehaviour
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePoint);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
            {
                objectRef = hit.collider.gameObject.GetComponent<T>();

                return objectRef != null;
            }

            objectRef = null;
            return false;
        }
        
        public static bool TryGetObjectOfType<T>(Vector3 origin, Vector3 direction, LayerMask layerMask, out T objectRef) where T : MonoBehaviour
        {
            Ray ray = new Ray(origin, direction);
            
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 0.1f);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
            {
                objectRef = hit.collider.gameObject.GetComponent<T>();

                return objectRef != null;
            }

            objectRef = null;
            return false;
        }

        public static Vector3 Clamp(this Vector3 v, float size)
        {
            return Vector3.ClampMagnitude(v, size);
        }

        public static float AngleBetween(this Vector3 v, Vector3 other, Vector3 axis)
        {
            Vector3 vs = new Vector3(0, 0, 0);
            
            return Vector3.SignedAngle(v.normalized, other.normalized,axis);
        }

        public static int Remap(int value, int start1, int stop1, int start2, int stop2)
        {
            int outgoing =
                start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));

            return outgoing;
        }

        
    }
}