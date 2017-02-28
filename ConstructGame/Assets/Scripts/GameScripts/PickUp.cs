using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickUp : MonoBehaviour {

    [SerializeField]List<GameObject> CubesLoc;
    ConstructData ConstructDisplayed;
    float DisplayRot = 0;
    float spin = 0;
    public void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            CubesLoc.Add(transform.GetChild(i).gameObject);
        }
        if (ConstructDirectory.IsLoaded == false)
        {
            ConstructDirectory.Load();
        }
        ConstructDisplayed = new ConstructData();
        ConstructDisplayed = ConstructDirectory.GetRandom();
        List<GameObject> SentCubes = new List<GameObject>();
        for (int i = 0; i < CubesLoc.Count; i++)
        {
            if (i < ConstructDisplayed.RequiredCubes)
            {
                CubesLoc[i].SetActive(true);
                SentCubes.Add(CubesLoc[i]);
            }
            else
            {
                CubesLoc[i].transform.position = transform.position;
                CubesLoc[i].SetActive(false);
            }
        }
        ConstructDisplayed.SetUp(this.transform, 0);
        ConstructDisplayed.ChangeInto(SentCubes);
    }

    public void Update()
    {
        if (ConstructDisplayed.Main())
        {
            ConstructDisplayed.ReConfigure();
        }
        else
        {
            if (spin >= 360)
            {
                spin = 0.5f;
            }
            spin += 0.5f;
            DisplayRot = spin;
            transform.localRotation = Quaternion.Euler(0, DisplayRot, 0);
        }
    }
}
