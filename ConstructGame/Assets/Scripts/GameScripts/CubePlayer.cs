using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;

public class CubePlayer : MonoBehaviour , IDamageable
{
    //Statics to keep track of Players
    public static List<CubePlayer> PlayerList;
    public static string[] Players;
    ////////////////////////////////////
    [SerializeField]List<GameObject> CubesLoc;
    public int PlayerBingUsed = 0;
    GamePad.Index PlayerNum;
    int Health;
	bool ChangingConfigureation;
	public ConstructData[] ConstructsObtained;
    List<GameObject> SentCubes;
    public ConstructData CurrentConstruct;
    public int ConstructIndex = 0;
    public Rigidbody rb;
    public int Currency = 0;
    MenuControl Menu;
    [SerializeField]GameObject MenuObj;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (PlayerList == null)
        {
            PlayerList = new List<CubePlayer>();
            Players = new string[4];
        }
        AssignPlayer();
        DontDestroyOnLoad(this.gameObject);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.tag == "Cube")
            {
                CubesLoc.Add(gameObject.transform.GetChild(i).gameObject);
            }
        }
        if (ConstructDirectory.IsLoaded == false)
        {
            ConstructDirectory.Load();
        }
        ConstructsObtained = new ConstructData[4];
        ConstructsObtained[0] = (ConstructDirectory.Find(ConstructType.CubeGuy));
        ConstructsObtained[1] = (ConstructDirectory.Find(ConstructType.Turret));
        ConstructsObtained[2] = (ConstructDirectory.Find(ConstructType.Car));
        ConstructsObtained[3] = (ConstructDirectory.Find(ConstructType.Tank));
        SentCubes = new List<GameObject>();
        Menu = new MenuControl(MenuObj, this);
        for(int i = 0; i < CubesLoc.Count; i++)
        {
           if (i < ConstructsObtained[0].RequiredCubes)
           {
              SentCubes.Add(CubesLoc[i]);
           }
           else
           {
              CubesLoc[i].transform.position = transform.position;
              CubesLoc[i].SetActive(false);
           }
        }
        CurrentConstruct = ConstructsObtained[0];
        CurrentConstruct.SetUp(this.transform, PlayerNum);
        CurrentConstruct.ChangeInto(SentCubes);
    }

    void AssignPlayer()
    {
        if (PlayerList.Count < 4)
        {
            PlayerList.Add(this);
            Players[PlayerList.Count-1] = this.gameObject.name;
            PlayerNum = (GamePad.Index)PlayerList.Count;
            return;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ClearToChange()
    {
            SentCubes.Clear();
            if (CurrentConstruct != null)
            {
                CurrentConstruct.Reset();
            }
            for (int i = 0; i < CubesLoc.Count; i++)
            {
                CubesLoc[i].SetActive(true);
            }
            for (int i = 0; i < CubesLoc.Count; i++)
            {
                if (i < ConstructsObtained[ConstructIndex].RequiredCubes)
                {
                    SentCubes.Add(CubesLoc[i]);
                }
                else
                {
                    CubesLoc[i].SetActive(false);
                    CubesLoc[i].transform.position = transform.position;
                }
            }
            CurrentConstruct = ConstructsObtained[ConstructIndex];
            CurrentConstruct.SetUp(transform, PlayerNum);
            CurrentConstruct.ChangeInto(SentCubes);
    }

    public void UpgradeConstrut(int ConstructIndex)
    {
        ConstructsObtained[ConstructIndex].Level++;
    }

    void Update()
    {
        if (this == PlayerList[PlayerBingUsed])
        {
            if (CurrentConstruct != null)
            {
                if (CurrentConstruct.Main())
                {
                    CurrentConstruct.ReConfigure();
                }
                else
                {
                    ChangingConfigureation = false;
                    Menu.MainLoop(PlayerNum);
                    CurrentConstruct.Controlles();
                }
            }

        }
    }

    public void GrabbedPickUp()
    {

    }

    public void TakeDamage(float amount)
    {
        CurrentConstruct.Damage(amount);
    }

}
