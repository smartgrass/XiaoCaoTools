using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XiaoCao
{
    public static class XCEditorTool
    {
        public static void SelectSelf(this UnityEngine.Object self)
        {
            Selection.activeObject = self;
        }

        public static Object[] ToObjectArray(this IEnumerable<UnityEngine.Object> list)
        {
            return list.ToArray();
        }


        public static List<Object> FindAssetListByName(string nameStr, string TypeName, string path = "Assets")
        {
            List<Object> objList = new List<Object>();
            string[] guids = AssetDatabase.FindAssets($"{nameStr} t:{TypeName}", new string[] { path });
            List<string> paths = new List<string>();
            new List<string>(guids).ForEach(m => paths.Add(AssetDatabase.GUIDToAssetPath(m)));
            for (int i = 0; i < paths.Count; i++)
            {
                objList.Add(AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object)));
            }
            return objList;
        }

        public static Object FindAssetByName(string nameStr, string TypeName, string path = "Assets")
        {
            Object obj = null;
            string[] guids = AssetDatabase.FindAssets($"{nameStr} t:{TypeName}", new string[] { path });
            List<string> paths = new List<string>();
            new List<string>(guids).ForEach(m => paths.Add(AssetDatabase.GUIDToAssetPath(m)));
            if (paths.Count > 0)
                obj = AssetDatabase.LoadAssetAtPath(paths[0], typeof(Object));
            return obj;
        }

        public static List<Object> FindAssetsByDir(string TypeName, string path = "Assets")
        {
            List<Object> objList = new List<Object>();
            string[] guids = AssetDatabase.FindAssets($"t:{TypeName}", new string[] { path });
            List<string> paths = new List<string>();
            new List<string>(guids).ForEach(m => paths.Add(AssetDatabase.GUIDToAssetPath(m)));
            for (int i = 0; i < paths.Count; i++)
            {
                objList.Add(AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object)));
            }
            return objList;
        }



        public static ObjectUsing GetOrNewSO(string path)
        {
            ObjectUsing objectUsing = AssetDatabase.LoadAssetAtPath<ObjectUsing>(path);
            if (objectUsing == null)
            {
                if (!Directory.Exists(path))
                {
                    Debug.Log($"yns  CreateDirectory {path}");
                    Directory.CreateDirectory(path);
                }

                var newObject = ScriptableObject.CreateInstance<ObjectUsing>();
                AssetDatabase.CreateAsset(newObject, path);
                AssetDatabase.Refresh();
                objectUsing = AssetDatabase.LoadAssetAtPath<ObjectUsing>(path);
                Debug.Log($"yns Creat");
            }
            return objectUsing;
        }

        public static ScriptableObject GetOrNewSO<T>(string path) where T : ScriptableObject
        {
            ScriptableObject objectUsing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (objectUsing == null)
            {
                var newObject = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(newObject, path);
                AssetDatabase.Refresh();
                objectUsing = AssetDatabase.LoadAssetAtPath<T>(path);
                Debug.Log($"yns Creat");
            }
            return objectUsing;
        }
    }

    public static class XiaocaoPathTool
    {
        public static string FindXicaoDirectory()
        {
            string[] directories = Directory.GetDirectories("Assets", "XiaoCaoTools", SearchOption.AllDirectories);
            return directories.Length > 0 ? directories[0] : string.Empty;
        }
    }
}
