using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public class Doll_hat : Doll
{

    public override bool PickItem(string name,string type,int durability)
    {
        //print(Owner.playerName + " picked " + name);
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

        //skillHurt
        SkillHurt();
        

        Skill();
    }

    protected override void Attack()
    {
        base.Attack();
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Hurt = 1f;
        }
    }

    protected override IEnumerator Wait()
    {
        return base.Wait();
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


    private void SkillHurt()
    {
        if (target_Hat != null)
        {
            if (target_Hat.isAttack)
            {
                AddBuff(Global.BuffSort.Stun, 5f);
                Hurt = target_Hat.SkillAttack();
                target_Hat.isAttack = false;
            }
        }
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
    public GameObject hat;
    
    public GameObject hatCone;

    void Skill()
    {
        if (usedSkill)
        {
            //cool down
            time += Time.deltaTime; 
            if (time >= 3.0f)
            {
                usedSkill = false;
                time = 0;
            }
        }

        //Fを押すと、shootRange出てくる
        if(!Input.GetKey(KeyCode.F)){
            shootRange.SetActive(false);
        }
        if(Input.GetKey(KeyCode.F) && !usedSkill)
        {
            //射擊方向
            ToMouse();
        }
        
        //使用道具
        if (Input.GetKeyUp(KeyCode.F) && !usedSkill)
            {
                usedSkill = true;
                isShoot = false;
                CreateHat();
            }


        //順移
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (hatCone != null)
            {
                StartCoroutine(UseSkill());
            }
            

        }
    }


    IEnumerator UseSkill()
    {
        var temp = hatCone.GetComponent<Item_hat>();
        temp.isAttack = true;
        yield return new WaitForSecondsRealtime(0.2f);
        
        //無敵;
        transform.position = hatCone.transform.position;
        AddBuff(Global.BuffSort.Invulnerable, 5);
        Destroy(hatCone.gameObject);
    }

    void CreateHat()
    {
        hatCone =Instantiate(hat,shootpoint,Quaternion.identity);
        hatCone.GetComponent<Item_hat>().player = gameObject;
        hatCone.transform.SetParent(null);
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
