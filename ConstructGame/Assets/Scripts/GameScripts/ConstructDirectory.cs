using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public static class ConstructDirectory
{
    public static List<ConstructData>   Directory;
    public static List<Projectile>      ProjectilePool;
    static GameObject[]                 ProjectilePrefabs;
    static GameObject[]                 TurretPrefabs;
    public static bool IsLoaded = false;
    public static int conNum = 0;

    public static void Load()
    {
        Directory = new List<ConstructData>();
        ProjectilePool = new List<Projectile>();
        ProjectilePrefabs = Resources.LoadAll<GameObject>("Projectiles");
        TurretPrefabs = Resources.LoadAll<GameObject>("Turrets");
<<<<<<< HEAD
        List<Constructs> Import = ImportConstructs("../ConstructGame/Constructs");
=======
        TextAsset xmldata = Resources.Load("XML/Constructs") as TextAsset;
        MemoryStream assetstream = new MemoryStream(xmldata.bytes);
        var serilaizer = new XmlSerializer(typeof(List<Constructs>));
        List<Constructs> Import = serilaizer.Deserialize(assetstream) as List<Constructs>;
>>>>>>> origin/master
        ConstructData Temp;
        for (int i = 0; i < Import.Count; i++)
        {
            Temp = null;
            switch (Import[i].ConstructName)
            {
                case "Tank":
                    Temp = new Construct_Vehicle_Tank();
                    Temp.isType = ConstructType.Tank;
                    break;
                case "Car":
                    Temp = new Construct_Vehicle();
                    Temp.isType = ConstructType.Car;
                    break;
                case "Turret":
                    Temp = new Construct_Immobile_Turret();
                    Temp.isType = ConstructType.Turret;
                    break;
                case "CubeGuy":
                    Temp = new Construct_Humanoid();
                    Temp.isType = ConstructType.CubeGuy;
                    break;
                case "Key":
                    Temp = new Construct_Immobile();
                    Temp.isType = ConstructType.Key;
                    break;
                case "ForkLift":
                    Temp = new Construct_Vehicle_ForkLift();
                    Temp.isType = ConstructType.ForkLift;
                    break;
            }
            Temp.ConstructContained = Import[i];
            Directory.Add(Temp);
        }
        IsLoaded = true;
    }

    public static ConstructData GetRandom()
    {
        int Index = 0;
        Index = Random.Range(0, Directory.Count);
        ConstructData temp = Directory[Index].Clone();
        temp.ConstructContained = new Constructs(Directory[Index].ConstructContained);
        return temp;
    } 

    public static ConstructData Find(int Index)
    {
        ConstructData temp = Directory[Index].Clone();
        temp.ConstructContained = new Constructs(Directory[Index].ConstructContained);
        conNum++;
        return temp;
    }

    public static ConstructData Find(ConstructType Name)
    {
        for(int i = 0; i < Directory.Count; i++)
        {
            if(Directory[i].isType == Name)
            {
                ConstructData temp = Directory[i].Clone();
                temp.ConstructContained = new Constructs(Directory[i].ConstructContained);
                temp.isType = Name;
                conNum++;
                return temp;
            }
        }
        return null;
    }

    public static List<Constructs> ImportConstructs(string path, string fileName = "Constructs.xml")
    {
        var serilaizer = new XmlSerializer(typeof(List<Constructs>));
        using (var stream = new FileStream(path + "/" + fileName, FileMode.Open))
        {
            return serilaizer.Deserialize(stream) as List<Constructs>;
        }
    }

    public static Projectile AddProjectile(ProjectileName name)
    {
        GameObject temp = null;
        switch(name)
        {
            case ProjectileName.Hammer :
                for(int i = 0; i < ProjectilePrefabs.Length; i++)
                {
                    if(ProjectilePrefabs[i].name == "Hammer")
                    {
                        temp = GameObject.Instantiate(ProjectilePrefabs[i], Vector3.zero, Quaternion.identity) as GameObject;
                        ProjectilePool.Add(temp.GetComponent<Projectile>());
                    }
                }
            return temp.GetComponent<Projectile>();

            case ProjectileName.Bullet :
                for (int i = 0; i < ProjectilePrefabs.Length; i++)
                {
                    if (ProjectilePrefabs[i].name == "Bullet")
                    {
                        temp = GameObject.Instantiate(ProjectilePrefabs[i], Vector3.zero, Quaternion.identity) as GameObject;
                        ProjectilePool.Add(temp.GetComponent<Projectile>());
                    }
                }
            return temp.GetComponent<Projectile>();

            case ProjectileName.Shell:
                for (int i = 0; i < ProjectilePrefabs.Length; i++)
                {
                    if (ProjectilePrefabs[i].name == "Shell")
                    {
                        temp = GameObject.Instantiate(ProjectilePrefabs[i], Vector3.zero, Quaternion.identity) as GameObject;
                        ProjectilePool.Add(temp.GetComponent<Projectile>());
                    }
                }
            return temp.GetComponent<Projectile>();

            default:
                Debug.Log("Error No Projectile Prefab " + name);
                Debug.Break();
            return null;

        }
    }

    public static Projectile GetProjectile(ProjectileName name)
    {
        Projectile temp = null; 
        for(int i = 0; i < ProjectilePool.Count; i++)
        {
            if(ProjectilePool[i].getpName == name && ProjectilePool[i].getOwner == null)
            {
                temp = ProjectilePool[i];
                break;
            }
        }
        if(temp != null)
        {
            return temp;
        }
        else
        {
            return AddProjectile(name);
        }
    }

    public static GameObject GetTurret(TurretTypes name)
    {
        GameObject temp = null;
        switch (name)
        {
            case TurretTypes.MG:
                for (int i = 0; i < TurretPrefabs.Length; i++)
                {
                    Debug.Log("Seaching...");
                    if (TurretPrefabs[i].name == "MachineGunTurret")
                    {
                        Debug.Log("Found");
                        temp = GameObject.Instantiate(TurretPrefabs[i], Vector3.zero, Quaternion.identity) as GameObject;
                    }
                }
                return temp;

            default:
                Debug.Log("Error No Projectile Prefab " + name);
                Debug.Break();
                return null;

        }
    }

}

public enum ConstructType
{
    Car,
    CubeGuy,
    Tank,
    Key,
    Turret,
    ForkLift,
    Last
}

