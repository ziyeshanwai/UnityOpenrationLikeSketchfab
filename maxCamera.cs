using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class maxCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    private Vector3 FirstPosition;
    private Vector3 SecondPosition;
    private Vector3 delta;
    private Vector3 lastOffset;
    private Vector3 lastOffsettemp;
    //private Vector3 CameraPosition;
    //private Vector3 Targetposition;
    //private Vector3 MoveDistance;


    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    /*
      * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
      */
    void LateUpdate()
    {
        // If Control and Alt and Middle button? ZOOM!
        if (Input.GetMouseButton(2))
        {
            desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
        }
        // If middle mouse and left alt are selected? ORBIT
        if (Input.GetMouseButton(0))
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation 


        }
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = transform.rotation;
        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
        transform.rotation = rotation;

        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        if (Input.GetMouseButton(2))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
//            target.rotation = transform.rotation;
//            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
//            target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
        }
        if (Input.GetMouseButtonDown (1)) //右键平移
        {
            FirstPosition = Input.mousePosition;
            lastOffset = targetOffset;
        }

        if (Input.GetMouseButton (1)) //右键键平移
        {
            SecondPosition = Input.mousePosition;
            delta = SecondPosition - FirstPosition;
            targetOffset = lastOffset + transform.right * delta.x*0.03f + transform.up * delta.y*0.03f;

        }

        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        position = target.position - (rotation * Vector3.forward * currentDistance );

        position = position - targetOffset;  

        transform.position = position;
       

       
       
    }
    private Vector3 Vec2toVec3(Vector2 Vec2)
    {
        return new Vector3(Vec2.x, Vec2.y, 0.0f);
    }
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}