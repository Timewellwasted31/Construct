using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class GeneratePyramid : MonoBehaviour {

    [SerializeField] bool DoGeneration = false;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (DoGeneration)
        {
            DoGeneration = false;
            GenerateMesh();
        }
	}

    void GenerateMesh()
    {

        if (GetComponent<MeshRenderer>() == null || GetComponent<MeshFilter>() == null) return;

        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();


        // Vector3[] points = new Vector3[] { new Vector3(1f, 0, -1f/Mathf.Sqrt(2)), new Vector3(-1f, 0, -1f / Mathf.Sqrt(2)), new Vector3(0, +1, 1f / Mathf.Sqrt(2)), new Vector3(0, -1, 1f / Mathf.Sqrt(2)) };

        Vector3[] points = new Vector3[] { new Vector3(1f, 1f, 1f), new Vector3(1f, -1f, -1f), new Vector3(-1f,1f,-1f), new Vector3(-1f,-1f,1f)};
        // we need to generate 4 triangles.
        // so, 4 * 3 * 2 * 1 = 24, which generates 8. 
        // so, we take point 1, generate 3 triangles from combinations of the remaining points.
        // then we do the final triangle.

        AddTriangle(points[0], points[2], points[1], verts, uvs, tris);
        AddTriangle(points[0], points[3], points[1], verts, uvs, tris);
        AddTriangle(points[0], points[2], points[3], verts, uvs, tris);
        AddTriangle(points[1], points[2], points[3], verts, uvs, tris);

        AddTriangle(points[0], points[1], points[2], verts, uvs, tris);
        AddTriangle(points[0], points[1], points[3], verts, uvs, tris);
        AddTriangle(points[0], points[3], points[2], verts, uvs, tris);
        AddTriangle(points[1], points[3], points[2], verts, uvs, tris);

        Quaternion q = Quaternion.FromToRotation(verts[0], Vector3.up);
        Vector3 temp = q * verts[1];
        Quaternion q2 = Quaternion.Euler(0, 255, 0);

        for (int i = 0; i < verts.Count; i++) {
            verts[i] = q * verts[i];
            verts[i] = q2 * verts[i];
        }

        m.SetVertices(verts);
        m.SetUVs(0, uvs);
        m.SetTriangles(tris, 0);

        m.RecalculateBounds();
        m.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = m;
        AssetDatabase.CreateAsset(m, "Assets/Exported/PyramidMesh.asset");
        Debug.Log("[GENERATEPYRAMID] Exported mesh: /Assets/Exported/PyramidMesh");
    }

    void AddTriangle(Vector3 a, Vector3 b, Vector3 c, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
    {
        int index = verts.Count;
        verts.Add(a / a.magnitude);
        verts.Add(b / b.magnitude);
        verts.Add(c / c.magnitude);

        uvs.Add(new Vector2(0,0));
        uvs.Add(new Vector2(1f, 0));
        uvs.Add(new Vector2(0.5f, 1f));

        tris.Add(index + 0);
        tris.Add(index + 1);
        tris.Add(index + 2);
    }

}
