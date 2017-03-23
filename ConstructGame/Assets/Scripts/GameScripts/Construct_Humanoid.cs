using UnityEngine;
using System.Collections;
using GamepadInput;
using System.Collections.Generic;
using System;

public class Construct_Humanoid : ConstructData
{
    Vector3 LookLocation;
    float t = 0.0f;
    GameObject rightHand, RightArm, LeftArm, RightLeg, LeftLeg;
    bool RightArmOverride = false, LeftArmOverride = false;
    GameObject Hammer;
    float ThrowPower = 50f;
    float AttackDamage = 1f;
    float angle = 20f;
    float fireRate = 1f;
    float timer;
    float AnimationLoop = 0;
    float ThrowLoop = 0f;
    float MovementSpeed = 16f;
    Vector3 MovementDirection = Vector3.zero;
    Projectile Temp = null;
    Rigidbody PlayerRb;
    bool isBuilding = false;
    BuildMode Mode = BuildMode.last;
    TurretTypes Ttype = TurretTypes.last;
    GameObject subject;

    public override void Controlles() { HumanoidControlles(); HumanoidControllesTest(); }


    public override void SetUp(Transform Player, GamePad.Index PlayerNum)
    {
        base.SetUp(Player, PlayerNum);
        LookLocation = Vector3.zero;
        LookLocation.y = Player.position.y;
        if (Owner.rotation.eulerAngles.x != 0)
        {
            Vector3 temp = new Vector3(0, Owner.transform.rotation.y, Owner.transform.rotation.z);
            Quaternion Qtemp = Quaternion.Euler(temp);
            Owner.rotation = Qtemp;
        }
        timer = fireRate;
    }

    public override void ChangeInto(List<GameObject> Cubes)
    {
        base.ChangeInto(Cubes);
        MovingPartController temp = new MovingPartController("RightHand", Owner, getPart("RightArm").getContainer.transform);
        temp.getContainer.transform.forward = -Owner.forward;
        RightArm = getPart("RightArm").getContainer;
        LeftArm = getPart("LeftArm").getContainer;
        LeftLeg = getPart("LeftLeg").getContainer;
        RightLeg = getPart("RightLeg").getContainer;
        rightHand = temp.getContainer;
        rightHand.transform.localPosition = new Vector3(0, -2f, 0);
        Obj_MovingParts.Add(temp);
        if (Owner.GetComponent<CubePlayer>() != null)
        {
            PlayerRb = Owner.GetComponent<CubePlayer>().rb;
            PlayerRb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void HumanoidControlles()

    {

        if (isBuilding)

        {

            BuildControlles();

        }

        WalkAnim();

        LookLocation = Vector3.zero;

        RaycastHit ht;

        if (GamePad.GetAxis(GamePad.Axis.RightStick, Player) != Vector2.zero)

        {

            LookLocation.x = GamePad.GetAxis(GamePad.Axis.RightStick, Player).x;

            LookLocation.z = GamePad.GetAxis(GamePad.Axis.RightStick, Player).y;

        }

        else

        {

            LookLocation.x = GamePad.GetAxis(GamePad.Axis.LeftStick, Player).x;

            LookLocation.z = GamePad.GetAxis(GamePad.Axis.LeftStick, Player).y;

        }

        Ray g;

        g = new Ray(Owner.position, -Owner.up);

        if (Physics.Raycast(g.origin, g.direction, out ht, 0.5f))

        {

            if (GamePad.GetAxis(GamePad.Axis.LeftStick, Player) != Vector2.zero)

            {

                MovementDirection.x = GamePad.GetAxis(GamePad.Axis.LeftStick, Player).x * MovementSpeed;

                MovementDirection.z = GamePad.GetAxis(GamePad.Axis.LeftStick, Player).y * MovementSpeed;

                PlayerRb.velocity = MovementDirection + new Vector3(0, PlayerRb.velocity.y, 0);

            }

            else

            {

                MovementDirection.x = 0;

                MovementDirection.z = 0;

            }

        }

        else

        {

            if (PlayerRb.velocity.y < 0)

            {

                PlayerRb.velocity = new Vector3(0, PlayerRb.velocity.y, 0);

            }

            else

            {

                PlayerRb.velocity = Vector3.zero;

            }

        }

        if (Owner.rotation.eulerAngles.x != 0 || Owner.rotation.eulerAngles.z != 0)

        {

            Vector3 oldRotation = Owner.transform.rotation.eulerAngles;

            Vector3 newRotation = new Vector3(0, Owner.transform.rotation.eulerAngles.y, 0);

            if (t <= 1.0) { t += 0.01f; }

            oldRotation = (Quaternion.Slerp(Quaternion.Euler(oldRotation), Quaternion.Euler(newRotation), t)).eulerAngles;

            Owner.transform.rotation = Quaternion.Euler(oldRotation);

        }

        else

        {

            t = 0.0f;

        }

        Vector3 targetDir = LookLocation;

        float step = 4f * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(Owner.transform.forward, targetDir, step, 0.0F);

        Owner.transform.rotation = Quaternion.LookRotation(newDir);

        timer -= Time.deltaTime;

        timer = Mathf.Clamp(timer, 0, Mathf.Infinity);

        if (GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, Player) != 0 && timer == 0)

        {

            RightArmOverride = true;

        }

    }

    /* KeyBoard Controlles*/

    public void HumanoidControllesTest()

    {

        if (isBuilding)

        {

            BuildControllesTest();

        }

        WalkAnim();

        LookLocation = Vector3.zero;

        RaycastHit ht;

        if (Input.GetKey(KeyCode.A))

        {

            LookLocation.x = -1f;

        }

        if (Input.GetKey(KeyCode.D))

        {

            LookLocation.x = 1f;

        }

        if (Input.GetKey(KeyCode.W))

        {

            LookLocation.z = 1f;

        }

        if (Input.GetKey(KeyCode.S))

        {

            LookLocation.z = -1f;

        }

        Ray g;

        g = new Ray(Owner.position, -Owner.up);

        if (Physics.Raycast(g.origin, g.direction, out ht, 0.5f))

        {

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))

            {



                if (Input.GetKey(KeyCode.LeftArrow))

                {

                    MovementDirection.x = -1 * MovementSpeed;

                }

                else if (Input.GetKey(KeyCode.RightArrow))

                {

                    MovementDirection.x = 1 * MovementSpeed;

                }

                else

                {

                    MovementDirection.x = 0;

                }

                if (Input.GetKey(KeyCode.UpArrow))

                {

                    MovementDirection.z = 1 * MovementSpeed;

                }

                else if (Input.GetKey(KeyCode.DownArrow))

                {

                    MovementDirection.z = -1 * MovementSpeed;

                }

                else

                {

                    MovementDirection.z = 0;

                }

                PlayerRb.velocity = Owner.TransformVector(MovementDirection) + new Vector3(0, PlayerRb.velocity.y, 0);

            }

            else

            {

                if (PlayerRb.velocity.y < 0)

                {

                    PlayerRb.velocity = new Vector3(0, PlayerRb.velocity.y, 0);

                }

                else

                {

                    PlayerRb.velocity = Vector3.zero;

                }

            }

        }

