using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public abstract class Item : MonoBehaviour
{
    public bool iscooldown = false;
    public bool isshooting = false;
    [SerializeField] protected GameObject[] effect;
    protected Rigidbody2D rigidbody;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected LayerMask groundLayer;
    public GameObject owner;
    public Transform preowner;
    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;
        if (((1 << layer) & groundLayer) != 0 && isshooting && !iscooldown)
        {
            isshooting = false;
            owner = null;
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
            StartCoroutine(Attacking(collision.gameObject));
        }
    }
    

    public virtual IEnumerator Attacking(GameObject target)
    {
        foreach (var item in effect)
        {
            item.SetActive(true);
        }
        yield return null;
    }

    public virtual void Eat()
    {
        owner.GetComponent<Entity>().Attack(transform, 10f, 0f);
        isshooting = false;
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
        yield return new WaitForSeconds(0.2f);
        owner = null;

        iscooldown = false;
    }
}