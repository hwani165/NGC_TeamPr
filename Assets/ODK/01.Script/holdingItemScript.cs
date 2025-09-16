using System.Collections;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
public abstract class Item : MonoBehaviour
{
    public AudioClip launchesound;
    public AudioClip grabsound;

    protected AudioSource audioSource;
    public static ushort Counter;
    public ushort Id;

    public bool iscooldown = false;
    public bool isShooting = false;
    [SerializeField] protected GameObject[] effect;
    protected Rigidbody2D rb;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected LayerMask groundLayer;
    public GameObject owner;
    public Transform preowner;
    public Vector2 shootingdir;
    public bool thisisnoforceobject = false;
    public bool thisownerfading = true;

    //netWork
    [SerializeField] public bool isHolding = false;
    private float _synkTime = 0f;
    private float _synkTime2 = 0f;
    byte[] _bff;
    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Id = Counter++;
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        _synkTime += Time.deltaTime;
        if (_synkTime >= 0.7)
        {
            if (Server.IsSuperGamer)
            {
                _synkTime = 0f;
                Send();
            }
        }
    }

    public void Send()
    {
        _bff = Server.Instance.SerializationItemPos(Id, transform.position);
        Server.Instance.Send(_bff);
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;
        if (((1 << layer) & groundLayer) != 0 && isShooting && !iscooldown)
        {
            isShooting = false;
            owner = null;
            Instantiate(effect[0], transform.position, Quaternion.identity);
            StartCoroutine(Attacking(collision.gameObject));
        }
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;



        if (((1 << layer) & targetLayer) != 0 &&
            collision.gameObject != owner && isShooting)
        {
            isShooting = false;
            owner = null;
            Instantiate(effect[0], transform.position, Quaternion.identity);
            try
            {
                StartCoroutine(Attacking(collision.gameObject)); 
            }
            catch
            {
                Debug.Log(collision.gameObject.name);
                Debug.Log(collision.gameObject);
            }
        }
    }
    public virtual void Launching()
    {
        audioSource.PlayOneShot(launchesound);
    }
    public virtual void Grab()
    {
        audioSource.PlayOneShot(grabsound);
        

    }
    public virtual IEnumerator Attacking(GameObject target)
    {
        Debug.Log($"Try Attack {gameObject.name} -> {target.name}");

        foreach (var item in effect)
        {
            item.SetActive(true);
        }
        yield return null;
    }

    public virtual void Eat()
    {
        Entity targetEntity = owner.GetComponent<Entity>();

        if(targetEntity.name == "P1" && targetEntity.Health <= 1)
        {
            targetEntity.Attack(transform, 10, 0f);
        }

        isShooting = false;
        Instantiate(effect[0], owner.transform.position, Quaternion.identity);
        owner = null;
        Destroy(gameObject);
    }
    public void CooldownActive()
    {
        StartCoroutine(HoldCooldown());
    }

    private IEnumerator HoldCooldown()
    {
        iscooldown = true;
        yield return new WaitForSeconds(0.1f);
        if (thisownerfading) owner = null;

        iscooldown = false;
    }
    private void OnEnable()
    {
        Spawner.OnItemSpawned?.Invoke();
    }

    private void OnDestroy()
    {
        byte[] bff = Server.Instance.SerializationItemDes(Id);
        Server.Instance.Send(bff);
    }
}