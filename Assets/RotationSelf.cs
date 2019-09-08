using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSelf : MonoBehaviour
{
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 60f;
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(new Vector3(0, 0, -1));
    }
}
