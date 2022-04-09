using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetObserver:AssetPostprocessor
{
    private const string COMMAND_NAME = "Tool/Create/Audio Enum";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetsPath)
    {
        List<string[]> assetsList = new List<string[]>()
        {
            importedAssets,deletedAssets,movedAssets,movedFromAssetsPath
        };

        List<string> targetDirectoryNameList = new List<string>()
        {
            "Assets/Sound/SE",
            "Assets/Sound/BGM",
        };

        string[] directorySENames = Directory.GetDirectories("Assets/Sound/SE", "*", System.IO.SearchOption.AllDirectories);
        string[] directoryBGMNames = Directory.GetDirectories("Assets/Sound/BGM", "*", System.IO.SearchOption.AllDirectories);
        foreach(string name in directorySENames)
        {
            targetDirectoryNameList.Add(name);
        }
        foreach(string name in directoryBGMNames)
        {
            targetDirectoryNameList.Add(name);
        }

        if (ExistsDirectoryInAssets(assetsList, targetDirectoryNameList))
        {
            Create();
        }
    }

    [MenuItem(COMMAND_NAME)]
    private static void Create()
    {
        List<AudioClip> ses= new List<AudioClip>();
        List<AudioClip> intros = new List<AudioClip>();
        List<AudioClip> loops = new List<AudioClip>();
        //SE
        List<string> seList = new List<string>();
        string[] seNames = Directory.GetFiles("Assets/Sound/SE","*",System.IO.SearchOption.AllDirectories);
        foreach (var name in seNames)
        {
            string capitalName = name.ToUpper();
            if ((capitalName.Contains(".MP3")||capitalName.Contains(".WAV"))&&!capitalName.Contains(".META"))
            {
                string newName = capitalName.Substring(name.LastIndexOf("/")+1).Replace(".WAV","").Replace(".MP3","");
                seList.Add(newName);
                ses.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(name));
            }
        }
        EnumCreator.Create("SE_Enum","Assets/Sound/Scripts/SE_Enum.cs",seList);

        //BGM
        List<string> bgmList = new List<string>();
        string[] bgmNames = Directory.GetFiles("Assets/Sound/BGM/Loop", "*", System.IO.SearchOption.AllDirectories);
        foreach(var name in bgmNames)
        {
            string capitalName = name.ToUpper();
            if ((capitalName.Contains("MP3")||capitalName.Contains("WAV")) && !capitalName.Contains(".META"))
            {
                string newName= capitalName.Substring(name.LastIndexOf("/")+1).Replace(".WAV", "").Replace(".MP3", "").Replace("LOOP","").ToUpper();
                bgmList.Add(newName);
                loops.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(name));
            }
        }
        EnumCreator.Create("BGM_Enum", "Assets/Sound/Scripts/BGM_Enum.cs", bgmList);

        string[] bgmNames2 = Directory.GetFiles("Assets/Sound/BGM/Intro","*", System.IO.SearchOption.AllDirectories);
        foreach(var name in bgmNames2)
        {
            string capitalName = name.ToUpper();
            if ((capitalName.Contains(".MP3") || capitalName.Contains(".WAV")) && !capitalName.Contains(".META"))
            {
                intros.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(name));
            }
        }

        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            if (obj.tag == "SoundManager")
            {
                SoundManager soundManager = obj.GetComponent<SoundManager>();
                if (soundManager != null)
                {
                    soundManager.SetAudioClips(ses.ToArray(), intros.ToArray(), loops.ToArray());
                }
            }
        }
    }

    private static bool ExistsDirectoryInAssets(List<string[]> assetList,List<string> targetDirectoryNameList)
    {
        return assetList
            .Any(assets => assets
            .Select(asset => System.IO.Path.GetDirectoryName(asset))
            .Intersect(targetDirectoryNameList)
            .Count() > 0);
    }
}
