using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuControl
{
    CubePlayer Owner;
    UIMeshScript UI;
    UITileMap[,] UIData;
    GameObject MeshObj;
    Texture2D Atlas;
    [SerializeField]bool isDisplaying = false;
    [SerializeField]MenuOptions Mode = MenuOptions.Home;
    static Dictionary<ConstructType, Vector2[]> DisplayMapping = new Dictionary<ConstructType, Vector2[]>
    {
        {ConstructType.CubeGuy, TileUVIndex.CubeGuy},
        {ConstructType.Turret, TileUVIndex.Turret},
        {ConstructType.Car, TileUVIndex.Car},
        {ConstructType.Tank, TileUVIndex.Tank},
    };


    public MenuControl(GameObject DisplayMesh, CubePlayer owner)
    {
        Owner = owner;
        Atlas = Resources.Load<Texture2D>("IconAtlas");
        UIData = new UITileMap[3,3];
        for(int i = 0; i < 3; i++)
        {
            UIData[i, 0] = new UITileMap(TileUVIndex.Trans);
            UIData[i, 1] = new UITileMap(TileUVIndex.Trans);
            UIData[i, 2] = new UITileMap(TileUVIndex.Trans);
        }
        UI = new UIMeshScript();
        MeshObj = DisplayMesh;
        UI.init(15,15,MeshObj);
        UI.BuildUVMap(Atlas, UIData);
        UI.SetTexture(Atlas);
        ChangeDisplay();
        UI.Show(isDisplaying);
    }

    public void MainLoop()
    {
        if (isDisplaying == true)
        {
            MeshObj.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                switch (Mode)
                {
                    case MenuOptions.Home:
                        Mode = MenuOptions.Construct;
                        ChangeDisplay();
                        break;
                    case MenuOptions.Construct:
                        Owner.CurrentConstruct.Reset();
                        Owner.ConstructIndex = 0;
                        Owner.ClearToChange();
                        Mode = MenuOptions.Home;
                        isDisplaying = false;
                        UI.Show(isDisplaying);
                        break;
                    case MenuOptions.Building:
                        Owner.CurrentConstruct.setTurret(TurretTypes.MG);
                        Owner.CurrentConstruct.setMode(BuildMode.Turrets);
                        Mode = MenuOptions.Home;
                        isDisplaying = false;
                        UI.Show(isDisplaying);
                        break;
                    default:
                        Debug.Log("was set to last");
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                switch (Mode)
                {
                    case MenuOptions.Construct:
                        Owner.CurrentConstruct.Reset();
                        Owner.ConstructIndex = 1;
                        Owner.ClearToChange();
                        Mode = MenuOptions.Home;
                        isDisplaying = false;
                        UI.Show(isDisplaying);
                        break;
                    default:
                        Debug.Log("was set to last");
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                switch (Mode)
                {
                    case MenuOptions.Home:
                        Mode = MenuOptions.Building;
                        ChangeDisplay();
                        break;
                    case MenuOptions.Construct:
                        Owner.CurrentConstruct.Reset();
                        Owner.ConstructIndex = 2;
                        Owner.ClearToChange();
                        Mode = MenuOptions.Home;
                        isDisplaying = false;
                        UI.Show(isDisplaying);
                        break;
                    case MenuOptions.Building:
                        Owner.CurrentConstruct.setMode(BuildMode.Wall);
                        Mode = MenuOptions.Home;
                        isDisplaying = false;
                        UI.Show(isDisplaying);
                        break;
                    default:
                        Debug.Log("was set to last");
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                switch (Mode)
                {
                    case MenuOptions.Construct:
                        Owner.CurrentConstruct.Reset();
                        Owner.ConstructIndex = 3;
                        Owner.ClearToChange();
                        Mode = MenuOptions.Home;
                        isDisplaying = false;
                        UI.Show(isDisplaying);
                        break;
                    default:
                        Debug.Log("was set to last");
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (Mode != MenuOptions.Home)
                {
                    Mode = MenuOptions.Home;
                }
                else
                {
                    isDisplaying = false;
                    UI.Show(isDisplaying);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                Mode = MenuOptions.Home;
                isDisplaying = true;
                ChangeDisplay();
                UI.Show(isDisplaying);
            }
        }

    }


    public void ChangeDisplay()
    {
        for (int i = 0; i < 3; i++)
        {
            UIData[i, 0] = new UITileMap(TileUVIndex.Trans);
            UIData[i, 1] = new UITileMap(TileUVIndex.Trans);
            UIData[i, 2] = new UITileMap(TileUVIndex.Trans);
        }
        if (Mode == MenuOptions.Home)
        {
            UIData[1, 0].UVindex = TileUVIndex.BigB;
            UIData[1, 2].UVindex = TileUVIndex.BigC;
            UI.BuildUVMap(Atlas, UIData);
            UI.SetTexture(Atlas);
        }
        else if(Mode == MenuOptions.Construct)
        {
            UIData[1, 2].UVindex = Owner.ConstructsObtained[0] == null ? TileUVIndex.Trans : DisplayMapping[Owner.ConstructsObtained[0].isType];
            UIData[2, 1].UVindex = Owner.ConstructsObtained[1] == null ? TileUVIndex.Trans : DisplayMapping[Owner.ConstructsObtained[1].isType];
            UIData[1, 0].UVindex = Owner.ConstructsObtained[2] == null ? TileUVIndex.Trans : DisplayMapping[Owner.ConstructsObtained[2].isType];
            UIData[0, 1].UVindex = Owner.ConstructsObtained[3] == null ? TileUVIndex.Trans : DisplayMapping[Owner.ConstructsObtained[3].isType];
            UI.BuildUVMap(Atlas, UIData);
            UI.SetTexture(Atlas);
            //get Player constructs 
        }
        else if(Mode == MenuOptions.Building)
        {
            UIData[1, 0].UVindex = TileUVIndex.Wall;
            UIData[1, 2].UVindex = TileUVIndex.MGTurret;
            UI.BuildUVMap(Atlas, UIData);
            UI.SetTexture(Atlas);
            //get TurretIcons
        }

    }
}

public class UIMeshScript
{

        int size_x, size_y;
        int tileSize = 10;
        int numTiles, numTris;
        int vsize_x, vsize_y;
        int numVerts;

        Vector3[] vertices;
        Vector3[] normals;
        public Vector2[] uv;

        int[] triangles;

        int squareIndex;
        int triOffset;

        Mesh mesh;
        MeshFilter mesh_filter;

        Texture2D Grid;
        GameObject DisplayLayer;

    public void init(int Sizex, int Sizey, GameObject DisplayLayer, int tileSize = 5)
    {
        
        this.DisplayLayer = DisplayLayer;
        this.tileSize = tileSize;
        size_x = Sizex / tileSize;
        size_y = Sizey / tileSize;
        numTiles = (size_x * size_y);
        numTris = numTiles * 2;

        vsize_x = (size_x) * 2;
        vsize_y = (size_y) * 2;
        numVerts = (vsize_x * vsize_y);

        // Generate the mesh data
        vertices = new Vector3[numVerts];
        normals = new Vector3[numVerts];
        uv = new Vector2[numVerts];

        triangles = new int[numTris * 3];
        int x, y;
        for (int i = 0; i < numVerts; i++)
        {
            normals[i] = Vector3.up;
        }
        int posx = 0, posy = 0;
        for (y = 0; y < vsize_y; y++)
        {
            posx = 0;
            for (x = 0; x < vsize_x; x++)
            {
                int index = y * vsize_x + x;
                vertices[index] = new Vector3((posx * tileSize - (vsize_x*2.5f)/2), 0, (posy * tileSize - (vsize_y*2.5f)/2));
                if (x / 2 == ((float)x) / 2)
                {
                    posx++;
                }
            }
            if (y / 2 == ((float)y) / 2)
            {
                posy++;
            }

        }

        for (y = 0; y < size_y; y++)
        {
            for (x = 0; x < size_x; x++)
            {
                squareIndex = y * size_x + x;
                int triIndex = (y * 2) * vsize_x + (x * 2);
                triOffset = squareIndex * 6;
                triangles[triOffset + 0] = triIndex;
                triangles[triOffset + 1] = triIndex + vsize_x;
                triangles[triOffset + 2] = triIndex + vsize_x + 1;

                triangles[triOffset + 3] = triIndex;
                triangles[triOffset + 4] = triIndex + vsize_x + 1;
                triangles[triOffset + 5] = triIndex + 1;
            }
        }


        // Create a new Mesh and populate with the data
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        // Assign our mesh to our filter/renderer/collider
        mesh_filter = DisplayLayer.GetComponent<MeshFilter>();

        mesh_filter.sharedMesh = mesh;

    }

    public void SetTexture(Texture2D Grid)
        {
            Grid.filterMode = FilterMode.Point;
            Grid.wrapMode = TextureWrapMode.Clamp;
            Grid.Apply();

            MeshRenderer mesh_renderer = DisplayLayer.GetComponent<MeshRenderer>();
            mesh_renderer.sharedMaterials[0].mainTexture = Grid;

            Debug.Log("Done Texture!");
        }

        public void BuildUVMap(Texture2D Grid, UITileMap[,] UIData)
        {
            this.Grid = Grid;

            for(int y = 0; y < UIData.GetLength(0) * 2; y +=2)
            {
               for(int x = 0; x < UIData.GetLength(1) * 2; x +=2)
               {
                    int index = y * ((UIData.GetLength(1)) * 2) + x;
                    uv[index] = UIData[x / 2, y / 2].UVindex[0]; //Debug.Log(index);
                    uv[index + 1] = UIData[x / 2, y / 2].UVindex[1]; //Debug.Log(index + 1);
                    uv[index + (UIData.GetLength(1) *2)] = UIData[x / 2, y / 2].UVindex[2];// Debug.Log(index + (UIData.TileMap.GetLength(1)*2));
                    uv[index + 1 + ((UIData.GetLength(1) * 2))] = UIData[x / 2, y / 2].UVindex[3];// Debug.Log(index + (UIData.TileMap.GetLength(1) * 2) + 1);
            } 
            }
            mesh_filter.mesh.uv = uv;
            Debug.Log("Done UVMap!");
            SetTexture(Grid);
        }

        public void Show(bool b)
        {
            DisplayLayer.SetActive(b);
        }
}

public class UITileMap
{

    public UITileMap(Vector2[] UVs)
    {
        UVindex = UVs;
    }

    public Vector2[] UVindex;


}

public static class TileUVIndex
{
    //Constructs
    public static Vector2[] CubeGuy = new Vector2[4] { new Vector2(0, 0), new Vector2(0.25f, 0), new Vector2(0, 0.25f), new Vector2(0.25f, 0.25f) };
    public static Vector2[] Car = new Vector2[4] { new Vector2(0, 0.25f), new Vector2(0.25f, 0.25f), new Vector2(0, 0.5f), new Vector2(0.25f, 0.5f) };
    public static Vector2[] Turret = new Vector2[4] { new Vector2(0, 0.5f), new Vector2(0.25f, 0.5f), new Vector2(0, 0.75f), new Vector2(0.25f, 0.75f) };
    public static Vector2[] Tank = new Vector2[4] { new Vector2(0f, 0.75f), new Vector2(0.25f, 0.75f), new Vector2(0f, 0.75f), new Vector2(0.25f, 0.75f) };

    //Turrets/wall
    public static Vector2[] MGTurret = new Vector2[4] { new Vector2(0.5f, 0), new Vector2(0.75f, 0), new Vector2(0.5f, 0.25f), new Vector2(0.75f, 0.25f) };
    public static Vector2[] Wall = new Vector2[4] { new Vector2(0.5f, 0.25f), new Vector2(0.75f, 0.25f), new Vector2(0.5f, 0.5f), new Vector2(0.75f, 0.5f) };

    //Menu Icons
    public static Vector2[] BigB = new Vector2[4] { new Vector2(0.25f, 0), new Vector2(0.5f, 0), new Vector2(0.25f, 0.25f), new Vector2(0.5f, 0.25f) };
    public static Vector2[] BigC = new Vector2[4] { new Vector2(0.25f, 0.25f), new Vector2(0.5f, 0.25f), new Vector2(0.25f, 0.5f), new Vector2(0.5f, 0.5f) };
    public static Vector2[] Trans = new Vector2[4] { new Vector2(0.25f, 0.75f), new Vector2(0.5f, 0.75f), new Vector2(0.25f, 1f), new Vector2(0.5f, 1f) };

}

public enum MenuOptions
{
    Construct,
    Building,
    Home,
    last
}
