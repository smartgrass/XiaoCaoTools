using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GG.Extensions
{
    public static class MiscExtensions
    {
        public delegate void UnityAction<T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


        /// Use this method to get all loaded objects of some type, including inactive objects. 
        /// This is an alternative to Resources.FindObjectsOfTypeAll (returns project assets, including prefabs), and GameObject.FindObjectsOfTypeAll (deprecated).
        public static List<T> FindObjectsOfTypeAll<T>()
        {
            List<T> results = new List<T>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);

                results.AddRange(FindObjectsOfTypeAllInScene<T>(s, false));
            }

            foreach (GameObject savedObject in GameObjectExtensions.GetDontDestroyOnLoadObjects())
            {
                results.AddRange(savedObject.GetComponentsInChildren<T>(true));
            }

            return results;
        }

        public static List<T> FindObjectsOfTypeAllInScene<T>(Scene scene, bool includeDontDestroyOnLoad = true)
        {
            List<T> results = new List<T>();

            var allGameObjects = scene.GetRootGameObjects();
            for (int j = 0; j < allGameObjects.Length; j++)
            {
                var go = allGameObjects[j];
                results.AddRange(go.GetComponentsInChildren<T>(true));
            }

            if (includeDontDestroyOnLoad)
            {
                foreach (GameObject savedObject in GameObjectExtensions.GetDontDestroyOnLoadObjects())
                {
                    results.AddRange(savedObject.GetComponentsInChildren<T>(true));
                }
            }

            return results;
        }

        public static IEnumerator WaitForSceneToLoad(string sceneName, LoadSceneMode loadSceneMode, UnityAction<float> updateAction, UnityAction OnComplete)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            async.allowSceneActivation = false;

            while (async.progress < 0.9f)
            {
                if (updateAction != null)
                {
                    updateAction(async.progress);
                }
                yield return null;
            }

            async.allowSceneActivation = true;

            while (SceneManager.GetSceneByName(sceneName).isLoaded == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (OnComplete != null)
            {
                OnComplete();
            }
        }

        /// <summary>
        /// Clone data from an object into a new version of it
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Clone<T>(T obj)
        {
            DataContractSerializer dcSer = new DataContractSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();

            dcSer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0;

            T newObject = (T)dcSer.ReadObject(memoryStream);
            return newObject;
        }

        /// <summary>
        /// Use Reflection from a source class to an inhearated class
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        public static void CopyAll<T, T2>(this T source, T2 target)
        {
            Type type = typeof(T);
            foreach (FieldInfo sourceField in type.GetFields())
            {
                FieldInfo targetField = type.GetField(sourceField.Name);
                targetField.SetValue(target, sourceField.GetValue(source));
            }
        }

        public static void GetColumnAndRowForVerticalGrid(this GridLayoutGroup glg, out int column, out int row)
        {
            column = 0;
            row = 0;

            if (glg.transform.childCount == 0)
                return;

            //Column and row are now 1
            column = 1;
            row = 1;

            //Get the first child GameObject of the GridLayoutGroup
            RectTransform firstChildObj = glg.transform.GetChild(0).GetComponent<RectTransform>();

            float currentY = firstChildObj.anchoredPosition.y;

            //Loop through the rest of the child object
            for (int i = 1; i < glg.transform.childCount; i++)
            {
                if (!glg.transform.GetChild(i).gameObject.activeSelf)
                {
                    continue;
                }

                //Get the next child
                RectTransform currentChildObj = glg.transform.GetChild(i).GetComponent<RectTransform>();

                Vector2 currentChildPos = currentChildObj.anchoredPosition;

                //if first child.x == otherchild.x, it is a row, else it's a column
                if (Math.Abs(currentY - currentChildPos.y) > Mathf.Epsilon)
                {
                    row++;
                    currentY = currentChildPos.y;
                }
                else
                {
                    column++;
                }
            }
        }

    }
}

