
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultAsset))]
public class ImportSTL : Base
{
    private string _currentClassify = "Model";
    private string _fileName = "";
    private string _trianglescount = "";
    private string _meshCompression = "Off";
    private int _singleTrianglesNumber = 15000;
    private bool _isSaveMesh = true;
    private bool _adjust = true;

    private int _total;
    private int _number;
    private BinaryReader _binaryReader;
    private List<Vector3> _vertices;
    private List<Vector3> _normals;
    private List<int> _triangles;

    //private Vector3 _v1;
    //private Vector3 _v2;
    //private Vector3 _v3;

    public override void OnInspectorGUI()
    {
        //目标为STL格式
        if (AssetDatabase.GetAssetPath(target).IsStl())
        {
            ShowUI();
        }
    }

    private void ShowUI()
    {
        GUI.enabled = true;

        #region Classify
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Model", "ButtonLeft", GUILayout.Width(80)))
        {
            _currentClassify = "Model";
        }
        if (GUILayout.Button("Rig", "ButtonMid", GUILayout.Width(80)))
        {
            _currentClassify = "Rig";
        }
        if (GUILayout.Button("Animations", "ButtonRight", GUILayout.Width(80)))
        {
            _currentClassify = "Animations";
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        #endregion

        if (_currentClassify == "Model")
        {
            #region STL Meshes
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("STL Meshes", "BoldLabel");
            GUILayout.Label("(File Type:Binary)", "BoldLabel");
            EditorGUILayout.EndHorizontal();

            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scale Factor", GUILayout.Width(120));
            GUILayout.TextField("1");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("File Scale", GUILayout.Width(120));
            GUILayout.TextField("1");
            EditorGUILayout.EndHorizontal();

            if (_fileName == "" && _trianglescount == "")
            {
                GetFileNameAndTrianglesCount();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("File Name", GUILayout.Width(120));
            _fileName = GUILayout.TextField(_fileName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Triangles Count", GUILayout.Width(120));
            _trianglescount = GUILayout.TextField(_trianglescount);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Mesh Compression", GUILayout.Width(120));
            if (GUILayout.Button(_meshCompression, "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Off"), "Off" == _meshCompression, delegate () { _meshCompression = "Off"; });
                gm.AddItem(new GUIContent("On"), "On" == _meshCompression, delegate () { _meshCompression = "On"; });
                gm.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Single Triangles Number", GUILayout.Width(120));
            _singleTrianglesNumber = EditorGUILayout.IntField(_singleTrianglesNumber);
            EditorGUILayout.EndHorizontal();

            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Read/Write Enabled", GUILayout.Width(120));
            GUILayout.Toggle(true, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Optimize Mesh", GUILayout.Width(120));
            GUILayout.Toggle(false, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Import BlendShapes", GUILayout.Width(120));
            GUILayout.Toggle(false, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Generate Colliders", GUILayout.Width(120));
            GUILayout.Toggle(true, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Keep Quads", GUILayout.Width(120));
            GUILayout.Toggle(false, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Swap UVs", GUILayout.Width(120));
            GUILayout.Toggle(false, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Generate Lightmap UVs", GUILayout.Width(120));
            GUILayout.Toggle(false, "");
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            #endregion

            #region Normals & Tangents
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Normals & Tangents", "BoldLabel");
            EditorGUILayout.EndHorizontal();

            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Normals", GUILayout.Width(120));
            GUILayout.Button("None", "MiniPopup");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Smoothing Angle", GUILayout.Width(120));
            GUILayout.HorizontalSlider(0.3f, 0, 1);
            GUILayout.TextField("60");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Tangents", GUILayout.Width(120));
            GUILayout.Button("None - (Normals required)", "MiniPopup");
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            #endregion

            #region Materials
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Materials", "BoldLabel");
            EditorGUILayout.EndHorizontal();

            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Import Materials", GUILayout.Width(120));
            GUILayout.Toggle(false, "");
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            #endregion

            #region Vectors

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vectors", "BoldLabel");
            EditorGUILayout.EndHorizontal();
            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Root to Top", GUILayout.Width(120));
            _v1.x = Convert.ToSingle(GUILayout.TextField(_v1.x.ToString()));
            _v1.y = Convert.ToSingle(GUILayout.TextField(_v1.y.ToString()));
            _v1.z = Convert.ToSingle(GUILayout.TextField(_v1.z.ToString()));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Lingual to Lip", GUILayout.Width(120));
            _v2.x = Convert.ToSingle(GUILayout.TextField(_v2.x.ToString()));
            _v2.y = Convert.ToSingle(GUILayout.TextField(_v2.y.ToString()));
            _v2.z = Convert.ToSingle(GUILayout.TextField(_v2.z.ToString()));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Cross", GUILayout.Width(120));
            _v3.x = Convert.ToSingle(GUILayout.TextField(_v3.x.ToString()));
            _v3.y = Convert.ToSingle(GUILayout.TextField(_v3.y.ToString()));
            _v3.z = Convert.ToSingle(GUILayout.TextField(_v3.z.ToString()));
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            #endregion
        }
        else if (_currentClassify == "Rig")
        {
            #region Rig
            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Animation Type", GUILayout.Width(120));
            GUILayout.Button("Legacy", "MiniPopup");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Generation", GUILayout.Width(120));
            GUILayout.Button("Don't Import", "MiniPopup");
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            #endregion
        }
        else if (_currentClassify == "Animations")
        {
            #region Animations
            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Import Animation", GUILayout.Width(120));
            GUILayout.Toggle(false, "");
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("No animation data available in this STL model.", MessageType.Info);
            EditorGUILayout.EndHorizontal();
            #endregion
        }

        #region Apply
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("");
        EditorGUILayout.EndHorizontal();

        GUI.enabled = false;
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Button("Revert");
        GUILayout.Button("Apply");
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
        #endregion

        #region CreateInstance
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("CreateInstance", GUILayout.Width(160)))
        {
            CreateInstance();
        }
        _isSaveMesh = GUILayout.Toggle(_isSaveMesh, "Save Mesh");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        _adjust = GUILayout.Toggle(_adjust, "Is Adjust");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        #endregion
    }

    /// <summary>
    /// 获取STL模型的文件名及三角面数量
    /// </summary>
    private void GetFileNameAndTrianglesCount()
    {
        string fullPath = Path.GetFullPath(AssetDatabase.GetAssetPath(target));

        using (BinaryReader br = new BinaryReader(File.Open(fullPath, FileMode.Open)))
        {
            _fileName = Encoding.UTF8.GetString(br.ReadBytes(80));
            _trianglescount = BitConverter.ToInt32(br.ReadBytes(4), 0).ToString();
        }
    }

    /// <summary>
    /// 创建STL模型实例
    /// </summary>
    private void CreateInstance()
    {
        if (_singleTrianglesNumber < 1000 || _singleTrianglesNumber > 20000)
        {
            Debug.LogError("Single Triangles Number: this value is unreasonable!");
            return;
        }
        if (int.Parse(_trianglescount) > 200000)
        {
            Debug.LogError("Triangles Count: this value is too much!");
            return;
        }

        string fullPath = Path.GetFullPath(AssetDatabase.GetAssetPath(target));

        _total = int.Parse(_trianglescount);
        _number = 0;
        _binaryReader = new BinaryReader(File.Open(fullPath, FileMode.Open));

        //抛弃前84个字节
        _binaryReader.ReadBytes(84);

        _vertices = new List<Vector3>();
        _normals = new List<Vector3>();
        _triangles = new List<int>();

        //读取顶点信息
        Thread t = new Thread(ReadVertex);
        t.Start();

        while (_number < _total)
        {
            EditorUtility.DisplayProgressBar("读取信息", "正在读取顶点信息（" + _number + "/" + _total + "）......", (float)_number / _total);
        }

        CreateGameObject();

        _binaryReader.Close();
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 读取顶点信息
    /// </summary>
    private void ReadVertex()
    {
        while (_number < _total)
        {
            byte[] bytes;
            bytes = _binaryReader.ReadBytes(50);

            if (bytes.Length < 50)
            {
                _number += 1;
                continue;
            }

            Vector3 vec0 = new Vector3(BitConverter.ToSingle(bytes, 0), BitConverter.ToSingle(bytes, 4), BitConverter.ToSingle(bytes, 8));
            Vector3 vec1 = new Vector3(BitConverter.ToSingle(bytes, 12), BitConverter.ToSingle(bytes, 16), BitConverter.ToSingle(bytes, 20));
            Vector3 vec2 = new Vector3(BitConverter.ToSingle(bytes, 24), BitConverter.ToSingle(bytes, 28), BitConverter.ToSingle(bytes, 32));
            Vector3 vec3 = new Vector3(BitConverter.ToSingle(bytes, 36), BitConverter.ToSingle(bytes, 40), BitConverter.ToSingle(bytes, 44));

            _normals.AddNormal(vec0);
            _triangles.AddTriangle(_vertices.AddGetIndex(vec1), _vertices.AddGetIndex(vec2), _vertices.AddGetIndex(vec3));

            _number += 1;
        }
    }

    /// <summary>
    /// 创建GameObject
    /// </summary>
    private void CreateGameObject()
    {
        string path = AssetDatabase.GetAssetPath(target);
        string fullPath = Path.GetFullPath(path);
        string assetPath = path.Substring(0, path.LastIndexOf("/")) + "/";
        string root_name = Path.GetFileNameWithoutExtension(fullPath);
        GameObject root = new GameObject(root_name);
        int flag_opacity = 0;
        if(root_name[root_name.Length-1]=='f'){
            flag_opacity = 1;
        }
        root.transform.localPosition = Vector3.zero;
        root.transform.localScale = Vector3.one;

        MeshFilter mf = root.AddComponent<MeshFilter>();
        MeshRenderer mr = root.AddComponent<MeshRenderer>();

        int length = _total * 3;
            
        List<Vector3> vertices = _vertices.GetRange(0, length);
        List<Vector3> normals = _normals.GetRange(0, length);
        List<int> triangles = _triangles.GetRange(0, length);

        Mesh m = new Mesh();
        m.name = root.name;
        m.vertices = vertices.ToArray();
        m.normals = normals.ToArray();
        m.triangles = triangles.ToArray();
        m.RecalculateNormals();
        mf.sharedMesh = m;
        mr.sharedMaterial = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        if(flag_opacity==1){
            mr.sharedMaterial.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        }

            //保存网格
        if (_isSaveMesh)
        {
            AssetDatabase.CreateAsset(mf.sharedMesh, assetPath + mf.sharedMesh.name + ".asset");
            AssetDatabase.SaveAssets();
        }

        Debug.Log("Create done! " + root.name + ": Vertex Number " + m.vertices.Length);

        //BoxCollider box = root.GetComponent<BoxCollider>();
        //if(box==null){
        //    box = root.AddComponent<BoxCollider>();
        //}
        //求OBB https://hewjunwei.wordpress.com/2013/01/26/obb-generation-via-principal-component-analysis/
        //协方差矩阵
        Vector3[] pVec = new Vector3[length];
        Vector3 arg = new Vector3(0.0f, 0.0f, 0.0f);
        float[,] covariance = new float[3, 3];
        for (int i = 0; i < length; i++)
        {
            arg = arg + vertices[i];
            pVec[i] = vertices[i];
        }
        arg = arg / length;
        for (int i = 0; i < length; i++)
        {
            pVec[i] = pVec[i] - arg;
        }
        float[] data = new float[3];
        for (int c = 0; c < 3; c++)
        {
            for (int r = c; r < 3; r++)
            {
                covariance[c, r] = 0.0f;
                float acc = 0.0f;
                for (int i = 0; i < length; i++)
                {
                    data[0] = pVec[i].x;
                    data[1] = pVec[i].y;
                    data[2] = pVec[i].z;
                    acc += data[c] * data[r];
                }
                acc /= length;
                covariance[c, r] = acc;
                covariance[r, c] = acc;
            }
        }
        //雅可比迭代法求特征向量
        float[] eValue = new float[3];
        Vector3[] eVectors=new Vector3[3];
        float eps1 = 0.00001f;
        float esp2 = 0.00001f;
        float esp3 = 0.00001f;
        float p,q,spq;
        float cosa = 0, sina = 0;
        float temp;
        float s1 = 0.0f;
        float s2;
        bool flag = true;
        int iteration = 0;
        Vector3 mik = new Vector3();
        float[,] matrix_t = new float[3, 3]{ { 1.0f, 0.0f, 0.0f },{0.0f,1.0f,0.0f},{0.0f,0.0f,1.0f}};
        do
        {
            iteration++;
            for (int i = 0; i < 2; i++)
            {
                for (int j = i + 1; j < 3; j++)
                {
                    if (Math.Abs(covariance[i, j]) < eps1)
                        covariance[i, j] = 0.0f;
                    else
                    {
                        q = Math.Abs(covariance[i, i] - covariance[j, j]);
                        if (q > esp2)
                        {
                            p = (2.0f * covariance[j, i] * q) / (covariance[i, i] - covariance[j, j]);
                            spq = (float)Math.Sqrt(p * p + q * q);
                            cosa = (float)Math.Sqrt((1.0f + q / spq) / 2.0f);
                            sina = p / (2.0f * cosa * spq);
                        }
                        else
                        {
                            sina = cosa = (float)(1.0 / Math.Sqrt(2.0));
                        }
                        for (int k = 0; k < 3; k++)
                        {
                            temp = matrix_t[i, k];
                            matrix_t[i, k] = (temp * cosa + matrix_t[j, k] * sina);
                            matrix_t[j, k] = (temp * sina - matrix_t[j, k] * cosa);
                        }
                        for (int k = i; k < 3; k++)
                        {
                            if (k > j)
                            {
                                temp = covariance[k, i];
                                covariance[k, i] = cosa * temp + sina * covariance[k, j];
                                covariance[k, j] = sina * temp - cosa * covariance[k, j];
                            }
                            else
                            {
                                data[k] = covariance[k, i];
                                covariance[k, i] = cosa * data[k] + sina * covariance[j, k];

                                if (k == j)
                                    covariance[k, j] = sina * data[k] - cosa * covariance[k, j];
                            }
                        }
                        data[j] = sina * data[i] - cosa * data[j];

                        for (int k = 0; k <= j; k++)
                        {
                            if (k <= i)
                            {
                                temp = covariance[i, k];
                                covariance[i, k] = cosa * temp + sina * covariance[j, k];
                                covariance[j, k] = sina * temp - cosa * covariance[j, k];
                            }
                            else
                            {
                                covariance[j, k] = sina * data[k] - cosa * covariance[j, k];
                            }
                        }
                    }
                }

            }
            s2 = 0.0f;
            for (int i = 0; i < 3; i++)
            {
                eValue[i] = covariance[i, i];
                s2 += eValue[i] * eValue[i];
            }
            if(Math.Abs(s2)<(float)1e-5||Math.Abs(1-s1/s2)<esp3){
                flag = false;
            }
            else{
                s1 = s2;
            }

        } while (flag);
        eVectors[0].x = matrix_t[0,0];
        eVectors[0].y = matrix_t[0, 1];
        eVectors[0].z = matrix_t[0, 2];
        eVectors[1].x = matrix_t[1, 0];
        eVectors[1].y = matrix_t[1, 1];
        eVectors[1].z = matrix_t[1, 2];
        eVectors[2].x = matrix_t[2, 0];
        eVectors[2].y = matrix_t[2, 1];
        eVectors[2].z = matrix_t[2, 2];

        mik.x = data[0];
        mik.y = data[1];
        mik.z = data[2];

       
        Vector3 cross=Vector3.Cross(eVectors[0], eVectors[1]);
        if (Vector3.Dot(cross, eVectors[2]) < 0.0f)
            eVectors[2] = -eVectors[2];

        //施密特正交化
        eVectors[0].Normalize();
        float v1dotv0 = Vector3.Dot(eVectors[1], eVectors[0]);
        eVectors[1] = eVectors[1] - v1dotv0 * eVectors[0];
        eVectors[1].Normalize();
        eVectors[2] = Vector3.Cross(eVectors[0], eVectors[1]);
        eVectors[2].Normalize();

        //计算半长度，中心

        position = new Vector3(0.0f, 0.0f, 0.0f);
        for (int i = 0; i < length; i++)
        {
            position.x += vertices[i].x;
            position.y += vertices[i].y;
            position.z += vertices[i].z;
        }
        position /= length;

        //eVectors[0] = Vector3.Cross(position, eVectors[2]);
        //eVectors[1] = Vector3.Cross(eVectors[2], eVectors[0]);

        eVectors[0] = Vector3.Normalize(eVectors[0]);
        eVectors[1] = Vector3.Normalize(eVectors[1]);
        eVectors[2] = Vector3.Normalize(eVectors[2]);

        _v1 = eVectors[0];
        _v2 = eVectors[1];
        _v3 = eVectors[2];

        float max, min;
        Vector3 maxV = new Vector3(), minV = new Vector3();
        size = new float[3];

        for (int i = 0; i < 3;i++){
            max = Vector3.Dot(vertices[0], eVectors[i]);
            min = Vector3.Dot(vertices[0], eVectors[i]);
            for (int j = 1; j < length;j++){
                temp = Vector3.Dot(vertices[j], eVectors[i]);
                if (temp > max) { max = temp; maxV = vertices[j]; }
                if (temp < min) { min = temp; minV = vertices[j]; }
            }
            size[i] = (max - min) / 2;
            temp = Vector3.Dot(position, eVectors[i]);
            float move = size[i] - (temp - min);
            position = position + move * eVectors[i];
        }

        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);

        box.transform.position = position;
        box.transform.localScale = new Vector3(2 * size[0], 2 * size[1], 2 * size[2]);
        Renderer renderer = box.GetComponent<Renderer>();
        renderer.sharedMaterial= new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        renderer.sharedMaterial.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        box.transform.up = _v2;
        box.transform.right = _v1;//_v1;
        box.transform.forward = _v3;
        //box.transform.up = Vector3.Normalize(_v2);
        box.name = "BoundingBox"+root_name;
        GameObject obj = new GameObject("Joint");
        obj.transform.parent = root.transform;
        obj.transform.localScale = Vector3.one;
        box.transform.parent = obj.transform;
        GameObject nobj = new GameObject("Joint");
        nobj.transform.parent = box.transform;
        nobj.transform.localScale = Vector3.one;

        if(_adjust){

            Matrix4x4 translate1 = Matrix4x4.identity;
            Vector3 relative_position = -box.transform.position + root.transform.position;
            translate1.m03 = relative_position.x;
            translate1.m13 = relative_position.y;
            translate1.m23 = relative_position.z;
            List<Vector3> meshList = new List<Vector3>();
            Mesh n_mesh = root.GetComponent<MeshFilter>().sharedMesh;

            for (int j = 0; j < n_mesh.vertexCount; j++)
            {
                meshList.Add(translate1.MultiplyPoint(n_mesh.vertices[j]));
                //meshList.Add(rotate1.MultiplyPoint(n_mesh.vertices[j]));
            }
            n_mesh.SetVertices(meshList);

            box.transform.parent = null;
            root.transform.position = box.transform.position;
            root.transform.parent = GameObject.Find(box.name + "/Joint").transform;
        }

    }



    // <summary>
    // 压缩网格
    // </summary>
    // <param name="meshName">网格名称</param>
    // <param name="vertices">需要压缩的网格顶点数组</param>
    // <param name="normals">与之对应的法线数组</param>
    // <param name="triangles">与之对应的三角面数组</param>
    private void MeshCompression(string meshName, List<Vector3> vertices, List<Vector3> normals, List<int> triangles)
    {
        //移位补偿，当顶点被标记为待删除顶点时
        int offset = 0;
        //需要删除的顶点索引集合
        List<int> removes = new List<int>();
        for (int i = 0; i < vertices.Count; i++)
        {
            EditorUtility.DisplayProgressBar("压缩网格", "正在压缩网格[ " + meshName + " ]（" + i + "/" + vertices.Count + "）......", (float)i / vertices.Count);
            if (removes.Contains(i))
            {
                offset += 1;
                continue;
            }

            triangles[i] = i - offset;
            for (int j = i + 1; j < vertices.Count; j++)
            {
                if (vertices[i] == vertices[j])
                {
                    removes.Add(j);
                    triangles[j] = triangles[i];
                }
            }
        }

        removes.Sort();
        removes.Reverse();

        for (int i = 0; i < removes.Count; i++)
        {
            vertices.RemoveAt(removes[i]);
            normals.RemoveAt(removes[i]);
        }
    }
}

public static class Extension
{
    public static bool IsStl(this string path)
    {
        string extension = Path.GetExtension(path);

        if (extension == ".stl" || extension == ".STL")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsOn(this string value)
    {
        return value == "On";
    }

    public static int AddGetIndex(this List<Vector3> vertices, Vector3 vec)
    {
        vertices.Add(vec);
        return vertices.Count - 1;
    }

    public static void AddNormal(this List<Vector3> normals, Vector3 vec)
    {
        normals.Add(vec);
        normals.Add(vec);
        normals.Add(vec);
    }

    public static void AddTriangle(this List<int> triangles, int vertex1, int vertex2, int vertex3)
    {
        triangles.Add(vertex1);
        triangles.Add(vertex2);
        triangles.Add(vertex3);
    }
}
