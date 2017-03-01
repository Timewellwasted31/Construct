using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("ConstList")]
[System.Serializable]
public class Constructs
{
    public string ConstructName;
    public Config[] Configurations;
    public CubeData[] StaticSlots(int ConfigNum)
    {
            if(Configurations[ConfigNum].StaticParts == null)
            {
                CubeData[] temp = new CubeData[0];
                return temp;
            }
            return Configurations[ConfigNum].StaticParts;
    }
    public MovingPart[] MovingSlots(int ConfigNum)
    {
            if (Configurations[ConfigNum].MovingParts == null)
            {
                MovingPart[] temp = new MovingPart[0];
                return temp;
            }
            return Configurations[ConfigNum].MovingParts;
    }

	public int ConstructCubeRequirement(int ConfigNum){ return Configurations[ConfigNum].StaticParts.Length + MovingPartCubeRequirement(ConfigNum);}

	public int MovingPartCubeRequirement(int ConfigNum)
    {
        if (Configurations[ConfigNum].MovingParts == null)
        {
           return 0;
        }
		int RequiredCubes = 0;
		for(int i = 0; i < Configurations[ConfigNum].MovingParts.Length; i++)
		{
			RequiredCubes += Configurations[ConfigNum].MovingParts[i].CubesRequired;
		}
		return RequiredCubes;
	}
    

	public Constructs()
	{
        Configurations = new Config[0];
	}

    public Constructs(Constructs c)
    {
        this.Configurations = new Config[c.Configurations.Length];
        for (int j = 0; j < c.Configurations.Length; j++)
        {
            this.Configurations[j] = new Config();
            this.Configurations[j].StaticParts = c.Configurations[j].StaticParts;
            this.Configurations[j].MovingParts = new MovingPart[c.Configurations[j].MovingParts.Length];
            for (int i = 0; i < c.Configurations[j].MovingParts.Length; i++)
            {
                this.Configurations[j].MovingParts[i] = new MovingPart(c.Configurations[j].MovingParts[i]);
            }
        }
    }

	public Constructs(Config[] Configurations, string Name)
	{
        ConstructName = Name;
        this.Configurations = Configurations;
	}


}

[System.Serializable]
public class Config
{
    public Config()
    {
        Name = "Null";
        StaticParts = new CubeData[0];
        MovingParts = new MovingPart[0];

    }
    public Config(GameObject[] InStaticParts, MovingPart[] InMovingParts, string Name)
    {
        StaticParts = new CubeData[InStaticParts.Length];
        MovingParts = new MovingPart[InMovingParts.Length];
        MovingParts = InMovingParts;
        for (int i = 0; i < InStaticParts.Length; i++)
        {
            StaticParts[i] = InStaticParts[i].transform;
        }
        this.Name = Name;
    }

    [SerializeField]
    public CubeData[] StaticParts;
    [SerializeField]
    public MovingPart[] MovingParts;
    [SerializeField]
    public string Name;
}



