using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Do : MonoBehaviour
{
    public static string testName;

    [NaughtyAttributes.OnValueChanged(nameof(ChangeColor))]
    public Color color;

    public Renderer renderer;

    public void ChangeColor()
    {
        Debug.Log($"yns ChangeColor");
        color = new Color(Random.Range(0,1f), Random.Range(0, 1f), Random.Range(0, 1f));
        renderer.material.color = color;
    }

}
