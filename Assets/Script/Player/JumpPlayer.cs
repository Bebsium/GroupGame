using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JumpPlayer : Controller
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
    public float gra = 8;
    public bool canJump = true;
    public float jumpForce;
    public Global.Faction nowFaction;

    protected override void Start()
    {
        base.Start();
        SetFaction(nowFaction);
        Rigi.freezeRotation = true;
    }

    protected override void Loop()
    {
        //相当于Update
        SetFaction(nowFaction);
        Rigi.AddForce(-Vector3.up * gra);
        print("Gravity force "+ gra);
    }

    protected override void Move()
    {
        ////重写移动
        //if(Faction == Global.Faction.Player1)
        //{
        //    float h=0, v=0;
        //    if (Input.GetKey(Global.Key.Forward))
        //    {
        //        v = 1;
        //    }else if (Input.GetKey(Global.Key.Back))
        //    {
        //        v = -1;
        //    }

        //    if (Input.GetKey(Global.Key.Right))
        //    {
        //        h = 1;
        //    }else if (Input.GetKey(Global.Key.Left))
        //    {
        //        h = -1;
        //    }

        //    float y = Camera.main.transform.rotation.eulerAngles.y;
        //    Vector3 targetDir = new Vector3(h, 0, v).normalized;
        //    Vector3 tempTarget = Vector3.Lerp(transform.forward, targetDir, 0.1f);
        //    float spd = Time.deltaTime * Global.Parameter.SoulSPD;
        //    if (Vector3.Dot(targetDir, tempTarget)<0)
        //    {
        //        spd *= 0.2f;
        //    }
        //    tempTarget = Quaternion.Euler(0, y, 0) * tempTarget;
        //    //transform.LookAt(tempTarget + transform.position);
        //    transform.Translate(targetDir * spd, Space.World);

        //}else if(Faction == Global.Faction.Player2)
        //{

        //}

        base.Move();
    }

    protected override void Jump()
    {
        //重写跳跃
        if (canJump && Input.GetKeyDown(KeyCode.Space) && Faction == Global.Faction.Player1)
        {
            Rigi.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
        }

        if (canJump && Input.GetKeyDown(KeyCode.Keypad0) && Faction == Global.Faction.Player2)
        {
            Rigi.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        canJump = true;
    }
}