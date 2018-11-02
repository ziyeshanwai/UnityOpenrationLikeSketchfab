using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchOp : MonoBehaviour {

    private float pinchZoomSensitivity = 0.5f;
    private float MoveSpeed =0.2f;//平移灵敏度
    private float yawSensitivity = 10.0f;
    private float pitchSensitivity = 12.0f;
    private float DistanceBetweenCamAndRef = 10.0f;
    public GameObject LookAtGameObject;
    void Start()
    {
        Screen.fullScreen = true;
        Screen.SetResolution(1920, 1080, true);
        InitialRotationOperation(LookAtGameObject);
    }
    #region Gesture Event Messages

    float nextDragTime = 0.0f;
    void OnDrag( DragGesture gesture )
    {
        
        if (gesture.Fingers.Count == 1)
        {
            Ray ray = Camera.main.ScreenPointToRay (Vec2toVec3(gesture.StartPosition));  //摄像机需要设置MainCamera的Tag这里才能找到  
            RaycastHit hitInfo;
            if (Physics.Raycast (ray, out hitInfo)) {  
                Vector3 hitPoint = hitInfo.point;//返回的是世界坐标系的坐标  

            } 
            Vector3 temp = hitInfo.point-this.transform.position;
            Vector3 ReferencePoint = this.transform.position + transform.forward * temp.magnitude;
            Vector3 DIS = ReferencePoint- this.transform.position;
            DistanceBetweenCamAndRef = DIS.magnitude;

            float mX = this.transform.eulerAngles.y;
            Debug.Log("mX is : " + mX.ToString());
            float mY = this.transform.eulerAngles.x;
            Debug.Log("mY is : " + mY.ToString());
            mX += gesture.DeltaMove.x.Centimeters() * yawSensitivity ;
            mY -= gesture.DeltaMove.y.Centimeters()* pitchSensitivity ;

            // control the mX or mY between -180 and 180
            while (mX >= 180)
            {
                mX -= 360;
            }
            while (mX < -180)
            {
                mX += 360;
            }
            while (mY >= 180)
            {
                mY -= 360;
            }
            while (mY < -180)
            {
                mY += 360;
            }
          // this is the bug of unity ? the following code is the tem solution
            if(mY>90.0f)
            {
                mY = 90.0f;

            }
            if(mY<-90.0f)
            {
                mY = -90.0f;
            }
                
            Quaternion mRotation = Quaternion.Euler(mY,mX, 0);//返回四元数 没有问题
            if( Time.time < nextDragTime )
                return;
            Debug.Log(mRotation);
            this.transform.position = ReferencePoint;
            //this.transform.localPosition= Vector3.Lerp(transform.localPosition, transform.position + mRotation * Vector3.back*DistanceBetweenCamAndRef,Time.deltaTime*smoothtime);
            this.transform.position += mRotation * Vector3.back*DistanceBetweenCamAndRef;
            //this.transform.position = tempPosition;
            this.transform.rotation =  mRotation;
           
        }
        else
            return;
      
    }

    void OnPinch( PinchGesture gesture )
    {
        if (gesture.Fingers.Count == 2)
        {
            float idealDistance = gesture.Delta.Centimeters() * pinchZoomSensitivity;
            Ray ray = Camera.main.ScreenPointToRay(Vec2toVec3(gesture.StartPosition));  //放大缩小
            //Vector3.SmoothDamp(transform.position, transform.position + idealDistance * ray.direction, ref cameraVelocity, smoothtime);
            this.transform.position += idealDistance * ray.direction;
            nextDragTime = Time.time + 0.25f;  
        }
        else
            return;

    }

    void OnTwoFingerDrag( DragGesture gesture )  //平移
    {
        if (gesture.Fingers.Count == 2)
        {
            Vector3 ScreenPosition = Vec2toVec3(gesture.StartPosition);
            Ray ray = Camera.main.ScreenPointToRay(ScreenPosition);  //摄像机需要设置MainCamera的Tag这里才能找到  
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {  

                Vector3 hitPoint = hitInfo.point;//返回的是世界坐标系的坐标 
                float distem = (hitInfo.point - this.transform.position).magnitude;
                if (distem < 8.0f)
                {
                    MoveSpeed = 0.2f;
                }
                else
                {
                    if (distem < 30.0f)
                    {
                        MoveSpeed = 2.0f;
                    }
                    else
                    {
                        MoveSpeed = 5.0f;
                    }
                }

            }
            else
            {
                MoveSpeed = 1.5f;
            }
            Vector3 move = -MoveSpeed * ( transform.right*gesture.DeltaMove.x.Centimeters()+  transform.up*gesture.DeltaMove.y.Centimeters());
            this.transform.position += move;
            nextDragTime = Time.time + 0.25f;
        }
        else
            return;

    }

    #endregion


    /// <summary>
    /// convert the type of vector2 to the type of vector3
    /// </summary>
    /// <returns>The vec3.</returns>
    /// <param name="Vec2">Vec2.</param>
    private Vector3 Vec2toVec3(Vector2 Vec2)
    {
        return new Vector3(Vec2.x, Vec2.y, 0.0f);
    }
        
    private void InitialRotationOperation(GameObject ro)//用来初始化带旋转物体的观察状态
    {
        float distance = 25.0f;
        this.transform.position = ro.transform.position+ new Vector3(-10.0f,10.0f,-10.0f);
        this.transform.rotation = Quaternion.LookRotation (ro.transform.position-this.transform.position);//要减去当前位置
        this.transform.position = ro.transform.position;
        this.transform.position += this.transform.rotation * Vector3.back*distance;
    }
}
