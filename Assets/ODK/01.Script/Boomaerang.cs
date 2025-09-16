using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Boomaerang : Item
{
    // 트윈 애니메이션 시, 이동할 때 사용하는 이징 함수
    [SerializeField] private Ease easeType = Ease.OutCubic;     // 처음 이동 시 부드럽게 감속
    [SerializeField] private Ease lasteaseType = Ease.InCubic;  // 돌아올 때 가속

    // 공격 관련 변수
    public float damage = 5;          // 공격력
    public float knockbackmulti = 1;  // 넉백 배율

    // 부메랑 이동 관련 시간 변수
    public float firstmovetime = 1f;  // 처음 날아가는 시간
    public float lastmovetime = 1f;   // 돌아오는 시간
    public float lifeTime = 2f;       // 생존 시간 (파괴까지)
    public float range = 5f;          // 최대 이동 거리

    private Vector2 originVector;     // 부메랑이 돌아올 원래 위치
    private bool isboomeranged = false; // 현재 부메랑이 날아가고 있는 상태인지?

    [SerializeField] private bool thisnotalreadyHit = false; // (사용 안 함, 충돌 중복 체크용일 가능성 있음)
    private float currentlifeTime = 2f; // 남은 생존 시간

    public DG.Tweening.Sequence seq; // DOTween 시퀀스 (부메랑 이동 경로 애니메이션 관리)

    private HashSet<Entity> alreadyHit = new HashSet<Entity>();
    // 이미 맞은 Entity 저장 → 같은 대상 여러 번 맞지 않게 방지

    [SerializeField] private float invisibleTime = 0.5f;
    // 무적/충돌 무시 시간. 이 시간 동안 지나면 맞은 Entity 목록 초기화
    private float realinvisibleTime;

    // 초기화
    public override void Awake()
    {
        base.Awake(); // Item 클래스의 Awake 호출 (Item 쪽 초기화 포함)
    }

    public void Update()
    {
        // 맞은 목록 초기화 타이머 처리
        if (invisibleTime > 0f)
        {
            realinvisibleTime += Time.deltaTime;
            if (realinvisibleTime >= invisibleTime)
            {
                realinvisibleTime = 0f;
                alreadyHit.Clear(); // 다시 충돌할 수 있게 초기화
            }
        }

        // 부메랑이 날아가고 있을 때
        if (isboomeranged)
        {
            // 회전 효과
            transform.Rotate(Vector3.forward * -1500 * Time.deltaTime);

            // 생존 시간 감소 → 0 되면 파괴
            currentlifeTime -= Time.deltaTime;
            if (currentlifeTime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    // 발사 (던져졌을 때 호출됨)
    public override void Launching()
    {
        if (isboomeranged) // 이미 날아가고 있으면 무시
        {
            return;
        }

        realinvisibleTime = 0f;     // 충돌 초기화 시간 리셋
        currentlifeTime = lifeTime; // 생존 시간 리셋
        isboomeranged = true;       // 날아가는 상태로 전환
        originVector = transform.position; // 현재 위치를 원점으로 저장

        // Rigidbody2D와 Collider 설정
        GetComponent<Rigidbody2D>().gravityScale = 0f;  // 중력 무시
        GetComponent<BoxCollider2D>().isTrigger = true; // 충돌 감지만 허용

        // 트윈 시퀀스 초기화
        seq = DOTween.Sequence();
        isShooting = false; // 부모(Item)에서 쓰는 shooting 플래그 해제
        alreadyHit.Clear(); // 맞은 대상 초기화

        // 부메랑이 앞으로 날아가도록 트윈 설정
        seq.Append(transform.DOMove((Vector2)transform.position + (shootingdir * range), firstmovetime).SetEase(easeType));

        // 날아간 뒤 돌아오기 시작
        seq.AppendCallback(retuning);
    }

    // 플레이어가 다시 잡았을 때 처리
    public override void Grab()
    {
        base.Grab();
        isboomeranged = false; // 날아가는 상태 해제
        realinvisibleTime = 0f; // 충돌 초기화 타이머 리셋

        // 트윈 중단
        seq?.Kill(true);
        transform.DOKill();
    }

    // 다른 오브젝트와 충돌 유지 중일 때
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isboomeranged) return; // 날아가고 있지 않으면 무시

        int layer = collision.gameObject.layer;

        // 타겟 레이어에 속하고, 던진 주인(preowner)이 아닐 때만 충돌 판정
        if (((1 << layer) & targetLayer) != 0 && collision.transform != preowner)
        {
            Entity entity = collision.GetComponent<Entity>();
            if (entity != null)
            {
                // 이미 맞은 대상이면 무시
                if (alreadyHit.Contains(entity))
                    return;

                // 히트 이펙트 생성
                Instantiate(effect[0], collision.transform.position, Quaternion.identity);

                // 맞은 대상 기록
                alreadyHit.Add(entity);

                // 공격 코루틴 실행
                try
                {
                    StartCoroutine(Attacking(collision.gameObject));
                }
                catch
                {
                    Debug.Log(collision.gameObject.name);
                }
            }
        }
    }

    // 실제 공격 처리
    public override IEnumerator Attacking(GameObject target)
    {
        base.Attacking(target); // 부모 클래스(Item)의 공격 로직 실행

        Entity entity = target.GetComponent<Entity>();
        if (entity != null)
        {
            // 여기서 NullReferenceException 발생 가능
            // preowner (던진 주인) 이 null이면 entity.Attack 내부에서 터짐
            entity.Attack(preowner, damage, knockbackmulti);
        }

        yield return null;
    }

    // 되돌아오기 동작
    void retuning()
    {
        alreadyHit.Clear(); // 되돌아올 때 충돌 기록 초기화
        transform.DOMove(originVector - (shootingdir * range), lastmovetime).SetEase(lasteaseType);
    }
}
