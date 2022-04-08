using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public  class EnumCreator
{ 
    private static string _code = "";
    private static string _tab = "";
    private static void Init(string enumName)
    {
        _code = "";
        _tab = "";

        _code +=  _tab +"public enum " +enumName+"\n{\n";

        _tab += "\t";
    }

    private static void Export(string exportPath,string enumName)
    {
        _code += "}";

        File.WriteAllText(exportPath, _code, Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
    }

    public static void Create(string enumName,string exportPath,List<string> itemNameList)
    {
        Init(enumName);

        for(int i = 0; i < itemNameList.Count; i++)
        {
            _code += _tab + itemNameList[i]+","+"\n";
        }

        Export(exportPath, enumName);
    }
}
