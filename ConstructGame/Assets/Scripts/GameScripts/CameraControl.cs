using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour {

    Vector3 CenterPoint;
    float Zoom;
    float RoomX = 10, RoomY = 10;

    void Update()
    {
        CalculateCenterPoint();
        if (transform.position != CenterPoint && Zoom < (RoomX * RoomY))
        {
            transform.position = CenterPoint + new Vector3(0, Zoom, 0);
        }
    }

    void CalculateCenterPoint()
    {
        Vector3 NewCenter = Vector3.zero;
        float NewZoom = (RoomX + RoomY)* 2;
        int i;
        for(i = 0; i < CubePlayer.PlayerList.Count; i++)
        {
            NewCenter += CubePlayer.PlayerList[i].gameObject.transform.position;
            if(NewZoom < ((CubePlayer.PlayerList[i].gameObject.transform.position - transform.position).magnitude))
            {
                NewZoom = ((CubePlayer.PlayerList[i].gameObject.transform.position - transform.position).magnitude);
            }
        }
        NewCenter /= i;
        NewCenter.y = 0;
        CenterPoint = NewCenter;
        if(NewZoom < RoomX * RoomY)
        {
            Zoom = NewZoom;
        }
        else
        {
            Zoom = RoomX * RoomY;
        }
    }
}
