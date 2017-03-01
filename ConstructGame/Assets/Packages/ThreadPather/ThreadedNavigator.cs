using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class ThreadedNavigator : MonoBehaviour {

    public static ThreadedNavigator main;

    [SerializeField]
    LayerMask PathingLayer;
    
    [SerializeField] Vector3 m_origin = Vector3.zero;
    [SerializeField] Vector3 m_scanradius = 250 * Vector3.one; //
    [SerializeField] float m_PathingHeight = 0.5f; 
    [SerializeField] float m_scanResolution = 0.1f;
    [SerializeField] float m_ScanDeltaMax = 0.06f;

    [SerializeField] bool m_execute = false;


    Dictionary<Vector2, List<ThreadedNavNode>> xz_NavTable;

    void Awake() {
        main = this;
    }

    // Use this for initialization
    void Start () {
        // main = this;
	}

    public ThreadedNavNode FindClosestNode(Vector3 worldPos) {
        // find nearest node
        float closest = Mathf.Infinity;
        float test = Mathf.Infinity;
        ThreadedNavNode start = null;
        foreach (List<ThreadedNavNode> list in xz_NavTable.Values) {
            for (int i = 0; i < list.Count; i++) {
                test = (list[i].getOrigin() - worldPos).sqrMagnitude;
                if (test < closest) {
                    closest = test;
                    start = list[i];
                    if (closest < m_scanResolution) break;
                }
            }
            if (closest < m_scanResolution) break;
        }
        return start;
    }

    public IEnumerator GetPathTest(Vector3 entry, Vector3 exit) {
        // NavPath = null;
        int task = ThreadedNavigator.GetPath(entry, exit);
        float time = Time.realtimeSinceStartup;

        while (!ThreadRunner.isComplete(task)) {
            Debug.Log("Waiting for nav thread to complete...");
            yield return null;
        }

        List<Vector3> myPath = (List<Vector3>)ThreadRunner.FetchData(task);
        // NavPath = myPath;



        if (myPath == null) {
            Debug.Log("No path found.");
        } else {
            Debug.Log("Path length: " + myPath.Count);

            for (int i = 0; i < myPath.Count - 1; i++) {
                Debug.DrawLine(myPath[i], myPath[i + 1], Color.magenta, 5f);
            }
        }
        Debug.Log("Took " + (Time.realtimeSinceStartup - time) + "s to generate.");

    }

    public static int GetPath(Vector3 entry, Vector3 exit, float tolerance = 5f) {
        // main.StartCoroutine(main.FindPath(entry, exit, tolerance));

        NavPathRequest r = new NavPathRequest(entry, exit, tolerance);
        int t = ThreadRunner.CreateThread(new ParameterizedThreadStart(main.ThreadFindPathObject), (object)r);

        Debug.Log("Created pathing thread #" + t);

        ThreadRunner.StartThread(t);
        return t;
    }

    public void ThreadFindPathObject(object o) {
        if (o.GetType() != typeof(NavPathRequest)) {
            ThreadRunner.ExportData(null);
            ThreadRunner.MarkComplete();
            return;
        }
        NavPathRequest r = (NavPathRequest)o;
        ThreadFindPath(r.entry, r.exit, r.tolerance);
    }

    public void ThreadFindPath(Vector3 entry, Vector3 exit, float tolerance = 5f) {
        List<Vector3> retList = new List<Vector3>();

        // find nearest node
        ThreadedNavNode start = FindClosestNode(entry);
        ThreadedNavNode end = FindClosestNode(exit);

        if (((start.getOrigin() - entry).magnitude > tolerance) || (end.getOrigin() - exit).magnitude > tolerance) {
            ThreadRunner.ExportData(null);
            ThreadRunner.MarkComplete();
            return;
        }

        if (start == end) {
            ThreadRunner.ExportData(new List<Vector3>());
            ThreadRunner.MarkComplete();
            return;
        }

        PriorityQueue<ThreadedNavNode> queue = new PriorityQueue<ThreadedNavNode>();
        Dictionary<ThreadedNavNode, float> costs = new Dictionary<ThreadedNavNode, float>();
        List<ThreadedNavNode> visited = new List<ThreadedNavNode>();

        ThreadedNavNode prev = start;
        ThreadedNavNode current = start;

        bool solved = false;
        float priority = 0f;

        queue.Add(start, 0f); // begin with the origin node
        costs.Add(start, 0f); // 

        while (queue.Count > 0) {
            current = queue.Get(0);
            priority = queue.GetPriority(0);
            queue.RemoveAt(0);

            if (visited.Contains(current)) continue;
            visited.Add(current);


            List<WeightedThreadedNavNodeExit> exits = current.accessExits();
            int count = exits.Count;

            if (count < 4) {
                //Debug.Log("short exits");
                // Debug.DrawRay(current.getOrigin(), 10f * Vector3.up, Color.red, 0.5f);
            }

            for (int i = 0; i < count; i++) {
                // calculate heuristic for each node and add to the queue
                // heuristic is cost + expected

                float h = (exits[i].exit.getOrigin() - end.getOrigin()).magnitude;


                if (!costs.ContainsKey(exits[i].exit)) {
                    // we have never been here before. 
                    costs.Add(exits[i].exit, costs[current] + exits[i].weight);
                    // now add it to the queue

                    queue.Add(exits[i].exit, h + costs[exits[i].exit]);
                    // Debug.DrawLine(exits[i].exit.getOrigin(), current.getOrigin(), Color.green, 0.5f);
                } else if (costs[current] + exits[i].weight < costs[exits[i].exit]) {
                    // we have found a shorter path to this location
                    costs[exits[i].exit] = costs[current] + exits[i].weight;
                    // so add it to the queue again
                    queue.Add(exits[i].exit, h + costs[exits[i].exit]);
                    // Debug.DrawLine(exits[i].exit.getOrigin(), current.getOrigin(), Color.green, 0.5f);
                } else {
                    // Debug.DrawLine(exits[i].exit.getOrigin(), current.getOrigin(), Color.red, 0.5f);
                }

            }

            if (current == end) {
                solved = true;
                break; // solved.
            }
            
        }

        if (!solved) {
            Debug.Log("[PATHER] Path finding failed.");
            ThreadRunner.ExportData(null);
            ThreadRunner.MarkComplete();
            return;
        }

        // then we reconstruct the path
        current = end;
        priority = Mathf.Infinity;
        prev = null; // temp


        while (current != start) {
            if (current == null) break;
            priority = Mathf.Infinity;
            retList.Add(current.getOrigin());
            prev = null;

            for (int i = 0; i < current.accessExits().Count; i++) {
                //Debug.Log("checking: " + current.accessExits()[i].exit.getOrigin());
                if (!costs.ContainsKey(current.accessExits()[i].exit)) {

                } else if (costs[current.accessExits()[i].exit] < priority) {
                    priority = costs[current.accessExits()[i].exit];
                    prev = current.accessExits()[i].exit;
                }
            }

            //Debug.Log(current.getOrigin() + " - " + priority);

            current = prev;
        }
        retList.Add(start.getOrigin());

        for (int i = 0; i < retList.Count - 1; i++) {
            // Debug.DrawLine(retList[i], retList[i + 1], Color.magenta, 5f);
        }

        ThreadRunner.ExportData(retList);
        ThreadRunner.MarkComplete();
        return;
    }

    // returns null, if no path found.
    // returns a list of Vector3s
    public IEnumerator FindPath(Vector3 entry, Vector3 exit, float tolerance = 5f) {
        float realtime = Time.realtimeSinceStartup;
        float totaltime = 0f;
        List<Vector3> retList = new List<Vector3>();

        // find nearest node
        ThreadedNavNode start = FindClosestNode(entry);
        ThreadedNavNode end = FindClosestNode(exit);

        if (((start.getOrigin() - entry).magnitude > tolerance) || (end.getOrigin() - exit).magnitude > tolerance) {
            //Debug.Log("Could not find nodes for entry or exit.");
            yield break;  // return null;
        }

        // if (start == end) { return new List<Vector3>(); }
        if (start == end) yield break;

        PriorityQueue<ThreadedNavNode> queue = new PriorityQueue<ThreadedNavNode>();
        Dictionary<ThreadedNavNode, float> costs = new Dictionary<ThreadedNavNode, float>();
        List<ThreadedNavNode> visited = new List<ThreadedNavNode>();

        ThreadedNavNode prev = start;
        ThreadedNavNode current = start;

        bool solved = false;
        float priority = 0f;

        queue.Add(start, 0f); // begin with the origin node
        costs.Add(start, 0f); // 

        while (queue.Count > 0) {
            current = queue.Get(0);
            priority = queue.GetPriority(0);
            queue.RemoveAt(0);

            if (visited.Contains(current)) continue;
            visited.Add(current);


            List<WeightedThreadedNavNodeExit> exits = current.accessExits();
            int count = exits.Count;

            if (count < 4) {
                //Debug.Log("short exits");
                Debug.DrawRay(current.getOrigin(), 10f * Vector3.up, Color.red, 0.5f);
            }

            for (int i = 0; i < count; i++) {
                // calculate heuristic for each node and add to the queue
                // heuristic is cost + expected

                float h = (exits[i].exit.getOrigin() - end.getOrigin()).magnitude;
                

                if (!costs.ContainsKey(exits[i].exit)) {
                    // we have never been here before. 
                    costs.Add(exits[i].exit, costs[current] + exits[i].weight);
                    // now add it to the queue
                    
                    queue.Add(exits[i].exit, h + costs[exits[i].exit]);
                    Debug.DrawLine(exits[i].exit.getOrigin(), current.getOrigin(), Color.green, 0.5f);
                } else if (costs[current] + exits[i].weight < costs[exits[i].exit]) {
                    // we have found a shorter path to this location
                    costs[exits[i].exit] = costs[current] + exits[i].weight;
                    // so add it to the queue again
                    queue.Add(exits[i].exit, h + costs[exits[i].exit]);
                    Debug.DrawLine(exits[i].exit.getOrigin(), current.getOrigin(), Color.green, 0.5f);
                } else {
                    Debug.DrawLine(exits[i].exit.getOrigin(), current.getOrigin(), Color.red, 0.5f);
                }

            }
            
            if (current == end) {
                solved = true;
                break; // solved.
            }

            if (Time.realtimeSinceStartup - realtime > m_ScanDeltaMax) {
                realtime = Time.realtimeSinceStartup;
                yield return null; }
        }

        if (!solved) {
            Debug.Log("[PATHER] Path finding failed.");
            yield break; // return null;
        }

        // then we reconstruct the path
        current = end;
        priority = Mathf.Infinity;
        prev = null; // temp


        while (current != start) {
            if (current == null) break;
            priority = Mathf.Infinity;
            retList.Add(current.getOrigin());
            prev = null;

            for (int i = 0; i < current.accessExits().Count; i++) {
                //Debug.Log("checking: " + current.accessExits()[i].exit.getOrigin());
                if (!costs.ContainsKey(current.accessExits()[i].exit)) {

                } else if (costs[ current.accessExits()[i].exit ] < priority) {
                    priority = costs[current.accessExits()[i].exit];
                    prev = current.accessExits()[i].exit;
                }
            }

            //Debug.Log(current.getOrigin() + " - " + priority);

            current = prev;
            if (Time.realtimeSinceStartup - realtime > m_ScanDeltaMax) { realtime = Time.realtimeSinceStartup; yield return null; }
        }
        retList.Add(start.getOrigin());

        for (int i = 0; i < retList.Count - 1; i++) {
            Debug.DrawLine(retList[i], retList[i + 1], Color.magenta, 5f);
        }

        ThreadRunner.ExportData(retList);

        yield return null; // return retList;
    }
	
	// Update is called once per frame
	void Update () {
        if (m_execute) {
            m_execute = false;
            StartCoroutine(ProduceData(m_origin, m_scanradius, m_scanResolution));
        }
	}

    public static void PatchNavMesh(Vector3 origin, float radius) {

    }

    IEnumerator PatchNav(Vector3 origin, float radius) {



        yield return null;
    }

    public static void GenerateNavMesh() {
        main.StartCoroutine(main.ProduceData(main.m_origin, main.m_scanradius, main.m_scanResolution));
    }

    IEnumerator ProduceData(Vector3 origin, Vector3 bounds, float resolution) {
        float realtime = Time.realtimeSinceStartup;
        xz_NavTable = new Dictionary<Vector2, List<ThreadedNavNode>>();

        Vector3 origin_at_height = origin + Vector3.up * bounds.y;
        Ray r;
        RaycastHit[] hits;
        Vector2 tv2;
        
        // raycast down to a central point.
        r = new Ray(origin_at_height, Vector3.down);
        hits = Physics.RaycastAll(r, bounds.y * 2, PathingLayer.value);

        tv2 = new Vector2(origin_at_height.x, origin_at_height.z);

        // if we need to, make a list in the table. 
        if (hits.Length > 0 && !xz_NavTable.ContainsKey(tv2))
                xz_NavTable.Add(tv2, new List<ThreadedNavNode>());
        
        for (int i = 0; i < hits.Length; i++) {


            xz_NavTable[tv2].Add(new ThreadedNavNode(hits[i].point));

        }

        Vector2 prekey = tv2;

        Vector3[] cardinals = new Vector3[4] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };



        // raycast along each axis, then do points to points
        for (int c = 0; c < 4; c++) {
            float boundingvalue = Mathf.Abs(Vector3.Dot(cardinals[c], bounds));
            for (float s = 0; s <= boundingvalue; s += resolution) {



                // a step to the left
                r.origin = origin_at_height + cardinals[c] * s;
                hits = Physics.RaycastAll(r, bounds.y * 2, PathingLayer.value);
                // Debug.DrawRay(r.origin, r.direction * bounds.y * 2, Color.blue, 0.5f, true);
                tv2 = new Vector2(r.origin.x, r.origin.z);

                if (hits.Length > 0 && !xz_NavTable.ContainsKey(tv2))
                    xz_NavTable.Add(tv2, new List<ThreadedNavNode>());

                for (int n = 0; n < hits.Length; n++) {

                    xz_NavTable[tv2].Add(new ThreadedNavNode(hits[n].point));
                    Ray d = new Ray(hits[n].point + m_PathingHeight * Vector3.up, Vector3.zero);
                    Ray d2 = new Ray();
                    //Debug.Log("pk: " + prekey);
                    for (int i = 0; xz_NavTable.ContainsKey(prekey) && i < xz_NavTable[prekey].Count; i++) {
                        // now, run this against placed nodes
                        d.direction = xz_NavTable[prekey][i].getOrigin() - d.origin + m_PathingHeight * Vector3.up;
                        // if there is no obstacle between the two nodes, they connect
                        d2.origin = xz_NavTable[prekey][i].getOrigin() + m_PathingHeight * Vector3.up;
                        d2.direction = -d.direction;


                        bool h = Physics.Raycast(d, d.direction.magnitude, PathingLayer.value);
                        if (!h) {
                            xz_NavTable[tv2][n].addExit(new WeightedThreadedNavNodeExit(xz_NavTable[prekey][i], d.direction.magnitude));
                            Debug.DrawRay(d.origin, 0.5f * d.direction, Color.green, 0.5f, false);
                        } else {

                            Debug.DrawRay(d.origin, 0.5f * d.direction, Color.red, 1f, false);
                        }

                        h = Physics.Raycast(d2, d2.direction.magnitude, PathingLayer.value);
                        if (!h) {
                            xz_NavTable[prekey][i].addExit(new WeightedThreadedNavNodeExit(xz_NavTable[tv2][n], d.direction.magnitude));
                            Debug.DrawRay(d2.origin, 0.5f * d2.direction, Color.green, 0.5f, false);
                        } else {

                            Debug.DrawRay(d2.origin, 0.5f * d2.direction, Color.red, 1f, false);
                        }

                        if (Time.realtimeSinceStartup - realtime > m_ScanDeltaMax) { realtime = Time.realtimeSinceStartup; yield return null; }

                    }

                }


                prekey = tv2;
                if (Time.realtimeSinceStartup - realtime > m_ScanDeltaMax) { realtime = Time.realtimeSinceStartup; yield return null; }
            }
        }
        //Debug.Log("p: " + prekey);

        int y = 0;

        Vector3 location, secondlocation, thirdlocation;
        Vector2 search, secondaddy;
        float nextlast, last;

        // now we use the cardinals to do each quadrant. for each axis:
        for (int c = 0; c < cardinals.Length; c++) {
            // we generate the last entry we expect to find in the table.
            last = Mathf.Abs(Vector3.Dot(cardinals[c], bounds));

            // we start at the end of the axis, and move in.
            for (float n = last; n >= 0; n -= resolution) {
                location = origin + cardinals[c] * n + bounds.y * Vector3.up; // this is the row coordinate
                search = new Vector2(location.x, location.z);

                nextlast = Mathf.Abs(Vector3.Dot(cardinals[(c+1)%4], bounds)) - resolution;
                for (float i = resolution; i <= nextlast; i += resolution) {
                    secondlocation = location + i * cardinals[(c + 1) % 4];
                    tv2 = new Vector2(secondlocation.x, secondlocation.z);

                    r.origin = secondlocation;
                    r.direction = Vector3.down;

                    // first, we raycast down.
                    hits = Physics.RaycastAll(r, 2 * bounds.y, PathingLayer.value);
                    /*
                    if (hits.Length > 0)
                        Debug.DrawRay(r.origin, r.direction * bounds.y * 2, Color.blue, 0.5f, false);
                    else {
                        Debug.DrawRay(r.origin, r.direction * bounds.y * 2, Color.red, 0.5f, false);
                    }*/
                    y++;

                    if (hits.Length > 0 && !xz_NavTable.ContainsKey(tv2))
                        xz_NavTable.Add(tv2, new List<ThreadedNavNode>());

                    for (int m = 0; m < hits.Length; m++) {
                        ThreadedNavNode mynode = new ThreadedNavNode(hits[m].point);
                            xz_NavTable[tv2].Add(mynode);

                        // raycast to the left
                        thirdlocation = secondlocation - cardinals[(c + 1) % 4] * resolution;
                        secondaddy = new Vector2(thirdlocation.x, thirdlocation.z);
                        if (xz_NavTable.ContainsKey(secondaddy)) {
                            for (int q = 0; q < xz_NavTable[secondaddy].Count; q++) {

                                r.origin = hits[m].point + (resolution / 4f) * Vector3.up;
                                r.direction = (xz_NavTable[secondaddy][q].getOrigin() + (resolution / 4f) * Vector3.up) - r.origin;

                                bool h = Physics.Raycast(r, resolution, PathingLayer.value);
                                if (!h) {
                                    xz_NavTable[secondaddy][q].addExit(new WeightedThreadedNavNodeExit(mynode, r.direction.magnitude / resolution));

                                    // try

                                    Vector2 fli = new Vector2(mynode.getOrigin().x, mynode.getOrigin().z);

                                    if (xz_NavTable.ContainsKey(fli)) 
                                    for (int t = 0; t < xz_NavTable[fli].Count; t++) {
                                        if (xz_NavTable[fli][t].getOrigin() != mynode.getOrigin()) continue;
                                        xz_NavTable[fli][t].addExit(new WeightedThreadedNavNodeExit(xz_NavTable[secondaddy][q], r.direction.magnitude / resolution));
                                    }
                                    // mynode.

                                    Debug.DrawLine(r.origin, xz_NavTable[secondaddy][q].getOrigin() + (resolution / 4f) * Vector3.up, Color.green, 0.5f);
                                } else {
                                    Debug.DrawLine(r.origin, xz_NavTable[secondaddy][q].getOrigin() + (resolution / 4f) * Vector3.up, Color.red, 0.5f);
                                }
                                

                                if (Time.realtimeSinceStartup - realtime > m_ScanDeltaMax) { realtime = Time.realtimeSinceStartup; yield return null; }

                            }
                        }

                        // raycast forward
                        thirdlocation = secondlocation - cardinals[(c+2) % 4] * resolution;
                        secondaddy = new Vector2(thirdlocation.x, thirdlocation.z);
                        if (xz_NavTable.ContainsKey(secondaddy)) {
                            for (int q = 0; q < xz_NavTable[secondaddy].Count; q++) {

                                r.origin = hits[m].point + (resolution/4f) *  Vector3.up;
                                r.direction = (xz_NavTable[secondaddy][q].getOrigin() + (resolution / 4f) * Vector3.up) - r.origin;

                                bool h = Physics.Raycast(r, resolution, PathingLayer.value);

                                if (!h) {
                                    // if we didn't hit, we should check the opposite direction
                                    r.origin = r.origin + resolution * r.direction.normalized;
                                    r.direction = -r.direction;
                                    h = Physics.Raycast(r, resolution, PathingLayer.value);
                                }


                                if (!h) {
                                    xz_NavTable[secondaddy][q].addExit(new WeightedThreadedNavNodeExit(mynode, r.direction.magnitude / resolution));
                                    // xz_NavTable[mynode.getOrigin()]

                                    // try


                                    Vector2 fli = new Vector2(mynode.getOrigin().x, mynode.getOrigin().z);
                                    if (xz_NavTable.ContainsKey(fli)) 
                                    for (int t = 0; t < xz_NavTable[fli].Count; t++) {
                                        if (xz_NavTable[fli][t].getOrigin() != mynode.getOrigin()) continue;
                                        xz_NavTable[fli][t].addExit(new WeightedThreadedNavNodeExit(xz_NavTable[secondaddy][q], r.direction.magnitude / resolution));
                                    }
                                    // mynode.

                                    Debug.DrawLine(r.origin, xz_NavTable[secondaddy][q].getOrigin() + (resolution / 4f) * Vector3.up, Color.green, 0.5f);
                                } else {
                                    Debug.DrawLine(r.origin, xz_NavTable[secondaddy][q].getOrigin() + (resolution / 4f) * Vector3.up, Color.red, 0.5f);
                                }

                                // second half not here yet.

                                if (Time.realtimeSinceStartup - realtime > m_ScanDeltaMax) { realtime = Time.realtimeSinceStartup; yield return null; }

                            }
                        }


                        // then take a step to the right
                        if (Time.realtimeSinceStartup - realtime > m_ScanDeltaMax) { realtime = Time.realtimeSinceStartup; yield return null; }
                    }

                    if (Time.realtimeSinceStartup - realtime > m_ScanDeltaMax) { realtime = Time.realtimeSinceStartup; yield return null; }
                }

                
            }

        }
        //Debug.Log("y: " + y);

        // now, for all entries, if we have an exit, we backcopy to the other...

    }
}

public class NavPathRequest {
    public NavPathRequest(Vector3 _o, Vector3 _e, float _t = 5.0f) {
        entry = _o;
        exit = _e;
        tolerance = _t;
    }

    public Vector3 entry;
    public Vector3 exit;
    public float tolerance;
}