using System.Collections.Generic;
using UnityEngine;

public class EditorObjects : ScriptableObject
{
    [NaughtyAttributes.Label("收藏夾")]
    public List<Object> ObjectList;
}
