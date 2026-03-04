using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tools
{
    public static class SaveAndLoadEditorFiles
    {

        public static void SaveToDisk<T>(T saveData, string path)
        {

            string json = JsonUtility.ToJson(saveData, true);
            System.IO.File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }

        public static T LoadFromDisk<T>(string path) where T : new()
        {
            if (!System.IO.File.Exists(path))
            {
                return new T();
            }

            string json = System.IO.File.ReadAllText(path);
            var data = JsonUtility.FromJson<T>(json);
            return data;
        }
    }
}
