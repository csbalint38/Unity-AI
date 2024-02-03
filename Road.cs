using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Road : MonoBehaviour
{
    public SplineContainer splineContainer;
    public MeshFilter meshFilter;
    public int splineIndex;
    public float width = 3;
    public int resolution = 10;

    private float3 position;
    private float3 forward;
    private float3 upVector;
    private List<Vector3> vertsP1;
    private List<Vector3> vertsP2;


    private void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        meshFilter = GetComponent<MeshFilter>();
        GetVerts();
    }

    private void Update()
    {
        GetVerts();
        BuildMesh();

    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < resolution; i++)
        {
            float3 p1 = transform.TransformPoint(vertsP1[i]);
            float3 p2 = transform.TransformPoint(vertsP2[i]);
            Handles.SphereHandleCap(0, p1, Quaternion.identity, .1f, EventType.Repaint);
            Handles.SphereHandleCap(1, p2, Quaternion.identity, .1f, EventType.Repaint);
        }
    }

    private void GetVerts()
    {
        vertsP1 = new List<Vector3>();
        vertsP2 = new List<Vector3>();

        float step = 1f / (float)resolution;
        for (int i = 0; i < resolution; i++)
        {
            float t = step * i;
            SampleSplineWidth(t, out Vector3 p1, out Vector3 p2);
            vertsP1.Add(p1);
            vertsP2.Add(p2);
        }
    }

    private void SampleSplineWidth(float t, out Vector3 p1, out Vector3 p2)
    {
        splineContainer.Evaluate(splineIndex, t, out position, out forward, out upVector);
        Handles.matrix = transform.localToWorldMatrix;

        float3 localPosition = transform.InverseTransformPoint(position);

        float3 right = Vector3.Cross(forward, upVector).normalized;
        p1 = localPosition + (right * width);
        p2 = localPosition + (-right * width);
    }

    private void BuildMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();
        int offset = 0;

        int length = vertsP2.Count;

        for (int i = 1; i <= length; i++)
        {
            Vector3 p1 = vertsP1[i - 1];
            Vector3 p2 = vertsP2[i - 1];
            Vector3 p3;
            Vector3 p4;

            if (i == length)
            {
                p3 = vertsP1[0];
                p4 = vertsP2[0];
            }
            else
            {
                p3 = vertsP1[i];
                p4 = vertsP2[i];
            }

            offset = 4 * (i - 1);

            int t1 = offset;
            int t2 = offset + 2;
            int t3 = offset + 3;
            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset;

            verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
            indices.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });
        }

        m.SetVertices(verts);
        m.SetTriangles(indices, 0);
        meshFilter.mesh = m;
    }
}