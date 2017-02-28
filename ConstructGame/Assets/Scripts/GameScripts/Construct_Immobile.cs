using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;

public class Construct_Immobile : ConstructData
{
    [SerializeField]protected bool isHeld;
    [SerializeField]protected ImmoblieType ImType;
    private float LanchPower;
    private float MaxPower;
    public Vector3 Position { get { return Owner.transform.position; } set { Owner.transform.position = value; } }
    public ImmoblieType immobileType { get { return ImType; } }
    public override void SetUp(Transform Player, GamePad.Index PlayerNum)
    {
        base.SetUp(Player, PlayerNum);
        LanchPower = 0;
        MaxPower = 5;
    }
    public override void Controlles()
    {
        if(Input.GetKey(KeyCode.X))
        {
            LanchPower += 0.1f;
            LanchPower = Mathf.Clamp(LanchPower,0,MaxPower);
        }
        if(Input.GetKeyUp(KeyCode.X) && Physics.Raycast(Owner.position,-Owner.up, 0.5f))
        {
            Owner.GetComponent<Rigidbody>().AddForce(Vector3.up * LanchPower, ForceMode.Impulse);
        }
    }

    public override void ChangeInto(List<GameObject> Cubes)
    {
        base.ChangeInto(Cubes);
    }

    public override ConstructData Clone()
    {
        Construct_Immobile temp = new Construct_Immobile();
        return temp;
    }
}

public enum ImmoblieType
{
    BossKey,
    Key,
    Ladder,
    Turret
}