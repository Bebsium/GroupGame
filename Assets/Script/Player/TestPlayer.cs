using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TestPlayer : Controller
{
    //可使继承用参数
    //public Global.Faction Faction     --玩家阵营
    //                                      Global.Faction.Player1
    //                                      Global.Faction.Player2
    //                                      Global.Faction.Computer
    //public bool hasDoll               --玩家是否拥有人偶
    //protected Rigidbody Rigi;         --Rigibody

    //可使用函数
    //SetFaction(nowFaction)            --重设玩家阵营

    public Global.Faction nowFaction;

    protected override void Start()
    {
        base.Start();
        Rigi.freezeRotation = true;
    }

    protected override void Loop()
    {
        //相当于Update
        SetFaction(nowFaction);
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
