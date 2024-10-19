using UnityEditor;
using UnityEngine;
using System.IO;

public class ShaderConverter : EditorWindow
{
    private Shader shaderToConvert;

    [MenuItem("Tools/Shader Converter")]
    public static void ShowWindow()
    {
        GetWindow<ShaderConverter>("Shader Converter");
    }

    private void OnGUI()
    {
        // 选择指定的 Shader
        shaderToConvert =
            (Shader)EditorGUILayout.ObjectField("Shader to Convert", shaderToConvert, typeof(Shader), false);

        if (GUILayout.Button("Convert Selected Shader to URP"))
        {
            if (shaderToConvert != null)
            {
                ConvertShader(AssetDatabase.GetAssetPath(shaderToConvert));
            }
            else
            {
                Debug.LogError("No Shader selected for conversion.");
            }
        }
    }

    private static void ConvertShader(string shaderPath)
    {
        if (!File.Exists(shaderPath))
        {
            Debug.LogError("Shader file not found: " + shaderPath);
            return;
        }

        string shaderText = File.ReadAllText(shaderPath);

        // Example replacement: Replace Built-in lighting with URP lighting


        shaderText = shaderText.Replace("ForwardBase", "UniversalForward");
        shaderText = shaderText.Replace("CGINCLUDE", "HLSLINCLUDE");
        shaderText = shaderText.Replace("ENDCG", "ENDHLSL");
        shaderText = shaderText.Replace("CGPROGRAM", "HLSLPROGRAM");
        shaderText = shaderText.Replace("fixed", "half");

        // Replace built-in include with URP includes
        shaderText = shaderText.Replace("#include \"UnityCG.cginc\"",
            "#include \"Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl\"\n" +
            "#include \"Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl\"");

        // Replace vertex transformation
        shaderText = shaderText.Replace("UnityObjectToClipPos",
            "TransformObjectToHClip");

        // Replace screen position calculation
        shaderText = shaderText.Replace("ComputeGrabScreenPos(o.pos)",
            "ComputeScreenPos(posInputs.positionCS)");

        // Replace view direction calculation
        shaderText = shaderText.Replace("UnityWorldSpaceViewDir(worldPos)",
            "_WorldSpaceCameraPos.xyz - worldPos");

        // Replace normal map unpacking
        shaderText = shaderText.Replace("UnpackNormal(tex2D(_BumpMap, i.uv.zw))",
            "UnpackNormal(tex2D(_BumpMap, i.uv.zw))");

        // Replace reflect direction and texture cube sampling
        shaderText = shaderText.Replace("reflect(-worldViewDir, bump)",
            "reflect(-worldViewDir, bump)");

        shaderText = shaderText.Replace("texCUBE(_Cubemap, reflDir).rgb * texColor.rgb",
            "texCUBE(_Cubemap, reflDir).rgb * texColor.rgb");

        // Handle the removal of GrabPass
        shaderText = shaderText.Replace("GrabPass { \"_RefractionTex\" }",
            "// GrabPass removed. Use _CameraOpaqueTexture instead if needed.");

        // Other necessary replacements for URP compatibility
        // Add more replacements here...

        File.WriteAllText(shaderPath, shaderText);
        AssetDatabase.ImportAsset(shaderPath);

        Debug.Log("Shader converted to URP: " + shaderPath);
    }
}