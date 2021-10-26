using UnityEngine;

namespace MonoObjects
{
    public class PopUpCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject[] stars;

        public void ActivateStars(int count)
        {
            int i = 0;
            foreach (var star in stars)
            {
                star.SetActive(i++ < count);
            }
        }
        
    }
}