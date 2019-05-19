using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingBillboard : MonoBehaviour
{
    private Transform _camera;

    private void Start()
    {
        _camera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _camera.rotation * Vector3.forward,
            _camera.rotation * Vector3.up);
    }
}
