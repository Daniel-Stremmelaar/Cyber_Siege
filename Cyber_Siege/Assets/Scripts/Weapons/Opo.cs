using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opo : MonoBehaviour
{
    [SerializeField] Transform cube;
    [SerializeField] GameObject sphere;
    [SerializeField]MeshFilter filter;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        for (int i = 0; i < filter.sharedMesh.triangles.Length; i += 3)
        {
            Vector3 normalPos = filter.sharedMesh.vertices[filter.sharedMesh.triangles[i]];
            normalPos += filter.sharedMesh.vertices[filter.sharedMesh.triangles[i + 1]];
            normalPos += filter.sharedMesh.vertices[filter.sharedMesh.triangles[i + 2]];
            normalPos /= 3;
            Gizmos.DrawSphere(cube.TransformPoint(normalPos), 0.01f);

            Vector3 normalDirection = filter.sharedMesh.normals[filter.sharedMesh.triangles[i]];
            normalDirection += filter.sharedMesh.normals[filter.sharedMesh.triangles[i + 1]];
            normalDirection += filter.sharedMesh.normals[filter.sharedMesh.triangles[i + 1]];
            normalDirection /= 3;
            Gizmos.DrawLine(cube.TransformPoint(normalPos), cube.TransformPoint(normalPos + normalDirection));
        }


    }
}
