using UnityEngine;
using System.Collections;

public class ConstructDataTester : MonoBehaviour {

    public ConstructData Test;
    public GameObject Testi;
    void Awake()
    {
        Test = GetComponent<ConstructData>();   
    }
	void Update ()
    {
        Test.Controlles();
	}
}
