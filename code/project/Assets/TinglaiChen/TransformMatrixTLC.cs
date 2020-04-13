using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMatrixTLC : MonoBehaviour
{
    public GameObject[] bb_final = new GameObject[33];
    public GameObject[] bb_start = new GameObject[33];
   // public GameObject[] tooth = new GameObject[33];
    public GameObject[] final_tooth = new GameObject[33];
    public Matrix4x4[] Transform = new Matrix4x4[33];
    public Vector3[] positions = new Vector3[33];
    public Quaternion[] rotations = new Quaternion[33];
    public int test = 3;
    public bool flag_test = false;

    // Use this for initialization
    void Start()
    {
        for (int i = 1; i < 33; i++)
        {
            if (i == 1 || i == 26 || i == 31 || i == 32)
            {
                bb_final[i] = null;
                bb_start[i] = null;
                continue;
            }
            if (flag_test && i != test) continue;
            else
            {
                Debug.Log(i);
                bb_final[i] = GameObject.Find("BoundingBox" + i.ToString() + "f");
                bb_start[i] = GameObject.Find("BoundingBox" + i.ToString());
                if (Vector3.Dot(bb_start[i].transform.forward, bb_final[i].transform.forward) < 0f)
                {
                    bb_final[i].transform.forward = -bb_final[i].transform.forward;
                }

            }
        }
        for (int i = 1; i < 33; i++)
        {
            string s = "";
            if (i == 1 || i == 26 || i == 31 || i == 32)
            {
                continue;
            }
            if (flag_test && i != test)
                continue;
            positions[i] = bb_start[i].transform.position;
            rotations[i] = bb_start[i].transform.rotation;
            Matrix4x4 translate = Matrix4x4.identity;
            translate.m03 = bb_final[i].transform.position.x - bb_start[i].transform.position.x;
            translate.m13 = bb_final[i].transform.position.y - bb_start[i].transform.position.y;
            translate.m23 = bb_final[i].transform.position.z - bb_start[i].transform.position.z;

            Matrix4x4 translate1 = Matrix4x4.identity;
            //Vector3 relative_position1 = bb_start[i].transform.position - tooth[i].transform.position;


            //计算两个坐标系之间的四元数
            Matrix4x4 m0 = Matrix4x4.Rotate(bb_start[i].transform.rotation);
            Matrix4x4 m1 = Matrix4x4.Rotate(bb_final[i].transform.rotation);
            Matrix4x4 m0_ = m0.inverse;
            Matrix4x4 rotate = m1 * m0_;

            Transform[i] = rotate * translate;


            Debug.Log("变换矩阵" + i.ToString() + " : \n" + Transform[i].ToString() + "\n");



        }

    }



    // Update is called once per frame
    void Update()
    {
        for (int i = 1; i < 33; i++)
        {
            if (i == 1 || i == 26 || i == 31 || i == 32)
            {
                continue;
            }
            if (flag_test && i != test)
                continue;

            bb_start[i].transform.rotation = Quaternion.Lerp(bb_start[i].transform.rotation, bb_final[i].transform.rotation, Time.deltaTime / 10f);
            bb_start[i].transform.position = Vector3.Lerp(bb_start[i].transform.position, bb_final[i].transform.position, Time.deltaTime / 10f);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            for (int i = 1; i < 33; i++)
            {
                if (i == 1 || i == 26 || i == 31 || i == 32)
                {
                    continue;
                }
                if (flag_test && i != test)
                    continue;
                Matrix4x4 translate = Matrix4x4.identity;
                translate.m03 = bb_start[i].transform.position.x - positions[i].x;
                translate.m13 = bb_start[i].transform.position.y - positions[i].y;
                translate.m23 = bb_start[i].transform.position.z - positions[i].z;

                // Matrix4x4 translate1 = Matrix4x4.identity;
                //            Vector3 relative_position1 = bb_start[i].transform.position - tooth[i].transform.position;


                //计算两个坐标系之间的四元数
                Matrix4x4 m0 = Matrix4x4.Rotate(rotations[i]);
                Matrix4x4 m1 = Matrix4x4.Rotate(bb_start[i].transform.rotation);
                Matrix4x4 m0_ = m0.inverse;
                Matrix4x4 rotate = m1 * m0_;

                Matrix4x4 matrix = rotate * translate;
                Debug.Log("变换矩阵" + i.ToString() + " : \n" + matrix.ToString() + "\n");
            }

        }
    }
}
