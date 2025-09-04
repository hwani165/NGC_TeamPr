using UnityEngine;

public class Object : MonoBehaviour
{
    private void OnEnable()
    {
        Spawner.OnItemSpawned?.Invoke();
    }

    private void OnDestroy()
    {
        Spawner.OnItemCollected?.Invoke();
    }
}
