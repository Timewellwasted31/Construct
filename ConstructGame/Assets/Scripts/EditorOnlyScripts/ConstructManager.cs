using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("ConstList")]
public class ConstructManager : MonoBehaviour {

    [XmlArray("ConstList"), XmlArrayItem("Constructs")]
	public List<Constructs> ConstList;
	[SerializeField]bool Export = false;
	[SerializeField]bool Import = false;
    [SerializeField]bool NewConfig = false;
    [SerializeField]string ConstructName;
    [SerializeField]bool Approve = false;
    [SerializeField]int Index = 0;
    [SerializeField]bool ChangeAtIndex = false;
    List<GameObject> StaticCubes;
    List<GameObject> Parts;
    [SerializeField]List<Config> CurrentConfigs;

	void Awake()
	{
		ConstList = new List<Constructs>();
        StaticCubes = new List<GameObject>();
        Parts = new List<GameObject>();
        CurrentConfigs = new List<Config>();
	}
	void Update()
	{
		if(Export)
		{
			ExportConstructs("../SwingingGame/Constructs");
			Export = false;
		}
		if(Import)
		{
			ConstList = ImportConstructs("../SwingingGame/Constructs");
			Import = false;
		}
        if(NewConfig)
        {
            CreateNewConfig();
            NewConfig = false;
        }
		if(Approve)
		{
            ApproveConstruct();
			Approve = false;
		}
        if (ChangeAtIndex)
        {
            Constructs Temp = new Constructs(CurrentConfigs.ToArray(), ConstructName);
            ConstList.RemoveAt(Index);
            ConstList.Insert(Index, Temp);
            Temp = null;
            CurrentConfigs.Clear();
            ChangeAtIndex = false;
        }

    }
    void ExportConstructs(string path, string fileName = "Constructs.xml")
	{
		var serilaizer = new XmlSerializer (typeof(List<Constructs>));
		using(var stream = new FileStream(path + "/" +fileName, FileMode.Create))
		{
			serilaizer.Serialize(stream, ConstList);
		}
	}

    List<Constructs> ImportConstructs(string path, string fileName = "Constructs.xml")
    {
        var serilaizer = new XmlSerializer(typeof(List<Constructs>));
        using (var stream = new FileStream(path + "/" + fileName, FileMode.Open))
        {
            return serilaizer.Deserialize(stream) as List<Constructs>;
        }
    }
    public void ApproveConstruct()
    {
        Constructs Temp = new Constructs(CurrentConfigs.ToArray(), ConstructName);
        ConstList.Add(Temp);
        Temp = null;
        CurrentConfigs.Clear();
    }
    public void CreateNewConfig()
    {
        GameObject ConstructModel = GameObject.FindGameObjectWithTag("Construct");
        StaticCubes.AddRange(GameObject.FindGameObjectsWithTag("StaticPart"));
        Parts.AddRange(GameObject.FindGameObjectsWithTag("MovingPart"));
        MovingPart[] getMovingParts = new MovingPart[Parts.Count];
        for (int i = 0; i < Parts.Count; i++)
        {
            CubeData[] CubesContained = new CubeData [0];
            List<CubeData> CubesContainedList = new List<CubeData>();
            for (int j = 0; j < Parts[i].transform.childCount; j++)
            {
                if (Parts[i].transform.GetChild(j).tag != "MovingPart")
                {
                    CubesContainedList.Add(Parts[i].transform.GetChild(j));
                }
            }
            CubesContained = CubesContainedList.ToArray();
            CubesContainedList = null;
            getMovingParts[i] = Parts[i].transform.parent.tag == "MovingPart" ? new MovingPart(Parts[i], CubesContained.Length, Parts[i].name, Parts[i].transform.parent.name) : new MovingPart(Parts[i], CubesContained.Length, Parts[i].name);
            getMovingParts[i].AddCube(CubesContained);
        }
        CurrentConfigs.Add(new Config(StaticCubes.ToArray(), getMovingParts, ConstructModel.name));
        Parts.Clear();
        StaticCubes.Clear();
    }

    /*public void SpawnCon(string num)
	{
		int GodDamit = int.Parse (num);
		GameObject Empty = new GameObject ();
		if(ConstList[GodDamit] != null)
		{
			for(int i = 0; i < ConstList[GodDamit].StaticParts.Length; i ++)
			{
				Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), ConstList[GodDamit].StaticParts[i], Quaternion.identity);
				if(i < ConstList[GodDamit].MovingParts.Length)
				{
					Instantiate(Empty, ConstList[GodDamit].MovingParts[i], Quaternion.identity);
				}
			}
		}
	}*/

    public Constructs FindConstruct(string Name)
    {
        for(int i = 0; i < ConstList.Count; i++)
        {
            if(ConstList[i].ConstructName == Name)
            {
                return ConstList[i];
            }
        }
        return null;
    }

}
