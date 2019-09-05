using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMove : MonoBehaviour
{
    bool alive = true;
    float speed = 4;
    float rotSpeed = 82;
    float gravity = 8;
    float rot = 0f;
    Vector3 moveDir = Vector3.zero;
    CharacterController controller;
    Animator anim;
    Rigidbody m_rigidbody;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!alive)
            return;
        if (Input.GetKeyDown(KeyCode.P))
        {
            anim.SetBool("die", true);
            alive = false;
            m_rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX;
            m_rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY;
            m_rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            anim.SetBool("run", true);
        }
        else
        {
            anim.SetBool("run", false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("jump");
        }
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("attack");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("throw");
        }


    }
}
