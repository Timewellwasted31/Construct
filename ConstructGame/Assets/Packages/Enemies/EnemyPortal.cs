using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPortal : MonoBehaviour {

    static List<EnemyPortal> m_allportals = new List<EnemyPortal>();
    public static List<EnemyPortal> getPortals() { return m_allportals; }

	// Use this for initialization
	void Start () {
        EnrollPortal();
	}

    public void EnrollPortal() {
        if (m_allportals == null) {
            m_allportals = new List<EnemyPortal>();
        }
        m_allportals.Add(this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
