using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public class TestDoll : Doll
{

    //可使继承用参数
    //public float Hurt                 --收到伤害值
    //protected int DamagedNumber;      --人偶被摧毁次数
    //protected Controller Owner;       --人偶拥有者
    //protected Rigidbody Rigi;         --Rigibody
    //protected float ATK;              --现攻击力
    //protected float HP;               --现生命值
    //protected float SPD;              --现速度

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
        if (itemSetting)
        {
           
        }
 
        //attack && ItemAttack
        Attack();

        //添加禁锢效果
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddBuff(Global.BuffSort.Prisoner, 2f);
        }
        if (HasBuff(Global.BuffSort.Prisoner))
        {
            print("Prisoner");
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            print("AttributePromotion");
            AttributePromotion(10, 10, 10, 5);
        }


    }

    protected override void Attack()
    {
        base.Attack();
    }

    protected override IEnumerator WaitAttack()
    {
        return base.WaitAttack(); 
    }
    //Item attack--------------------------
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }



    protected override void Move()
    {

        if (HasBuff(Global.BuffSort.Prisoner))
            return;
        //重写移动
        if (!isShoot)
        {
            base.Move();
        }
    }

    protected override void Jump()
    {
        //重写跳跃
        base.Jump();
    }




    //射擊---------------------------------
    protected override void ToMouse()
    {
        base.ToMouse();
    }

    protected override  void DrawLine(Vector3 origin, Vector3 hight, Transform target)
    {
        base.DrawLine(origin, hight, target);
    }

    protected override  Vector3 CalculatQuadraticPoint(Vector3 origin, Vector3 hight, Vector3 target, float speed)
    {
        return base.CalculatQuadraticPoint(origin,hight,target,speed);
    }
    //---------------------------------------


    protected override  IEnumerator NotFall(GameObject temp)
    {
        return base.NotFall(temp);
    }


    protected override void ItemType(string name)
    {
        base.ItemType(name);
    }

}
