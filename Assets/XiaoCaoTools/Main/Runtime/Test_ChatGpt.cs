using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Test_ChatGpt : MonoBehaviour
{
    public Transform startTF;
    public Transform endTf;

    public Transform onMoveTF;

    public Vector3 Start;
    public Vector3 End;

    public float ScaleY=1;
  
    [Range(0,10)]
    [NaughtyAttributes.OnValueChanged(nameof(Move))]
    //当t为1,则会到达终点
    public float t;
    //omega 是周期长
    public int omega = 1;

    public float xB => End.x;
    public float xA => Start.x;
    public float yB => End.y;
    public float yA => Start.y;
    public float zB => End.z;
    public float zA => Start.z;

    private void SetPoints()
    {
        Start = startTF.position;
        End = endTf.position;
    }

    private void Move()
    { 
        SetPoints();
        //原公式 float x = (float)(xA + t * (xB - xA) + Scale * Math.Sin(Omega * t + phi));
        // phi 偏移量(0) , Scale是各轴的缩放,默认Y方向, omega 是周期长, 一般是PI倍数
        float x =(float)(xA + t * (xB - xA));
        float y =(float)(yA + t * (yB - yA) + ScaleY* Math.Sin(omega*Mathf.PI * t));
        float z =(float)(zA + t * (zB - zA));
        onMoveTF.position = new Vector3(x, y, z);
    }

}
