using System;
using System.Collections.Generic;
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

    private int itemCount;
    private float currentTimer;

    // 현재 필드에 존재하는 아이템 프리팹 인덱스
    private HashSet<int> spawnedObjectIndices = new HashSet<int>();

    public static Action<int> OnItemDestroyed; // 인덱스 전달

    private void Start()
    {
        itemCount = 0;
        currentTimer = 0;
    }

    private void OnEnable()
    {
        OnItemDestroyed += OnObjectDestroyed;
    }

    private void OnDisable()
    {
        OnItemDestroyed -= OnObjectDestroyed;
    }

    private void Update()
    {
        if (itemCount < maxCount)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= spawnertimer)
            {
                currentTimer = 0;

                // 아직 스폰되지 않은 아이템 인덱스만 추출
                List<int> availableIndices = new List<int>();
                for (int i = 0; i < Objects.Length; i++)
                {
                    if (!spawnedObjectIndices.Contains(i))
                        availableIndices.Add(i);
                }

                if (availableIndices.Count > 0)
                {
                    int objectIdx = availableIndices[Random.Range(0, availableIndices.Count)];
                    int spawnPointIdx = Random.Range(0, spawnerPoints.Length);

                    var spawned = Instantiate(Objects[objectIdx],
                        spawnerPoints[spawnPointIdx].transform.position,
                        Quaternion.identity);

                    // Object 스크립트에 인덱스 전달
                    var objScript = spawned.GetComponent<Object>();
                    if (objScript != null)
                        objScript.SpawnedIndex = objectIdx;

                    spawnedObjectIndices.Add(objectIdx);
                    itemCount++;
                }
            }
        }
    }

    // 아이템이 Destroy될 때 호출
    private void OnObjectDestroyed(int objectIdx)
    {
        spawnedObjectIndices.Remove(objectIdx);
        itemCount--;
    }
}
