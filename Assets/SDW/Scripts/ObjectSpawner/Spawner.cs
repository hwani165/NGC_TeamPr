using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("���� ����Ʈ & ������")]
    [SerializeField] private GameObject[] spawnerPoints;
    [SerializeField] private GameObject[] Objects;

    [Header("���� Ÿ�̸� & �ִ� ������ ����")]
    [SerializeField] private float spawnertimer;
    [SerializeField] private int maxCount = 5;

    static public int itemCount;
    private int maxItemCount;
    private float currentTimer;
    private int randomPoint;

    private void Start()
    {
        maxItemCount = maxCount;
        itemCount = 0;
        currentTimer = 0;
    }

    private void Update()
    {
        if (itemCount < maxItemCount)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= spawnertimer)
            {
                currentTimer = 0;
                randomPoint = Random.Range(0, spawnerPoints.Length);

                // ������ ��ġ�� ������Ʈ ����
                Instantiate(Objects[Random.Range(0, Objects.Length)],
                            spawnerPoints[randomPoint].transform.position,
                            Quaternion.identity);

                itemCount++;
            }
        }

    }
}
