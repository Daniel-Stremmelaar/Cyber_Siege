using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EdgeCalculator : EditorWindow
{

    [MenuItem("Window/EdgeCalculator")]
    public static void ShowWindow()
    {
        GetWindow<EdgeCalculator>("EdgeCalculator");
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Calculate Edges"))
        {
            CalculateEdges();
        }
    }

    void CalculateEdges()
    {
        foreach(GameObject obj in Selection.gameObjects)
        {
            List<Triangle> triangles = new List<Triangle>();
            int[] objectTriangles = obj.GetComponent<MeshFilter>().sharedMesh.triangles;
            for (int currentVert = 0; currentVert < objectTriangles.Length; currentVert += 3)
            {
                Triangle triangle = new Triangle(objectTriangles[currentVert], objectTriangles[currentVert + 1], objectTriangles[currentVert + 2]);
                triangles.Add(triangle);
            }

            Debug.Log("There are " + triangles.Count.ToString() + " triangles.");
            foreach(Triangle triangle in triangles)
            {
                foreach(int vert in triangle.verts)
                {
                    Debug.Log(vert);
                }
            }
            Debug.Log("DONE");
        }
    }

    [System.Serializable]
    public class Triangle
    {
        public int[] verts;

        public Triangle(int one, int two, int three)
        {
            verts = new int[3];
            verts[0] = one;
            verts[1] = two;
            verts[2] = three;
        }
    }
}
