using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThreadNav : MonoBehaviour {

    static ThreadNav instance;
    public static ThreadNav getInstance {  get { return instance; } }

    Dictionary<Vector2, List<float>> xzNav;
    [SerializeField] float HeightTolerance = 1.0f; // nodes with a vertical offset less than this from another node will be discarded.
    [SerializeField]
    float ScanningResolution = 1.0f; // sets distance between raycast scans.

	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;

            xzNav = new Dictionary<Vector2, List<float>>();

            return;
        }

        Destroy(this.gameObject);
	}
	
    IEnumerator Deploy(Vector3 origin)
    {

        float t = Time.realtimeSinceStartup;

        xzNav.Clear();
        List<Vector2> queue = new List<Vector2>();


        queue.Add(new Vector2(origin.x, origin.z));

        while (queue.Count > 0)
        {
            
            if (xzNav.ContainsKey(queue[0]))
            {
                // we have already dealt with this location
                queue.RemoveAt(0);
                continue;
            }

            xzNav.Add(queue[0], new List<float>());

            RaycastHit[] hits = Physics.RaycastAll(new Ray(new Vector3(queue[0].x, Mathf.Infinity, queue[0].y), Vector3.down));

            if (hits.Length == 0)
            {
                // no hits -- nonnavigatable.
                queue.RemoveAt(0);
                continue;
            }



            for (int i = 0; i < hits.Length; i++)
            {
                
                if (xzNav[queue[0]].Count > 0)
                {
                    bool tooClose = false;
                    for (int n = 0; n < xzNav[queue[0]].Count; n++)
                    {
                        if (Mathf.Abs(hits[i].point.y - xzNav[queue[0]][n]) < HeightTolerance)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                    if (tooClose)
                    {
                        continue;
                    }
                }

                xzNav[queue[0]].Add(hits[i].point.y);

            }


            queue.Add(queue[0] + ScanningResolution * Vector2.up);
            queue.Add(queue[0] + ScanningResolution * Vector2.down);
            queue.Add(queue[0] + ScanningResolution * Vector2.left);
            queue.Add(queue[0] + ScanningResolution * Vector2.right);
            

            if (Time.realtimeSinceStartup - t > 0.16f)
            {
                t = Time.realtimeSinceStartup;
                yield return null;
            }

        }

        // now we produce a classic node net


    }

}
