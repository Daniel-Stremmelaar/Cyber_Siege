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
    public void CalculateEdges()
    {
        List<Triangle> triangles = GetTriangles();

        foreach (Triangle triangle in triangles)
        {
            for (int i = 0; i < triangle.verts.Length; i++)
            {
                //Gets to know what edge we are gonna calculate
                Vector2 edge;
                edge.x = triangle.verts[i];
                if (i == triangle.verts.Length - 1)
                {
                    edge.y = triangle.verts[0];
                }
                else
                {
                    edge.y = triangle.verts[i + 1];
                }
            }
        }
    }

    List<Triangle> GetTriangles()
    {
        List<Triangle> triangles = new List<Triangle>();
        foreach (GameObject obj in Selection.gameObjects)
        {
            int[] objectTriangles = obj.GetComponent<MeshFilter>().sharedMesh.triangles;
            for (int currentVert = 0; currentVert < objectTriangles.Length; currentVert += 3)
            {
                Triangle triangle = new Triangle(objectTriangles[currentVert], objectTriangles[currentVert + 1], objectTriangles[currentVert + 2]);
                triangles.Add(triangle);
            }

            Debug.Log("There are " + triangles.Count.ToString() + " triangles.");
        }
        return triangles;
    }
    Triangle GetConnectedEdge(List<Triangle> allTriangles, Triangle ownerTriangle, Vector2 requiredEdge)
    {
        foreach (Triangle triangleToCheck in allTriangles)
        {
            if (triangleToCheck != ownerTriangle)
            {
                foreach (int vertOne in triangleToCheck.verts)
                {
                    if (vertOne == requiredEdge.x)
                    {
                        foreach (int vertTwo in triangleToCheck.verts)
                        {
                            if (vertTwo == requiredEdge.y)
                            {
                                return triangleToCheck;
                            }
                        }
                    }
                    break;
                }
            }
        }
        return null;
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
