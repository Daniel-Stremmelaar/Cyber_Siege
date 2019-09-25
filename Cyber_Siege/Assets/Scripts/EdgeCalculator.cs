using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EdgeCalculator : EditorWindow
{
    public float maxAngle, minAngle;
    public GameObject indicator;

    List<GameObject> test = new List<GameObject>();

    List<ObjectData> allObjectData = new List<ObjectData>();
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
        if (GUILayout.Button("Remove EdgePoints"))
        {
            RemoveIndicationPoints();
        }
    }

    public void CalculateAngles()
    {
        foreach(ObjectData objectData in allObjectData)
        {
            if (objectData.indicationPointHolder)
            {
                DestroyImmediate(objectData.indicationPointHolder.gameObject);
            }
            objectData.indicationPointHolder = GameObject.Instantiate(new GameObject("IndicationHolder"), objectData.thisObject.transform.position, Quaternion.identity, objectData.thisObject.transform).transform;
            objectData.allIndicationPoints = new List<GameObject>();
            objectData.trianglesWithRightAngle = new List<CombinedTriangleData>();
            Debug.Log(objectData.combinedTriangles.Count);
            foreach (CombinedTriangleData data in objectData.combinedTriangles)
            {
                int ogTriIndex = 0;
                int connectedTriIndex = 0;
                for (int index = 0; index < objectData.allTriangles.Count; index++)
                {
                    if (objectData.allTriangles[index] == data.connectedTriangleOne)
                    {
                        ogTriIndex = index;
                    }
                    if (objectData.allTriangles[index] == data.connectedTriangleTwo)
                    {
                        connectedTriIndex = index;
                    }
                }
                if (HasRightAngle(connectedTriIndex, ogTriIndex, objectData))
                {
                    AddIndicators(data, objectData).transform.SetParent(objectData.indicationPointHolder);
                }
            }
        }
    }

    public GameObject AddIndicators(CombinedTriangleData data_, ObjectData ownerData)
    {
        MeshFilter filter = ownerData.thisObject.GetComponent<MeshFilter>();
        ownerData.trianglesWithRightAngle.Add(data_);

        Vector3 centerPosition = (filter.sharedMesh.vertices[(int)data_.edgeOne.x] + filter.sharedMesh.vertices[(int)data_.edgeOne.y]) / 2;
        GameObject newIndicator = Instantiate(indicator, ownerData.thisObject.transform.TransformPoint(centerPosition), Quaternion.identity, ownerData.thisObject.transform);
        newIndicator.transform.LookAt(ownerData.thisObject.transform.TransformPoint(filter.sharedMesh.vertices[(int)data_.edgeOne.x]));

        Vector3 newColliderSize = newIndicator.GetComponent<BoxCollider>().size;
        newColliderSize.z = Mathf.Abs(Vector3.Distance(ownerData.thisObject.transform.TransformPoint(filter.sharedMesh.vertices[(int)data_.edgeOne.x]), ownerData.thisObject.transform.TransformPoint(filter.sharedMesh.vertices[(int)data_.edgeOne.y])));
        newColliderSize *= (1 / newIndicator.transform.localScale.z);
        newIndicator.GetComponent<BoxCollider>().size = newColliderSize;

        return newIndicator;
    }
    public void GetCombinedEdges()
    {
        allObjectData = GetObjectsData();

        foreach(ObjectData objectData in allObjectData)
        {
            objectData.combinedTriangles = new List<CombinedTriangleData>();
            objectData.trianglesWithRightAngle = new List<CombinedTriangleData>();
            foreach (Triangle triangle in objectData.allTriangles)
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
                    ConnectedTriangle connectedTriangle = GetConnectedTriangle(objectData, triangle, edge);
                    if (connectedTriangle.connectedTriangle != null)
                    {
                        objectData.combinedTriangles.Add(new CombinedTriangleData(triangle, edge, connectedTriangle.connectedTriangle, connectedTriangle.edge));
                    }
                }
            }
        }
    }

    List<ObjectData> GetObjectsData()
    {
        List<ObjectData> objectData = new List<ObjectData>();
        foreach (GameObject obj in Selection.gameObjects)
        {
            ObjectData newData = new ObjectData(obj);
            newData.allTriangles = new List<Triangle>();
            int[] objectTriangles = obj.GetComponent<MeshFilter>().sharedMesh.triangles;
            for (int currentVert = 0; currentVert < objectTriangles.Length; currentVert += 3)
            {
                Triangle triangle = new Triangle(objectTriangles[currentVert], objectTriangles[currentVert + 1], objectTriangles[currentVert + 2]);
                newData.allTriangles.Add(triangle);
            }
            objectData.Add(newData);
        }
        return objectData;
    }
    ConnectedTriangle GetConnectedTriangle(ObjectData ownerObject, Triangle ownerTriangle, Vector2 requiredEdge)
    {
        MeshFilter filter = ownerObject.thisObject.GetComponent<MeshFilter>();
        Vector3 requiredX = ownerObject.thisObject.transform.TransformPoint(filter.sharedMesh.vertices[(int)requiredEdge.x]);
        Vector3 requiredY = ownerObject.thisObject.transform.TransformPoint(filter.sharedMesh.vertices[(int)requiredEdge.y]);
        foreach (Triangle triangleToCheck in ownerObject.allTriangles)
        {
            if (triangleToCheck != ownerTriangle)
            {
                foreach (CombinedTriangleData combinedTriangles in ownerObject.combinedTriangles)
                {
                    if (ownerTriangle == combinedTriangles.connectedTriangleTwo && triangleToCheck == combinedTriangles.connectedTriangleOne)
                    {
                        return new ConnectedTriangle();
                    }
                }
                foreach (int vertOne in triangleToCheck.verts)
                {
                    if (ownerObject.thisObject.transform.TransformPoint(filter.sharedMesh.vertices[vertOne]) == requiredX)
                    {
                        foreach (int vertTwo in triangleToCheck.verts)
                        {
                            if (ownerObject.thisObject.transform.TransformPoint(filter.sharedMesh.vertices[vertTwo]) == requiredY)
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

    bool HasRightAngle(int connectedTriangleIndex, int ogTriangleIndex, ObjectData ownerObject)
    {
        
        MeshFilter filter = ownerObject.thisObject.GetComponent<MeshFilter>();
        Vector3 ogTriangleNormal = filter.sharedMesh.normals[ownerObject.allTriangles[ogTriangleIndex].verts[0]];
        Vector3 connectedTriangleNormal = filter.sharedMesh.normals[ownerObject.allTriangles[connectedTriangleIndex].verts[0]];
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

    void RemoveIndicationPoints()
    {
        foreach(GameObject thisObject in Selection.gameObjects)
        {
            for(int i = thisObject.transform.childCount - 1; i >= 0; i--)
            {
                Transform thisChild = thisObject.transform.GetChild(i);
                if (thisChild.name == "IndicationHolder" + "(Clone)")
                {
                    DestroyImmediate(thisChild.gameObject);
                }
            }
        }
    }
    public class ObjectData
    {
        public List<GameObject> allIndicationPoints = new List<GameObject>();

        public List<Triangle> allTriangles;
        public List<CombinedTriangleData> trianglesWithRightAngle;
        public List<CombinedTriangleData> combinedTriangles;

        public GameObject thisObject;

        public Transform indicationPointHolder;

        public ObjectData(GameObject thisObject_)
        {
            thisObject = thisObject_;
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
