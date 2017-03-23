using UnityEngine;

using System.Collections;

using GamepadInput;

using System.Collections.Generic;



public class Construct_Immobile_Turret : Construct_Immobile

{

    float fireRate = 0.1f;

    float ThrowPower = 100f;

    float AttackDamage = 1f;

    float angle = 0f;

    float timer;

    GameObject Bullet;

    Vector3 LookLocation;

    GameObject Turret;

    GameObject Barrel;

    Projectile Temp = null;



    public override void Controlles() { TurretControlles(); TurretControllesTest(); }



    public override void SetUp(Transform Player, GamePad.Index PlayerNum)

    {

        base.SetUp(Player, PlayerNum);

        ImType = ImmoblieType.Turret;

    }



    public override void ChangeInto(List<GameObject> Cubes)

    {

        base.ChangeInto(Cubes);

        for (int i = 0; i < Obj_MovingParts.Count; i++)

        {

            if (Obj_MovingParts[i].Part.Name == "TurretTop")

            {

                Turret = Obj_MovingParts[i].getContainer;

            }

            if (Obj_MovingParts[i].Part.Name == "Barrel")

            {

                Barrel = Obj_MovingParts[i].getContainer;

            }

        }

        LookLocation = Vector3.zero;

    }



    public void TurretControlles()

    {

        LookLocation = Vector3.zero;

        if (GamePad.GetAxis(GamePad.Axis.LeftStick, Player) != Vector2.zero)

        {

            LookLocation.x = GamePad.GetAxis(GamePad.Axis.LeftStick, Player).x;

            LookLocation.z = GamePad.GetAxis(GamePad.Axis.LeftStick, Player).y;

        }

        Vector3 targetDir = LookLocation;

        float step = 1f * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(Turret.transform.forward, targetDir, step, 0.0F);

        Turret.transform.rotation = Quaternion.LookRotation(newDir);

        timer -= Time.deltaTime;

        timer = Mathf.Clamp(timer, 0, Mathf.Infinity);

        if (GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, Player) != 0 && timer == 0)

        {

            Shoot();

        }

    }

    /*KeyBoard Controlles*/

    public void TurretControllesTest()

    {

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

        Bullet = null;

        Temp = ConstructDirectory.GetProjectile(ProjectileName.Bullet);

        Bullet = Temp.gameObject;

        Temp = null;

        Bullet.GetComponent<Projectile>().initialize(Owner.gameObject, Barrel.transform, AttackDamage, ThrowPower, angle, projectileType.Normal, Allegiance.Player, ProjectileName.Bullet, 0f);

        timer = fireRate;

        Bullet.GetComponent<Projectile>().Launch();

    }



    public override ConstructData Clone()

    {

        Construct_Immobile_Turret temp = new Construct_Immobile_Turret();

        return temp;

    }



}

