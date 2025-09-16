using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MyAction : Player
{
    // --- 네트워크 전송용 상태 변수 ---
    public bool IsHolding;   // 아이템을 들고 있는지 여부
    public bool IsThrowing;  // 아이템을 던지고 있는지 여부
    private Vector2 _throwDir; // 아이템을 던질 방향

    // --- 인스펙터에서 연결할 오브젝트 ---
    [SerializeField] private Transform HoldTransform;   // 아이템을 붙잡을 위치
    [SerializeField] private GameObject HoldObject;     // 현재 들고 있는 아이템
    private Rigidbody2D rb;                             // 플레이어 자신의 Rigidbody2D

    [SerializeField] private float ThrowPower = 30f;    // 아이템 던지는 힘
    [SerializeField] private float UpwardForce = 30f;   // 위로 가해지는 추가 힘
    [SerializeField] private float PlayerRecoil = 40f;  // 플레이어가 반동으로 밀리는 힘
    [SerializeField] private GameObject ChargeUiObject; // 차징 UI 전체 오브젝트
    [SerializeField] private Image ChargeImage;         // 차징 게이지 이미지

    private GameObject SoundGroup;   // 사운드 그룹 (던질 때 소리 재생용)
    private byte _chargeGauge = 0; // 차징 게이지 (0~3)
    private float _chargeGauageTimmer = 0f;

    private void Awake()
    {
        // DontDestroyOnLoadObjs 오브젝트에서 사운드 그룹 찾아오기
        DontDestroyOnLoadObjs objs = FindAnyObjectByType<DontDestroyOnLoadObjs>();
        if (objs != null)
        {
            SoundGroup = objs.gameObject;
        }

        // 컴포넌트 캐싱
        rb = GetComponent<Rigidbody2D>();

        // HoldTransform, UI, Image 자동 할당 (없으면 트랜스폼에서 찾기)
        if (HoldTransform == null) HoldTransform = transform.Find("Hold");
        if (ChargeUiObject == null) ChargeUiObject = transform.Find("ChageCanvas").gameObject;
        if (ChargeImage == null) ChargeImage = transform.Find("ChageCanvas/ChageBackground/ChageImage").GetComponent<Image>();
    }
    private void UpdateChageGauge()
    {
        _chargeGauageTimmer += Time.deltaTime /*+ ((2f - _chargeGauge) * Time.deltaTime)*/;

        if (_chargeGauageTimmer >= 1)
        {
            _chargeGauageTimmer = 0f;
            _chargeGauge += 1;
        }

        // 차징이 3 이상이면 고정
        if (_chargeGauge > 3)
        {
            _chargeGauge = 3;
            return;
        }
    }
    void Update()
    {
        // [E 키를 누르고 있을 때] → 차징 중
        if (Input.GetKey(KeyCode.E))
        {
            ChargeUiObject.SetActive(true); // UI 보이기

            UpdateChageGauge();

            // 차징이 2.5 이상이면 빨간색, 아니면 흰색
            if (_chargeGauge >= 2.5f) ChargeImage.color = Color.red;
            else ChargeImage.color = Color.white;

            // 차징 UI 채워주기
            ChargeImage.fillAmount = _chargeGauge / 3f;
        }
        // [E 키를 뗐을 때] → 차징값이 남아 있으면 던지기
        else if (_chargeGauge > 0)
        {
            ThrowItem(); // 아이템 던지기
            _chargeGauge = 0; // 초기화
            _chargeGauageTimmer = 0f;
            ChargeUiObject.gameObject.SetActive(false); // UI 숨김
        }
    }

    // 방향키 입력으로 던질 방향 계산
    private Vector2 GetInputDirection()
    {
        Vector2 dir = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow)) dir.x -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) dir.x += 1f;
        if (Input.GetKey(KeyCode.UpArrow)) dir.y += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) dir.y -= 1f;
        return dir;
    }

    // Trigger 충돌 상태일 때 (예: 아이템 위에 올라가 있거나 닿았을 때)
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Keyboard.current.sKey.isPressed) // S키를 누르고 있으면 아이템 줍기
        {
            Item item = collision.gameObject.GetComponent<Item>();
            if (item == null) return;

            // 이미 내가 소유 중인 아이템이면 충돌 무시
            if (item.owner == gameObject)
            {
                Collider2D myCol = GetComponent<Collider2D>();
                Collider2D itemCol = item.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(myCol, itemCol, true);
                return;
            }

            // 다른 사람이 안 들고 있고, 쿨타임이 아니고, 날아가고 있지 않다면 주움
            if (item.owner == null && !item.iscooldown && !item.isShooting)
            {
                // 이미 들고 있던 아이템 처리 → 땅에 내려놓음
                if (HoldObject != null)
                {
                    HoldObject.GetComponent<Item>().owner = null;
                    Rigidbody2D oldRb = HoldObject.GetComponent<Rigidbody2D>();
                    oldRb.simulated = true;
                    oldRb.gravityScale = 2.75f;
                    oldRb.transform.parent = null;
                    oldRb.GetComponent<Collider2D>().isTrigger = false;
                    HoldObject.GetComponent<Item>().CooldownActive();
                }

                // 새 아이템 들기
                Hold(item.gameObject);
                IsHolding = true;
                Send(); // 네트워크 전송
            }
        }
    }

    // Collision 충돌 상태일 때 (Trigger가 아닌 일반 충돌)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Keyboard.current.sKey.isPressed)
        {
            Item item = collision.gameObject.GetComponent<Item>();
            if (item == null) return;

            if (item.owner == gameObject) // 내가 이미 들고 있는 경우 충돌 무시
            {
                Collider2D myCol = GetComponent<Collider2D>();
                Collider2D itemCol = item.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(myCol, itemCol, true);
                return;
            }
            else if (item.owner == null && !item.iscooldown && !item.isShooting)
            {
                // 기존 들고 있는 아이템 내려놓기
                if (HoldObject != null)
                {
                    HoldObject.GetComponent<Item>().owner = null;
                    Rigidbody2D oldRb = HoldObject.GetComponent<Rigidbody2D>();
                    oldRb.simulated = true;
                    oldRb.transform.parent = null;
                    oldRb.gravityScale = 2.75f;
                    oldRb.GetComponent<Collider2D>().isTrigger = false;
                    HoldObject.GetComponent<Item>().CooldownActive();
                }

                // 새 아이템 들기
                Hold(item.gameObject);
                Send();
            }
        }
    }

    // 아이템 던지기
    private void ThrowItem()
    {
        if (HoldObject == null) return;
        _throwDir = GetInputDirection(); // 입력 방향

        Item itemScript = HoldObject.GetComponent<Item>();
        Rigidbody2D hrb = HoldObject.GetComponent<Rigidbody2D>();

        itemScript.isShooting = true;

        // 차지 게이지가 2 이상이라면 먹기
        if (_chargeGauge >= 3)
        {
            itemScript.preowner = transform;
            itemScript.shootingdir = Vector2.zero;
            itemScript.Eat();
            HoldObject = null;
            return;
        }
        // 방향이 없으면 던지지 않음
        else if (_throwDir == Vector2.zero) return;

        //내 상태를 던지는 상태로 갱신
        IsHolding = false;
        IsThrowing = true;
        PlayThrowSound();

        //네트워크 데이터 전송
        Send();

        //방향 노말라이즈
        _throwDir.Normalize();

        //아이템 부모 해제 + 위치 지정
        itemScript.preowner = transform;
        HoldObject.transform.parent = null;
        HoldObject.transform.position = transform.position + (Vector3)(GetInputDirection() * 1.25f);

        // 쿨타임 시작
        itemScript.CooldownActive();
        hrb.simulated = true;

        //부메랑이라면 처리
        if (!itemScript.thisisnoforceobject)
        {
            //부메랑용 방향 저장
            itemScript.shootingdir = _throwDir * _chargeGauge;

            hrb.linearVelocity = Vector2.zero;

            // 전방 + 위쪽 힘 추가
            hrb.AddForce(_throwDir * ThrowPower + (_throwDir.y == 0 ? new Vector2(0, UpwardForce) : Vector2.zero), ForceMode2D.Impulse);

            // 랜덤 회전
            hrb.angularVelocity += Random.Range(-180f, 180f);
        }

        // 플레이어 반동
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(-_throwDir * PlayerRecoil, ForceMode2D.Impulse);

        // 아이템 상태 갱신
        itemScript.Launching();
        itemScript.isShooting = true;
        itemScript.isHolding = false;

        // 손 비우기
        Release(HoldObject);
        IsThrowing = false;
    }

    // 던질 때 사운드 재생
    private void PlayThrowSound()
    {
        SoundGroup.transform.GetChild(0).GetComponent<AudioSource>().Play();
    }

    // 아이템 줍기 처리
    private void Hold(GameObject obj)
    {
        IsHolding = true;

        // 아이템 스크립트 상태 갱신
        if (obj.TryGetComponent(out Item itemSc))
        {
            HoldObject = obj;
            itemSc.isHolding = true;
            itemSc.owner = gameObject;
            itemSc.Grab();
        }

        // 물리/위치 조정
        if (obj.TryGetComponent(out Rigidbody2D rb))
        {
            rb.simulated = false;       // 물리 정지
            rb.transform.parent = HoldTransform; // 플레이어 손에 붙이기
            rb.gravityScale = 2.75f;
            rb.GetComponent<Collider2D>().isTrigger = false;
            obj.transform.localPosition = Vector2.zero; // 정확히 손 위치로 고정
        }
    }
    private void Release(GameObject obj)
    {
        if (obj.TryGetComponent(out Item itemSc))
        {
            //itemSc.isHolding = false;
            HoldObject = null;
            itemSc.owner = null;
        }

        if (obj.TryGetComponent(out Rigidbody2D rb))
        {
            rb.simulated = true;
            rb.transform.parent = null;
            rb.gravityScale = 1;
            rb.GetComponent<Collider2D>().isTrigger = false;
        }
    }

    // 네트워크 전송 (아이템 ID, 상태, 차징, 방향 등 전송)
    public override void Send()
    {
        ushort id = HoldObject.GetComponent<Item>().Id;
        byte chargeGauge = (byte)_chargeGauge;
        byte[] bff = Server.Instance.SerializationActionData(id, IsHolding, IsThrowing, chargeGauge, _throwDir);
        Server.Instance.Send(bff);
    }
}
