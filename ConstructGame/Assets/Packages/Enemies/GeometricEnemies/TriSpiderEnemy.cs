using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriSpiderEnemy : ConstructEnemy {

    static List<TriSpiderEnemy> EnemySpiderList = new List<TriSpiderEnemy>();
    [Header("Body Parts")]
    [SerializeField] GameObject[] FrontLeftLeg;
    [SerializeField] GameObject[] FrontRightLeg;
    [SerializeField] GameObject[] BackLeg;
    [Header("Spider Specific")]
    [SerializeField] float CurlHeight = 2.5f;


    static Vector3[] Legs_Uncurled = new Vector3[] {
        new Vector3(1.5f, -0.5f, 0),
        new Vector3(2.5f, -1.5f, 0),
        new Vector3(3f, -3f, 0) };

    static Vector3[] Legs_Curled = new Vector3[] {
        new Vector3(1f, -0.5f, 0),
        new Vector3(1f, -1.5f, 0),
        new Vector3(0.5f, -2.5f, 0) };
    
    void Curl(float Delta) {
        for (int i = 0; i < 3; i++) {
            BackLeg[i].transform.localPosition = Vector3.Lerp(Legs_Uncurled[i], Legs_Curled[i], Delta);
            FrontLeftLeg[i].transform.localPosition = Vector3.Lerp(Legs_Uncurled[i], Legs_Curled[i], Delta);
            FrontRightLeg[i].transform.localPosition = Vector3.Lerp(Legs_Uncurled[i], Legs_Curled[i], Delta);
        }
        MoveSpeed = 0f;

        StickToGround(Mathf.Lerp(RideHeight, CurlHeight, Delta));

    }

    void Uncurl(float Delta) {
        Curl(1f - Delta);
        if (Delta >= 1) MoveSpeed = 5f;
    }

    void FlingBackLeg(float Delta) {
        for (int i = 0; i < 3; i++) {
            // BackLeg[i].transform.localPosition = 
        }
    }

    // Use this for initialization
    void Start () {
        base.Start();
	}



    float timer = 0f;

	// Update is called once per frame
	void Update () {
        base.Update();

        timer += Time.deltaTime;
        timer %= 2f;

        if (timer < 1) {
            Curl(timer);
        }else if (timer > 1f) {
            Uncurl(timer - 1f);
        }

	}
}
