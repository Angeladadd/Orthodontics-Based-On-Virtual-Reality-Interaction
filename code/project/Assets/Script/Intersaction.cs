using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersaction : MonoBehaviour
{
    int myname;
    GameObject tooth;
    GameObject box;
    GameObject oldbox;
    GameObject Center;
    Vector3 center_position;
    int myoldname;
    float speed = 1.0f;
    public GameObject[] bb_start = new GameObject[33];
    public Vector3[] original_position = new Vector3[33];
    public Quaternion[] original_rotation = new Quaternion[33];
    Vector3 toTop;
    Vector3 toLip;
    Vector3 cross;

    void Start()
    {
        myname = -1;
        myoldname = -1;
        Center = GameObject.Find("Center");
        center_position = Center.transform.position;

        for (int i = 0; i < 33; i++)
        {
            if (i==0||i == 1 || i == 14 || i == 16 || i == 17 || i == 32)
            {
                bb_start[i] = null;
            }
            else
            {
                bb_start[i] = GameObject.Find(i.ToString() + "/BoundingBox");
                if (bb_start[i] == null) continue;
                original_position[i] = bb_start[i].transform.position;
                original_rotation[i] = bb_start[i].transform.rotation;
            }
        }
    }
    void chooseTooth(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject go = hit.collider.gameObject;    //获得选中物体
            tooth = go.transform.parent.gameObject;
            box = tooth.transform.Find("BoundingBox").gameObject;
            //box.GetComponent<Material>().color = new Color(1.0f, 1.0f, 0.0f, 0.2f);
            Renderer renderer = box.GetComponent<Renderer>();
            renderer.material.color = new Color(1.0f, 1.0f, 0.0f, 0.2f);
            myname = int.Parse(tooth.name);
            if (myoldname != -1 && myoldname != myname)
            {
                oldbox.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
            oldbox = box;
            myoldname = myname;
        }
    }
    void calculateThreeVectors(){
        toTop = Vector3.Normalize(box.transform.forward);
        if (myname < 17 && toTop.z > 0)
            toTop = -toTop;
        if (myname > 16 && toTop.z < 0)
            toTop = -toTop;
        toLip = Vector3.Normalize(box.transform.position - center_position);
        cross = Vector3.Normalize(Vector3.Cross(toTop, toLip));
    }
    void calculateTransformMatrix(){
        Vector3 current_position = box.transform.position;
        Quaternion current_rotation = box.transform.rotation;

        Matrix4x4 translate = Matrix4x4.identity;
        translate.m03 = current_position.x - original_position[myname].x;
        translate.m13 = current_position.y - original_position[myname].y;
        translate.m23 = current_position.z - original_position[myname].z;

        //计算两个坐标系之间的四元数
        Matrix4x4 m0 = Matrix4x4.Rotate(original_rotation[myname]);
        Matrix4x4 m1 = Matrix4x4.Rotate(current_rotation);
        Matrix4x4 m0_ = m0.inverse;
        Matrix4x4 rotate = m1 * m0_;

        Matrix4x4 transform_matrix = rotate * translate;

        Debug.Log("变换矩阵" + myname.ToString() + "\n:" + transform_matrix.ToString());
    }
    void keyboardInput(){
        if (Input.GetMouseButtonDown(0))
        {
            chooseTooth();
        }
        if (myname != -1)
        {//myname=-1为没有选中任何牙齿的状态
            calculateThreeVectors();
            if (Input.GetKey(KeyCode.I))
            {
                Vector3 direction = toTop;
                tooth.transform.position += direction * Time.deltaTime * speed;
            }
            if (Input.GetKey(KeyCode.K))
            {
                Vector3 direction = -toTop;
                tooth.transform.position += direction * Time.deltaTime * speed;
            }
            if (Input.GetKey(KeyCode.J))
            {
                Vector3 direction = cross;
                tooth.transform.position += direction * Time.deltaTime * speed;
            }
            if (Input.GetKey(KeyCode.L))
            {
                Vector3 direction = -cross;
                tooth.transform.position += direction * Time.deltaTime * speed;
            }
            if (Input.GetKey(KeyCode.U))
            {
                Vector3 direction = toLip;
                tooth.transform.position += direction * Time.deltaTime * speed;
            }
            if (Input.GetKey(KeyCode.O))
            {
                Vector3 direction = -toLip;
                tooth.transform.position += direction * Time.deltaTime * speed;
            }
            if (Input.GetKey(KeyCode.M))
            {
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                tooth.transform.position += relative_position;
                tooth.transform.Rotate(0.1f * toTop);
                relative_position = box.transform.position - tooth.transform.position;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKey(KeyCode.N))
            {
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                tooth.transform.position += relative_position;
                tooth.transform.Rotate(-0.1f * toTop);
                relative_position = box.transform.position - tooth.transform.position;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKey(KeyCode.B))
            {
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                tooth.transform.position += relative_position;
                tooth.transform.Rotate(0.1f * cross);
                relative_position = box.transform.position - tooth.transform.position;
                tooth.transform.position -= relative_position;
            }

            if (Input.GetKey(KeyCode.V))
            {
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                tooth.transform.position += relative_position;
                tooth.transform.Rotate(-0.1f * cross);
                relative_position = box.transform.position - tooth.transform.position;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKey(KeyCode.E))
            {
                float bias = box.transform.localScale.x / 2.0f;
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.right;
                tooth.transform.position += relative_position;
                tooth.transform.Rotate(0.1f * toTop);
                relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.right;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKey(KeyCode.R))
            {
                float bias = box.transform.localScale.x / 2.0f;
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.right;
                tooth.transform.position += relative_position;
                tooth.transform.Rotate(-0.1f * toTop);
                relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.right;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKey(KeyCode.T))
            {
                float bias = -box.transform.localScale.x / 2.0f;
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.right;
                tooth.transform.position += relative_position;
                tooth.transform.Rotate(0.1f * toTop);
                relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.right;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKey(KeyCode.Y))
            {
                float bias = -box.transform.localScale.x / 2.0f;
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.right;
                tooth.transform.position += relative_position;
                tooth.transform.Rotate(-0.1f * toTop);
                relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.right;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKey(KeyCode.Z))
            {
                float bias = box.transform.localScale.z / 2.0f;
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.forward;
                tooth.transform.position += relative_position;
                Vector3 axis = Vector3.Normalize(Vector3.Cross(relative_position, toTop));
                tooth.transform.Rotate(0.1f * axis);
                relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.forward;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKey(KeyCode.X))
            {
                float bias = box.transform.localScale.z / 2.0f;
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.forward;
                tooth.transform.position += relative_position;
                Vector3 axis = Vector3.Normalize(Vector3.Cross(relative_position, toTop));
                tooth.transform.Rotate(-0.1f * axis);
                relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.forward;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKey(KeyCode.C))
            {
                float bias = -box.transform.localScale.z / 2.0f;
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.forward;
                tooth.transform.position += relative_position;
                Vector3 axis = Vector3.Normalize(Vector3.Cross(relative_position, toTop));
                tooth.transform.Rotate(0.1f * axis);
                relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.forward;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKey(KeyCode.F))
            {
                float bias = -box.transform.localScale.z / 2.0f;
                Vector3 relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.forward;
                tooth.transform.position += relative_position;
                Vector3 axis = Vector3.Normalize(Vector3.Cross(relative_position, toTop));
                tooth.transform.Rotate(-0.1f * axis);
                relative_position = box.transform.position - tooth.transform.position;
                relative_position += bias * box.transform.forward;
                tooth.transform.position -= relative_position;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                calculateTransformMatrix();
            }

        }
    }
    // Update is called once per frame
    void Update()
    {
        keyboardInput();
    }
}
