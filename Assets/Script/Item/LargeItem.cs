using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeItem : MonoBehaviour
{
    public int durability;
    public List<GameObject> itemList=new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<transform.childCount;i++){
            itemList.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(durability<=0){
            foreach (var index in itemList)
            {
                index.transform.parent=null;
                //index.transform.GetComponent<BoxCollider>().enabled=true;
            }
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Doll")
        {
            if(other.gameObject.GetComponent<Doll>().isAttack){
                durability--;
            }
            
        }
    }
}
