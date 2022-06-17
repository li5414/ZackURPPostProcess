using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zack.Editor;

public class GenerateMesh : EditorWindow
{
    [MenuItem("Editor/网格生成", isValidateFunction:false)]
    static void OpenEditor()
    {
        if (EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
        {
            EditorUtility.DisplayDialog("Error", "Compile Error", "Ok");
            return;
        }

        EditorWindow.GetWindow<GenerateMesh>().Close();
        EditorWindow.GetWindow<GenerateMesh>().Show();
    }

    // 顶点数量
    int _VertexCount = 0;
    // 顶点列表
    List<Vector3> _Vertices = new List<Vector3>();
    // 索引列表数量
    int _IndexCount = 0;
    // 顶点索引列表
    List<int> _Indices = new List<int>();
    // 顶点uv列表
    List<Vector2> _Uv0s = new List<Vector2>();
    
    
    private Vector2 _ScrollPosition;

    void Awake()
    {
        this.minSize = new Vector2(500, 500);
        this.title = "网格生成";
    }
    
    void OnGUI()
    {
        using (new GUILayoutScrollView(_ScrollPosition))
        {
            int vertexCount = EditorUtils.CreateIntField("顶点数量:", this._VertexCount);
            if (vertexCount != this._VertexCount)
            {
                if (this._Vertices.Count < vertexCount)
                {
                    for (int i = this._VertexCount; i < vertexCount; ++i)
                    {
                        this._Vertices.Add(Vector3.zero);
                        this._Uv0s.Add(Vector2.zero);
                    }
                }
                this._VertexCount = vertexCount;
            }

            for (int i = 0; i < this._VertexCount; ++i)
            {
                this._Vertices[i] = EditorUtils.CreateVector3Field($"顶点{i} 位置", this._Vertices[i]);
                this._Uv0s[i] = EditorUtils.CreateVector2Field($"顶点{i} uv0", this._Uv0s[i]);
            }
            
            int indexCount = EditorUtils.CreateIntField("索引数组长度:", this._IndexCount);
            if (indexCount != this._IndexCount)
            {
                if (this._Indices.Count < indexCount)
                {
                    for (int i = this._IndexCount; i < indexCount; ++i)
                    {
                        this._Indices.Add(0);
                    }
                }
                this._IndexCount = indexCount;
            }
            for (int i = 0; i < this._IndexCount; ++i)
            {
                this._Indices[i] = EditorUtils.CreateIntField($"索引{i} uv0", this._Indices[i]);
            }
            
            EditorUtils.CreateButton("生成网格", EditorParameters.k_ACButton, () =>
            {
                generateMesh();
            }, GUILayout.Height(20));
        }
    }

    void generateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector2> uv0s = new List<Vector2>();
        for (int i = 0; i < this._VertexCount; ++i)
        {
            vertices.Add(this._Vertices[i]);
            uv0s.Add(this._Uv0s[i]);
        }
        for (int i = 0; i < this._IndexCount; ++i)
        {
            indices.Add(this._Indices[i]);
        }
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv0s.ToArray();
        mesh.triangles = indices.ToArray();
        
        Debug.Log(Application.dataPath+"/"+"a.asset");
        AssetDatabase.CreateAsset(mesh, "Assets"+"/"+"a.asset");
        AssetDatabase.Refresh();
    }
}
