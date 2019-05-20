using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    public string itemName;
    public bool picked = false;

    private GameObject itemAreaPrefab;
    
    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Item");
        transform.tag = "Item";
        itemAreaPrefab = Resources.Load<GameObject>("Prefab/Item/ItemArea");
        if(!picked)
            Instantiate(itemAreaPrefab, transform);
    }

    private void OnTriggerStay(Collider other)
    {
 
        if (other.gameObject.tag == "Doll")
        {
            if (Input.GetKey(KeyCode.G))
            {
                if (other.GetComponent<Doll>().PickItem(itemName))
                    Destroy(gameObject);
            }
        }
    }

}
