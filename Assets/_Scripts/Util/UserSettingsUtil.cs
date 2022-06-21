using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class UserSettingsUtil
{
    private static string SettingsDirectory { get { return Application.persistentDataPath + "/Settings"; } }
    private static string UserSettingsFilepath { get { return string.Format("{0}/{1}", SettingsDirectory, "userSettings.json"); } }

    public static UserSettings ImportUserSettings()
    {
        var userSettings = ScriptableObject.CreateInstance<UserSettings>();
        string settingsJson = null;
        if (!Directory.Exists(SettingsDirectory))
            Directory.CreateDirectory(SettingsDirectory);
        if(File.Exists(UserSettingsFilepath))
            settingsJson = File.ReadAllText(UserSettingsFilepath);

        if (settingsJson is null)
        {
            Debug.Log("No settings file found.");
            return null;
        }
        
        JsonUtility.FromJsonOverwrite(settingsJson, userSettings);

        return userSettings;
    }

    public static void SaveUserSettings(UserSettings userSettings)
    {
        var json = JsonUtility.ToJson(userSettings);
        if (!Directory.Exists(SettingsDirectory))
            Directory.CreateDirectory(SettingsDirectory);
        File.WriteAllText(UserSettingsFilepath, json);
        Debug.Log("User settings json saved.");
    }

    /// <summary>
    /// Delete all saved app files
    /// </summary>
    public static void ClearAppData()
    {
        ClearSettings();
    }

    public static void ClearSettings()
    {
        if (Directory.Exists(SettingsDirectory) && File.Exists(UserSettingsFilepath))
            File.Delete(UserSettingsFilepath);

        Debug.Log("User settings cleared!");
    }
}
