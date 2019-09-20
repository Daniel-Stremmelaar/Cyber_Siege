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
                ConnectedTriangle connectedTriangle = GetConnectedTriangle(triangles, triangle, edge);
                if(connectedTriangle.connectedTriangle != null)
                {
                    Debug.Log("FOUND");
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
    ConnectedTriangle GetConnectedTriangle(List<Triangle> allTriangles, Triangle ownerTriangle, Vector2 requiredEdge)
    {
        MeshFilter filter = Selection.activeGameObject.GetComponent<MeshFilter>();
        Vector3 requiredX = Selection.activeGameObject.transform.TransformPoint(filter.sharedMesh.vertices[(int)requiredEdge.x]);
        Vector3 requiredY = Selection.activeGameObject.transform.TransformPoint(filter.sharedMesh.vertices[(int)requiredEdge.y]);
        foreach (Triangle triangleToCheck in allTriangles)
        {
            if (triangleToCheck != ownerTriangle)
            {
                foreach (int vertOne in triangleToCheck.verts)
                {
                    if (Selection.activeGameObject.transform.TransformPoint(filter.sharedMesh.vertices[vertOne]) == requiredX)
                    {
                        foreach (int vertTwo in triangleToCheck.verts)
                        {
                            if (Selection.activeGameObject.transform.TransformPoint(filter.sharedMesh.vertices[vertTwo]) == requiredY)
                            {
                                ConnectedTriangle connectedTriangle = new ConnectedTriangle(triangleToCheck, new Vector2(vertOne, vertTwo));
                                return connectedTriangle;
                            }
                        }
                        break;
                    }
                }
            }
        }
        return new ConnectedTriangle();
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
    public struct ConnectedTriangle
    {
        public Triangle connectedTriangle;
        public Vector2 edge;

        public ConnectedTriangle(Triangle connectedTri, Vector2 connectedEdge)
        {
            connectedTriangle = connectedTri;
            edge = connectedEdge;
        }
    }
}
