using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_Cat : Doll
{
    public GameObject item;

    //攻擊類型
    public enum AttackState
    {
        StickAttack,
        HeavyAttack,
        CampstoolAttack,
    }

    //投擲類型
    public enum ThrowState
    {
        lightThrow,
        heavyThrow,
        stickThrow,
    }

    bool used;
    bool used_cd;
    float maxSoundTime;
    float cdTime;

    public AttackState attackState;
    public ThrowState throwState;

    public bool itemSetting;
    public bool isAttack;
    //KO值回傳-------------
    public float Knockout { set { _ko -= value; } }
    private float _ko;
    //--------------------

    AudioSource sound;

    public override bool PickItem(string name)
    {
        //print(Owner.playerName + " picked " + name);
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
        if (Input.GetKey(KeyCode.Alpha1))
        {
            if (used)
            {
                Hurt = 10 * 0.3f;
            }
            else
            {
                Hurt = 10f;
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            temp.transform.position = transform.position;
            temp.name = "Putted Item";
            temp.AddComponent<Item>().picked = true;
        }

        Skill();
    }

    private void OnTriggerStay(Collider other)
    {




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

    private float defence = 0f;

    void Skill()
    {

        if (!used && !used_cd)
        {
            SoundOn();     
        }

        if (used && !used_cd)
        {
            CoolDown();
        }

        if (used_cd)
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
            used_cd = false;
        }
    }

    private void CoolDown()
    {
        cdTime -= Time.deltaTime;
        //print("sound" + cdTime);
        if (cdTime <= 0)
        {
            print("stop");
            used = false;
            used_cd = true;
        }
    }

    private void SoundOn()
    {
        cdTime = Random.Range(1, 11); //1~10秒
        maxSoundTime = Random.Range(20, 30);
        print(maxSoundTime + "     " + cdTime);
        print("Play");
        used = true;
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
}
