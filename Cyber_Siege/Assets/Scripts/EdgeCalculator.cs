using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EdgeCalculator : EditorWindow
{
    public float maxAngle, minAngle;
    public GameObject indicator;

    List<GameObject> test = new List<GameObject>();

    List<Triangle> allTriangles;
    List<CombinedTriangleData> trianglesWithRightAngle = new List<CombinedTriangleData>();
    List<CombinedTriangleData> combinedTriangles = new List<CombinedTriangleData>();
    [MenuItem("Window/EdgeCalculator")]
    public static void ShowWindow()
    {
        GetWindow<EdgeCalculator>("EdgeCalculator");
    }

    private void OnGUI()
    {
        indicator = (GameObject)EditorGUILayout.ObjectField(indicator, typeof(GameObject), false);
        minAngle = EditorGUILayout.FloatField("Minimal Angle", minAngle);
        minAngle = Mathf.Clamp(minAngle, 0, 180);
        maxAngle = EditorGUILayout.FloatField("Maximum Angle", maxAngle);
        maxAngle = Mathf.Clamp(maxAngle, 0, 180);
        if(maxAngle < minAngle)
        {
            maxAngle = minAngle;
        }
        else
        {
            if(minAngle > maxAngle)
            {
                minAngle = maxAngle;
            }
        }
        if (GUILayout.Button("Get Combined Edges"))
        {
            GetCombinedEdges();
        }
        if (GUILayout.Button("Calculate Angles"))
        {
            CalculateAngles();
        }
    }

    public void CalculateAngles()
    {
        foreach(GameObject obj in test)
        {
            DestroyImmediate(obj);
        }
        test = new List<GameObject>();
        trianglesWithRightAngle = new List<CombinedTriangleData>();
        foreach (CombinedTriangleData data in combinedTriangles)
        {
            int ogTriIndex = 0;
            int connectedTriIndex = 0;
            for (int index = 0; index < allTriangles.Count; index++)
            {
                if (allTriangles[index] == data.connectedTriangleOne)
                {
                    ogTriIndex = index;
                }
                if (allTriangles[index] == data.connectedTriangleTwo)
                {
                    connectedTriIndex = index;
                }
            }
            if (HasRightAngle(connectedTriIndex, ogTriIndex))
            {
                AddIndicators(data);
            }
        }
    }

    public void AddIndicators(CombinedTriangleData data_)
    {
        MeshFilter filter = Selection.activeGameObject.GetComponent<MeshFilter>();
        trianglesWithRightAngle.Add(data_);

        Vector3 centerPosition = (filter.sharedMesh.vertices[(int)data_.edgeOne.x] + filter.sharedMesh.vertices[(int)data_.edgeOne.y]) / 2;
        test.Add(Instantiate(indicator, Selection.activeGameObject.transform.TransformPoint(centerPosition), Quaternion.identity, Selection.activeGameObject.transform));
        test[test.Count - 1].transform.LookAt(Selection.activeGameObject.transform.TransformPoint(filter.sharedMesh.vertices[(int)data_.edgeOne.x]));

        Vector3 newColliderSize = test[test.Count - 1].GetComponent<BoxCollider>().size;
        newColliderSize.z = Mathf.Abs(Vector3.Distance(Selection.activeGameObject.transform.TransformPoint(filter.sharedMesh.vertices[(int)data_.edgeOne.x]), Selection.activeGameObject.transform.TransformPoint(filter.sharedMesh.vertices[(int)data_.edgeOne.y])));
        newColliderSize *= (1 / test[test.Count - 1].transform.localScale.z);
        test[test.Count - 1].GetComponent<BoxCollider>().size = newColliderSize;

    }
    public void GetCombinedEdges()
    {
        allTriangles = GetTriangles();

        foreach (Triangle triangle in allTriangles)
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
                ConnectedTriangle connectedTriangle = GetConnectedTriangle(allTriangles, triangle, edge);
                if (connectedTriangle.connectedTriangle != null)
                {
                    combinedTriangles.Add(new CombinedTriangleData(triangle, edge, connectedTriangle.connectedTriangle, connectedTriangle.edge));
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
                foreach (CombinedTriangleData combinedTriangles in combinedTriangles)
                {
                    if (ownerTriangle == combinedTriangles.connectedTriangleTwo && triangleToCheck == combinedTriangles.connectedTriangleOne)
                    {
                        return new ConnectedTriangle();
                    }
                }
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

    bool HasRightAngle(int connectedTriangleIndex, int ogTriangleIndex)
    {
        
        MeshFilter filter = Selection.activeGameObject.GetComponent<MeshFilter>();
        Vector3 ogTriangleNormal = filter.sharedMesh.normals[allTriangles[ogTriangleIndex].verts[0]];
        Vector3 connectedTriangleNormal = filter.sharedMesh.normals[allTriangles[connectedTriangleIndex].verts[0]];
        if(Mathf.Abs(Vector3.Angle(ogTriangleNormal, connectedTriangleNormal)) >= minAngle && Mathf.Abs(Vector3.Angle(ogTriangleNormal, connectedTriangleNormal)) <= maxAngle)
        {
            Debug.Log("RIGHT ANGLE");
            return true;
        }
        else
        {
            Debug.Log("WRONG ANGLE");
        }
        return false;
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
    public struct CombinedTriangleData
    {
        public Triangle connectedTriangleOne;
        public Vector2 edgeOne;

        public Triangle connectedTriangleTwo;
        public Vector2 edgeTwo;

        public CombinedTriangleData(Triangle connectedTriangleOne_, Vector2 edgeOne_, Triangle connectedTriangleTwo_, Vector2 edgeTwo_)
        {
            connectedTriangleOne = connectedTriangleOne_;
            edgeOne = edgeOne_;

            connectedTriangleTwo = connectedTriangleTwo_;
            edgeTwo = edgeTwo_;
        }
    }
}
