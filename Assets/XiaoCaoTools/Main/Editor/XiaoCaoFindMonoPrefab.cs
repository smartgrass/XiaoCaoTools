using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XiaoCao;
using Object = UnityEngine.Object;
/// <summary>
/// 根据输入的Mono类查找Asset中的预制体
/// </summary>
public class XiaoCaoFindMonoPrefab : XiaoCaoWindow
{
	[MenuItem("Tools/XiaoCao/Mono类预制体查找")]
	static void Open()
	{
		OpenWindow<XiaoCaoFindMonoPrefab>();
	}
	[Header("可根据Mono类搜索Prefab")]
	public string testSearchStr = "";

	public string testType = "Consolation.ConsoleGUI";

	[OnValueChanged(nameof(OnTypeChnage))]
	[Dropdown(nameof(GetTpyeName))]
	public string testType1;

	[Header("搜索文件夹")]
	[OnValueChanged(nameof(OnDirChange))]
	public Object dir;
	[ReadOnly]
	public string dirPath = "Assets";

	public List<string> unSearchs;

	public List<Object> assets;

	public Type tmpType;

	Assembly tmpAss;

	private bool isStop;

	public DropdownList<string> GetTpyeName()
	{
		if (tmpAss == null)
		{
			tmpAss = System.Reflection.Assembly.Load("Assembly-CSharp");
		}
		DropdownList<string> list = new DropdownList<string> { };
		var types = tmpAss.GetTypes();

		Type monoType = typeof(MonoBehaviour);
		foreach (var item in types)
		{
			if (monoType.IsAssignableFrom(item))
			{
				list.Add(item.Name, item.FullName);
			}
		}
		return list;
	}

	private void OnTypeChnage()
	{
		testType = testType1;
	}

	private void OnDirChange()
	{
		if (dir != null)
		{
			dirPath = AssetDatabase.GetAssetPath(dir);
		}
	}

	[Button("开始搜索")]
	private void TestSearch()
	{
		isStop = false;
		string dir = dirPath;
		string[] guids = AssetDatabase.FindAssets(testSearchStr + " t:GameObject", new string[] { dir });
		List<string> paths = new List<string>();
		new List<string>(guids).ForEach(m => paths.Add(AssetDatabase.GUIDToAssetPath(m)));
		assets = new List<Object>();
		tmpType = TestType();

		SearchObject(paths);
	}
	[Button("停止搜索")]
	private void StopSearch()
	{
		isStop = true;
	}

	private async void SearchObject(List<string> paths)
	{
		int len = paths.Count;
		int len_small = Mathf.Max(1, len / 25);

		List<Object> tmpList = new List<Object>();
		for (int i = 0; i < len; i++)
		{
			if (tmpType != null)
			{
				if (isStop)
				{
					Debug.Log($"yns stop!");
					return;
				}

				if (i % len_small == 0)
				{
					Debug.Log($"{i}/{len}...");
					await Task.Delay(20);
				}
				var obj = AssetDatabase.LoadAssetAtPath(paths[i], tmpType);
				if (obj != null)
				{
					Debug.Log($"yns find {obj.name}");
					tmpList.Add(obj);
				}
			}
		}
		assets = tmpList;
		var property = Editor.serializedObject.FindProperty(nameof(assets));
		property.isExpanded = true;
		Debug.Log($"===end=== len ={len}");
	}


	private Type TestType()
	{
		if (testType == "")
		{
			return null;
		}
		Assembly assembly = System.Reflection.Assembly.Load("Assembly-CSharp");
		Type _type = assembly.GetType(testType);
		Debug.Log($"yns Type √");
		return _type;
	}

	[Button("搜索例子参数")]
	private void TestValue()
	{
		testType = "Consolation.ConsoleGUI";
		testSearchStr = "";
		dirPath = "Assets";
	}
	[Button("删除Assets")]
	private void DelAseets()
	{
		Debug.Log($"yns del {assets.Count}");
		foreach (var item in assets)
		{
			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
		}
	}

	public void FindByFileName(string nameStr)
	{
		//返回的是资源GUID
		//"t:ScriptObj l:helmet"
		string[] guids = AssetDatabase.FindAssets("name t:GameObject", new string[] { "Assets" });
		List<string> paths = new List<string>();
		new List<string>(guids).ForEach(m => paths.Add(AssetDatabase.GUIDToAssetPath(m)));
		paths.ForEach(p =>
		{
			GameObject obj = AssetDatabase.LoadAssetAtPath(p, typeof(GameObject)) as GameObject;
			if (obj.name == nameStr)
			{
				Selection.objects = new Object[] { obj };
				return;
			}
		});
	}
}


