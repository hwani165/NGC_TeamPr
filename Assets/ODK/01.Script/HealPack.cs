using System.Collections;
using UnityEngine;

public class HealPack : Item
{
    public sbyte damage = 1;
    public float knockbackmulti = 1;

    public override void Eat()
    {
        owner.GetComponent<Entity>().Attack(transform, damage, 0f);
        isShooting = false;
        Instantiate(effect[0], owner.transform.position, Quaternion.identity);
        owner = null;
        Destroy(gameObject);
    }
    public override IEnumerator Attacking(GameObject target)
    {
        base.Attacking(target);

        //if (target.GetComponent<Entity>() != null)
        //{
        //    target.GetComponent<Entity>().Attack(preowner, damage, knockbackmulti);
        //}
        Destroy(gameObject);
        yield return null;
        
    }
}
