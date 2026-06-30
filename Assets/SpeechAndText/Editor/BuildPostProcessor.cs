#if UNITY_IOS && UNITY_EDITOR
// Modified BuildPostProcessor for iOS Speech.framework setup
// Works across Unity 2019.3 and newer

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class BuildPostProcessor
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS) return;

        // --- Load Xcode project
        string projectPath = PBXProject.GetPBXProjectPath(path);
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);

#if UNITY_2019_3_OR_NEWER
        string mainTargetGuid = project.GetUnityMainTargetGuid();
        string frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
#else
        string targetName = PBXProject.GetUnityTargetName();
        string mainTargetGuid = project.TargetGuidByName(targetName);
        string frameworkTargetGuid = mainTargetGuid;
#endif

        // --- Add frameworks
        AddFrameworks(project, frameworkTargetGuid);

        // --- Add Speech recognition usage description
        string plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        plist.root.SetString("NSSpeechRecognitionUsageDescription",
            "This app needs access to Speech Recognition");
        plist.WriteToFile(plistPath);

        // --- Save Xcode project
        project.WriteToFile(projectPath);
    }

    static void AddFrameworks(PBXProject project, string targetGuid)
    {
        // Add Speech.framework
        project.AddFrameworkToProject(targetGuid, "Speech.framework", false);

        // Ensure ObjC categories link correctly
        project.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
    }
}
#endif