using System.Collections;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
public abstract class Item : MonoBehaviour
{
    public static ushort Counter;
    public ushort Id;

    public bool iscooldown = false;
    public bool isshooting = false;
    [SerializeField] protected GameObject[] effect;
    protected Rigidbody2D rigidbody;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected LayerMask groundLayer;
    public GameObject owner;
    public Transform preowner;
    public Vector2 shootingdir;
    public bool thisisnoforceobject = false;
    public bool thisownerfading = true;

    //netWork
    private float _synkTime = 0f;
    byte[] _bff;
    public virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        Id = Counter++;
    }
    private void Update()
    {
        if(!isshooting && !isshooting)
        {
            _synkTime += Time.deltaTime;
            if (_synkTime >= 1)
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
        if (((1 << layer) & groundLayer) != 0 && isshooting && !iscooldown)
        {
            isshooting = false;
            owner = null;
            Instantiate(effect[0], transform.position, Quaternion.identity);
            StartCoroutine(Attacking(collision.gameObject));
        }
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;

        

        if (((1 << layer) & targetLayer) != 0 &&
            collision.gameObject != owner && isshooting)
        {
            isshooting = false;
            owner = null;
            Instantiate(effect[0], transform.position, Quaternion.identity);
            StartCoroutine(Attacking(collision.gameObject)); //¹ö±×
        }
    }
    public virtual void Launching()
    {

    }
    public virtual void Grab()
    {

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
        owner.GetComponent<Entity>().Attack(transform, 10, 0f);
        isshooting = false;
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

        //Game.Instance.Map.Spawner.Items.Remove(Id);

        Spawner.OnItemCollected?.Invoke();
    }
}