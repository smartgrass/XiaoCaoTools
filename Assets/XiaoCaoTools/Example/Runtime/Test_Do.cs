using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Do : MonoBehaviour
{
    public static string testName;

    public Color color;

    public void ChangeColor()
    {
        Debug.Log($"yns ChangeColor");
        color = new Color(Random.Range(0,1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }

}
