using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct WeightedThreadedNavNodeExit {
    public ThreadedNavNode exit;
    public float weight;

    public WeightedThreadedNavNodeExit(ThreadedNavNode e, float w) {
        exit = e;
        weight = w;
    }
}

public class ThreadedNavNode {

    Vector3 m_origin;
    List<WeightedThreadedNavNodeExit> m_exits;

    public ThreadedNavNode(Vector3 origin) {
        m_origin = origin;
        m_exits = new List<WeightedThreadedNavNodeExit>();
    }

    public Vector3 getOrigin() { return m_origin; }
    public void addExit(WeightedThreadedNavNodeExit e) {
        m_exits.Add(e);
    }
    public List<WeightedThreadedNavNodeExit> accessExits() { return m_exits; }
}
