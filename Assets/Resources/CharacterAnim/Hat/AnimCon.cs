using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCon : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float speed_rot=20;
    Vector3 movement;
    Rigidbody playRigidbody;
    int floorMask;
    int Speed_rot;

    private int State;
    private int oldState = 0;
    private int UP = 0;
    private int RIGHT = 1;
    private int DOWN = 2;
    private int LEFT = 3;

    protected Animator animator;
    void Start()
    {
        floorMask = LayerMask.GetMask("Floor");
        animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        animator.SetFloat("Speed", 2.0f);

        //----------Walk---------------
        if (Input.GetKey("w"))
        {
            setState(UP);
        }
        else if (Input.GetKey("s"))
        {
            setState(DOWN);
        }
        if (Input.GetKey("a"))
        {
            setState(LEFT);
        }
        else if (Input.GetKey("d"))
        {
            setState(RIGHT);
        }
        if (!Input.GetKey("w") && !Input.GetKey("s") && !Input.GetKey("a") && !Input.GetKey("d"))
        {
            animator.SetBool("Walk", false);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("Jump", true);
        }
        else
        {
            animator.SetBool("Jump", false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetBool("Atk", true);
        }
        else
        {
            animator.SetBool("Atk", false);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetBool("Skill", true);
        }
        else
        {
            animator.SetBool("Skill", false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetBool("Throw", true);
            animator.SetBool("Carrying", false);
        }
        else
        {
            animator.SetBool("Throw", false);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetBool("Hthrow", true);
            animator.SetBool("Carrying", false);
        }
        else
        {
            animator.SetBool("Hthrow", false);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetBool("Wakeup", true);
            //animator.SetBool("Carrying", false);
        }
        else
        {
            animator.SetBool("Wakeup", false);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetBool("Down", true);
            //animator.SetBool("Carrying", false);
        }
        else
        {
            animator.SetBool("Down", false);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetBool("Knockdown", true);
            //animator.SetBool("Carrying", false);
        }
        else
        {
            animator.SetBool("Knockdown", false);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetBool("Hit_stun", true);
            //animator.SetBool("Carrying", false);
        }
        else
        {
            animator.SetBool("Hit_stun", false);
        }
        //Move();
    }
    void setState(int currState)
    {
        Vector3 transformValue = new Vector3();
        int rotateValue = (currState - State) * 90;
        animator.SetBool("Walk", true);
        //
        switch (currState)
        {
            case 0:
                transformValue = Vector3.forward * Time.deltaTime * moveSpeed;
                break;
            case 1:
                transformValue = Vector3.right * Time.deltaTime * moveSpeed;
                break;
            case 2:
                transformValue = Vector3.back * Time.deltaTime * moveSpeed;
                break;
            case 3:
                transformValue = Vector3.left * Time.deltaTime * moveSpeed;
                break;
        }
        //
        transform.Rotate(Vector3.up, rotateValue);
        transform.Translate(transformValue, Space.World);
        oldState = State;
        State = currState;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            animator.SetBool("Carrying", true);
        }
    }
}