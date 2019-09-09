using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    public string itemName;
    public string typeName;
    public bool picked = false;

    public int hurt = 10;
    public int throwHurt = 10;
    public int knockout = 10;
    public int durability = 2;
    public int explosion = 0;

    private GameObject itemAreaPrefab;
    private GameObject itemClone;

    public Transform unUsed;
    public Collider used;
    private Color color;
    private Color colTemp;

    public Transform player;
    private Rigidbody _rigidbody;
    public bool istouch;
    public bool item;
    int movepoint;
    public Vector3[] itemMove;
    float speed = 40;

    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Item");
        transform.tag = "Item";
        itemAreaPrefab = Resources.Load<GameObject>("Prefab/Item/ItemArea");
        _rigidbody=GetComponent<Rigidbody>();
        player = gameObject.transform.parent;
        istouch=false;
        if (!picked)
        {
            //remove itemArea
            
            itemClone = Instantiate(itemAreaPrefab, transform);
            if(typeName=="BoardHeavy"){
                itemClone.transform.localScale=itemAreaPrefab.transform.localScale*10;
            }
        }

        //Find Collider
        unUsed = transform.Find("Unused");
        used = gameObject.GetComponent<BoxCollider>();

        used.enabled=false;
        color = GetComponent<Renderer>().material.color;
        colTemp = color;
        color.a = 0.75f;
        colTemp.a = 0.1f;

    }

    private void Update()
    {
        if (picked)
        {
            _rigidbody.isKinematic=true;
            StartCoroutine(PickedChange());
        }

        if (item)
        {
            //player used item
            if (!isAttack)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isAttack = true;
                    //isAttack false
                    StartCoroutine(Wait());
                }
            }
            
            //throw
            if (Input.GetMouseButtonDown(1))
            {
                _rigidbody.isKinematic = true;
                used.enabled=true;
            }
            if(Input.GetMouseButtonUp(1)){
                isThrow =true;
                _rigidbody.isKinematic = false;
                unUsed.gameObject.SetActive(true);
                used.enabled=false;
            }
            if(isThrow && !istouch){
                StartCoroutine(Move());
            }
            if (istouch){
                itemClone = Instantiate(itemAreaPrefab, transform);
            }
            
        }

        if (durability <= 0)
        {
            //透明になって、消える
            //GetComponent<Renderer>().material.color = Color.Lerp(new Color(0, 0, 0, 0.75f), new Color(0,0,0,0.25f), Mathf.PingPong(Time.time, 0.75f));
            GetComponent<Renderer>().material.color = Color.Lerp(color, colTemp, Mathf.PingPong(Time.time, 0.5f));
            //GetComponent<Renderer>().material.color = Color.Lerp(color, colTemp, Mathf.Sin(Time.time)*1);
            //if(cor == null)
            //    cor =  StartCoroutine(ColorChange(1f));
            Destroy(gameObject,1);
            player.gameObject.GetComponent<Doll>().itemSetting = false;
        }


    }

    IEnumerator Move()
    {
        yield return new WaitForSeconds(0.25f);
        //item移動
        if (movepoint <= itemMove.Length - 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, itemMove[movepoint], speed * Time.deltaTime);
            if (transform.position == itemMove[movepoint])
            {
                movepoint++;
            }
        }

    }

    IEnumerator PickedChange()
    {
        yield return new WaitForSeconds(1);
        unUsed.gameObject.SetActive(false);
        used.enabled=true;
        picked = false;
        itemMove = player.GetComponent<Doll>().itmeMove;
        istouch = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Doll")
        {
            //get item
            if (Input.GetKeyDown(Key.Take))
            {
                if (other.GetComponent<Doll>().PickItem(itemName,typeName,durability))
                    Destroy(gameObject);
            }

            unUsed.gameObject.SetActive(true);
            used.enabled = false;
            
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != player)
        {
            isAttack = false;
        }
    }

    Transform target;
    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject != player.gameObject)
        {      
            istouch = true;
            isThrow=false;
        }
    }
    //-----------------------------------
    //透明になって、消える
    //private Coroutine cor;

    //IEnumerator ColorChange(float time)
    //{
    //    Color temp2 = new Color(0, 0, 0, 0.2f);
    //    while (time > 0)
    //    {
    //        yield return new WaitForSeconds(Time.deltaTime);
    //        GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color - temp2;
    //        if(GetComponent<Renderer>().material.color.a < 0.1f)
    //        {
    //            temp2 = new Color(0, 0, 0, -0.2f);
    //        }
    //        else if(GetComponent<Renderer>().material.color.a > 0.9f)
    //        {
    //            temp2 = new Color(0, 0, 0, 0.2f); 
    //        }
    //        time -= Time.deltaTime;
    //    }
    //    Destroy(gameObject);
    //}
    //---------------------------------

    public bool isAttack = false;
    public bool isThrow = false;
    //Attack
    public float Attack()
    {
        if (isAttack)
        {
            return hurt;
        }
        else
        {
            return 0;
        }
    }
    //Throw
    public float Throw()
    {
        if (isThrow)
        {
            return throwHurt;
        }
        else
        {
            return 0;
        }
    }

    public float Knockout()
    {
        if (isAttack || isThrow)
            {
                return knockout;
            }
            else
            {
                return 0;
            }
    }

    

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        durability--;
        isAttack = false;
    }
}
