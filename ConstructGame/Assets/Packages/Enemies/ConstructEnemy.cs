using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyAttackStatus
{
    None, // a null state, to declare that no attack animations should occur.
    Leap, // a leaping attack. Also used to cross barriers.
    Melee, // a local short-range attack.
    Ranged // a long range attack.
}

public class ConstructEnemy : MonoBehaviour, IAmEnemy {

    static protected List<ConstructEnemy> EnemyList = new List<ConstructEnemy>();

    public static void SendAllEnemiesTo(Vector3 goal) {
        for (int i = 0; i < EnemyList.Count; i++) {

            EnemyList[i].Goto(goal);

        }
    }

    [SerializeField] protected float MoveSpeed = 5f;
    [SerializeField] protected float SteeringAngle = 0f; // y axis steering angle
    [SerializeField] protected float SteerSpeed = 0f;
    [SerializeField] protected Vector3 Movement; // relative to world.
    [SerializeField] protected float RideHeight = 1f;
    [SerializeField] protected EnemyAttackStatus AIAttackStatus = EnemyAttackStatus.None;
    [SerializeField] protected float m_arrival_tolerance = 0.1f;
    [SerializeField] bool hasCube = false;

    [SerializeField] List<Vector3> NavPath; // currently tasked nodes for movement

    [SerializeField] protected bool EnemyInBin = true; // this enemy is in the bin, and is ready to be spawned into the world.

    RaycastHit groundcheck;
    SiegeCube m_heldcube = null;

    /*
     * VIRTUAL FUNCTIONS
     */
    // queues up a leap attack for the requested enemy
    virtual protected void LeapAttack(Vector3 WorldTarget, float Height) { }

    // queues up a projectile attack.
    // most aspects of projectiles should be defined in their projectile.
    // TODO: Implement ProjectileClass and replace GameObject with ProjectileClass.
    virtual protected void ProjectileAttack(GameObject projectile, Vector3 WorldTarget) { }

    // standard movement behaviour
    // this is just a standardized name.
    virtual protected void MovementAnimation() { }

    /*
     * CORE FUNCTIONS
     */
    void LoadPath(Vector3 entry, Vector3 exit) {
        StartCoroutine(GetPath(entry, exit));
    }

    public void Goto(Vector3 goal) {
        StartCoroutine(GetPath(transform.position, goal));
    }

    bool _currentlypathing = false;
    IEnumerator GetPath(Vector3 entry, Vector3 exit) {
        if (_currentlypathing) yield break;

        _currentlypathing = true;

        NavPath = null;
        int task = ThreadedNavigator.GetPath(exit, entry); // Whoops. I think I might have wrote the pather backwards.

        while (!ThreadRunner.isComplete(task)) {
            // Debug.Log("Waiting for nav thread to complete...");
            yield return null;
        }

        List<Vector3> myPath = (List<Vector3>)ThreadRunner.FetchData(task);
        NavPath = myPath;
        
        if (myPath == null) {
            Debug.Log("No path found.");
        }else {
            // if the path terminates at a node not precisely at the requested destination
            // then we add an additional node to the end.
            if (myPath.Count == 0 || myPath[myPath.Count - 1] != exit) {
                myPath.Add(exit);
            }

            // Debug.Log("Path length: " + myPath.Count);
        }


        _currentlypathing = false;

    }

    protected void EnterPortal() {
        float dist = Mathf.Infinity;

        for (int i = 0; i < EnemyPortal.getPortals().Count; i++) {
            float test = (EnemyPortal.getPortals()[i].transform.position - transform.position).sqrMagnitude;
            if (dist > test) {
                dist = test;
            }
        }

        if (dist >= 25f) {
            Debug.Log("Too far to exit.");
            return;
        }
        if (m_heldcube != null) m_heldcube.CaptureCube();
        Bin();

    }

    protected bool Bin()
    {
        if (EnemyInBin) return true; // this enemy has already been binned.
        // transfers this enemy to the bin

        EnemyInBin = true;
        this.gameObject.SetActive(false);
        NavPath.Clear();
        
        return true;
    }

    protected bool Deploy(Vector3 WorldLocation)
    {
        if (!EnemyInBin) return false;

        EnemyInBin = false;
        this.gameObject.SetActive(true);
        transform.position = WorldLocation;
        return true;
    }

    protected void Start()
    {
        EnemyList.Add(this);
        NavPath = new List<Vector3>();
    }

