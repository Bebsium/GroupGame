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
    private CharacterController cc;
    public float speed = 0;
    public Global.Faction nowFaction;

    protected override void Start()
    {
        cc = GetComponent<CharacterController>();
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
        if(Faction == Global.Faction.Player1)
        {
            float h=0, v=0;
            if (Input.GetKey(Global.Key.Forward))
            {
                v = 1;
            }else if (Input.GetKey(Global.Key.Back))
            {
                v = -1;
            }

            if (Input.GetKey(Global.Key.Right))
            {
                h = 1;
            }else if (Input.GetKey(Global.Key.Left))
            {
                h = -1;
            }
            
            //float h = Input.GetAxis("Horizontal");
            //float v = Input.GetAxis("Vertical");
            if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1)
            {
                Vector3 targetDir = new Vector3(h, 0, v);
                transform.LookAt(targetDir + transform.position);
                cc.SimpleMove(transform.forward * speed);
            }
        }else if(Faction == Global.Faction.Player2)
        {

        }
        
        //base.Move();
    }

protected override void Jump()
    {
        //重写跳跃
        base.Jump();
    }
}
