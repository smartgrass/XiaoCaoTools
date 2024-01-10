using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GG.Extensions
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Generate a texture from base64 string
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Texture2D CreateTextureFromBase64(string base64, string name = "")
        {
            byte[] data = Convert.FromBase64String(base64);
            Texture2D tex = new Texture2D(16, 16, TextureFormat.ARGB32, false, true) {hideFlags = HideFlags.HideAndDontSave, name = name, filterMode = FilterMode.Bilinear};
            tex.LoadImage(data);
            return tex;
        }
        public static Sprite CreateSpriteFromBase64(string base64)
        {
            Texture2D tex = CreateTextureFromBase64(base64);
            return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        /// <summary>
        /// Creates a runtime sprite from a texture2D
        /// </summary>
        public static Sprite CreateSprite(this Texture2D t)
        {
            Rect r = new Rect(0, 0, t.width, t.height);
            Sprite s = Sprite.Create(t, r, Vector2.zero);
            return s;
        }
        /// <summary>
        /// convert a texture to base 64 string
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        public static string ToBase64Image(Texture2D texture2D)
        {
            byte[] imageData = texture2D.EncodeToPNG();
            return Convert.ToBase64String(imageData);
        }

        /// <summary>
        /// Convert image file to base64 string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToBase64Image(string path)
        {
            byte[] asBytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(asBytes);
        }

        public static byte[] LoadImageAsBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}
