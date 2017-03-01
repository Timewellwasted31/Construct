using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class SiegeTerrainChunk : MonoBehaviour, IClickable {

    static Dictionary<Vector2, SiegeTerrainChunk> m_list = new Dictionary<Vector2, SiegeTerrainChunk>();
    public static SiegeTerrainChunk Find(Vector2 v) {
        SiegeTerrainChunk s = null;
        m_list.TryGetValue(v, out s);
        return s;
    }

    [SerializeField] Vector2 m_gridaddress;
    public void setAddress(int x, int y) {
        m_gridaddress = new Vector2(x, y);
        m_list.Add(m_gridaddress, this);
    }
    public const float TileWidth = 5.0f;
    public const float TileHeight = 5.0f;
    public const int ChunkWidth = 8;

    protected MeshFilter mf;
    protected MeshRenderer mr;
    protected MeshCollider mc;

    void UpdateTile(Vector3 worldpos) {
        byte b = SiegeTerrain.GetTile((int)(worldpos.x / TileWidth), (int)(worldpos.z / TileWidth));
        if (b == 0) b = 1;
        else b = 0;
        SiegeTerrain.UpdateTile((int)(worldpos.x / TileWidth), (int)(worldpos.z / TileWidth), b);
        UpdateMesh();

    }

    public void UpdateMesh() {

        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
        mr.material = Resources.Load<Material>("TestBlack");

        mc = GetComponent<MeshCollider>();

        transform.position = new Vector3(m_gridaddress.x * ChunkWidth * TileWidth, 0, m_gridaddress.y * ChunkWidth * TileWidth);

        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        for (int x = 0; x < ChunkWidth; x++) {
            for (int y = 0; y < ChunkWidth; y++) {
                byte b = SiegeTerrain.GetTile((int)m_gridaddress.x * ChunkWidth + x, (int)m_gridaddress.y * ChunkWidth + y);
                int index = verts.Count;

                // if ((x + y) % 2 == 0) b = 1;

                verts.Add(new Vector3(x * TileWidth, b * TileHeight, y*TileWidth));
                verts.Add(new Vector3(x * TileWidth + TileWidth, b * TileHeight, y * TileWidth));
                verts.Add(new Vector3(x * TileWidth, b * TileHeight, y * TileWidth + TileWidth));
                verts.Add(new Vector3(x * TileWidth + TileWidth, b * TileHeight, y * TileWidth + TileWidth));

                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));

                tris.Add(index + 0);
                tris.Add(index + 2);
                tris.Add(index + 1);

                tris.Add(index + 1);
                tris.Add(index + 2);
                tris.Add(index + 3);


                if (b > 0) {
                    // wall 1
                    index = verts.Count;

                    tris.Add(index + 0);
                    tris.Add(index + 1);
                    tris.Add(index + 2);

                    tris.Add(index + 1);
                    tris.Add(index + 3);
                    tris.Add(index + 2);

                    verts.Add(new Vector3(x * TileWidth, 0, y * TileWidth));
                    verts.Add(new Vector3(x * TileWidth, 0, (y+1) * TileWidth));
                    verts.Add(new Vector3(x * TileWidth, b * TileHeight, y * TileWidth));
                    verts.Add(new Vector3(x * TileWidth, b * TileHeight, (y + 1) * TileWidth));


                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(0, 1));
                    uvs.Add(new Vector2(1, 1));

                    // wall 2
                    index = verts.Count;

                    tris.Add(index + 0);
                    tris.Add(index + 2);
                    tris.Add(index + 1);

                    tris.Add(index + 1);
                    tris.Add(index + 2);
                    tris.Add(index + 3);

                    verts.Add(new Vector3(x * TileWidth + TileWidth, 0, y * TileWidth));
                    verts.Add(new Vector3(x * TileWidth + TileWidth, 0, (y + 1) * TileWidth));
                    verts.Add(new Vector3(x * TileWidth + TileWidth, b * TileHeight, y * TileWidth));
                    verts.Add(new Vector3(x * TileWidth + TileWidth, b * TileHeight, (y + 1) * TileWidth));


                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(0, 1));
                    uvs.Add(new Vector2(1, 1));

                    // wall 3
                    index = verts.Count;

                    tris.Add(index + 0);
                    tris.Add(index + 2);
                    tris.Add(index + 1);

                    tris.Add(index + 1);
                    tris.Add(index + 2);
                    tris.Add(index + 3);

                    verts.Add(new Vector3(x * TileWidth, 0, y * TileWidth));
                    verts.Add(new Vector3(x * TileWidth + TileWidth, 0, y * TileWidth));
                    verts.Add(new Vector3(x * TileWidth, b*TileHeight, y * TileWidth));
                    verts.Add(new Vector3(x * TileWidth + TileWidth, b * TileHeight, y * TileWidth));

                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(0, 1));
                    uvs.Add(new Vector2(1, 1));

                    // wall 4
                    index = verts.Count;

                    tris.Add(index + 0);
                    tris.Add(index + 1);
                    tris.Add(index + 2);

                    tris.Add(index + 1);
                    tris.Add(index + 3);
                    tris.Add(index + 2);

                    verts.Add(new Vector3(x * TileWidth, 0, (y+1) * TileWidth));
                    verts.Add(new Vector3(x * TileWidth + TileWidth, 0, (y + 1) * TileWidth));
                    verts.Add(new Vector3(x * TileWidth, b * TileHeight, (y + 1) * TileWidth));
                    verts.Add(new Vector3(x * TileWidth + TileWidth, b * TileHeight, (y + 1) * TileWidth));

                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(0, 1));
                    uvs.Add(new Vector2(1, 1));




                }

            }
        }

        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        mf.sharedMesh = mesh;
        mc.sharedMesh = mesh;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ClickAction(Vector3 WorldPos) {
        Vector3 localpos = WorldPos - transform.position;

        byte b = SiegeTerrain.GetTile(WorldPos.x / TileWidth, WorldPos.z / TileWidth);
        if (b > 0) b = 0; else b = 1;

        SiegeTerrain.UpdateTile(WorldPos.x / TileWidth, WorldPos.z / TileWidth, b);
        UpdateMesh();


        // TODO: Call navmesh patch function for this tile.
        ThreadedNavigator.GenerateNavMesh();

    }
}
