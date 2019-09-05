using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_Queen : Doll
{


    public override bool PickItem(string name)
    {
        //print(Owner.playerName + " picked " + name);
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
        base.Move();
    }

    protected override void Jump()
    {
        //重写跳跃
        base.Jump();
    }

    public GameObject rose;
    GameObject temp;
    int usedCount;
    float rose_cd=5f;
    bool rose_reset;
    bool used;
    float time;
    Vector3 shootpoint;
    public GameObject cursor;
    public LayerMask layer;
    private Camera cam;
    public GameObject shootRange;
    public Vector3[] roseMove = new Vector3[50];
    int numPoints = 50;
    LineRenderer lineRenderer;
    public Material line;
    public float line_hight=10;
    bool isShoot;

    void Skill()
    {
        if (rose_reset)
        {
            //cool down
            rose_cd-=Time.deltaTime;
            if(rose_cd<=0){
                rose_cd=5;
                usedCount=2;
                rose_reset=false;
            }
        }
        
        //右クリックすると、shootRange出てくる
        if(!Input.GetMouseButton(1)){
            shootRange.SetActive(false);
        }
        if(Input.GetMouseButton(1) && !rose_reset)
        {
            //射擊方向
            ToMouse();
            
        }
        
        //道具生成
        if (Input.GetMouseButtonUp(1) && !rose_reset)
            {
                isShoot = false;
                if (usedCount <= 3 && !rose_reset)
            {
                usedCount++;

                if(usedCount==3){
                    rose_reset=true;
                }
            }
                CreateRose();
            }

    }

    void CreateRose()
    {
        var temp=Instantiate(rose,shootpoint,Quaternion.identity);
        temp.GetComponent<Item_Rose>().player = gameObject;
        temp.transform.SetParent(null);
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
        }

        isShoot = true;
    }

    void DrawLine(Vector3 origin,Vector3 hight, Transform target)
    {
        for (int i = 1; i < numPoints + 1; i++)
        {
            float sd = i / (float)numPoints;
            roseMove[i - 1] = CalculatQuadraticPoint(origin,hight, target.position,  sd);
        }
        lineRenderer.SetPositions(roseMove);
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