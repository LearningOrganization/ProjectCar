using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// public class SaveLoadService : ISaveLoadService
// {
//     private string basePath => Path.Combine(Application.persistentDataPath, "PlayerProfiles");

//     public string GetSavePath(string profileName)
//     {
//         string profilePath = Path.Combine(basePath, profileName);
//         if (!Directory.Exists(profilePath))
//             Directory.CreateDirectory(profilePath);

//         return Path.Combine(profilePath, "wallet.json");
//     }

//     public void SaveWallet(WalletService wallet, string profileName)
//     {
//         string path = GetSavePath(profileName);
//         wallet.SaveWallet(path);
//     }

//     public void LoadWallet(WalletService wallet, string profileName)
//     {
//         string path = GetSavePath(profileName);
//         wallet.LoadWallet(path);
//     }
// }
