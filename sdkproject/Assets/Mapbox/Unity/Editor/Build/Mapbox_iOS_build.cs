#if UNITY_IOS
namespace Mapbox.Editor.Build
{
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Globalization;

	public static class Mapbox_iOS_build
	{
		private const string defaultIncludePath = "Mapbox/Core/Plugins/iOS/MapboxMobileEvents/include";

		[PostProcessBuild]
		public static void AppendBuildProperty(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget == BuildTarget.iOS)
			{
				PBXProject proj = new PBXProject();
				// path to pbxproj file
				var projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

				var file = File.ReadAllText(projPath);
				proj.ReadFromString(file);
#if UNITY_2019_1_OR_NEWER
				string target = proj.GetUnityFrameworkTargetGuid();
#else
				string target = proj.TargetGuidByName("Unity-iPhone");
#endif

				var includePaths =
					Directory.GetDirectories(Application.dataPath, "include", SearchOption.AllDirectories);
				var includePath = includePaths
					.Select(path => Regex.Replace(path, Application.dataPath + "/", ""))
					.Where(path => path.EndsWith(defaultIncludePath, true, CultureInfo.InvariantCulture))
					.DefaultIfEmpty(defaultIncludePath)
					.First();

				proj.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/" + includePath);
				proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC -lz");

				File.WriteAllText(projPath, proj.WriteToString());
			}
		}

		public static void BuildProject()
		{
			var args = System.Environment.GetCommandLineArgs();
			var path = args[0];

			var activeScenePaths = new string[SceneManager.sceneCount];
			for (var i = 0; i < activeScenePaths.Length; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (scene.IsValid())
					activeScenePaths[i] = scene.path;
			}

			var options = new BuildPlayerOptions
			{
				scenes = activeScenePaths,
				target = BuildTarget.iOS,
				locationPathName = path
			};

			BuildPipeline.BuildPlayer(options);
			EditorApplication.Exit(0);
		}
	}
}
#endif
