using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance != null)
                    DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    protected void Awake()
    {
        if (_instance != null)
        {
            if (_instance != this)
                Destroy(gameObject);

            return;
        }

        //SingletonBehaviour�� Instance ���� �� Awake()�� ����Ǵ� ���.
        _instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }
}
