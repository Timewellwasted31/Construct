using UnityEngine;
using System.Collections;

public class ClickController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButtonDown(0)) {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Debug.Log("Click: " + r.origin);
            RaycastHit hit;

            Debug.DrawRay(r.origin, r.direction * 10f, Color.red);
            if (Physics.Raycast(r, out hit, Mathf.Infinity)) {
                // Debug.Log("Hit.");
                
                IClickable c = hit.collider.gameObject.GetComponent<IClickable>();
                if (c != null) {
                    Debug.Log("Click action: " + hit.point);
                    c.ClickAction(hit.point);
                }
            }

        }
	}
}
