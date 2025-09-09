using UnityEngine;

public class Object : MonoBehaviour
{
    public int SpawnedIndex { get; set; } = -1;

    private void OnDestroy()
    {
        if (Spawner.OnItemDestroyed != null && SpawnedIndex >= 0)
            Spawner.OnItemDestroyed.Invoke(SpawnedIndex);
    }
}
