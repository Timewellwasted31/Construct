using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CubeColours {
    Blue,
    Green,
    Orange,
    Purple,
    Red,
    Yellow,
    Last
}

public class SiegeCube : MonoBehaviour {

    static GameObject prefab;
    static List<SiegeCube> m_siegecubes;

    [SerializeField] CubeColours m_color = CubeColours.Red;

    public static SiegeCube GetCube(int n) {
        if (n >= m_siegecubes.Count) return null;

        return m_siegecubes[n];

    }

    public static void EnableCubes(int n) {

        if (prefab == null) prefab = Resources.Load<GameObject>("GoalCube");
        if (prefab == null) Debug.Log("Could not get prefab.");
        if (m_siegecubes == null) m_siegecubes = new List<SiegeCube>();

        int activated = 0;
        for (int i = 0; i < m_siegecubes.Count; i++) {
            if (activated < n) {
                if (m_siegecubes[i].isCaptured) {
                    m_siegecubes[i].EnableCube();
                }
                activated++;
            } else {
                m_siegecubes[i].CaptureCube();
            }
        }

        for (;activated < n && prefab != null; activated++) {
            Instantiate(prefab);
        }

    }

    bool m_captured = false;
    public bool isCaptured {
        get { return m_captured; }
    }

    public void EnableCube() {
        m_captured = false;
        gameObject.SetActive(true);
    }

    public void CaptureCube() {
        m_captured = true;
        gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {
        if (prefab == null) prefab = Resources.Load<GameObject>("GoalCube");
        if (prefab == null) Debug.Log("Could not get prefab.");
        if (m_siegecubes == null) {
            m_siegecubes = new List<SiegeCube>();
        }

        GetComponent<MeshRenderer>().material = Resources.Load<Material>("CubeColours/" + m_color.ToString());
        GetComponent<ParticleSystem>().startColor = GetComponent<MeshRenderer>().material.color;

        if (!m_siegecubes.Contains(this)) m_siegecubes.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
