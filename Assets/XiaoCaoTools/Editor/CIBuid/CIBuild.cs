
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

//namespace XiaoCao
//{
    public static class CIBuild
    {
        [MenuItem("Tools/Build")]
        public static void Build()
        {
            Debug.Log($"yns Build");
            ProjectBuildExecute(BuildTarget.StandaloneWindows);
        }


        public static void ProjectBuildExecute(BuildTarget target)
        {
            //Switch Platform
            SwitchPlatform(target);

            var scenes = GetScenes().ToArray();

            BuildReport report = null;
            if (target == BuildTarget.iOS)
            {
                report = BuildPipeline.BuildPlayer(scenes, "Builds/iOS", BuildTarget.iOS, BuildOptions.None);
            }
            if (target == BuildTarget.Android)
            {
                 report = BuildPipeline.BuildPlayer(scenes, "Builds/Android", BuildTarget.Android, BuildOptions.None);
            }
            else
            {
                report =BuildPipeline.BuildPlayer(scenes, "Builds/Win/win.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
            }


            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError("打包失败。(" + report.summary.ToString() + ")");
            }

        }

        private static void SwitchPlatform(BuildTarget target)
        {
            if (EditorUserBuildSettings.activeBuildTarget != target)
            {
                if (target == BuildTarget.iOS)
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                }
                if (target == BuildTarget.Android)
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                }
                else
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
                }
            }
        }

        private static List<string> GetScenes()
        {
            List<string> scenes = new List<string>();
            foreach (UnityEditor.EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    if (System.IO.File.Exists(scene.path))
                    {
                        Debug.Log("Add Scene (" + scene.path + ")");
                        scenes.Add(scene.path);
                    }
                }
            }
            return scenes;
        }




    }
//}
