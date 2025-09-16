using UnityEngine;

public class DontDestroyOnLoadObjs : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
