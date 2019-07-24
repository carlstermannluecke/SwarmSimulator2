#if UNITY_EDITOR //This script is intended for in editor use only!

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

//Attach this script to a GameObject.
//Then right click the attached script in the inspector at editor time (click on the script name).
//A context menu appears, which offers the specified functions.

/// <summary>
/// Can be used to create line meshes without faces out of Wavefront .obj files.
/// </summary>
public class GizmoEditorUtility : MonoBehaviour
{
    [ContextMenu("CreateLineMeshFromOBJ")]
    public void CreateLineMeshFromOBJ()
    {
        string path_obj = UnityEditor.EditorUtility.OpenFilePanel("Select Wavefront .obj", "Assets/", "obj");
        if (CheckPath(path_obj) == false) return;

        string obj_name = Path.GetFileNameWithoutExtension(path_obj);
        FileStream fileStream = new FileStream(path_obj, FileMode.Open, FileAccess.Read);
        StreamReader fileReader = new StreamReader(fileStream, System.Text.Encoding.UTF8, true, 128);

        Mesh mesh = new Mesh();
        mesh.name = obj_name;
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        string textline;
        while ((textline = fileReader.ReadLine()) != null)
        {
            //Debug.Log(textline);
            string[] items = textline.Split(null); //Splitting by null equals splitting by whitespace

            if (items[0] == "v")
            {
                Vector3 vertex = new Vector3(float.Parse(items[1]), float.Parse(items[2]), float.Parse(items[3]));
                vertices.Add(vertex);
            }
            if (items[0] == "l")
            {
                //Wavefront obj indices start at 1 not 0, hence -1
                indices.Add(int.Parse(items[1])-1);
                indices.Add(int.Parse(items[2])-1);
            }
        }
        
        int[] edgeArray = indices.ToArray();
        mesh.SetVertices(vertices);
        mesh.SetIndices(edgeArray, MeshTopology.Lines, 0);

        string path_save = UnityEditor.EditorUtility.SaveFilePanel("Save mesh asset", "Assets/", mesh.name, "asset");
        if (CheckPath(path_save) == false) return;

        path_save = FileUtil.GetProjectRelativePath(path_save);
        AssetDatabase.CreateAsset(mesh, path_save);
        AssetDatabase.SaveAssets();
    }

    private bool CheckPath(string path)
    {
        bool isFeasable = true;
        if (string.IsNullOrEmpty(path))
        {
            isFeasable = false;
            Debug.Log("Chosen path is infeasable.");
        }

        return isFeasable;
    }
}

#endif