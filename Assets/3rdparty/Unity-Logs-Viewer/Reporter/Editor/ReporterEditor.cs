using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System.IO;
using System.Collections;
using System.Globalization;


public class ReporterEditor : Editor
{
    [MenuItem("Reporter/Create")]
    public static void CreateReporter()
    {
        const int reporterExecOrder = -12000;
        var reporterObj = new GameObject { name = "Reporter" };
        var reporter = reporterObj.AddComponent<Reporter>();
        reporterObj.AddComponent<ReporterMessageReceiver>();
        //reporterObj.AddComponent<TestReporter>();

        // Register root object for undo.
        Undo.RegisterCreatedObjectUndo(reporterObj, "Create Reporter Object");

        var reporterScript = MonoScript.FromMonoBehaviour(reporter);
        var reporterPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(reporterScript));

        if (MonoImporter.GetExecutionOrder(reporterScript) != reporterExecOrder)
        {
            MonoImporter.SetExecutionOrder(reporterScript, reporterExecOrder);
            //Debug.Log("Fixing exec order for " + reporterScript.name);
        }

        reporter.Images = new Images
        {
            ClearImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/clear.png"),
                    typeof(Texture2D)),
            CollapseImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/collapse.png"),
                    typeof(Texture2D)),
            ClearOnNewSceneImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/clearOnSceneLoaded.png"),
                    typeof(Texture2D)),
            ShowTimeImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/timer_1.png"),
                    typeof(Texture2D)),
            ShowSceneImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/UnityIcon.png"),
                    typeof(Texture2D)),
            UserImage = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/user.png"),
                typeof(Texture2D)),
            ShowMemoryImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/memory.png"),
                    typeof(Texture2D)),
            SoftwareImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/software.png"),
                    typeof(Texture2D)),
            DateImage = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/date.png"),
                typeof(Texture2D)),
            ShowFpsImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/fps.png"), typeof(Texture2D)),
            InfoImage = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/info.png"),
                typeof(Texture2D)),
            SearchImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/search.png"),
                    typeof(Texture2D)),
            CloseImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/close.png"),
                    typeof(Texture2D)),
            BuildFromImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/buildFrom.png"),
                    typeof(Texture2D)),
            SystemInfoImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/ComputerIcon.png"),
                    typeof(Texture2D)),
            GraphicsInfoImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/graphicCard.png"),
                    typeof(Texture2D)),
            BackImage = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/back.png"),
                typeof(Texture2D)),
            LogImage = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/log_icon.png"),
                typeof(Texture2D)),
            WarningImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/warning_icon.png"),
                    typeof(Texture2D)),
            ErrorImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/error_icon.png"),
                    typeof(Texture2D)),
            BarImage = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/bar.png"),
                typeof(Texture2D)),
            ButtonActiveImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/button_active.png"),
                    typeof(Texture2D)),
            EvenLogImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/even_log.png"),
                    typeof(Texture2D)),
            OddLogImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/odd_log.png"),
                    typeof(Texture2D)),
            SelectedImage =
                (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/selected.png"),
                    typeof(Texture2D)),
            ReporterScrollerSkin =
                (GUISkin)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/reporterScrollerSkin.guiskin"),
                    typeof(GUISkin))
        };
        //reporter.images.graphImage           = (Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(reporterPath, "Images/chart.png"), typeof(Texture2D));

    }
}

public class ReporterModificationProcessor : UnityEditor.AssetModificationProcessor
{
    [InitializeOnLoad]
    public class BuildInfo
    {
        static BuildInfo()
        {
            EditorApplication.update += Update;
        }

        private static bool _isCompiling = true;

        private static void Update()
        {
            if (!EditorApplication.isCompiling && _isCompiling)
            {
                //Debug.Log("Finish Compile");
                if (!Directory.Exists(Application.dataPath + "/StreamingAssets"))
                {
                    Directory.CreateDirectory(Application.dataPath + "/StreamingAssets");
                }
                var infoPath = Application.dataPath + "/StreamingAssets/build_info.txt";
                var buildInfo = new StreamWriter(infoPath);
                buildInfo.Write("Build from " + SystemInfo.deviceName + " at " + System.DateTime.Now.ToString(CultureInfo.InvariantCulture));
                buildInfo.Close();
            }

            _isCompiling = EditorApplication.isCompiling;
        }
    }
}
