using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JsonSaver : MonoBehaviour {
    const string folder = "saves";
    const string extension = ".json";

    static readonly int[] breakingMinorVersions = new int[] {

    };

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

        bool minorVersionsCompatible = true;
        int saveMinorVersion = int.Parse(saveVersion[1]);
        int currentMinorVersion = int.Parse(currentVersion[1]);
        if (currentMinorVersion > saveMinorVersion) {
            foreach (int i in breakingMinorVersions) {
                if (saveMinorVersion <= i && currentMinorVersion > i) {
                    minorVersionsCompatible = false;
                    break;
                }
            }
        }

        return saveVersion[0].Equals(currentVersion[0]) && minorVersionsCompatible;
    }
}
