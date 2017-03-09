using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeEnemyDeployer : MonoBehaviour {
    static SiegeEnemyDeployer singleton;
    public static SiegeEnemyDeployer main {
        get { return singleton; }
    }

    [Header("Constants")]
    [SerializeField] float m_edge = 640f; // further reaches of the map
    [SerializeField] float m_border = 200f; // space along inside of the map where portals can be placed
    [SerializeField] float m_buffer = 40f; // space along the out edge of the map that is not portal eligible.
    [SerializeField] float m_portal_space = 50f; // minimum distance between portals
    [Header("Round Variables")]
    [SerializeField] int m_round = 0;
    [SerializeField] float m_timer;
    [Header("Instance Variables")]
    [SerializeField] List<GameObject> m_portals;
    [SerializeField] bool m_active = false;
    [Header("Prototypes")]
    [SerializeField] GameObject m_portal;
    [SerializeField] GameObject m_enemy;


    public void AdvanceRound() {
        StartRound(m_round + 1);
    }

    public void StartRound(int r) {
        m_round = r;

        for (int i = 0; i < m_round; i++) {
            DeployPortal();
        }

        StartCoroutine(RoundLogic(m_round));

    }

    IEnumerator RoundLogic(int round) {

        for (int r = 0; r < round; r++) {
            for (int i = 0; i < m_portals.Count; i++) {
                SpawnEnemy(i);
            }

            yield return new WaitForSeconds(2f);
        }
    }

    public void DeployPortal() {
        if (m_portal == null) return;

        float x = Random.Range(0, 2 * m_border);
        if (x > m_border) {
            x = m_edge - m_buffer - x;
        }else {
            x = x + m_buffer;
        }

        float y = Random.Range(0, 2 * m_border);
        if (y > m_border) {
            y = m_edge - m_buffer - y;
        } else {
            y = y + m_buffer;
        }

        Ray r = new Ray(new Vector3(x, 50, y), Vector3.down);
        RaycastHit hit;

        if (!Physics.Raycast(r, out hit)) {
            DeployPortal(); // we missed. Somehow. Try again.
            return;
        }

        if (hit.point.y > 0.1f) {
            DeployPortal(); // too high. Try again.
            return;
        }

        for (int i = 0; i < m_portals.Count; i++) {
            if ((m_portals[i].transform.position - new Vector3(x, 0.1f, y)).magnitude < m_portal_space) {
                DeployPortal(); // too close. Try again.
                return;
            }
        }

        GameObject newPortal = Instantiate(m_portal, hit.point + 2f * Vector3.up, Quaternion.identity) as GameObject;
        m_portals.Add(newPortal);
    }

    public void SpawnEnemy(int portal = -1) {
        if (m_enemy == null) {

            return;
        }
        if (m_portals.Count == 0) {
            return;
        }

        int rand = 0;
        if (portal != -1) rand = portal;
        else rand = Random.Range(0, m_portals.Count);

        if (rand >= m_portals.Count) return;

        GameObject g = Instantiate(m_enemy, m_portals[rand].transform.position, Quaternion.identity);
        ConstructEnemy e = g.GetComponent<ConstructEnemy>();
        e.GoGetACube();
    }

    IEnumerator DelayedStartup() {
        yield return new WaitForSeconds(3f);
        StartRound(4);
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(DelayedStartup());
    }
	
	// Update is called once per frame
	void Update () {
        if (m_active) m_timer += Time.deltaTime;
	}
}
