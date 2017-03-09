using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;

[System.Serializable]
public class ConstructData : IBuilder
{
    protected Constructs Construct;
    protected Transform Owner;
    protected List<GameObject> CubesLoc = new List<GameObject>();
    public bool ChangingConfigureation;
    protected List<MovingPartController> Obj_MovingParts = new List<MovingPartController>();
    protected List<CubeController> MovingCubes = new List<CubeController>();
    [SerializeField]protected ConstructType Type;
    protected GamePad.Index Player;
    public int Level = 0;
    public float Rarity = 0;
    public float Health = 100f;


    public virtual void Controlles() { Debug.Log("WTF"); }
    public virtual void SetUp(Transform Player, GamePad.Index PlayerNum)
    {
        //Debug.Log(Player.gameObject.name);
        Owner = Player;
        this.Player = PlayerNum;
    }
    public int RequiredCubes { get { return Construct.ConstructCubeRequirement(Level); } }
    public bool canChangeInto(int CubeCount) { return CubeCount >= Construct.ConstructCubeRequirement(Level); }
    public ConstructType isType { get { return Type; } set { Type = value; } }

    public virtual void ChangeInto(List<GameObject> Cubes)
    {
        SetParts();
        int i = 0;
        CubesLoc = Cubes;
        int MovinPartIterator = 0;
        for (i = 0; i < Construct.StaticSlots(Level).Length; i++)
        {
            CubeController NewStatic = new CubeController();
            NewStatic.Cube = CubesLoc[i];
            NewStatic.Setup();
            NewStatic.Destination = Construct.Configurations[Level].StaticParts[i];
            MovingCubes.Add(NewStatic);
        }
        for (; i < Construct.ConstructCubeRequirement(Level);)
        {
            if (Obj_MovingParts[MovinPartIterator].Part != null)
            {
                for (int j = 0; j < Obj_MovingParts[MovinPartIterator].Part.CubeLoc.Length && i < Construct.ConstructCubeRequirement(Level); j++, i++)
                {
                    CubeController NewMoving = new CubeController();
                    NewMoving.Cube = CubesLoc[i];
                    // Debug.Log("Line 57 : " + Obj_MovingParts[MovinPartIterator].getContainer.transform.parent.gameObject.name);
                    CubesLoc[i].transform.parent = Obj_MovingParts[MovinPartIterator].getContainer.transform;
                    CubesLoc[i].transform.localScale = Vector3.one;
                    NewMoving.Destination = Obj_MovingParts[MovinPartIterator].Part.NextPosition(j);
                    NewMoving.Setup();
                    MovingCubes.Add(NewMoving);
                }
            }
            MovinPartIterator++;
        }
        ReConfigure();
    }

    public virtual void SetParts()
    {
        Obj_MovingParts = new List<MovingPartController>();
        int i = 0;
        for (i = 0; i < Construct.MovingSlots(Level).Length; i++)
        {
            MovingPartController Temp = new MovingPartController(Construct.Configurations[Level].MovingParts[i], Owner);
            if (Temp.Part.ParentMovingPart == "")
            {
                Temp.ParentTo();
            }
            Obj_MovingParts.Add(Temp);
        }
        for (i = 0; i < Obj_MovingParts.Count; i++)
        {
            if (Obj_MovingParts[i].Part.ParentMovingPart != "")
            {
                for (int j = 0; j < Obj_MovingParts.Count; j++)
                {
                    if (Obj_MovingParts[i].Part.ParentMovingPart == Obj_MovingParts[j].Part.Name)
                    {
                        Obj_MovingParts[i].ParentTo(Obj_MovingParts[j].getContainer.transform);
                    }
                }
            }
        }
    }

    public virtual void Reset()
    {
        for (int i = 0; i < Obj_MovingParts.Count; i++)
        {
            for (int j = 0; j < Obj_MovingParts[i].getContainer.transform.childCount;)
            {
                Obj_MovingParts[i].getContainer.transform.localScale = Vector3.one;
                Obj_MovingParts[i].getContainer.transform.GetChild(j).gameObject.transform.parent = Obj_MovingParts[i].getContainer.transform.parent;
            }
            GameObject.Destroy(Obj_MovingParts[i].getContainer);
        }
        Obj_MovingParts.Clear();
        MovingCubes.Clear();
    }

