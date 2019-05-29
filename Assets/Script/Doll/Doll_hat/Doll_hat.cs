using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_hat : Doll
{
    public GameObject hat;
    bool used;
    float time;
    float power;
    GameObject temp;

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

    void CreateHat()
    {
        temp = Instantiate(hat, transform);
        temp.GetComponent<Item_hat>().player = gameObject;
        temp.transform.position = transform.position + transform.forward;
        temp.GetComponent<Rigidbody>().AddForce((transform.forward + Vector3.up) * 500f * power);
        temp.transform.SetParent(null);
    }


    void Skill()
    {
        if (used)
        {
            time += 1 * Time.deltaTime;
            if (time >= 3.0f)
            {
                used = false;
                time = 0;
                power = 0;
            }
        }
        
        if (Input.GetKey(KeyCode.H) && !used)
        {
            power += 2 * Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.H) && !used)
        {
            used = true;
            CreateHat();
            power = 0;
  
        }

        //if (used)
        //{
        //    Instantiate(hat, transform);
        //}

        //time += 1 * Time.deltaTime;

        //if (time >= 3.0f)
        //{
        //    used = false;
        //    time = 0;
        //}

    }

    
}
