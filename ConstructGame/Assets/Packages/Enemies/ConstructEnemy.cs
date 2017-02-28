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

    [SerializeField] protected float SteeringAngle = 0f; // y axis steering angle
    [SerializeField] protected float SteerSpeed = 0f;
    [SerializeField] protected Vector3 Movement; // relative to rotation. z is forward, x is a strafe.
    [SerializeField] protected float RideHeight = 1f;
    [SerializeField] protected EnemyAttackStatus AIAttackStatus = EnemyAttackStatus.None;

    List<Vector3> NavPath; // currently tasked nodes for movement

    [SerializeField] protected bool EnemyInBin = true; // this enemy is in the bin, and is ready to be spawned into the world.

    RaycastHit groundcheck;

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

    IEnumerator GetPath(Vector3 entry, Vector3 exit) {
        NavPath = null;
        int task = ThreadedNavigator.GetPath(entry, exit);

        while (!ThreadRunner.isComplete(task)) {
            // Debug.Log("Waiting for nav thread to complete...");
            yield return null;
        }

        List<Vector3> myPath = (List<Vector3>)ThreadRunner.FetchData(task);
        NavPath = myPath;
        
        if (myPath == null) {
            Debug.Log("No path found.");
        }else {
            Debug.Log("Path length: " + myPath.Count);
        }

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

        if (Physics.Raycast(r, out groundcheck))
        {
            // we hit the ground casting down.
            newpos = groundcheck.point + RideHeight * Vector3.up;
        }else
        {
            // there is nothing below us. This is troubling. Let's look up.
            r.direction = Vector3.up;
            if (Physics.Raycast(r, out groundcheck)) {
                // we found something above us
                newpos = groundcheck.point + RideHeight * Vector3.up;
            }
            else
            {
                // Bin(); // we found nothing. Bin this enemy.
                return;
            }
        }
    }
}
