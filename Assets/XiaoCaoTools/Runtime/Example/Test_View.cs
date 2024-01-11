using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Test_View : MonoBehaviour
{
    private Vector3 _vector3;
    public MonoBehaviour showMono;
    public Test_View_Data data = new Test_View_Data();
    public Dictionary<int, Test_View_Data> dic = new Dictionary<int, Test_View_Data>();

    private List<int> ints;

    private void Start()
    {
        dic.Add(1,new Test_View_Data(){num = 1});
        dic.Add(2,new Test_View_Data(){num = 2});
    }
}

public class Test_View_Data
{
    public int num;
    private Vector3 _vector3;
}