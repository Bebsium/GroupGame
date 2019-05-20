using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    public bool autoRotation = false;
    public float autoRotationSpeed = 0.1f;
    public bool canRotation = false;
    public float minDistance = 5f;
    public float maxDistance = 20f;
    public float zoomFactor = 1.5f;
    private GameObject[] _target;
    private Camera _camera;

    private void Start()
    {
        _target = GameObject.FindGameObjectsWithTag("Player");
        _camera = Camera.main;
    }

    private void Update()
    {
        if(autoRotation) transform.RotateAround(Vector3.zero, Vector3.up, autoRotationSpeed);
        FixedCameraPosition();
    }

    public void FixedCameraPosition()
    {
        Vector3 midPoint = Vector3.zero;
        foreach(GameObject t in _target)
        {
            midPoint += t.transform.position;
        }
        midPoint /= _target.Length;

        if (canRotation && !autoRotation) _camera.transform.LookAt(midPoint, Vector3.up);

        float maxLength = 0f;
        foreach(GameObject t in _target)
        {
            float tl = Vector3.Distance(t.transform.position, midPoint);
            if (maxLength < tl)
                maxLength = tl;
        }
        maxLength *= 2;

        Vector3 forward = _camera.transform.forward;
        Vector3 cameraDestination = midPoint - forward * maxLength * zoomFactor;
        if (cameraDestination.magnitude < minDistance)
            cameraDestination = midPoint - forward * minDistance;
        else if(cameraDestination.magnitude > maxDistance)
            cameraDestination = midPoint - forward * maxDistance;

        _camera.transform.position = Vector3.Slerp(_camera.transform.position, cameraDestination, 0.1f);
        if (Vector3.Distance(_camera.transform.position, cameraDestination) <= 0.05f)
            _camera.transform.position = cameraDestination;
    }

    //public List<Transform> targets;
    //public Vector3 offset;

    //public float smoothTime=.5f;

    //public float minZoom = 70f;
    //public float maxZoom = 50f;
    //public float zoomLimiter =50f;

    //private Vector3 velocity;
    //private Camera cam;


    //private void Start()
    //{

    //    cam = GetComponent<Camera>();

    //}
    //void LateUpdate()
    //{
    //    if (targets.Count == 0)
    //        return;
    //    Move();
    //    Zoom();
    //}
    //void Zoom()
    //{
    //    float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
    //    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,newZoom,Time.deltaTime);
    //}
    //void Move()
    //{
    //    Vector3 centerPoint = GetCenterPoint();
    //    Vector3 newPosition = centerPoint + offset;
    //    transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    //}
    //float GetGreatestDistance()
    //{
    //    var bounds = new Bounds(targets[0].position, Vector3.zero);
    //    for(int i = 0; i < targets.Count; i++)
    //    {
    //        bounds.Encapsulate(targets[i].position);
    //    }
    //    return bounds.size.x;
    //}
    //Vector3 GetCenterPoint()
    //{
    //    if (targets.Count == 1)
    //    {
    //        return targets[0].position;

    //    }
    //    var bounds = new Bounds(targets[0].position, Vector3.zero);
    //    for(int i =0; i<targets.Count; i++)
    //    {
    //        bounds.Encapsulate(targets[i].position);
    //    }

    //    return bounds.center;
    //}
}