    protected void StandardMovement() {
        if (NavPath == null || NavPath.Count == 0) {

            return;
        }
        // standard movement pattern is to find the next
        Vector3 position = transform.position;

        // figure out which node we are closest to.
        float dist = Mathf.Infinity;
        Vector3 cur = Vector3.zero;
        Vector3 offset = Vector3.zero;
        int index = -1;

        for (int i = 0; i < NavPath.Count; i++) {
            // this is the direction from the previous node to the next one
            offset = NavPath[i] - position;
            if (offset.sqrMagnitude < dist) {
                dist = offset.sqrMagnitude;
                cur = NavPath[i];
                index = i;
            }
        }

        if (index == -1) {
            // we have no path
            return;
        }

        // since we have nodes on a tight resolution, we always path to the next node.
        // maybe we path a few nodes forward, based on move speed. I don't know.

        // direction to next node
        if (index + 1 >= NavPath.Count) {
            // if this is the last node in the path, offset doesn't point to the next node.
            offset = NavPath[index] - position;
        } else {
            offset = NavPath[index + 1] - position;
        }

        offset.y = 0;

        Movement = offset.normalized;
        Debug.DrawLine(transform.position, transform.position + Movement, Color.red);

        if (Physics.Raycast(new Ray(transform.position, Movement), 5f*Time.deltaTime * MoveSpeed, LayerMask.NameToLayer("Terrain"))) {
            NavPath.Clear();
            Debug.Log("obstructed by changed terrain.. Clearing Navpath.");
            return;
        }

        // Debug.Log("what?" + offset);

        transform.position += Time.deltaTime * MoveSpeed * Movement;

        // now prune all nodes below index
        if (index > 0) NavPath.RemoveRange(0, index);

        if (NavPath.Count == 1) {
            Vector3 test = NavPath[0];
            if (transform.position.y - test.y < 2 * RideHeight) test.y = transform.position.y;
            if ((transform.position - test).sqrMagnitude < m_arrival_tolerance) {
                NavPath.Clear();
            }
        }

    }

    public void GoGetACube() {
        GameObject c = SiegeCube.GetRandomActiveCube();
        Goto(c.transform.position);
    }

    bool EscapeOrdered = false;
    public void Escape() {
        if (EnemyPortal.getPortals() == null) {
            Debug.Log("No portals to escape to.");
            return;
        }

        float dist = Mathf.Infinity;
        float test;

        GameObject g = null;

        for (int i = 0; i  < EnemyPortal.getPortals().Count; i++) {
            test = (EnemyPortal.getPortals()[i].gameObject.transform.position - transform.position).sqrMagnitude;
            if (test < dist) {
                dist = test;
                g = EnemyPortal.getPortals()[i].gameObject;
            }

        }

        if (g == null) {
            Debug.Log("wait, what?");
            return; // wtf?
        }

        EscapeOrdered = true;
        Goto(g.transform.position);


    }

    public bool TryCubeCapture(float grabdistance = 2.5f) {
        SiegeCube s = null;
        for (int i = 0; i < SiegeCube.GetCubeCount(); i++) {
            s = SiegeCube.GetCube(i);
            if (s.isCaptured || s.isHeld) continue;

            if ((transform.position - s.transform.position).sqrMagnitude < grabdistance) {
                s.HoldCube();
                s.transform.parent = this.transform;
                s.transform.localPosition = Vector3.up;
                m_heldcube = s;
                Escape();
                return true;
            }
        }

        return false;
    }

    void AttackPlayer() {
        Escape();
    }

    /*
     * BASE UPDATE
     * Provides required physics for running enemies. Does no internal logic.
     */
    protected void Update()
    {
        // standard enemy physics
        if (EnemyInBin) { Bin(); return; } // this enemy does not require any updates

        // Current in nonmovement mode for speed testing.
        // transform.position += transform.rotation * (Time.deltaTime * Movement);

        Ray r = new Ray(transform.position, Vector3.down);
        
        Vector3 newpos = transform.position - RideHeight * Vector3.up;

        if (Physics.Raycast(r, out groundcheck)) {
            // we hit the ground casting down.
            newpos = groundcheck.point + RideHeight * Vector3.up;
        }else{
            // there is nothing below us. This is troubling. Let's look up.
            r.direction = Vector3.up;
            if (Physics.Raycast(r, out groundcheck)) {
                // we found something above us
                newpos = groundcheck.point + RideHeight * Vector3.up;
            } else {
                // Bin(); // we found nothing. Bin this enemy.
                return;
            }
        }
        transform.position = newpos;

        StandardMovement();
        TryCubeCapture();

        if (NavPath == null || NavPath.Count == 0) {

            GameObject tg = SiegeCube.GetRandomActiveCube();
            if (EscapeOrdered) {
                EnterPortal(); 
            } else if (m_heldcube != null)
                Escape();
            else if (tg != null)
                GoGetACube();
            else if (tg == null)
                AttackPlayer(); 
        }
        

    }
}
