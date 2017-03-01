using UnityEngine;
using System.Collections;
using GamepadInput;

public class Construct_Immobile_Ladder : Construct_Immobile
{
    //options for position checking
    public bool IsStable = false;
    RaycastHit Hit;

    public override void SetUp(Transform Player, GamePad.Index PlayerNum)
    {
        base.SetUp(Player, PlayerNum);

    }

    public override void Controlles()
    {
        /*if(Physics.Raycast(Owner.transform.position, -Owner.transform.up, out Hit, 0.5f))
        {
            if(Hit.transform.gameObject.GetComponent<SomeScript>() != null)
            {
                Snap Ladder to Position
            }
        }
        else
        {
            base.Controlles();
        }*/
        if(IsStable)
        {
            //Snap Ladder to Position
        }
        else
        {
            base.Controlles();
        }
    }
}
