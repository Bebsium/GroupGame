using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeItem : MonoBehaviour
{
    public int durability;
    private GameObject _staticScene;
    public List<GameObject> itemList=new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        _staticScene=GameObject.Find("StaticScene");
        // for(int i=0;i<transform.childCount;i++){
        //     itemList.Add(transform.GetChild(i).gameObject);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if(durability<=0){
            for(int i=0;i<itemList.Count;i++){
                GameObject temp= Instantiate(itemList[i],transform.position,transform.rotation);
                temp.transform.parent=_staticScene.transform;
                if(i>=itemList.Count-1){
                    Destroy(gameObject);
                }
            }
        }
    }
    void OnCollisionEnter(Collision other)
    {
        //if(other.gameObject.GetComponent<Doll>().isAttack){
            durability--;
        //}
    }
}
