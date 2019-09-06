using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public class Doll_Queen : Doll
{


    public override bool PickItem(string name,string type,int durability)
    {
        //print(Owner.playerName + " picked " + name);
        anim.SetBool("carry", true);
        item = Resources.Load<GameObject>("Prefab/Item/" + name);
        item.GetComponent<Item>().durability=durability;
        //Item種類を付ける
        ItemType(type);
        return true;
    }

    protected override void Start()
    {
        base.Start();
        Rigi.freezeRotation = true;
    }

    protected override void Loop()
    {

        //相当于Update
        //attack && ItemAttack
        Attack();
 
        Skill();

    }

    protected override void Move()
    {
        //重写移动
        base.Move();
    }

    protected override void Jump()
    {
        //重写跳跃
        base.Jump();
    }
    protected override void Attack()
    {
        base.Attack();
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Hurt = 1f;
        }
    }

    public GameObject rose;
    int usedCount;
    float rose_cd=5f;
    bool rose_reset;

    void Skill()
    {
        if (rose_reset)
        {
            //cool down
            rose_cd-=Time.deltaTime;
            if(rose_cd<=0){
                rose_cd=5;
                usedCount=2;
                rose_reset=false;
            }
        }
        
        //右クリックすると、shootRange出てくる
        if(!Input.GetKey(KeyCode.F)){
            shootRange.SetActive(false);
        }
        if(Input.GetKey(KeyCode.F) && !rose_reset)
        {
            //射擊方向
            ToMouse();
            
        }
        
        //道具生成
        if (Input.GetKeyUp(KeyCode.F) && !rose_reset)
            {
                isShoot = false;
                if (usedCount <= 3 && !rose_reset)
            {
                usedCount++;

                if(usedCount==3){
                    rose_reset=true;
                }
            }
                CreateRose();
            }

    }

    void CreateRose()
    {
        var temp=Instantiate(rose,shootpoint,Quaternion.identity);
        temp.GetComponent<Item_Rose>().player = gameObject;
        temp.transform.SetParent(null);
        lineRenderer.positionCount = 0;
    }

    //射擊---------------------------------
    protected override void ToMouse()
    {
        base.ToMouse();
    }

    protected override void DrawLine(Vector3 origin, Vector3 hight, Transform target)
    {
        base.DrawLine(origin, hight, target);
    }

    protected override Vector3 CalculatQuadraticPoint(Vector3 origin, Vector3 hight, Vector3 target, float speed)
    {
        return base.CalculatQuadraticPoint(origin, hight, target, speed);
    }
    //---------------------------------------


    protected override IEnumerator NotFall(GameObject temp)
    {
        return base.NotFall(temp);
    }


    protected override void ItemType(string name)
    {
        base.ItemType(name);
    }

}