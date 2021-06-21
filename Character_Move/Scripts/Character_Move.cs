using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ver 0.1 2021.06.19//
[System.Serializable]
public class MoveOption
{
    public float MoveSpeed = 1;
    public float RotateSpeed = 1;

    public float JumpPower = 1;
    public float JumpSpeed = 10;
}
class Character_Physics
{
    GameObject _Obj;
    GameObject Obj
    {
        get
        {
            return _Obj;
        }
        set
        {
            _Obj = value;

            if (Obj.GetComponent<BoxCollider>() == null) Obj.AddComponent<BoxCollider>();
            col = Obj.GetComponent<BoxCollider>();
            if (Obj.GetComponent<Rigidbody>() == null) Obj.AddComponent<Rigidbody>();
            rig = Obj.GetComponent<Rigidbody>();
        }
    }
    BoxCollider _col;
    BoxCollider col
    {
        get
        {
            return _col;
        }
        set
        {
            _col = value;
            if (Obj.GetComponent<MeshFilter>() != null)
            {
                Mesh tempMesh = Obj.GetComponent<MeshFilter>().mesh;

                float[] height = { 0, 0 };
                float[] width = { 0, 0 };
                float[] weight = { 0, 0 };

                foreach (Vector3 e in tempMesh.vertices)
                {
                    height[0] = e.y > height[0] ? e.y : height[0];
                    height[1] = e.y < height[1] ? e.y : height[1];

                    width[0] = e.x > width[0] ? e.x : width[0];
                    width[1] = e.x < width[1] ? e.x : width[1];

                    weight[0] = e.z > weight[0] ? e.z : weight[0];
                    weight[1] = e.z < weight[1] ? e.z : weight[1];
                }

                col.size = new Vector3(width[0] - width[1], height[0] - height[1], weight[0] - weight[1]);
            }
        }
    }
    Rigidbody _rig;
    Rigidbody rig
    {
        get
        {
            return _rig;
        }
        set
        {
            _rig = value;

            //mass is Object Kg 오브젝트의 질량(kg)
            //rig.mass

            //Drag is move resistance 오브젝트 이동시 저항
            //rig.drag

            //angularDrag is rotate resistance 오브젝트 회전시 저항
            //rig.angularDrag

            //interpolation is Smooth move 오브젝트의 부드러운 움직임(None(부드럽게 움직이지 않음),Interpolate(이전 프레임의 움직임을 참조), Extrapolate(다음 프레임의 움직임을 예상))
            rig.interpolation = RigidbodyInterpolation.Extrapolate;
            rig.freezeRotation = true;
            rig.useGravity = false;
        }
    }

    public Rigidbody myRig
    {
        get
        {
            return rig;
        }
    }
     public BoxCollider myCol
    {
        get
        {
            return col;
        }
    }

    public Character_Physics(GameObject Obj)
    {
        this.Obj = Obj;
    }
}
public class Character_Move : MonoBehaviour
{
    float PersonalTime = 0;
    Character_Physics myCha;
    public MoveOption moveOption;

    public bool LongMove = true;


    bool JumpSwitch = false;
    float JumpDist = 0;
    float Gravity_Delta = 9.8f;


    Vector3 fb_move = Vector3.zero;
    Vector3 lr_move = Vector3.zero;
    Vector3 nowrotate = Vector3.zero;
    Vector3 jumpVector = Vector3.zero;


