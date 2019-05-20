using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject itemAreaPrefab;
    // Start is called before the first frame update
    void Start()
    {
        itemAreaPrefab = Resources.Load<GameObject>("Prefab/Item/ItemArea");
        Instantiate(itemAreaPrefab, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
 
        if (other.gameObject.tag == "Untagged")
        {
            
            if (Input.GetKey(KeyCode.G))
            {
                Destroy(gameObject);
            }
        }
    }

}
