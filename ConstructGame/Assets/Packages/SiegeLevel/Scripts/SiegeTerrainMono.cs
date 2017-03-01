using UnityEngine;
using System.Collections;

public class SiegeTerrainMono : MonoBehaviour {

    [SerializeField]
    Vector2 TerrainWidth = new Vector2(256, 256);

	// Use this for initialization
	void Start () {
        SiegeTerrain.CreateBlankMap((int)TerrainWidth.x, (int)TerrainWidth.y);

        ProduceTiles();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void ProduceTiles() {
        int c = SiegeTerrainChunk.ChunkWidth;

        for (int x = 0; x < TerrainWidth.x / c; x++) {
            for (int y = 0; y < TerrainWidth.y / c; y++) {
                SiegeTerrainChunk t = new GameObject().AddComponent<SiegeTerrainChunk>();
                t.transform.parent = transform;
                t.gameObject.layer = LayerMask.NameToLayer("Terrain");
                t.gameObject.name = "SiegeTerrainChunk (" + x + ", " + y + ")";
                t.setAddress(x, y);
                t.UpdateMesh();
            }
        }

        ThreadedNavigator.GenerateNavMesh();

    }

}
