using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BinarySaver : MonoBehaviour {
    const string folder = "saves";
    const string extension = ".json";

    static BinaryFormatter binaryFormatter = new BinaryFormatter();

    void OnEnable() {
        if (!Directory.Exists(GetFolderPath())) {
            Directory.CreateDirectory(GetFolderPath());
        }
    }

    public static void SaveFile(Save save, int slot) {
        save.BeforeSerialize();

        using (StreamWriter jsonWriter = new StreamWriter(GetSavePath(slot), append: false)) {
            jsonWriter.Write(JsonConvert.SerializeObject(save, Formatting.Indented));
        }
    }

    public static Save LoadFile(int slot) {
        Save save;
        using (StreamReader r = new StreamReader(GetSavePath(slot))) {
            string fileJson = r.ReadToEnd();
            save = (Save) JsonConvert.DeserializeObject<Save>(fileJson);
        }
        save.AfterDeserialize();
        return save;
    }

    public static bool HasFile(int slot) {
        if (!File.Exists(GetSavePath(slot))) return false;
        try {
            if (!CompatibleVersions(slot)) {
                return false;
            }
            return true;
        } catch (Exception) {
            // deal with legacy saves/changed formats
            return false;
        }
    }

    static string GetFolderPath() {
        return Path.Combine(Application.persistentDataPath, folder);
    }
    
    static string GetSavePath(int slot) {
        return Path.Combine(GetFolderPath(), slot+extension);
    }

    public static bool CompatibleVersions(int saveSlot) {
        string version;

        using (StreamReader r = new StreamReader(GetSavePath(saveSlot))) {
            string fileJson = r.ReadToEnd();
            version = JObject.Parse(fileJson)["version"].ToString();
        }

        string[] saveVersion = version.Split('.');
        string[] currentVersion = Application.version.Split('.');

        return saveVersion[0].Equals(currentVersion[0]) && saveVersion[1].Equals(currentVersion[1]);
    }
}
