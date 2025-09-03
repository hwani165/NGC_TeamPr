using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Header("스폰 포인트 & 아이템")]
    [SerializeField] private GameObject[] SpawnerPoints;
    [SerializeField] private GameObject[] Items;

    [Header("스폰 타이머 & 최대 아이템 갯수")]
    [SerializeField] private float Spawnertimer;
    [SerializeField] private int MaxCount = 5;

    [SerializeField] private Item spawnObject; 

    private int _itemCount;
    private float _currentTimer;
    private int _randomPoint;

    public static Action OnItemSpawned;
    public static Action OnItemCollected;

    //network data
    private sbyte _spawnX;

    private void Start()
    {
        _itemCount = 0;
        _currentTimer = 0;
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
        if (_itemCount < MaxCount)
        {
            _currentTimer += Time.deltaTime;
            if (_currentTimer >= Spawnertimer)
            {
                _currentTimer = 0;
                _randomPoint = Random.Range(0, SpawnerPoints.Length);

                while (SpawnerPoints[_randomPoint] == null)
                {
                    _randomPoint = Random.Range(0, SpawnerPoints.Length);
                }

                if (SpawnerPoints[_randomPoint] != null)
                {
                    Instantiate(Items[Random.Range(0, Items.Length)],
                                SpawnerPoints[_randomPoint].transform.position,
                                Quaternion.identity);

                    #region network data
                    _spawnX = (sbyte)Mathf.RoundToInt(SpawnerPoints[_randomPoint].transform.position.x);
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
        _itemCount++;
    }
    private void DecreaseItemCount()
    {
        _itemCount--;
    }
}
