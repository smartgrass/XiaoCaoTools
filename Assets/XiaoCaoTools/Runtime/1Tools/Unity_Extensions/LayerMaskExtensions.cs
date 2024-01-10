#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace GG.Extensions
{
    public static class LayerMaskExtensions
    {
        public static LayerMask CreateLayerMask(params string[] layerNames)
        {
            return NamesToMask(layerNames);
        }

        public static LayerMask CreateLayerMask(params int[] layerNumbers)
        {
            return LayerNumbersToMask(layerNumbers);
        }

        public static LayerMask NamesToMask(params string[] layerNames)
        {
            LayerMask ret = 0;
            foreach (string name in layerNames)
            {
                ret |= 1 << LayerMask.NameToLayer(name);
            }

            return ret;
        }

        public static LayerMask LayerNumbersToMask(params int[] layerNumbers)
        {
            LayerMask ret = 0;
            foreach (int layer in layerNumbers)
            {
                ret |= 1 << layer;
            }

            return ret;
        }

        public static LayerMask Inverse(this LayerMask original)
        {
            return ~original;
        }

        public static LayerMask AddToMask(this LayerMask original, params string[] layerNames)
        {
            return original | NamesToMask(layerNames);
        }

        public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames)
        {
            LayerMask invertedOriginal = ~original;
            return ~(invertedOriginal | NamesToMask(layerNames));
        }

        public static string[] MaskToNames(this LayerMask original)
        {
            List<string> output = new List<string>();

            for (int i = 0; i < 32; ++i)
            {
                int shifted = 1 << i;
                if ((original & shifted) == shifted)
                {
                    string layerName = LayerMask.LayerToName(i);
                    if (!string.IsNullOrEmpty(layerName))
                    {
                        output.Add(layerName);
                    }
                }
            }

            return output.ToArray();
        }

        public static string MaskToString(this LayerMask original)
        {
            return MaskToString(original, ", ");
        }

        public static string MaskToString(this LayerMask original, string delimiter)
        {
            return string.Join(delimiter, MaskToNames(original));
        }

        public static void MoveToLayer(this Transform root, string layer, bool recursive = true)
        {
            MoveToLayer(root, LayerMask.NameToLayer(layer), recursive);
        }

        public static void MoveToLayer(this Transform root, int layer, bool recursive = true)
        {
            root.gameObject.layer = layer;

            if (recursive)
            {
                foreach (Transform child in root)
                {
                    MoveToLayer(child, layer);
                }
            }
        }
        
        public static void MoveToLayer<T>(this Transform root, string layer) where T : Component
        {
            MoveToLayer<T>(root, LayerMask.NameToLayer(layer));
        }
        
        public static void MoveToLayer<T>(this Transform root, int layerNumber) where T : Component
        {
            foreach (T trans in root.GetComponentsInChildren<T>(true))
            {
                trans.gameObject.layer = layerNumber;
            }
        }

        public static bool ContainsLayer(this LayerMask mask, int layer)
        {
            return ((1 << layer) & mask) > 0;
        }

        public static bool ContainsLayer(this LayerMask mask, string layer)
        {
            return ((1 << LayerMask.NameToLayer(layer)) & mask) > 0;
        }
    }
}