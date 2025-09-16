using UnityEngine;

namespace SDW
{
    public class Item : MonoBehaviour
    {
        private void OnDestroy()
        {
            Spawner.OnItemCollected?.Invoke();
        }
    }
}