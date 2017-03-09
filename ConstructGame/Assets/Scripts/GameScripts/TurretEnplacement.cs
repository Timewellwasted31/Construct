using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class TurretEnplacement : MonoBehaviour, IDamageable, IPlaceableObj
{
    public float health = 100;
    public float fireRate = 1f;
    public float timePassed = 0f;
    public float Range = 5f;
    public float Damage = 10f;
    public Vector3 Position;
    public Transform Target = null;
    public Transform TurretTop;
    public Transform[] ContactPoints;

    public void MainLoop()
    {
        if(Target == null)
        {
            //Ask For Target Ex. Target = EnemyScript.isEnemysInRange(Range)
            LookAt(Vector3.forward);
        }
        else
        {
            LookAt(Target.position);
            CheckToFire();
            if((Target.position - transform.position).magnitude > Range)
            {
                Target = null;
            }
        }
    }

    public void LookAt(Vector3 pos)
    {
        TurretTop.rotation = Quaternion.LookRotation(new Vector3(pos.x, TurretTop.position.y, pos.z));
    }

    public void CheckToFire()
    {
        if(timePassed <= 0)
        {
            Target.GetComponent<IDamageable>().TakeDamage(Damage);
            timePassed = fireRate;
        }
        else
        {
            timePassed -= Time.deltaTime;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    public bool Place(Vector3 pos)
    {
        float first = 0;
        RaycastHit hit;
        Vector3 CentrePoint = new Vector3(transform.position.x, ContactPoints[0].position.y, transform.position.z);
        Debug.DrawRay(transform.position, Vector3.down * Mathf.Infinity);
        if (Physics.Raycast(new Ray(CentrePoint, Vector3.down), out hit, Mathf.Infinity))
        {
            if (first == 0)
            {
                first = Mathf.RoundToInt((CentrePoint - hit.point).magnitude);
            }
            else
            {
                return false;
            }
        }
        for (int i = 0; i < ContactPoints.Length; i++)
        {
            Debug.DrawRay(ContactPoints[i].position, Vector3.down * Mathf.Infinity);
            if (Physics.Raycast(new Ray(ContactPoints[i].position, Vector3.down), out hit, Mathf.Infinity))
            {
                if (first < Mathf.RoundToInt((ContactPoints[i].position - hit.point).magnitude) || first > Mathf.RoundToInt((ContactPoints[i].position - hit.point).magnitude) || Physics.Raycast(new Ray(hit.point - ContactPoints[i].position, Vector3.down), (hit.point - ContactPoints[i].position).magnitude))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}

public enum TurretTypes
{
    MG,
    Mortor,
    Gate,
    last
}
