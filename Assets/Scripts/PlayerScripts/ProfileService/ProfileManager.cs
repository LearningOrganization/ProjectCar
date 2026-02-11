using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using UnityEngine;

public class ProfileManager
{
    public List<string> Profiles = new List<string>();
    public Profile CurrentProfile;

    private string _basePath => Path.Combine(Application.persistentDataPath, "Profiles");
    private string _profilesPath => Path.Combine(Application.persistentDataPath, "profiles.json");

    public ProfileManager()
    {
        if(!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }

        LoadProfiles();
    }

    public bool CreateProfile(string name)
    {
        if(Profiles.Contains(name))
        {
            Debug.Log($"Profile name: {name} already exists");
            return false;
        }

        Profiles.Add(name);
        SaveProfiles();

        Directory.CreateDirectory(Path.Combine(_basePath, name));

        //create start state of profile
        Profile profile = new Profile(name);
        CurrentProfile = profile;

        // should be replaced in future for general saving
        SaveWallet(profile);
        return true;
    }

    public bool LoadProfile(string name)
    {
        if(!Profiles.Contains(name))
        {
            Debug.Log($"Profile {name} was not found");
            return false;
        }

        string profilePath = Path.Combine(_basePath, name);
        if(!Directory.Exists(profilePath))
        {
            Debug.LogWarning("Profile folder missing, creating: " + profilePath);
            Directory.CreateDirectory(profilePath);
        }

        Profile profile = new Profile(name);
        // should also be replaced for general profile loading  
        LoadWallet(profile);
        CurrentProfile = profile;

        return true;
    }

    // for now only wallet, but in the future it will be for all components of player profile
    private void SaveWallet(Profile profile)
    {
        string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string profilePath = Path.Combine(_basePath, profile.ProfileName);
        string walletJsonPath = Path.Combine(profilePath, "wallet.json");

        string zipPathName = $"save_{timeStamp}.zip";

        string zipPath = Path.Combine(profilePath, zipPathName);

        profile.Wallet.SaveWallet(walletJsonPath);

        if (File.Exists(zipPath)) File.Delete(zipPath);
        using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
        {
            archive.CreateEntryFromFile(walletJsonPath, "wallet.json");
        }

        File.Delete(walletJsonPath);
        Debug.Log("Wallet saved to zip: " + zipPath);
    }

    // for now only wallet, but in the future it will be for all components of player profile
    private void LoadWallet(Profile profile)
    {
        string profilePath = Path.Combine(_basePath, profile.ProfileName);

        if(!Directory.Exists(profilePath))
        {
            Debug.LogWarning("Profile folder not found, creating new: " + profilePath);
            Directory.CreateDirectory(profilePath);
            SaveWallet(profile); // first save
            return;
        }

        var files = Directory.GetFiles(profilePath, "save_*.zip");
        if(files.Length == 0)
        {
            Debug.LogWarning("No saves found, creating new...");
            SaveWallet(profile);
            return;
        }

        Array.Sort(files);
        string latestZip = files[files.Length - 1];

        string tempjJson = Path.Combine(profilePath, "temp_save,zip");

        try
        {
            using (var achive = ZipFile.OpenRead(latestZip))
            {
                var entry = achive.GetEntry("wallet.json");
                if(entry != null)
                {
                    entry.ExtractToFile(tempjJson, true);
                    profile.Wallet.LoadWallet(tempjJson);
                    File.Delete(tempjJson);
                    Debug.Log("Wallet loaded from: " + latestZip);

                }
                else
                {
                    Debug.LogWarning("wallet.json not found in zip: " + latestZip);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to load wallet: " + ex.Message);
        }
    }

    private void LoadProfiles()
    {
        if(!File.Exists(_profilesPath))
        {
            File.WriteAllText(_profilesPath, "[]");
            Profiles = new List<string>();
        }
        else
        {
            string json = File.ReadAllText(_profilesPath);
            Profiles = JsonUtility.FromJson<ProfileListWrapper>(json).Profiles;
        }
    }

    private void SaveProfiles()
    {
        string json = JsonUtility.ToJson(new ProfileListWrapper {Profiles = Profiles}, true);
        File.WriteAllText(_profilesPath, json);
    }

    [Serializable]
    private class ProfileListWrapper
    {
        public List<string> Profiles = new List<string>();
    }

}