#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GG.Extensions
{
    public static class EditorExtensions
    {
        /// <summary>
        /// Return the location of this script
        /// </summary>
        /// <param name="scriptableObject"></param>
        /// <returns></returns>
        static string GetMonoScriptFilePath(ScriptableObject scriptableObject)
        {
            MonoScript ms   = MonoScript.FromScriptableObject(scriptableObject);

            string filePath = AssetDatabase.GetAssetPath(ms);

            FileInfo fi     = new FileInfo(filePath);

            if (fi.Directory != null)
            {
                filePath = fi.Directory.ToString();

                return filePath.Replace
                (
                    oldChar: '\\',
                    newChar: '/'
                );
            }
            return null;
        }
    }
}
#endif
