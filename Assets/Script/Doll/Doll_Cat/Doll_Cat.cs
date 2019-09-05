using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public class Doll_Cat : Doll
{
    float maxSoundTime;
    float cdTime;

    AudioSource sound;

    public override bool PickItem(string name,string type,int durability)
    {
        //print(Owner.playerName + " picked " + name);
        item = Resources.Load<GameObject>("Prefab/Item/" + name);
        item.GetComponent<Item>().durability=durability;
        //Item種類を付ける
        ItemType(type);
        return true;
    }

    public override float Hurt { set => base.Hurt = value*(1-defence); }

    protected override void Start()
    {
        base.Start();
        Rigi.freezeRotation = true;
       // sound= GetComponent<AudioSource>();
    }

    protected override void Loop()
    {

        //相当于Update
        if (itemSetting)
        {
            
        }

        //attack && ItemAttack
        Attack();

        Skill();
    }

    protected override void Attack()
    {
        base.Attack();
        if (Input.GetKey(Key.Skill))
        {
            if (usedSkill)
            {
                Hurt = 10 * 0.3f;
            }
            else
            {
                Hurt = 10f;
            }
            
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

    private float defence = 0f;

    void Skill()
    {

        if (!usedSkill && !usedSkill_cd)
        {
            SoundOn();     
        }

        if (usedSkill && !usedSkill_cd)
        {
            CoolDown();
        }

        if (usedSkill_cd)
        {
            SoundOff();
        }
    }

    private void SoundOff()
    {
        maxSoundTime -= Time.deltaTime;
        //print("CD" + maxSoundTime);
        if (maxSoundTime <= 0)
        {
            usedSkill_cd = false;
        }
    }

    private void CoolDown()
    {
        cdTime -= Time.deltaTime;
        //print("sound" + cdTime);
        if (cdTime <= 0)
        {
            print("stop");
            usedSkill = false;
            usedSkill_cd = true;
        }
    }

    private void SoundOn()
    {
        cdTime = Random.Range(1, 11); //1~10秒
        maxSoundTime = Random.Range(20, 30);
        print(maxSoundTime + "     " + cdTime);
        print("Play");
        usedSkill = true;
        StartCoroutine(AbilityUp(maxSoundTime));
        
    }

    IEnumerator AbilityUp(float time)
    {

        //buff add
        AddBuff(Global.BuffSort.Invulnerable, maxSoundTime);
        defence = 0.5f;
        //add ATK.HP.SPD
        AttributePromotion(30, 30, 30, maxSoundTime);
        yield return new WaitForSeconds(time);
        //buff remove
        RemoveBuff(Global.BuffSort.Invulnerable);
        defence = 0f;
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
