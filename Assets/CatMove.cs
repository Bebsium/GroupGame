using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMove : MonoBehaviour
{
   
    Vector3 moveDir = Vector3.zero;
    CharacterController controller;
    Animator anim;
 
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();      
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) //落命
        {
            anim.SetBool("die", true);
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))//移动+拾取道具
        {
            anim.SetBool("run", true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                anim.SetBool("carry", true);               
            }
        }
        else
        {
            anim.SetBool("run", false);
        }
        if (Input.GetKeyDown(KeyCode.Space))// 跳
        {
            anim.SetTrigger("jump");
            Debug.Log("jump");
        }
        if (Input.GetMouseButtonDown(0))//攻击
        {
            anim.SetTrigger("attack");
            anim.SetBool("carry", false);
            Debug.Log("atk");
        }
        if (Input.GetKeyDown(KeyCode.E))//拾取
        {
            anim.SetBool("carry", true);
            
        }
        if (Input.GetKeyDown(KeyCode.R))//扔
        {
            anim.SetBool("carry", false);
            anim.SetTrigger("throw");      
            Debug.Log("throw");
        }
        if (Input.GetKeyDown(KeyCode.F))//技能
        {   
            anim.SetTrigger("Skill");
            Debug.Log("fuck");
        }

    }
}
