using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
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

        public static Object FindAssetByName(string nameStr, string typeName, string path = "Assets")
        {
            Object obj = null;
            string[] guids = AssetDatabase.FindAssets($"{nameStr} t:{typeName}", new string[] { path });
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
    
    public static class XCEditorTool_TMP
    {

        [MenuItem("Assets/Check/TextToTmp")]
        private static void OnTextToTmp()
        {
            foreach (var item in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(item);
                Text2TextMeshPro(path);
            }
        }

        static void Text2TextMeshPro(string path)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            if (root)
            {
                Text[] list = root.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < list.Length; i++)
                {
                    Text text = list[i];
                    Transform target = text.transform;
                    Vector2 size = text.rectTransform.sizeDelta;
                    string strContent = text.text;
                    Color color = text.color;
                    int fontSize = text.fontSize;
                    FontStyle fontStyle = text.fontStyle;
                    TextAnchor textAnchor = text.alignment;
                    bool richText = text.supportRichText;

                    HorizontalWrapMode horizontalWrapMode = text.horizontalOverflow;
                    VerticalWrapMode verticalWrapMode = text.verticalOverflow;
                    bool raycastTarget = text.raycastTarget;
                    bool isBestFit = text.resizeTextForBestFit;
                    int minSize = text.resizeTextMinSize;
                    int maxSize = text.resizeTextMaxSize;
                    GameObject.DestroyImmediate(text);

                    TextMeshProUGUI textMeshPro = target.gameObject.AddComponent<TextMeshProUGUI>();
                    ComponentUtility.MoveComponentUp(textMeshPro); //上移

                    textMeshPro.rectTransform.sizeDelta = size;
                    textMeshPro.text = strContent;
                    textMeshPro.color = color;
                    textMeshPro.fontSize = fontSize;
                    textMeshPro.enableAutoSizing = isBestFit;
                    textMeshPro.fontSizeMax = maxSize;
                    textMeshPro.fontSizeMin = minSize;



                    textMeshPro.fontStyle = fontStyle == FontStyle.BoldAndItalic ? FontStyles.Bold : (FontStyles)fontStyle;
                    switch (textAnchor)
                    {
                        case TextAnchor.UpperLeft:
                            textMeshPro.alignment = TextAlignmentOptions.TopLeft;
                            break;
                        case TextAnchor.UpperCenter:
                            textMeshPro.alignment = TextAlignmentOptions.Top;
                            break;
                        case TextAnchor.UpperRight:
                            textMeshPro.alignment = TextAlignmentOptions.TopRight;
                            break;
                        case TextAnchor.MiddleLeft:
                            textMeshPro.alignment = TextAlignmentOptions.MidlineLeft;
                            break;
                        case TextAnchor.MiddleCenter:
                            textMeshPro.alignment = TextAlignmentOptions.Midline;
                            break;
                        case TextAnchor.MiddleRight:
                            textMeshPro.alignment = TextAlignmentOptions.MidlineRight;
                            break;
                        case TextAnchor.LowerLeft:
                            textMeshPro.alignment = TextAlignmentOptions.BottomLeft;
                            break;
                        case TextAnchor.LowerCenter:
                            textMeshPro.alignment = TextAlignmentOptions.Bottom;
                            break;
                        case TextAnchor.LowerRight:
                            textMeshPro.alignment = TextAlignmentOptions.BottomRight;
                            break;
                    }
                    textMeshPro.richText = richText;
                    if (verticalWrapMode == VerticalWrapMode.Overflow)
                    {
                        textMeshPro.enableWordWrapping = true;
                        textMeshPro.overflowMode = TextOverflowModes.Overflow;
                    }
                    else
                    {
                        textMeshPro.enableWordWrapping = horizontalWrapMode == HorizontalWrapMode.Overflow ? false : true;
                    }
                    textMeshPro.raycastTarget = raycastTarget;
                }
            }
            PrefabUtility.SaveAsPrefabAsset(root, path, out bool success);
            if (!success)
            {
                Debug.LogError($"预制体：{path} 保存失败!");
            }
            else
            {
                Debug.Log($"预制体：{path} 保存成功!");
            }
        }


        [MenuItem("GameObject/XiaoCao/替换子物体TMP字体", priority = 0)]
        private static void CheckEnTMPFont()
        {
            string fontPath = "Assets/RawResources/Font/Aldrich-Regular SDF.asset";
            
            Transform tf = Selection.activeTransform;
            var tmps = tf.GetComponentsInChildren<TMP_Text>(true);

            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
            if (font == null)
            {
                Debug.Log("没有找到字体");
                return;
            }

            foreach (var item in tmps)
            {
                if (item.font!= font)
                {
                    item.font = font;
                    EditorUtility.SetDirty(item);
                }
            }
        }

    }
    
}
