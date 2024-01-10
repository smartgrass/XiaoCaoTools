using System.Collections.Generic;
using UnityEngine;

namespace GG.Extensions
{
    public static class CameraExtensions
    {
        public static void LayerCullingShow(this Camera cam, int layerMask) {
            cam.cullingMask |= layerMask;
        }

        public static void LayerCullingShow(this Camera cam, string layer) {
            LayerCullingShow(cam, 1 << LayerMask.NameToLayer(layer));
        }
        
        public static void LayerCullingShow(this Camera camera, params string[] layerNames)
        {
            foreach (string layerName in layerNames)
            {
                LayerCullingShow(camera,layerName);
            }
        }

        public static void LayerCullingHide(this Camera cam, int layerMask) {
            cam.cullingMask &= ~layerMask;
        }

        public static void LayerCullingHide(this Camera cam, string layer) {
            LayerCullingHide(cam, 1 << LayerMask.NameToLayer(layer));
        }
        
        public static void LayerCullingHide(this Camera camera, params string[] layerNames)
        {
            foreach (string layerName in layerNames)
            {
                LayerCullingHide(camera,layerName);
            }
        }

        public static void LayerCullingToggle(this Camera cam, int layerMask) {
            cam.cullingMask ^= layerMask;
        }

        public static void LayerCullingToggle(this Camera cam, string layer) {
            LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer));
        }

        public static bool LayerCullingIncludes(this Camera cam, int layerMask) {
            return (cam.cullingMask & layerMask) > 0;
        }

        public static bool LayerCullingIncludes(this Camera cam, string layer) {
            return LayerCullingIncludes(cam, 1 << LayerMask.NameToLayer(layer));
        }

        public static void LayerCullingToggle(this Camera cam, int layerMask, bool isOn) {
            bool included = LayerCullingIncludes(cam, layerMask);
            if (isOn && !included) {
                LayerCullingShow(cam, layerMask);
            } else if (!isOn && included) {
                LayerCullingHide(cam, layerMask);
            }
        }

        public static void LayerCullingToggle(this Camera cam, string layer, bool isOn) {
            LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer), isOn);
        }

        public static void SetCullingMask(this Camera cam, List<string> layers)
        {
            cam.cullingMask = 0;
            foreach (string layer in layers)
            {
                cam.LayerCullingShow(layer);
            }
        }
        
        public static void SetCullingMask(this Camera cam, string layer)
        {
            cam.cullingMask = 0;
            cam.LayerCullingShow(layer);
        }
    }
}