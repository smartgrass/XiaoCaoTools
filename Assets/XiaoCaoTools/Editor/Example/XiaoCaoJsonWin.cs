using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XiaoCao;
public class XiaoCaoJsonWin : XiaoCaoWindow
{
    [MenuItem("Tools/XiaoCao/Json编辑窗口")]
    static void Open()
    { 
        OpenWindow<XiaoCaoJsonWin>("Json编辑窗口");
    }

    public override void OnEnable()
    {
        base.OnEnable();
        string path = "Assets/XiaoCaoTools/Assets/Editor/JsonObjectUsing.asset";
        jsonObjectUsing = XCEditorTool.GetOrNewSO<JsonObjectUsing>(path) as JsonObjectUsing;
        FromJson();
        //Debug.Log($"yns OnEnable");
    }

    public JsonObjectUsing jsonObjectUsing;

    public JsonUsing CurUsing
    {
        get
        {
            return jsonObjectUsing.UsingList.Find(s => s.configType == type);
        }
    }

    [OnValueChanged(nameof(OnConifgTypeChange))]
    public ConfigType type;

    public TextAsset JsonText => CurUsing.textAsset;

    [ShowIf(nameof(IsConfigA))]
    public Test_JsonA configA;
    [ShowIf(nameof(IsConfigB))]
    public Test_JsonB configB;

    [Space]
    public bool isWriteToText = true;

    [XCLabel("json写入格式化")]
    public bool isFromat = true;


    public bool IsConfigA => type == ConfigType.ConfigA;
    public bool IsConfigB => type == ConfigType.ConfigB;



    //[Button("读取")]
    private void FromJson()
    {
        switch (type)
        {
            case ConfigType.ConfigB:
                configB = JsonUtility.FromJson<Test_JsonB>(JsonText.text);
                break;
            case ConfigType.ConfigA:
                configA = JsonUtility.FromJson<Test_JsonA>(JsonText.text);
                break;
            default:
                break;
        }
    }

    private object GetObject()
    {
        switch (type)
        {
            case ConfigType.ConfigA:
                return configA;
            case ConfigType.ConfigB:
                return configB;
        }
        return null;
    }

    private void OnConifgTypeChange()
    {
        FromJson();
    }


    [Button("转换/写入")]
    private void ToJson()
    {
        string json = GetJson();
        json.LogStr();
        if (isWriteToText)
        {
            string path = AssetDatabase.GetAssetPath(JsonText);
            string fullPath = Application.dataPath.Replace("/Assets", "/" + path);
            FileTool.WriteToFile(json, fullPath);
            EditorUtility.SetDirty(JsonText);
            AssetDatabase.Refresh();
        }
    }

    private string GetJson()
    {
        if (isFromat)
        {
            return JsonConvert.SerializeObject(GetObject(),Formatting.Indented);
        }
        else
        {
            return JsonConvert.SerializeObject(GetObject());
        }
    }
}