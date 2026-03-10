
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Data
{
    public static class SaveAndLoadConfigurationDataFiles
    {

        public static void SaveToDisk<T>(T saveData, string path)
        {
            var json = JsonConvert.SerializeObject(saveData, Formatting.Indented);

            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }

        public static T LoadFromDisk<T>(string path) where T : new()
        {
            if (!File.Exists(path))
            {
                return new T();
            }

            string json = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogWarning($"File at {path} is empty. Returning new {typeof(T).Name}.");
                return new T();
            }

            try
            {
                var data = JsonConvert.DeserializeObject<T>(json);

                if (data == null)
                {
                    Debug.LogWarning($"Deserialization returned null for {typeof(T).Name}. Returning new instance.");
                    return new T();
                }

                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to deserialize {typeof(T).Name} from {path}: {ex.Message}");
                return new T();
            }
        }
    }
}
