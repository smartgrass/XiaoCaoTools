using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GG.Extensions
{
	public static class GameObjectExtensions
	{
		static List<GameObject> dontDestoryOnLoadObjects = new List<GameObject>();
		
		/// <summary>
		/// Gets or add a component. Usage example:
		/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
		/// </summary>
		static public T GetOrAddComponent<T>(this Component child) where T : Component
		{
			T result = child.GetComponent<T>();
			if (result == null)
			{
				result = child.gameObject.AddComponent<T>();
			}
			return result;
		}
	
		/// <summary>
		/// Gets or add a component. Usage example:
		/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
		/// </summary>
		/// <param name="child"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		static public T GetOrAddComponent<T>(this GameObject child) where T : Component
		{
			return GetOrAddComponent<T>(child.transform);
		}
		
		public static void ChangeMaterial(this GameObject go, Material newMat)
		{
			Renderer[] children = go.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer rend in children)
			{
				Material[] mats = new Material[rend.materials.Length];
				for (int j = 0; j < rend.materials.Length; j++)
				{
					mats[j] = newMat;
				}
				rend.materials = mats;
			}
		}
		
		public static T GetComponentInParentIgnoreSelf<T>(this GameObject go)
		{
			return go.transform.parent.GetComponentInParent<T>();
		}
		
		/// <summary>
		/// Use this for dont destory on load to keep referances for the find object of type all
		/// </summary>
		/// <param name="obj"></param>
		public static void DontDestroyOnLoad(this GameObject obj)
		{
			dontDestoryOnLoadObjects.Add(obj);
			Object.DontDestroyOnLoad(obj);
		}
     
		public static void DestoryDontDestroyOnLoad(this GameObject obj)
		{
			dontDestoryOnLoadObjects.Remove(obj); 
			Object.Destroy(obj);
		}
     
		public static List<GameObject> GetDontDestroyOnLoadObjects()
		{
			dontDestoryOnLoadObjects = dontDestoryOnLoadObjects.Where(x => x != null).ToList();
			return new List<GameObject>(dontDestoryOnLoadObjects); 
		}
		
		/// <summary>
		/// Return a list of all child objects
		/// </summary>
		/// <param name="gameObject"></param>
		/// <returns></returns>
		public static List<GameObject> GetAllChildren(this GameObject gameObject)
		{
			Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();
			List<GameObject> allChildren = new List<GameObject>(childTransforms.Length);

			foreach(Transform child in childTransforms)
			{
				if(child.gameObject != gameObject) allChildren.Add(child.gameObject);
			}

			return allChildren;
		}

		/// <summary>
		/// Return a list of all child objects including itself
		/// </summary>
		/// <param name="gameObject"></param>
		/// <returns></returns>
		public static List<GameObject> GetAllChildrenAndSelf(this GameObject gameObject)
		{
			Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();
			List<GameObject> allChildren = new List<GameObject>(childTransforms.Length);

			for (int transformIndex = 0; transformIndex < childTransforms.Length; ++transformIndex)
			{
				allChildren.Add(childTransforms[transformIndex].gameObject);
			}

			return allChildren;
		}
	}
}
