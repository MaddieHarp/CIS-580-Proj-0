using System;
using System.IO;
using System.Text.Json;

namespace JumpJump
{
    public class SaveData
    {
        // Bump this if you change the format later so old saves can be migrated
        public int Version { get; set; } = 1;

        // Seed for the random platform/coin layout, so it's identical on reload
        public int Seed { get; set; }

        public int Level { get; set; } = 1;
        public float PlayerX { get; set; }
        public float PlayerY { get; set; }

        // One entry per coin, in the same order as the coins array
        public bool[] CollectedCoins { get; set; } = new bool[7];

        public bool MetLilJump { get; set; }

        private static readonly string SavePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "JumpJump", "save.json");

        public static bool Exists() => File.Exists(SavePath);

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SavePath)!);
                string tmp = SavePath + ".tmp";
                File.WriteAllText(tmp, JsonSerializer.Serialize(this));
                File.Move(tmp, SavePath, overwrite: true); // avoids a half-written file if the game crashes mid-save
            }
            catch (Exception) { /* don't let a failed save crash the game */ }
        }

        public static SaveData Load()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(SavePath));
                    if (data != null)
                    {
                        // guard against a save with the wrong number of coins
                        if (data.CollectedCoins == null || data.CollectedCoins.Length != 7)
                            data.CollectedCoins = new bool[7];
                        return data;
                    }
                }
            }
            catch (Exception) { /* corrupt file: fall back to a fresh game */ }
            return new SaveData();
        }

        public static void Delete()
        {
            try { if (File.Exists(SavePath)) File.Delete(SavePath); }
            catch (Exception) { }
        }
    }
}