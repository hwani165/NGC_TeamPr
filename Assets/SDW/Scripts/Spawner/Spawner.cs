using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour,IReceiver
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
    private byte _spawnItemIndex;

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
                    //0번부터 아이템 배열의 길이까지 인덱스를 랜덤하게 구해서 랜덤한 아이템 객체를 가져옴
                    byte randomPoint = (byte)Random.Range(0, Items.Length);
                    GameObject _item = Items[randomPoint];
                    //랜덤한 아이템 스폰 포인트의 위치를 가져옴
                    Vector2 spawnPos = SpawnerPoints[_randomPoint].transform.position;
                    //가져온 아이템을 스폰 포인트의 위치로 생성시킴.
                    Instantiate(_item, spawnPos,Quaternion.identity);

                    #region network data
                    //RoundToInt : float 데이터 값을 가장 가까운 정수 자료형 값으로 바꿈.
                    _spawnX = (sbyte)Mathf.RoundToInt(spawnPos.x);
                    _spawnItemIndex = randomPoint;
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

    public void ApplyByteData(byte byteData)
    {
        _spawnItemIndex = byteData;
    }

    public void ApplySbyteData(sbyte sbyteData)
    {
        _spawnX = sbyteData;
    }

    public void ApplySbyteData(sbyte sbyteData1, sbyte sbyteData2, sbyte sbyteData3)
    {
        throw new NotImplementedException("If you want to use this method, you must override it.");
    }
}
