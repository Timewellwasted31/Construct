using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
[XmlRoot("ConstList")]
[System.Serializable]

public class CubeData
{
    public Vector3      position;
    public Quaternion   rotation;

    public CubeData()
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
    }

    public CubeData(Transform Cube)
    {
        position = Cube.localPosition;
        rotation = Cube.localRotation;
    }

    public static implicit operator CubeData(Transform t)
    {
        return new CubeData(t);
    }

    public static CubeData[] ConverTo(Transform[] t)
    {
        CubeData[] temp = new CubeData[t.Length];
        for (int i = 0; i < t.Length; i++)
        {
            temp[i] = t[i];
        }
        return temp;
    }

    public static Vector3 MoveTowards(GameObject Cube, Vector3 Destination, float Dist)
    {
        Vector3 Dif = Destination - Cube.transform.localPosition;
        float Mag = (Destination - Cube.transform.localPosition).magnitude;
        Vector3 temp = Dif.normalized * Dist;
        if (Mag > Dist)
        {
            temp = Cube.transform.localPosition + temp;
            return temp;
        }
        else
        {
            return Destination;
        }
    }
}


