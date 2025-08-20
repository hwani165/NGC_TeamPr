using System.Collections;
using UnityEngine;

public class Bomb : Item
{
    public float damage = 2;
    public float knockbackmulti = 0.5f;

    public GameObject explosion;
    public override void Eat()
    {
        GameObject ex = Instantiate(explosion, transform.position, Quaternion.identity);
        ex.GetComponent<ExplosionScript>().preowner = preowner;
        base.Eat();
    }
    public override IEnumerator Attacking(GameObject target)
    {
        base.Attacking(target);
        GameObject ex = Instantiate(explosion, transform.position, Quaternion.identity);
        ex.GetComponent<ExplosionScript>().preowner = preowner;
        if (target.GetComponent<Entity>() != null)
        {
            target.GetComponent<Entity>().Attack(transform, damage, knockbackmulti);
        }
        Destroy(gameObject);
        yield return null;
        
    }
}
