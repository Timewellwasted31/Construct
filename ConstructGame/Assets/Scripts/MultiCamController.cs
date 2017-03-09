using UnityEngine;
using System.Collections;

public class MultiCamController : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] float m_FollowCamSizeRatio = 3f;

    [Header("Cameras")]
    [SerializeField] Camera CoreCamera;
    [SerializeField] Camera[] FollowCameras;

    [Header("Follow Objects")]
    [SerializeField] Transform[] Follows;

    [Header("Core Camera")]
    [SerializeField] float CameraGap;

	// Use this for initialization
	void Start () {
        CoreCamera = Camera.main;
        CoreCamera.orthographic = true;
        CoreCamera.orthographicSize = 5f;
        
        if (Follows.Length > 0) {
            CoreCamera.transform.position = Follows[0].transform.position + CameraGap * (CoreCamera.transform.rotation * Vector3.back);
        }

        for (int i = 0; i < FollowCameras.Length; i++) {
            FollowCameras[i].orthographic = true;
            FollowCameras[i].rect = new Rect(Vector2.zero, Vector2.one / m_FollowCamSizeRatio);
            FollowCameras[i].orthographicSize = CoreCamera.orthographicSize / m_FollowCamSizeRatio;
        }
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 maincam = Vector3.zero;

        for (int i = 0;  i < Follows.Length; i++) {
            // is the follow in the core camera?
            Vector2 pos = CoreCamera.WorldToViewportPoint(Follows[i].transform.position);
            if (pos.x < 1 && pos.y < 1 && pos.x > 0 && pos.y > 0) {
                // if yes, add to maincam vector and average
                if (maincam.sqrMagnitude > 0) {
                    maincam = (maincam + Follows[i].transform.position + CameraGap * (CoreCamera.transform.rotation * Vector3.back)) / 2f;
                }else {
                    maincam = Follows[i].transform.position + CameraGap * (CoreCamera.transform.rotation * Vector3.back);
                }
                // is there a better way to do this? A WIZARD WAY?
                
                FollowCameras[i].enabled = false;
                continue;
            }
            // if no, enable a follow cam.
            FollowCameras[i].enabled = true;
        }

        // move the main camera to the position that fits requested objects
        if (maincam.magnitude > 0)
        CoreCamera.transform.position = maincam;

        for (int i = 0; i < FollowCameras.Length; i++) {
            if (FollowCameras[i].enabled == false) continue; // if camera is not tasked, 

            // move the follow cam to the appropriate position for the follow object
            FollowCameras[i].transform.position = Follows[i].transform.position + CameraGap * (CoreCamera.transform.rotation * Vector3.back);
            FollowCameras[i].transform.rotation = Camera.main.transform.rotation;
            // move the follow cam display frame to the relative direction...
            // FollowCameras[i].rect;
            Rect t = FollowCameras[i].rect;

            Vector3 v = CoreCamera.transform.position - FollowCameras[i].transform.position;
            v.y = 0;
            v.Normalize();
            // okay. So, we get a value between 1 and -1, which sets the side it appears on.
            // however...
            // okay.
            if (Mathf.Abs(v.x) >= Mathf.Abs(v.z)) {
                v.z = v.z * (1 / Mathf.Abs(v.x));  // multiply v.z to same value...
                v.x = Mathf.Sign(v.x); // normalize v.x to 1/-1
            }else {
                v.x = v.x * (1 / Mathf.Abs(v.z));  // multiply v.z to same value...
                v.z = Mathf.Sign(v.z); // normalize v.x to 1/-1
            }

            
            t.position = (0.5f*Vector2.one - 0.5f*new Vector2(v.x, v.z))*(1f - (1f/ m_FollowCamSizeRatio));
            FollowCameras[i].rect = t;

            // Debug.Log(t.position);



        }



    }
}
