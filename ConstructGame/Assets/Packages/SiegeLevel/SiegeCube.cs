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

    public static GameObject GetRandomActiveCube() {
        int rand = Random.Range(0, m_siegecubes.Count);
        int lastrand = rand - 1;
        if (lastrand == -1) lastrand = m_siegecubes.Count - 1;

        while (true) {

            if (!m_siegecubes[rand].isCaptured && !m_siegecubes[rand].isHeld) {
                return m_siegecubes[rand].gameObject;
            }

            if (rand == lastrand) return null;
            rand++;
            if (rand >= m_siegecubes.Count) rand = 0;
        }
        return null; // we should never get here, but hey, why not.
    }

    public static int GetCubeCount() {
        if (m_siegecubes == null) return 0;
        return m_siegecubes.Count;
    }

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
    
    [SerializeField] CubeColours m_color = CubeColours.Red;
    [SerializeField] float m_hover = 2.5f;
    [SerializeField] float m_hover_variation = 0.5f;
    bool m_captured = false; // if a cube has been captured, it is removed from the level.
    bool m_held = false; // if a cube is held, it is currently being held by a player, or by an enemy.
    float m_timer = 0f;

    public bool isCaptured {
        get { return m_captured; }
    }

    public bool isHeld {
        get { return m_held; }
    }

    public void HoldCube() {
        if (m_held) return;
        m_held = true;
    }

    public void DropCube() {
        if (!m_held) return;
        m_timer = 0f;
        m_held = false;
    }

    public void EnableCube() {
        m_captured = false;
        m_held = false;
        gameObject.SetActive(true);
    }

    public void CaptureCube() {
        m_captured = true;
        if (m_held) DropCube();
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
        m_timer = Random.value * 10f;
	}
	
	// Update is called once per frame
	void Update () {
        if (!m_held) {
            // unheld. Bob up and down, or something.
            Ray r = new Ray(transform.position, Vector3.down);
            RaycastHit groundcheck;
            m_timer += Time.deltaTime;

            if (Physics.Raycast(r, out groundcheck, Mathf.Infinity)) {
                transform.position = groundcheck.point + (m_hover + m_hover_variation * Mathf.Sin(m_timer)) * Vector3.up;
            }
        }

	}
}
