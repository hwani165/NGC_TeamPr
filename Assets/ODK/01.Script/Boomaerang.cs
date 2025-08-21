using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Boomaerang : Item
{
    public float damage = 5;
    public float knockbackmulti = 1;
    public float firstmovetime = 1f;
    public float lastmovetime = 1f;
    public float lifeTime = 2f;
    public float range = 5f;
    private Vector2 originVector;
    private bool isboomeranged = false;
    public override void Awake()
    {

        base.Awake();

    }
    public void Update()
    {
        if (isboomeranged)
        {
            transform.Rotate(Vector3.forward * -1500 * Time.deltaTime);
        }
        //if (isshooting && !isboomeranged)
        //{
        //    isboomeranged = true;
        //    originVector = transform.position;
        //    Destroy(gameObject, lifeTime);
        //    GetComponent<Rigidbody2D>().gravityScale = 0f;
        //    GetComponent<BoxCollider2D>().isTrigger = true;
        //    Sequence seq = DOTween.Sequence();


        //    seq.Append(transform.DOMove((Vector2)transform.position + (shootingdir * range), firstmovetime).SetEase(Ease.OutCubic));
        //    seq.AppendCallback(retuning);
        //}
    }

    public override void Launching()
    {
        if (isboomeranged)
        {
            return;
        }
        isboomeranged = true;
        originVector = transform.position;
        Destroy(gameObject, lifeTime);
        GetComponent<Rigidbody2D>().gravityScale = 0f;
        GetComponent<BoxCollider2D>().isTrigger = true;
        Sequence seq = DOTween.Sequence();


        seq.Append(transform.DOMove((Vector2)transform.position + (shootingdir * range), firstmovetime).SetEase(Ease.OutCubic));
        seq.AppendCallback(retuning);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        int layer = collision.gameObject.layer;
        if (((1 << layer) & targetLayer) != 0 && collision.gameObject != owner && isshooting)
        {
            owner = null;
            StartCoroutine(Attacking(collision.gameObject));
        }
    }
    void retuning()
    {
        transform.DOMove(originVector, lastmovetime).SetEase(Ease.InCubic);
    }

    public override IEnumerator Attacking(GameObject target)
    {
        base.Attacking(target);

        if (target.GetComponent<Entity>() != null)
        {
            target.GetComponent<Entity>().Attack(preowner, damage, knockbackmulti);
        }

        yield return null;

    }
}