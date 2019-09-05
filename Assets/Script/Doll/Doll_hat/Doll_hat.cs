using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_hat : Doll
{

    public override bool PickItem(string name)
    {
        //print(Owner.playerName + " picked " + name);
        item = Resources.Load<GameObject>("Prefab/Item/" + name);
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

    protected override void Move()
    {
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
    
    void ToMouse(){
        shootpoint = transform.position + transform.forward;
        lineRenderer.positionCount = numPoints;
        Ray ray=cam.ScreenPointToRay(Input.mousePosition);
        layer = 1<<0;
        if(Physics.Raycast(ray,out RaycastHit hit,Mathf.Infinity,layer)){
            shootRange.SetActive(true);
            shootRange.transform.position=hit.point+Vector3.up*0.1f;

            //射擊的位置

            Vector3 hight =(hit.point+transform.position)/2;
            hight+=new Vector3(0,line_hight,0);
  
            DrawLine(shootpoint,hight, shootRange.transform);
            transform.LookAt(shootRange.transform);
            float y = transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, y,0f);
        }

        isShoot = true;
    }

    void DrawLine(Vector3 origin,Vector3 hight, Transform target)
    {
        for (int i = 1; i < numPoints + 1; i++)
        {
            float sd = i / (float)numPoints;
            hatMove[i - 1] = CalculatQuadraticPoint(origin,hight, target.position,  sd);
        }
        lineRenderer.SetPositions(hatMove);
    }

    Vector3 CalculatQuadraticPoint(Vector3 origin,Vector3 hight, Vector3 target, float speed)
    {
       //return =  (1-t)2P0 + 2(1-t)tP1 + t2P2 
       float x=1-speed;
       float sd=speed*speed;
       float xx=x*x;
       Vector3 pos=xx*origin;
       pos+=2*x*speed*hight;
       pos+=sd*target;

       return pos;
    }
}
