using UnityEngine;

namespace MonoObjects
{
    public class BasketCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.parent.parent.TryGetComponent(out Item item))
            {
                item.ResetItem();

                // Debug.Log(item.id);
            }
        }
    }
}