    public void ReConfigure()
    {
        ChangingConfigureation = false;
        for (int i = 0; i < MovingCubes.Count; i++)
        {
            MovingCubes[i].Shift();
            if (ChangingConfigureation == false)
            {
                ChangingConfigureation = MovingCubes[i].Moving;
            }
        }
    }

    public bool Main()
    {
        if (ChangingConfigureation)
        {
            return true;
        }
        return false;
    }

    public MovingPartController getPart(string part)
    {
        for (int i = 0; i < Obj_MovingParts.Count; i++)
        {
            if (Obj_MovingParts[i].Part != null && Obj_MovingParts[i].Part.Name == part)
            {
                return Obj_MovingParts[i];
            }
        }
        return null;
    }

    public virtual ConstructData Clone()
    {
        ConstructData temp = new ConstructData();
        return temp;
    }

    public Constructs ConstructContained { get { return Construct; } set { Construct = value; } }

    public void Damage(float amount)
    {
        Health -= amount;
        if (Health < 0)
        {
            if (Level > 0)
            {
                Level--;
            }
            else
            {
                Reset();
                ChangeInto(CubesLoc);
            }
        }
    }
    //Local Class
    public class CubeController
    {
        public GameObject Cube;
        public CubeData Destination;
        public bool Moving;

        public CubeController() { }

        public void Setup()
        {
            Moving = true;
        }

        public void Shift()
        {
            Moving = true;
            Cube.transform.localRotation = Destination.rotation;
            Cube.transform.localPosition = CubeData.MoveTowards(Cube, Destination.position, 0.1f);
            if (Cube.transform.localPosition == Destination.position)
            {
                Moving = false;
            }
        }
    };
    //
    public virtual void setMode(BuildMode mode) { }

    public virtual void setTurret(TurretTypes type) { }
}

public class MovingPartController
{
    public MovingPart Part;
    public Transform Owner;
    GameObject Container;
    public GameObject getContainer { get { return Container; } set { Container = value; } }

    public MovingPartController(string CubeLessPart, Transform Owner)
    {
        Part = null;
        this.Owner = Owner;
        Container = new GameObject(CubeLessPart);
    }

    public MovingPartController(string CubeLessPart, Transform Owner, Transform parent)
    {
        Part = null;
        this.Owner = Owner;
        Container = new GameObject(CubeLessPart);
        Container.transform.parent = parent;
        Container.transform.rotation = Quaternion.Euler(0,0,0);
    }

    public MovingPartController(MovingPart InPart, Transform Owner)
    {
        Part = InPart;
        this.Owner = Owner;
        Container = new GameObject((Part.Name));
    }

    public void ParentTo()
    {
        if (Part != null)
        {
            // Debug.Log("Line 202 : " + Container.name);
            Container.transform.parent = Owner.transform;
            Container.transform.localPosition = Part.ParentLoc;
            Container.transform.localRotation = Part.ParentRot;
            Container.transform.localScale = Part.Scale;
            AddCollider();
        }
    }

    public void ParentTo(Transform Parent)
    {
        if (Part != null)
        {
            // Debug.Log("Line 215 : " + Parent.gameObject.name);
            Container.transform.parent = Parent.transform;
            Container.transform.localPosition = Part.ParentLoc;
            Container.transform.localRotation = Part.ParentRot;
            Container.transform.localScale = Part.Scale;
            AddCollider();
        }
    }

    public void AddCollider()
    {
        if (Part.hasCollider)
        {
            switch (Part.colliderType)
            {
                case ColliderType.Sphere:
                    Container.AddComponent<SphereCollider>();
                    Container.GetComponent<SphereCollider>().radius = Part.colliderRadius;
                    Container.GetComponent<SphereCollider>().center = Part.colliderPosition;
                    break;

                case ColliderType.Capsule:
                    Container.AddComponent<CapsuleCollider>();
                    Container.GetComponent<CapsuleCollider>().radius = Part.colliderRadius;
                    Container.GetComponent<CapsuleCollider>().height = Part.colliderHeight;
                    Container.GetComponent<CapsuleCollider>().center = Part.colliderPosition;
                    break;

                case ColliderType.Box:
                    break;
            }
        }
    }
}

