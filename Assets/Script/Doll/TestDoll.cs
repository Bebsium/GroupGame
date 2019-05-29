using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override bool PickItem(string name)
    {
        print(Owner.Faction.ToString() + " picked " + name);
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
        if ((Input.GetKey(KeyCode.Alpha1) && Owner.Faction == Global.Faction.Player1)
            || (Input.GetKey(KeyCode.KeypadDivide) && Owner.Faction == Global.Faction.Player2))
        {
            Hurt = 1f;
        }

        if ((Input.GetKeyDown(KeyCode.Alpha2) && Owner.Faction == Global.Faction.Player1)
            || (Input.GetKeyDown(KeyCode.KeypadMultiply) && Owner.Faction == Global.Faction.Player2))
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            temp.transform.position = transform.position;
            temp.name = "Putted Item";
            temp.AddComponent<Item>().picked = true;
        }

        //Add Buff Test
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddBuff(Global.BuffSort.Invulnerable, 1f);
        }
        if (HasBuff(Global.BuffSort.Invulnerable))
        {
            print("Invulnerable");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            AddBuff(Global.BuffSort.Prisoner, 2f);
        }
        if (HasBuff(Global.BuffSort.Prisoner))
        {
            print("Prisoner");
        }
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
}
