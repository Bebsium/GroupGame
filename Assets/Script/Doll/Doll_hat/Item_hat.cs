using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_hat : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SphereCollider sphereCollider=gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 2.0f;
            sphereCollider.isTrigger = true;
            
            StartCoroutine(Example());
            //Destroy(gameObject);
        }
    }

    void Damage()
    {
        Ray ray = new Ray(transform.position, Vector3.up);
        if (Physics.SphereCast(ray, 20,out RaycastHit hit))
        {
            if (hit.transform.tag == "Doll" && hit.transform.gameObject != player) {
                hit.transform.GetComponent<Doll>().Hurt = 10;
            }
        }
    }

    IEnumerator Example()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Damage();
        player.transform.position = transform.position;
        Destroy(gameObject);
    }
}
