using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
[XmlRoot("ConstList")]
[System.Serializable]
public class MovingPart
{
    public string Name;
    public Vector3 ParentLoc;
    public Quaternion ParentRot;
    public string ParentMovingPart;
    public Vector3 Scale = Vector3.one;
    public bool hasCollider = false;
    public float colliderRadius = 0;
    public float colliderHeight = 0;
    public Vector3 colliderPosition = Vector3.zero;
    public ColliderType colliderType;
    [XmlArray("CubeLoc"), XmlArrayItem("Cubes")]
    public CubeData[] CubeLoc;
    //////////////////////////////////////////////
    public int CubesRequired { get { return CubeLoc.Length; } }

    MovingPart()
    {

    }

    public MovingPart (MovingPart m)
    {
        this.ParentLoc = Vector3.zero;
        this.ParentLoc.x = m.ParentLoc.x;
        this.ParentLoc.y = m.ParentLoc.y;
        this.ParentLoc.z = m.ParentLoc.z;
        this.ParentRot = Quaternion.Euler(m.ParentRot.eulerAngles.x, m.ParentRot.eulerAngles.y, m.ParentRot.eulerAngles.z);
        this.Scale = m.Scale;
        this.CubeLoc = m.CubeLoc;
        this.Name = m.Name;
        this.ParentMovingPart = m.ParentMovingPart;
        this.hasCollider = m.hasCollider;
        this.colliderRadius = m.colliderRadius;
        this.colliderPosition = m.colliderPosition;
        this.colliderType = m.colliderType;
        this.colliderHeight = m.colliderHeight;
    }

    public MovingPart(GameObject Part, int CubesRequired, string InName)
    {
        ParentLoc = Part.transform.localPosition;
        ParentRot = Part.transform.localRotation;
        Scale = Part.transform.localScale;
        CubeLoc = new CubeData[CubesRequired];
        Name = InName;
        ParentMovingPart = null;
        if (Part.GetComponent<SphereCollider>() != null)
        {
            hasCollider = true;
            colliderRadius = Part.GetComponent<SphereCollider>().radius;
        }
        if (Part.GetComponent<CapsuleCollider>() != null)
        {
            hasCollider = true;
            colliderRadius = Part.GetComponent<CapsuleCollider>().radius;
            colliderHeight = Part.GetComponent<CapsuleCollider>().height;
            colliderPosition = Part.GetComponent<CapsuleCollider>().center;
            colliderType = ColliderType.Capsule;
        }
    }

    public MovingPart(GameObject Part, int CubesRequired, string InName, string InParentMovingPart)
    {
        ParentLoc = Part.transform.localPosition;
        ParentRot = Part.transform.localRotation;
        Scale = Part.transform.localScale;
        CubeLoc = new CubeData[CubesRequired];
        Name = InName;
        ParentMovingPart = InParentMovingPart;
        if (Part.GetComponent<SphereCollider>() != null)
        {
            hasCollider = true;
            colliderRadius = Part.GetComponent<SphereCollider>().radius;
            colliderType = ColliderType.Sphere;
        }
        if (Part.GetComponent<CapsuleCollider>() != null)
        {
            hasCollider = true;
            colliderRadius = Part.GetComponent<CapsuleCollider>().radius;
            colliderHeight = Part.GetComponent<CapsuleCollider>().height;
            colliderPosition = Part.GetComponent<CapsuleCollider>().center;
            colliderType = ColliderType.Capsule;
        }
    }


    public void AddCube(CubeData[] Cubes)
    {
        CubeLoc = Cubes;
    }

    public CubeData NextPosition(int Next)
    {
        CubeData temp;
        try
        {

            temp = CubeLoc[Next];
        }
        catch (System.Exception)
        {
            Debug.Break();
            throw;
        }
        return temp;
    }


}
[XmlRoot("ConstList")]
[SerializeField]
public enum ColliderType
{
    Sphere,
    Capsule,
    Box,
    Last
}

