using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;

public class Construct_Vehicle : ConstructData {

    protected float                MaxSpeed = 60f;
    protected float                MinSpeed = 1f;
    protected float                Acceleration = 1f;
    protected Vector3              Lead;
    protected Vector3              SlowConst = Vector3.one; 
    protected Vector3              LocalVelocity = Vector3.zero;
    protected float                BreakPower = 0f;
    protected float                Mass = 200;
    protected float                TurningCircle = 5f;
    protected float                TurningSpeed = 0.5f;
    protected bool                 isOnGround;
    Rigidbody PlayerRb;

    public override void Controlles(){ DrivingControlles(); }
    public Construct_Vehicle() { }

    public override void SetUp(Transform Player, GamePad.Index PlayerNum)
    {
        base.SetUp(Player, PlayerNum);
        SlowConst.y = -9.81f;
        SlowConst.x = 0.72f * SlowConst.y;
        SlowConst.z = 0.72f * SlowConst.y;

        if (Owner.GetComponent<CubePlayer>() != null)
        {
            PlayerRb = Owner.GetComponent<CubePlayer>().rb;
        }
    }

    public override void ChangeInto(List<GameObject> Cubes)
    {
        base.ChangeInto(Cubes);
        if (PlayerRb != null)
        {
            PlayerRb.constraints = RigidbodyConstraints.FreezeRotationY;
        }
    }
    /* KeyBoard Controlles
    public void DrivingControllesTest()
    {
        isOnGround = Physics.Raycast(Owner.position, -Owner.up, 1f);
        BreakPower = 0;
        LocalVelocity = Owner.InverseTransformVector(PlayerRb.velocity);
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
           if (Input.GetKey(KeyCode.UpArrow))
           {
                Lead.z += Acceleration;
                Lead.z = Mathf.Clamp(Lead.z, -MaxSpeed, MaxSpeed);
           }
           if (Input.GetKey(KeyCode.DownArrow))
           {
                Lead.z -= Acceleration;
                Lead.z = Mathf.Clamp(Lead.z, -MaxSpeed, MaxSpeed);
            }
        }
        else if(Mathf.Abs(Lead.z) > 0)
        {
            if (Mathf.Abs(Lead.z) > (-SlowConst.z * Time.deltaTime))
            {
                Lead.z = Lead.z + (Mathf.Sign(Lead.z) * (SlowConst.z * Time.deltaTime));
            }
            else
            {
                Lead.z = 0;
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
        }
        else
        {
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.LeftArrow) && Mathf.Abs(Lead.z)  >= MinSpeed)
            {
                Lead.x = -TurningCircle;
            }
            if (Input.GetKey(KeyCode.RightArrow) && Mathf.Abs(Lead.z) >= MinSpeed)
            {
                Lead.x = TurningCircle;
            }

        }
        else
        {
            Lead.x = 0;
        }
        if (isOnGround)
        {
            //Debug.DrawLine(Owner.transform.position, Owner.transform.rotation * Lead + Owner.transform.position, Color.blue, 10f);
            PlayerRb.velocity = new Vector3((Owner.rotation * Lead).x, PlayerRb.velocity.y,(Owner.rotation * Lead).z);
            Vector3 targetDir = Owner.rotation * Lead;
            float step = TurningSpeed * Time.deltaTime;
            targetDir = Vector3.Lerp(Owner.forward, targetDir * Mathf.Sign(Lead.z), step);
            Owner.transform.rotation =  Quaternion.Euler(Owner.transform.rotation.eulerAngles.x, Quaternion.LookRotation(targetDir, Owner.up).eulerAngles.y, Owner.transform.rotation.eulerAngles.z);
        }
        else
        {
            Lead = Vector3.zero;
        }
    }
    */

    public void DrivingControlles()
    {
        isOnGround = Physics.Raycast(Owner.position, -Owner.up, 1f);
        BreakPower = 0;
        LocalVelocity = Owner.InverseTransformVector(PlayerRb.velocity);

        if (GamePad.GetAxis(GamePad.Axis.LeftStick, Player) != Vector2.zero && Mathf.Abs(Lead.z) > MinSpeed)
        { 
             Lead.x = GamePad.GetAxis(GamePad.Axis.LeftStick, Player).x * TurningCircle;
        }
        else { Lead.x = 0; }

        if (GamePad.GetTrigger(GamePad.Trigger.RightTrigger, Player) != 0)
        {
            Lead.z += Acceleration;
            Lead.z = Mathf.Clamp(Lead.z, -MaxSpeed, MaxSpeed);
        }
        if (GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, Player) != 0)
        {
            Lead.z -= Acceleration;
            Lead.z = Mathf.Clamp(Lead.z, -MaxSpeed, MaxSpeed);
        }
        else if (Mathf.Abs(Lead.z) > 0)
        {
            if (Mathf.Abs(Lead.z) > (-SlowConst.z * Time.deltaTime))
            {
                Lead.z = Lead.z + (Mathf.Sign(Lead.z) * (SlowConst.z * Time.deltaTime));
            }
            else
            {
                Lead.z = 0;
            }
        }
        if (isOnGround)
        {
            //Debug.DrawLine(Owner.transform.position, Owner.transform.rotation * Lead + Owner.transform.position, Color.blue, 10f);
            PlayerRb.velocity = new Vector3((Owner.rotation * Lead).x, PlayerRb.velocity.y, (Owner.rotation * Lead).z);
            Vector3 targetDir = Owner.rotation * Lead;
            float step = TurningSpeed * Mathf.Abs(Lead.z/2) * Time.deltaTime;
            targetDir = Vector3.Lerp(Owner.forward, targetDir * Mathf.Sign(Lead.z), step);
            Owner.transform.rotation = Quaternion.Euler(Owner.transform.rotation.eulerAngles.x, Quaternion.LookRotation(targetDir, Owner.up).eulerAngles.y, Owner.transform.rotation.eulerAngles.z);
        }
        else
        {
            Lead = Vector3.zero;
        }
    }


    public override ConstructData Clone()
    {
        Construct_Vehicle temp = new Construct_Vehicle();
        return temp;
    }

}