    void Jump_Key()
    {
        if (JumpSwitch)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                JumpSwitch = false;
                JumpDist = moveOption.JumpPower;
            }
    }
    void C_Jump()
    {
        if (DownCheck() == false && JumpDist == 0)
        {
            jumpVector = Vector3.zero;
            JumpSwitch = true;
        }

        float delta =  PersonalTime * moveOption.JumpSpeed;
        if (JumpDist > 0)
        {
            if (JumpDist - delta < 0) delta = JumpDist;
            JumpDist -= delta;
            jumpVector = transform.up * delta;
        }
        else if(JumpSwitch==false)
        {
            delta = PersonalTime * Gravity_Delta;
            jumpVector = -transform.up* delta;
        }          
    }
    bool DownCheck()
    {
        bool value = true;

        Vector3 ori = transform.position;
        Vector3 width = transform.forward/2;
        Vector3 weight = transform.right / 2;

        Vector3[] oriGroup = new Vector3[9];
        oriGroup[0] = ori+width+weight;
        oriGroup[1] = ori+width-weight;
        oriGroup[2] = ori+width;
        oriGroup[3] = ori-width+weight;
        oriGroup[4] = ori-width-weight;
        oriGroup[5] = ori-width;
        oriGroup[6] = ori+weight;
        oriGroup[7] = ori-weight;
        oriGroup[8] = ori;
        for (int i = 0; i < oriGroup.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(oriGroup[i], -transform.up, out hit))
            {
                if (hit.distance <= myCha.myCol.size.y / 2)
                {
                    return false;
                }
            }
        }
        return value;
    }

    void C_Move(float speed)
    {
        Vector3 delta = (lr_move + fb_move) * PersonalTime * speed;
        delta += jumpVector;
        if (myCha != null && delta != Vector3.zero)
            myCha.myRig.MovePosition(transform.position + delta);
    }

    void C_Move()
    {
        C_Move(moveOption.MoveSpeed);
    }


    void MoveInput()
    {
        if (Input.GetKey(KeyCode.UpArrow)) FrontSwitch = true;
        if (Input.GetKey(KeyCode.DownArrow)) BackSwitch = true;

       // if (Input.GetKey(KeyCode.LeftArrow)) LeftSwitch = true;
       // if (Input.GetKey(KeyCode.RightArrow)) RightSwitch = true;
    }
    bool FrontSwitch = false;
    bool BackSwitch = false;
    void FrontMove()
    {
        if (FrontSwitch)
        {
            if (Vector3.Distance(Vector3.zero, fb_move + transform.forward * PersonalTime) <= 1)
                fb_move += transform.forward * PersonalTime;
            else
                fb_move = transform.forward;
        }
        else if (BackSwitch)
        {
            if (Vector3.Distance(Vector3.zero, fb_move - transform.forward * PersonalTime) <= 1)
                fb_move -= transform.forward * PersonalTime;
            else
                fb_move =-transform.forward;
        }
        else
        {
            if (Vector3.Distance(Vector3.zero, fb_move) != 0)
            {
                Vector3 tempdir = Vector3.zero - fb_move;
                tempdir = tempdir.normalized;

                tempdir *= PersonalTime;
                if (Mathf.Abs(Vector3.Distance(Vector3.zero, fb_move)) - Mathf.Abs(Vector3.Distance(Vector3.zero, tempdir)) > 0) fb_move += tempdir;
                else fb_move = Vector3.zero;
            }
        }

        FrontSwitch = false;
        BackSwitch = false;
    }
    bool LeftSwitch = false;
    bool RightSwitch = false;
    void LR_Move()
        {
            if (RightSwitch)
            {
                if (Vector3.Distance(Vector3.zero, lr_move + transform.right * PersonalTime) <= 1)
                lr_move += transform.right * PersonalTime;
                else
                lr_move = transform.right;
            }
            else if (LeftSwitch)
            {
                if (Vector3.Distance(Vector3.zero, lr_move - transform.right * PersonalTime) <= 1)
                lr_move -= transform.right * PersonalTime;
                else
                lr_move = -transform.right;
            }
            else
            {
                if (Vector3.Distance(Vector3.zero, lr_move) != 0)
                {
                    Vector3 tempdir = Vector3.zero - lr_move;
                    tempdir = tempdir.normalized;

                    tempdir *= PersonalTime;
                    if (Mathf.Abs(Vector3.Distance(Vector3.zero, lr_move)) - Mathf.Abs(Vector3.Distance(Vector3.zero, tempdir)) > 0) lr_move += tempdir;
                    else lr_move = Vector3.zero;
                }
            }

       LeftSwitch = false;
       RightSwitch = false;
    }


    void RotateInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) leftRotateSwitch = true;
       if (Input.GetKey(KeyCode.RightArrow)) rightRotateSwitch = true;
    }

    bool leftRotateSwitch = false;
    bool rightRotateSwitch = false;
    void L_R_Rotate()
    {
        nowrotate = Vector3.zero;
        if (leftRotateSwitch)
        {
            nowrotate = -transform.up;
        }
        if (rightRotateSwitch)
        {
            nowrotate = transform.up;
        }
        leftRotateSwitch = false;
        rightRotateSwitch = false;
    }
    void C_Rotate()
    {
        C_Rotate(moveOption.RotateSpeed);
    }
    void C_Rotate(float rotspe)
    {
        Vector3 delta = nowrotate * rotspe;//moveOption.RotateSpeed;
        if (myCha != null&&nowrotate!=Vector3.zero)
        {
            Quaternion quaternion = new Quaternion();
            quaternion.eulerAngles = transform.eulerAngles + delta;
            myCha.myRig.MoveRotation(quaternion);
        }
    }
   float PointDist = 0;
    float PointRot = 0;
    void PointMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (fb_move != Vector3.zero) fb_move = Vector3.zero;

            Ray ori = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ori, out hit)) 
            {
                PointRot = Rotate_Value(transform, hit.point);
                Vector3 tempOri = new Vector3(transform.position.x, transform.position.y - myCha.myCol.size.y / 2, transform.position.z);
                PointDist =Vector3.Distance(tempOri, hit.point);
            }
            
        }
        
    }

    public static float Rotate_Value(Transform Ori, Vector3 Target)
    {
        Vector3 a = Ori.transform.forward;
        Vector3 b = Target - Ori.transform.position;
        b = b.normalized;
        float rot = 0.0f;

        rot = Vector3.Dot(a, b);

        rot = Mathf.Acos(rot);
        rot = (rot * 180) / Mathf.PI;
        Vector3 right = Vector3.Cross(Vector3.up, a);
        right.Normalize();
        if (Vector3.Dot(right, b) < 0.0f)
            rot *= -1;

        if (float.IsNaN(rot)) rot = 0;

        return rot;
    }

    private void Awake()
    {
        myCha = new Character_Physics(gameObject);
    }

    private void Start()
    {
    }
    private void Update()
    {
        PersonalTime = Time.smoothDeltaTime;

        // F_Move();

        //키보드 조작
        //MoveInput();
        //RotateInput();
        Jump_Key();
        //

        PointMove();
        //방향입력
        FrontMove();
        if (PointRot < 0) leftRotateSwitch = true;
        if (PointRot > 0) rightRotateSwitch = true;
        L_R_Rotate();
        //LR_Move();
        //

        //기본 움직임
        C_Jump();
        if (PointRot != 0)
        {
            if (Mathf.Abs(PointRot) > moveOption.RotateSpeed)
            {
                C_Rotate();
                PointRot = PointRot > 0 ? PointRot - moveOption.RotateSpeed : PointRot + moveOption.RotateSpeed;
            }
            else if (Mathf.Abs(PointRot) < moveOption.RotateSpeed)
            {
                C_Rotate(Mathf.Abs(PointRot));
                PointRot = 0;
            }
        }

        if (LongMove)
        {
            if ((PointRot == 0 && PointDist > 0)||!JumpSwitch)
            {
                float delta = moveOption.MoveSpeed * PersonalTime*Vector3.Distance(Vector3.zero,fb_move);
                if (PointDist - delta >= 0)
                {
                    PointDist -= delta;
                    FrontSwitch = true;
                }
                else
                {
                    PointDist = 0;
                }
                C_Move();
            }
        }
        else
        {
            if ((PointRot == 0 && PointDist > 0) || !JumpSwitch)
            {
                float delta = moveOption.MoveSpeed * PersonalTime;
                if (PointDist - delta >= 0)
                {
                    PointDist -= delta;
                    FrontSwitch = true;
                }
                else
                {
                    PointDist = 0;
                }
            }
            C_Move();
        }
    }
}
