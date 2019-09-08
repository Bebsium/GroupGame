using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Rose : MonoBehaviour
{

    public GameObject player;
    public GameObject explosion;
    int movepoint;
    Vector3[] roseMove;
    float speed =50;
    bool istouch;
    float skillAttack = 20;
    private Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        roseMove=player.GetComponent<Doll_Queen>().itmeMove;
        _rigidbody=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //rose move
        if (!istouch && !player.GetComponent<Doll>().shoot)
        {
            _rigidbody.isKinematic=false;
            if(movepoint<=roseMove.Length-1){
                transform.position=Vector3.MoveTowards(transform.position,roseMove[movepoint],speed*Time.deltaTime);
        
                if(transform.position==roseMove[movepoint]){
                    movepoint++;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject != player)
        {
            //碰到物體時，爆炸
            Instantiate(explosion, transform);
            float r = 10;
            var cols = Physics.OverlapSphere(transform.position, r);
            foreach (var col in cols)
            {
                if (col.gameObject.tag == "Doll" && col.gameObject != player)
                {
                    //碰到對手時，
                    isAttack=true;
                }
            }
            Destroy(gameObject,0.5f);
        }

        //OnDrawGizmos();
    }

    public bool isAttack = false;

    public float SkillAttack()
    {
        if (isAttack)
        {
            return skillAttack;
        }
        else
        {
            return 0;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(this.transform.position,10);
    }
}
