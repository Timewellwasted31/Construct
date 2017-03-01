using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;

public class Construct_Vehicle_ForkLift : Construct_Vehicle
{
    public GameObject Lift;

    public override void SetUp(Transform Player, GamePad.Index PlayerNum)
    {
        base.SetUp(Player, PlayerNum);
        TurningCircle = 0.15f;
        TurningSpeed = 0.001f;
        MaxSpeed = 2f;
        Owner = Player;
        MinSpeed = 0f;
        Mass = 60f;
    }

    public override void ChangeInto(List<GameObject> Cubes)
    {
        base.ChangeInto(Cubes);
        for (int i = 0; i < Obj_MovingParts.Count; i++)
        {
            if (Obj_MovingParts[i].Part != null && Obj_MovingParts[i].Part.Name == "Lift")
            {
                Lift = Obj_MovingParts[i].getContainer;
            }
        }
    }

}
