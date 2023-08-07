using UnityEngine;
using XiaoCao;

[System.Serializable]
public class Test_JsonA
{
    [XCLabel("名字")]
    public string Name;

    public TestEnumA typeA;

    public int[] intlist;

}
[System.Serializable]
public class Test_JsonB
{
    [XCLabel("名字")]
    public string Name;

    public TestEnumA typeA;

}

public enum TestEnumA
{
    A,
    B
}
