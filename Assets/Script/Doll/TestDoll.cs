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

    protected override void Start()
    {
        base.Start();
        Rigi.freezeRotation = true;
    }

    protected override void Loop()
    {
        //相当于Update
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Hurt = 1f;
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