        if (Owner.rotation.eulerAngles.x != 0 || Owner.rotation.eulerAngles.z != 0)

        {

            Vector3 oldRotation = Owner.transform.rotation.eulerAngles;

            Vector3 newRotation = new Vector3(0, Owner.transform.rotation.eulerAngles.y, 0);

            if (t <= 1.0) { t += 0.01f; }

            oldRotation = (Quaternion.Slerp(Quaternion.Euler(oldRotation), Quaternion.Euler(newRotation), t)).eulerAngles;

            Owner.transform.rotation = Quaternion.Euler(oldRotation);

        }

        else

        {

            t = 0.0f;

        }

        Vector3 targetDir = LookLocation;

        float step = 4f * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(Owner.transform.forward, targetDir, step, 0.0F);

        Owner.transform.rotation = Quaternion.LookRotation(newDir);

        timer -= Time.deltaTime;

        timer = Mathf.Clamp(timer, 0, Mathf.Infinity);

        if (Input.GetKeyDown(KeyCode.Space) && timer == 0)

        {

            RightArmOverride = true;

        }

    }



    public void BuildControllesTest()

    {

        if (subject != null)

        {

            subject.transform.position = Owner.transform.position + ((Owner.transform.forward * 8) + Owner.transform.up * 10);

        }

        if (Input.GetKeyDown(KeyCode.O))

        {

            if (Mode == BuildMode.Turrets)

            {

                if (Ttype != TurretTypes.last && subject == null)

                {

                    Debug.Log("Getting Turret");

                    subject = ConstructDirectory.GetTurret(Ttype);

                }

                else

                {

                    if (subject.GetComponent<IPlaceableObj>().Place(subject.transform.position))

                    {

                        subject = null;

                    }

                }

            }

            else if (Mode == BuildMode.Wall)

            {

                Ray r = new Ray(Owner.transform.position + Owner.transform.forward * 8 + Vector3.up * 8, Vector3.down);

                Debug.Log("Click: " + r.origin);

                RaycastHit hit;

                Debug.DrawRay(r.origin, r.direction * 10f, Color.red);

                if (Physics.Raycast(r, out hit, Mathf.Infinity))

                {

                    Debug.Log("Hit.");



                    IClickable c = hit.collider.gameObject.GetComponent<IClickable>();

                    if (c != null)

                    {

                        Debug.Log("Click action: " + hit.point);

                        c.ClickAction(hit.point);

                    }

                }

            }

            else { Debug.Log("Error Build mode not set correctly"); }

        }

        else if (Input.GetKeyDown(KeyCode.P))

        {

            if (subject != null)

            {

                GameObject.Destroy(subject.gameObject);

                subject = null;

                isBuilding = false;

                Mode = BuildMode.last;

            }

        }

    }



    public override void setMode(BuildMode mode)
    {
        Mode = mode;
        isBuilding = true;
    }

    public override void setTurret(TurretTypes type)
    {
        Ttype = type;
    }

    public void BuildControlles()
    {
        if (subject != null)
        {
            subject.transform.position = Owner.transform.position + ((Owner.transform.forward * 8) + Owner.transform.up * 10);
        }
        if (GamePad.GetButtonDown(GamePad.Button.A, Player))
        {
            if (Mode == BuildMode.Turrets)
            {
                if (Ttype != TurretTypes.last && subject == null)
                {
                    subject = ConstructDirectory.GetTurret(Ttype);
                }
                else
                {
                    if (subject.GetComponent<IPlaceableObj>().Place(subject.transform.position))
                    {
                        subject = null;
                    }
                }
            }
            else if (Mode == BuildMode.Wall)
            {
                Ray r = new Ray(Owner.transform.position + ((Owner.transform.forward * 8) + Owner.transform.up * 10), Vector3.down);
                Debug.Log("Click: " + r.origin);
                RaycastHit hit;
                Debug.DrawRay(r.origin, r.direction * 10f, Color.red);
                if (Physics.Raycast(r, out hit, Mathf.Infinity))
                {
                    Debug.Log("Hit.");

                    IClickable c = hit.collider.gameObject.GetComponent<IClickable>();
                    if (c != null)
                    {
                        Debug.Log("Click action: " + hit.point);
                        c.ClickAction(hit.point);
                    }
                }
            }
            else { Debug.Log("Error Build mode not set correctly"); }
        }
        else if (GamePad.GetButtonDown(GamePad.Button.B, Player))
        {
            if (subject != null)
            {
                GameObject.Destroy(subject.gameObject);
                subject = null;
                isBuilding = false;
                Mode = BuildMode.last;
            }
        }
    }
    void Shoot()
    {
        Temp = ConstructDirectory.GetProjectile(ProjectileName.Hammer);
        Hammer = Temp.gameObject;
        Hammer.transform.parent = rightHand.transform;
        Temp = null;
        Hammer.GetComponent<Projectile>().initialize(Owner.gameObject, rightHand.transform, AttackDamage, ThrowPower, angle, projectileType.Normal, Allegiance.Player, ProjectileName.Hammer, 19.81f);
        Hammer.GetComponent<Projectile>().Launch();
        Hammer = null;
        timer = fireRate;
    }

    public override void Reset()
    {
        PlayerRb.constraints = ~RigidbodyConstraints.FreezeAll;
        base.Reset();
    }

    public void WalkAnim()
    {
        if (PlayerRb.velocity.magnitude > 1)
        {
            AnimationLoop = AnimationLoop >= 360 ? 0 : AnimationLoop + (MovementSpeed / 4);
        }
        else
        {
            AnimationLoop = 0;
        }
        float Swing = (50 * Mathf.Sin(AnimationLoop * Mathf.Deg2Rad));
        if (!LeftArmOverride)
        {
            LeftArm.transform.localRotation = Quaternion.Euler(Swing, 0, 0);
        }
        if (!RightArmOverride)
        {
            RightArm.transform.localRotation = Quaternion.Euler(-Swing, 0, 0);
        }
        else
        {
            ThrowAnim();
        }
        LeftLeg.transform.localRotation = Quaternion.Euler(-Swing, 0, 0);
        RightLeg.transform.localRotation = Quaternion.Euler(Swing, 0, 0);
    }

    public void ThrowAnim()
    {
        ThrowLoop += 5f;
        float ArmAngle = 120 + (100 * Mathf.Sin(ThrowLoop * Mathf.Deg2Rad));
        RightArm.transform.localRotation = Quaternion.Euler(ArmAngle, 0, 0);
        if (ArmAngle >= 220)
        {
            Shoot();
            RightArmOverride = false;
            ThrowLoop = 0;
        }

    }

    public override ConstructData Clone()
    {
        Construct_Humanoid temp = new Construct_Humanoid();
        return temp;
    }
}
