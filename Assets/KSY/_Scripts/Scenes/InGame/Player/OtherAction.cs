using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static BackendFunctionInGame;

public class OtherAction : Player
{
    //network
    public bool IsItemHolding; // 아이템을 들었는가?
    public bool IsItemShoting;// 아이템을 던졌는가?
    private byte _chargeGauge = 0;
    private Vector2 _throwDir;

    [SerializeField] private Transform HoldTransform;
    [SerializeField] private GameObject HoldObject;
    private Rigidbody2D _rb;

    [SerializeField] private float ThrowPower = 30f;
    [SerializeField] private float DefaultShotForce = 60f;
    [SerializeField] private float UpwardForce = 30f;
    //던지기 반동
    [SerializeField] private float PlayerRecoil = 40f;
    [SerializeField] private GameObject ChargeUiObject;
    [SerializeField] private Image ChargeImage;

    private GameObject SoundGroup;

    private void Awake()
    {
        DontDestroyOnLoadObjs objs = FindAnyObjectByType<DontDestroyOnLoadObjs>();
        if (objs != null)
        {
            SoundGroup = objs.gameObject;
        }
        _rb = GetComponent<Rigidbody2D>();
        if (HoldTransform == null) HoldTransform = transform.Find("Hold");
        if (ChargeUiObject == null) ChargeUiObject = transform.Find("ChageCanvas").gameObject;
        if (ChargeImage == null) ChargeImage = transform.Find("ChageCanvas/ChageBackground/ChageImage").GetComponent<Image>();
    }
    private void Hold(GameObject obj)
    {
        //수정
        if (HoldObject != null)
        {
            Item itemScc = HoldObject.GetComponent<Item>();
            itemScc.owner = null;
            Rigidbody2D oldRb = HoldObject.GetComponent<Rigidbody2D>();
            oldRb.simulated = true;
            oldRb.gravityScale = 2.75f;
            oldRb.transform.parent = null;
            oldRb.GetComponent<Collider2D>().isTrigger = false;
            HoldObject.GetComponent<Item>().CooldownActive();
        }

        //아이템 들기 처리
        if (obj.TryGetComponent(out Item itemSc))
        {
            Debug.Log("other hold");
            HoldObject = obj;
            itemSc.owner = gameObject;
            itemSc.Grab();
        }

        if (obj.TryGetComponent(out Rigidbody2D rb))
        {
            rb.simulated = false;
            rb.transform.parent = HoldTransform;
            rb.gravityScale = 2.75f;
            rb.GetComponent<Collider2D>().isTrigger = true;

            obj.transform.localPosition = Vector2.zero;
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

    private void ThrowItem(GameObject item, Vector2 throwDir, byte chargeGuage)
    {
        if (HoldObject == null) return;
        Item itemScript = HoldObject.GetComponent<Item>();
        Rigidbody2D hrb = HoldObject.GetComponent<Rigidbody2D>();

        Debug.Log($"Other Action ChargeGuage: {chargeGuage}");

        if (chargeGuage >= 3)
        {
            itemScript.preowner = transform;
            itemScript.shootingdir = Vector2.zero;
            itemScript.Eat();
            HoldObject = null;
            return;
        }
        if (throwDir == Vector2.zero && throwDir == Vector2.down) return;

        //방향 노말라이즈
        throwDir.Normalize();

        //아이템 부모 해제 + 위치 지정
        itemScript.preowner = transform;
        HoldObject.transform.parent = null;
        HoldObject.transform.position = transform.position + (Vector3)(throwDir * 1.25f);

        itemScript.isShooting = true;
        itemScript.preowner = transform;
        itemScript.CooldownActive();

        //아이템 물리연산 체크
        hrb.simulated = true;

        //만약 부메랑이면 처리
        if (!itemScript.thisisnoforceobject)
        {
            itemScript.shootingdir = throwDir * _chargeGauge;
            hrb.linearVelocity = Vector2.zero;
            hrb.AddForce(throwDir * ThrowPower + (throwDir.y == 0 ? new Vector2(0, UpwardForce)
            : new Vector2(0, 0)), ForceMode2D.Impulse);
            hrb.angularVelocity += Random.Range(-180f, 180f);
        }

        //플레이어 던지는 반동 이펙트
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(-_throwDir * PlayerRecoil, ForceMode2D.Impulse);

        itemScript.isShooting = true;
        itemScript.Launching();
        HoldObject = null;
        PlayThrowSound();
    }
    private void PlayThrowSound()
    {
        SoundGroup.transform.GetChild(0).GetComponent<AudioSource>().Play();
    }

    //수정할코드
    public override void ApplyUShortData(ushort id)
    {
        //들고 있는 아이템이 있었다면 
        if (HoldObject != null)
        {
            Release(HoldObject);
        }

        GameObject hdIt = Game.Instance.MapCompo.SpawnerCompo.FindItem(id);
        Hold(hdIt);
    }
    public override void ApplyByteData(byte state, byte charge)
    {
        bool isThrowing = (state & (byte)flagActionState.IsThrowing) != 0;

        _chargeGauge = charge;

        if (isThrowing)
        {
            ThrowItem(HoldObject, _throwDir, _chargeGauge);
        }
    }
    public override void ApplySbyteData(sbyte dirX, sbyte dirY)
    {
        _throwDir = new Vector2(dirX, dirY);
    }
}
