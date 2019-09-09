using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //public bool run = false;
    //public Vector3 ori;
    //public Vector3 axis;
    //public float rotateSpeed;

    //public float CameraMoveSpeed = 120f;
    //public GameObject CameraFollowObj;
    //Vector3 FollowPOS;
    //public float clampAngle = 80.0f;
    //public float inputSensitivty = 150.0f;
    //public GameObject CameraObj;
    //public GameObject PlayerObj;
    //public float camDistanceXToPlayer;
    //public float camDistanceYToPlayer;
    //public float camDistanceZToPlayer;
    //public float mouseX;
    //public float mouseY;
    //public float finalInputX;
    //public float finalInputZ;
    //public float smoothX;
    //public float smoothY;
    //private float rotY = 0.0f;
    //private float rotX = 0.0f;

    //public static CameraFollow instance;


    //// Start is called before the first frame update
    //void Awake()
    //{
    //    if (instance != null)
    //        Destroy(instance.gameObject);
    //    instance = this;

    //    Vector3 rot = transform.localRotation.eulerAngles;
    //    rotY = rot.y;
    //    rotX = rot.x;
    //    //Cursor.lockState = CursorLockMode.Locked;
    //    //Cursor.visible = false;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (run)
    //    {
    //        //rotation
    //        float inputX = Input.GetAxis("RightStickHorizontal");
    //        float inputZ = Input.GetAxis("RightStickVertical");
    //        mouseX = Input.GetAxis("Mouse X");
    //        mouseY = Input.GetAxis("Mouse Y");
    //        finalInputX = inputX + mouseX;
    //        finalInputZ = inputZ + mouseY;

    //        rotY += finalInputX * inputSensitivty * Time.deltaTime;
    //        rotX += finalInputZ * inputSensitivty * Time.deltaTime;
    //        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
    //        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
    //        transform.rotation = localRotation;
    //    }
    //}

    //void LateUpdate()
    //{
    //    if (run)
    //        CameraUpdater();
    //    else
    //        CameraRotate();
    //}
    //void CameraUpdater()
    //{
    //    if (CameraFollowObj == null)
    //        return;
    //    Transform target = CameraFollowObj.transform;
    //    float step = CameraMoveSpeed * Time.deltaTime;
    //    transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    //}

    //void CameraRotate()
    //{
    //    transform.RotateAround(ori, axis, rotateSpeed);
    //    transform.LookAt(ori);
    //    transform.position += new Vector3(0, -1, 0) * Time.deltaTime;
    //}

    public static CameraFollow instance;

    public void Init(Transform tar)
    {
        target = tar;
        //transform.position = target.position;
        StartCoroutine(RotateAroundSphere());
    }

    private Transform target;
    Transform _camera;
    [SerializeField]
    float _radiusStandard;
    [SerializeField]
    float _radius;
    float _angle;
    [SerializeField]
    float _rcs;
    [SerializeField]
    float _acs;
    Vector3 pos;

    private float mouseX;
    private float mouseY;
    private float finalInputX;
    private float finalInputZ;
    private float smoothX;
    private float smoothY;
    private float rotY = 0.0f;
    private float rotX = 0.0f;
    private float clampAngle = 80.0f;
    private float inputSensitivty = 150.0f;

    private bool start = false;

    private void Start()
    {
        instance = this;
        _camera = Camera.main.transform;
        //_camera.position = new Vector3(0, _radius, 0);
    }

    private void Update()
    {
        if (start)
        {
            transform.position = target.position;
            OriRotate();
            if (Physics.SphereCast(transform.position, 0.5f, -transform.forward, out RaycastHit hit, _radiusStandard))
            {
                pos = hit.point;
            }
            else
            {
                pos = transform.position - transform.forward * _radiusStandard;
            }
        }
            
    }

    private void LateUpdate()
    {
        _camera.LookAt(transform.position);
        _camera.position = Vector3.Lerp(_camera.position, pos, Time.deltaTime);
    }

    private void OriRotate()
    {
        float inputX = Input.GetAxis("RightStickHorizontal");
        float inputZ = Input.GetAxis("RightStickVertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        finalInputX = inputX + mouseX;
        finalInputZ = inputZ + mouseY;

        rotY += finalInputX * inputSensitivty * Time.deltaTime;
        rotX += finalInputZ * inputSensitivty * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    private IEnumerator RotateAroundSphere()
    {
        pos.y = _radius;
        while (_angle < 10)
        {
            print(_angle);
            yield return new WaitForSeconds(Time.deltaTime);
            pos.x = _radius * Mathf.Cos(_angle);
            pos.z = _radius * Mathf.Sin(_angle);
            pos.y -= Time.deltaTime * _rcs;
            //pos += target.position;
            //_radius -= Time.deltaTime * _rcs;
            _angle += Time.deltaTime * _acs;
            //_camera.localPosition = pos;
        }
        start = true;
    }
}