using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDoll : Doll
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

    public AttackState attackState;
    public ThrowState throwState;

    public bool itemSetting;
    public bool isAttack;
    //KO值回傳-------------
    public float Knockout { set { _ko -= value; } }
    private float _ko;
    //--------------------

    //可使继承用参数
    //public float Hurt                 --收到伤害值
    //protected int DamagedNumber;      --人偶被摧毁次数
    //protected Controller Owner;       --人偶拥有者
    //protected Rigidbody Rigi;         --Rigibody
    //protected float ATK;              --现攻击力
    //protected float HP;               --现生命值
    //protected float SPD;              --现速度

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
        //初始KO值
        _ko = 100;

        // throw
        cam=Camera.main;
        shootRange = Instantiate(cursor, transform);
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.material = line;
    }

    protected override void Loop()
    {
        //相当于Update
        if (itemSetting)
        {
            ItemType();
        }
 
        //attack && ItemAttack
        Attack();

    

        
        //skillHurt
        SkillHurt();

        //添加禁锢效果
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddBuff(Global.BuffSort.Prisoner, 2f);
        }
        if (HasBuff(Global.BuffSort.Prisoner))
        {
            print("Prisoner");
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            print("AttributePromotion");
            AttributePromotion(10, 10, 10, 5);
        }


    }

    bool used;
    float time;
    Vector3 shootpoint;
    public GameObject cursor;
    public LayerMask layer;
    private Camera cam;
    public GameObject shootRange;
    public Vector3[] itmeMove = new Vector3[50];
    int numPoints = 50;
    LineRenderer lineRenderer;
    public Material line;
    public float line_hight = 10;
    bool isShoot;

    //attack-------------------
    [SerializeField]
    public GameObject temp;
    private void Attack()
    {
        //右クリックすると、shootRange出てくる
        if (!Input.GetMouseButton(1))
        {
            shootRange.SetActive(false);
        }

        if (item != null)
        {
            //GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            temp = Instantiate(item, gameObject.transform);
            temp.transform.position = transform.position + transform.forward*0.5f;
            temp.name = "Putted Item";
            StartCoroutine(NotFall(temp));
            itemSetting = true;
            
            temp.GetComponent<Item>().picked = true;
            temp.GetComponent<Item>().item = true;
            item = null;
            used = true;
        }
        else
        {
            if(temp == null){
                itemSetting = false;
                //attack
                if (Input.GetMouseButtonDown(0) && !isAttack)
                {
                    isAttack = true;
                    //ものをあってた場合は1秒過ぎるとhurtなし
                    StartCoroutine(Wait());
                }

            }
            //throw
            if(Input.GetMouseButton(1) && used)
            {
                //射擊方向
                ToMouse();
            }
        
            //丟道具
            if (Input.GetMouseButtonUp(1) && used)
                {
                    used = false;
                    isShoot = false;
                    temp.transform.SetParent(null);
                    temp=null;
                    lineRenderer.positionCount = 0;
                    
                }
        }
        
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        if(item != null)
        {
            temp.GetComponent<Item>().isAttack = false;
        }
        else
        {
            isAttack = false;
        }
        
    }
    //Item attack--------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Item" && other.gameObject != temp)
        {
            target_Item = other.gameObject.GetComponent<Item>();
            if (target_Item.isAttack)
            {
                //print(other.gameObject);
                //碰到道具 -HP -KO
                Hurt = target_Item.Attack();
                Knockout = target_Item.Knockout();
                target_Item.isAttack = false;
            }

        }
        if (other.gameObject.tag == "DollHand")
        {
            Hurt = 5;
            isAttack = false;
        }

    }


 //-------------------------------------------

Item_hat target_Hat;
Item_Rose target_Rose;
Item target_Item;

//-------------------------------------------
    private void OnCollisionEnter(Collision collision)
    {
        //Doll_hot 以外的其他玩偶共通-----
        if (collision.gameObject.tag == "Hat")
        {
            target_Hat = collision.gameObject.GetComponent<Item_hat>();

            if(target_Hat.isWear){
                //碰到對方時，產生效果
                AddBuff(Global.BuffSort.Prisoner, 2f);
                //print("AddBuff");
            }
        }
        //---------------------------------
        //Doll_Rose 以外的其他玩偶共通-----
        if (collision.gameObject.tag == "Rose")
        {
            target_Rose = collision.gameObject.GetComponent<Item_Rose>();

            if(target_Rose.isAttack){
                //碰到對方時，產生效果
                Hurt = target_Rose.SkillAttack();
                AddBuff(Global.BuffSort.Stun,5f);
                //print("AddBuff");
            }
        }
        //------------------------------
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



    //Attack
    void StickAttack()
    {
        
    }

    void HeavyAttack()
    {

    }

    void CampstoolAttack()
    {

    }

    //Throw
    void LightThrow()
    {

    }

    void HeavyThrow()
    {

    }

    void StickThrow()
    {

    }

    //射擊---------------------------------
    void ToMouse()
    {
        shootpoint = transform.position + transform.forward;
        lineRenderer.positionCount = numPoints;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        layer = 1 << 0;
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
        {
            shootRange.SetActive(true);
            shootRange.transform.position = hit.point + Vector3.up * 0.1f;

            //射擊的位置

            Vector3 hight = (hit.point + transform.position) / 2;
            hight += new Vector3(0, line_hight, 0);

            DrawLine(shootpoint, hight, shootRange.transform);
            transform.LookAt(shootRange.transform);
        }

        isShoot = true;
    }

    void DrawLine(Vector3 origin, Vector3 hight, Transform target)
    {
        for (int i = 1; i < numPoints + 1; i++)
        {
            float sd = i / (float)numPoints;
            itmeMove[i - 1] = CalculatQuadraticPoint(origin, hight, target.position, sd);
        }
        lineRenderer.SetPositions(itmeMove);
    }

    Vector3 CalculatQuadraticPoint(Vector3 origin, Vector3 hight, Vector3 target, float speed)
    {
        //return =  (1-t)2P0 + 2(1-t)tP1 + t2P2 
        float x = 1 - speed;
        float sd = speed * speed;
        float xx = x * x;
        Vector3 pos = xx * origin;
        pos += 2 * x * speed * hight;
        pos += sd * target;

        return pos;
    }
    //---------------------------------------

   
    IEnumerator NotFall(GameObject temp)
    {
        //防止落下
        yield return new WaitForSeconds(1f);
        temp.GetComponent<Rigidbody>().isKinematic = true;
    }


    private void ItemType(string name)
    {
        if (name == "Stick")
        {
            attackState = AttackState.StickAttack;
            throwState = ThrowState.lightThrow;
        }
        else if (name == "Nightstand")
        {
            attackState = AttackState.HeavyAttack;
            throwState = ThrowState.heavyThrow;
        }
        else if (name == "Pillow" || name == "Board")
        {
            attackState = AttackState.CampstoolAttack;

            if (name == "Pillow")
            {
                throwState = ThrowState.heavyThrow;
            }
            else
            {
                throwState = ThrowState.stickThrow;
            }
        }
    }

    private void ItemType()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            switch (attackState)
            {
                case AttackState.StickAttack:
                    StickAttack();
                    break;
                case AttackState.HeavyAttack:
                    HeavyAttack();
                    break;
                case AttackState.CampstoolAttack:
                    CampstoolAttack();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            switch (throwState)
            {
                case ThrowState.lightThrow:
                    LightThrow();
                    break;
                case ThrowState.heavyThrow:
                    HeavyThrow();
                    break;
                case ThrowState.stickThrow:
                    StickThrow();
                    break;
            }
        }
    }
}
