using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_hat : MonoBehaviour
{
    public GameObject player;
    private GameObject rival;
    public bool isWear;
    int movepoint;
    Vector3[] hatMove;
    float speed =50;

    float skillAttack = 20;

    private bool istouch;
    private Rigidbody _rigidbody;
    private Doll_hat hatDoll;
    // Start is called before the first frame update
    void Start()
    {
        hatDoll=player.GetComponent<Doll_hat>();
        hatMove=hatDoll.itmeMove;
        _rigidbody=GetComponent<Rigidbody>();
        gameObject.tag = "Hat";
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        //如果碰到Doll，帽子座標=Doll座標
        if(isWear){
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
        
    }

    bool Move()
    {
        //帽子移動
        if (!istouch && !hatDoll.shoot)
        {
            _rigidbody.isKinematic=false;
            if(movepoint<=hatMove.Length-1){
                transform.position=Vector3.MoveTowards(transform.position,hatMove[movepoint],speed*Time.deltaTime);
        
                if(transform.position==hatMove[movepoint]){
                    movepoint++;
                }
            }
        }
        //順移
        if (Input.GetKeyDown(KeyCode.F))
        {
            return false;
        }
        return false;
    }

    Transform target;

    private void OnCollisionEnter(Collision collision)
    {
        if (!istouch)
        {
            if(collision.gameObject != player)
            {
                //target=任何物體
                target = collision.transform;
                //如果碰到Doll
                if (collision.gameObject.tag == "Doll")
                {
                    if (!isWear)
                    {
                        //wear
                        isWear = true;
                        _rigidbody.isKinematic=true;
                    }
                }
                istouch = true;
            }
        }
        Destroy(gameObject, 10f);
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

}
