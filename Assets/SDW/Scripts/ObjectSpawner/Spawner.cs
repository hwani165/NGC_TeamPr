using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Header("스폰 포인트 & 아이템")]
    [SerializeField] private GameObject[] spawnerPoints;
    [SerializeField] private GameObject[] Objects;

    [Header("스폰 타이머 & 최대 아이템 갯수")]
    [SerializeField] private float spawnertimer;
    [SerializeField] private int maxCount = 5;

    [SerializeField] private Item spawnObject; 

    private int itemCount;
    private float currentTimer;
    private int randomPoint;

    public static Action OnItemSpawned;
    public static Action OnItemCollected;

    //network data
    private sbyte spawnX;

    private void Start()
    {
        itemCount = 0;
        currentTimer = 0;
    }

    private void OnEnable()
    {
        OnItemSpawned += IncreaseItemCount;
        OnItemCollected += DecreaseItemCount;
    }

    private void OnDisable()
    {
        OnItemSpawned -= IncreaseItemCount;
        OnItemCollected -= DecreaseItemCount;
    }

    private void Update()
    {
        if (itemCount < maxCount)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= spawnertimer)
            {
                currentTimer = 0;
                randomPoint = Random.Range(0, spawnerPoints.Length);

                while (spawnerPoints[randomPoint] == null)
                {
                    randomPoint = Random.Range(0, spawnerPoints.Length);
                }

                if (spawnerPoints[randomPoint] != null)
                {
                    Instantiate(Objects[Random.Range(0, Objects.Length)],
                                spawnerPoints[randomPoint].transform.position,
                                Quaternion.identity);

                    #region network data
                    spawnX = (sbyte)Mathf.RoundToInt(spawnerPoints[randomPoint].transform.position.x);
                    #endregion
                }
                else
                {
                }
            }
        }
    }

    private void IncreaseItemCount()
    {
        itemCount++;
    }
    private void DecreaseItemCount()
    {
        itemCount--;
    }
}
