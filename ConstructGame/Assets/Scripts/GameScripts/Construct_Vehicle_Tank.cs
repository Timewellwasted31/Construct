using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;

public class Construct_Vehicle_Tank : Construct_Vehicle
{
    Vector3 LookLocation;
    GameObject Turret;
    GameObject Barrel;
    float fireRate = 1f;
    float ThrowPower = 100f;
    float AttackDamage = 10f;
    float angle = 0f;
    float timer;
    GameObject Shell;
    Projectile Temp = null;

    public override void SetUp(Transform Player, GamePad.Index PlayerNum)
    {
        base.SetUp(Player, PlayerNum);
        TurningCircle = 0.1f;
        MaxSpeed = 10f;
        Owner = Player;
        MinSpeed = 0f;
        TurningSpeed = 10f;
        Mass = 200f;
    }

    public override void ChangeInto(List<GameObject> Cubes)
    {
        base.ChangeInto(Cubes);
        for (int i = 0; i < Obj_MovingParts.Count; i++)
        {
            if (Obj_MovingParts[i].Part != null && Obj_MovingParts[i].Part.Name == "Turret")
            {
                Turret = Obj_MovingParts[i].getContainer;
            }
        }
        MovingPartController temp = new MovingPartController("Barrel", Owner, getPart("Turret").getContainer.transform);
        Barrel = temp.getContainer;
        Barrel.transform.localPosition = new Vector3(0, 0.5f, 4f);
        Barrel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        Obj_MovingParts.Add(temp);
        LookLocation = Vector3.zero;
        LookLocation.y = Turret.transform.position.y;
    }


    public override void Controlles()
    {
        base.Controlles();
        LookLocation = Vector3.zero;
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
        Vector3 targetDir = LookLocation;
        float step = 1f * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(Turret.transform.forward, targetDir, step, 0.0F);
        Turret.transform.rotation = Quaternion.LookRotation(newDir);
        timer -= Time.deltaTime;
        timer = Mathf.Clamp(timer, 0, Mathf.Infinity);
        if (Input.GetKey(KeyCode.Space) && timer == 0)
        {
            Shoot();
        }
    }


    void Shoot()
    {
        Shell = null;
        Temp = ConstructDirectory.GetProjectile(ProjectileName.Shell);
        Shell = Temp.gameObject;
        Temp = null;
        Shell.GetComponent<Projectile>().initialize(Owner.gameObject, Barrel.transform, AttackDamage, ThrowPower, angle, projectileType.Normal, Allegiance.Player, ProjectileName.Shell, 0f);
        timer = fireRate;
        Shell.GetComponent<Projectile>().Launch();
    }

    public override ConstructData Clone()
    {
        Construct_Vehicle_Tank temp = new Construct_Vehicle_Tank();
        return temp;
    }
}
