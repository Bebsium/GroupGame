using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDoll : Doll
{
    public GameObject groundCheck;
    public Rigidbody2D playerRd;
    public bool canJump = true;
    public float jumpForce;


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
    }

    protected override void Move()
    {
        //重写移动
        base.Move();
    }

    protected override void Jump()
    {
        //重写跳
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            Rigi.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
        }
        

    }
    //private bool IsJump()
    //{
    //    bool isjump = false;
    //    Physics.Linecast(transform.position-Vector3.up,)
    //    isjump= Physics2D.Linecast(playerRd.transform.position, groundCheck.transform.position, 1 << LayerMask.NameToLayer("Ground"));
    //    Debug.Log(isjump);
    //    return isjump;
    //}

    private void OnCollisionEnter(Collision collision)
    {
        //if(other.tag == )
        canJump = true;
    }

}