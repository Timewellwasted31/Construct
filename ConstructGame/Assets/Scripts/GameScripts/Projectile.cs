using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    CubePlayer                  Owner;
    GameObject                  Child;
    protected projectileType    type;
    protected Allegiance        allegiance;
    protected ProjectileName    pName;
    protected bool              lanched;
    protected float             lifeTime;
    protected float             distanceTraveled;
    protected float             height;
    protected float             dropRate;
    protected Vector2           velocity;
    protected Vector3           origin;
    protected float             initalVelocity;
    protected float             angle;
    protected Vector3           forward;
    protected Vector3           newPosition = Vector3.zero;
    protected Vector3           newDirection = Vector3.zero;
    protected RaycastHit        hit;
    protected float             damage;
    protected float             aoe = 0;
    //Anim Var
    float spin = 0;
    //Accessors
    public CubePlayer           getOwner{ get { return Owner; } }
    public projectileType       getType { get { return type; } }
    public Allegiance           getAllleigance { get { return allegiance; } }
    public ProjectileName       getpName {get{ return pName; } }
    //functions
    public void initialize(GameObject Owner, Transform LaunchPoint, float Damage, float initalVelocity, float angle, projectileType type, Allegiance allegiance, ProjectileName name)
    {
        initialize(Owner, LaunchPoint, Damage, initalVelocity, angle, type, allegiance, name, 0f);
    }

    public void initialize(GameObject Owner, Transform LaunchPoint, float Damage, float initalVelocity, float angle, projectileType type, Allegiance allegiance, ProjectileName name, float DropRate)
    {
        this.gameObject.SetActive(true);
        this.Owner = Owner.GetComponent<CubePlayer>();
        this.transform.position = LaunchPoint.position;
        this.transform.rotation = LaunchPoint.rotation;
        this.forward = LaunchPoint.forward;
        forward.y = 0;
        this.initalVelocity = initalVelocity;
        this.angle = angle;
        this.type = type;
        this.allegiance = allegiance;
        this.pName = name;
        this.dropRate = DropRate;
        Child = transform.GetChild(0).gameObject;
    }

    public void Launch()
    {
        velocity.x = initalVelocity * (Mathf.Cos((angle * Mathf.PI) / 180));
        velocity.y = initalVelocity * (Mathf.Sin((angle * Mathf.PI) / 180));
        origin = transform.position;
        transform.parent = null;
        lanched = true;
    }

    public void TimeStep()
    {
        if (pName == ProjectileName.Hammer)
        {
            HammerAnim();
        }
        lifeTime += Time.deltaTime;
        distanceTraveled = velocity.x * lifeTime;
        height = ((velocity.y - (dropRate * lifeTime)) * lifeTime);
        newPosition = origin + (forward * distanceTraveled);
        newPosition.y += height;
        transform.position = newPosition;
        ///
        distanceTraveled = velocity.x * (lifeTime + Time.deltaTime);
        height = ((velocity.y - (dropRate * (lifeTime + Time.deltaTime))) * (lifeTime + Time.deltaTime));
        newDirection = origin + (forward * distanceTraveled);
        newDirection.y += height;
        ///
        Vector3 hitDirection = newDirection - newPosition;
        Debug.DrawRay(newPosition, hitDirection, Color.red,10f);
        if (Physics.Raycast(newPosition, hitDirection, out hit, hitDirection.magnitude))
        {
            if (hit.transform.gameObject != Owner)
            {
                
                if (hit.collider.gameObject.GetComponent<IDamageable>() != null)
                {
                    hit.collider.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                }
                
                if (type == projectileType.Explosive)
                {
                    //Report Hit.point to Ai Controller;
                }
                Reset();
            }
        }
        if (Owner != null && (transform.position - Owner.transform.position).magnitude > 100)
        {
            Reset();
        }
    }

    void Update()
    {
        if (lanched)
        {
            TimeStep();
        }
    }

    public void Reset()
    {
        this.Owner = null;
        this.initalVelocity = 0;
        this.angle = 0;
        this.allegiance = Allegiance.Neutral;
        this.damage = 0;
        this.transform.position = Vector3.zero;
        this.transform.parent = null;
        this.lanched = false;
        this.newPosition = Vector3.zero;
        this.newDirection = Vector3.zero;
        this.lifeTime = 0;
        this.height = 0;
        this.distanceTraveled = 0;
        this.forward = Vector3.zero;
        this.spin = 0;
        this.gameObject.SetActive(false);
    }

    public void HammerAnim()
    {
        if (spin >= 90)
        {
            spin = 0;
        }
        spin += 0.1f * initalVelocity;
        float ArmAngle = (360 * Mathf.Sin(spin * Mathf.Deg2Rad));
        Child.transform.localRotation = Quaternion.Euler(-ArmAngle, 0, 0);
    }
}

public enum ProjectileName
{
    Hammer, 
    Bullet,
    Shell,
    Last
}

public enum projectileType
{
    Unused,
    Normal,
    Explosive,// Ask for AOE
    Last
}

public enum Allegiance
{
    Player,//only damages enemy mobs
    Mob,// only damages player
    Neutral,// damages both **for when ff is on**
    Last
}
