using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public class Doll_hat : Doll
{

    public override bool PickItem(string name)
    {
        //print(Owner.playerName + " picked " + name);
        item = Resources.Load<GameObject>("Prefab/Item/" + name);
        //Item種類を付ける
        ItemType(name);
        return true;
    }

    protected override void Start()
    {
        base.Start();
        Rigi.freezeRotation = true;

        cam=Camera.main;
        shootRange=Instantiate(cursor,transform);

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.material=line;
    }

    protected override void Loop()
    {
        //相当于Update
        if (itemSetting)
        {
            ItemUse(attackState, throwState);
        }

        //attack && ItemAttack
        Attack();

        //skillHurt
        SkillHurt();
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Hurt = 1f;
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

    protected override void Attack()
    {
        base.Attack();
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


    protected override void SkillHurt()
    {
        base.SkillHurt();
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
    bool used;
    float time;
    Vector3 shootpoint;
    public GameObject cursor;
    public LayerMask layer;
    private Camera cam;
    public GameObject shootRange;
    public Vector3[] hatMove = new Vector3[50];
    int numPoints = 50;
    LineRenderer lineRenderer;
    public Material line;
    public float line_hight=10;
    bool isShoot;
    public GameObject hatCone;

    void Skill()
    {
        if (used)
        {
            //cool down
            time += Time.deltaTime; 
            if (time >= 3.0f)
            {
                used = false;
                time = 0;
            }
        }

        //右クリックすると、shootRange出てくる
        if(!Input.GetMouseButton(1)){
            shootRange.SetActive(false);
        }
        if(Input.GetMouseButton(1) && !used)
        {
            //射擊方向
            ToMouse();
        }
        
        //丟道具
        if (Input.GetMouseButtonUp(1) && !used)
            {
                used = true;
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

    protected override void ItemUse(AttackState attackState, ThrowState throwState)
    {
        base.ItemUse(attackState, throwState);
    }
}
