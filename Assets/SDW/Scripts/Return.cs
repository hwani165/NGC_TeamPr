using UnityEngine;

public class Return : MonoBehaviour
{
    [SerializeField] private Transform[] responPos;
    [SerializeField] static public byte p1MaxLife = 3;
    [SerializeField] static public byte p2MaxLife = 3;

    // ¿”Ω√¿”
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer != LayerMask.NameToLayer("Player")) Destroy(collision.gameObject);
        else
        {
            if(collision.gameObject.name == "P1")
            {
                p1MaxLife--;
                if (p1MaxLife <= 0)
                {
                    string otherName = Server.Instance.GetOtherData().Value.nickname;
                    Game.Instance.EndGame(true, otherName);
                }
            }
            else
            {
                p2MaxLife--;
                if (p2MaxLife <= 0)
                {
                    string myName = Server.Instance.GetMyData().Value.nickname;
                    Game.Instance.EndGame(true, myName);
                }
            }

            int rand = Random.Range(0, responPos.Length);
            collision.transform.position = responPos[rand].position;
        }
    }
}
