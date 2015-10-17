
using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Camera))]
public class CameraCtrl : MonoBehaviour
{

    protected GameObject mGo;
    protected Transform mTrans;

    public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    Vector3 mouseWheelDownPos;

    public float moveDelta = 0.17f;
    public float zoomScale = 7;

    public float dragDistScale = 0.05f;

    public Vector2 mouseMoveScale = new Vector2(-1f, -1f);
    public Vector2 rotScale = new Vector2(3, -2.5f);
    public Vector2 orbitRotScale = new Vector2(2.5f, -4.5f);

    Vector3 orbitPos;

    Camera mCamera;
    Transform mCameraTrans;


    void Awake()
    {
        mCamera = cachedGameObject.GetComponent<Camera>();
        mCameraTrans = mCamera.transform;
    }

    void Update()
    {
        inputAction();
    }

    void cameraCtrl(Vector3 deltaPos, Vector3 deltaRot, Vector3 deltaOrbitRot)
    {
        if(!Vector3.zero.Equals(deltaOrbitRot))
        {
            Vector3 orbitPos_World = mCameraTrans.TransformPoint(orbitPos);
            mCameraTrans.RotateAround(orbitPos_World, mCameraTrans.right, deltaOrbitRot.x);
            mCameraTrans.RotateAround(orbitPos_World, Vector3.up, deltaOrbitRot.y);
        }
        else
        {
            mCameraTrans.Rotate(deltaRot.x, 0,0, Space.Self);
            mCameraTrans.Rotate(0, deltaRot.y, 0, Space.World);

            mCameraTrans.Translate(deltaPos);
            orbitPos.z -= deltaPos.z;
        }
    }

    void inputAction()
    {
        sceneCameraMultiCtrl();
    }

    void sceneCameraMultiCtrl()
    {
        Vector3 deltaPos = Vector3.zero, deltaRot = Vector3.zero, deltaOrbitRot = Vector3.zero;
        Vector2 tempVct2 = Vector2.zero;

        bool leftMouseDown = Input.GetMouseButton(0);
        bool rightMouseDown = Input.GetMouseButton(1);
        bool middleMouseDown = Input.GetMouseButton(2);

        Vector2 mouseDelta = Vector2.zero;
        mouseDelta.x = Input.GetAxis("Mouse X");
        mouseDelta.y = Input.GetAxis("Mouse Y");

        //mouse right button down +W,A,S,D,Q,E controll camera X,Y,Z translate.
        if(rightMouseDown)
        {
            if (Input.GetKey(KeyCode.W))
                deltaPos.z += moveDelta;
            else if (Input.GetKey(KeyCode.S))
                deltaPos.z += -moveDelta;

            if (Input.GetKey(KeyCode.A))
                deltaPos.x += -moveDelta;
            else if (Input.GetKey(KeyCode.D))
                deltaPos.x += moveDelta;

            if (Input.GetKey(KeyCode.Q))
                deltaPos.y += -moveDelta;
            else if (Input.GetKey(KeyCode.E))
                deltaPos.y += moveDelta;

            //mouse right button down + mouse move controll camera rotate by Self_X, World_Y axis.
            tempVct2 = Vector2.Scale(mouseDelta, rotScale);
            deltaRot.x = tempVct2.y;
            deltaRot.y = tempVct2.x;
        }

        //mouse wheel scroll controll camera Z translate.
        deltaPos.z += Input.GetAxis("Mouse ScrollWheel") * zoomScale;

        //mouse wheel button down + mouse move controll camera X,Y translate.
        if(middleMouseDown)
        {
            Vector2 dragScale = mouseMoveScale;
            if(!orbitPos.Equals(Vector3.zero))
            {
                float dist = Vector3.Distance(mCameraTrans.position, orbitPos);
                dragScale *= dist * dragDistScale;
            }

            tempVct2 = Vector2.Scale(mouseDelta, dragScale);
            deltaPos.x += tempVct2.x;
            deltaPos.y += tempVct2.y;
        }


        //mouse wheel button click change camera focus.
        if (Input.GetMouseButtonDown(2))
            mouseWheelDownPos = Input.mousePosition;
        else if(Input.GetMouseButtonUp(2) &&
            mouseWheelDownPos.Equals(Input.mousePosition))
        {
            RaycastHit hitInfo;
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hitInfo, Mathf.Infinity, -1))
            {
                Vector3 hitPos = mCamera.transform.worldToLocalMatrix.MultiplyPoint(hitInfo.point);
                deltaPos.x = hitPos.x;
                deltaPos.y = hitPos.y;
                orbitPos = hitPos;
            }
        }

        //Alt + mouse left button down + mouse move controll camera rotate around focus.
        if(leftMouseDown &&
            (Input.GetKey(KeyCode.LeftAlt) ||
            Input.GetKey(KeyCode.RightAlt)))
        {
            tempVct2 = Vector2.Scale(mouseDelta, orbitRotScale);
            deltaOrbitRot.x = tempVct2.y;
            deltaOrbitRot.y = tempVct2.x;
        }

        cameraCtrl(deltaPos, deltaRot, deltaOrbitRot);
    }


}
