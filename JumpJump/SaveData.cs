using System;
using System.IO;
using System.Text.Json;

namespace JumpJump
{
    public class SaveData
    {
        // Bump this if you change the format later so old saves can be migrated
        public int Version { get; set; } = 1;

        public int Level { get; set; } = 1;
        public float PlayerX { get; set; }
        public float PlayerY { get; set; }

        // One entry per coin, in the same order as the coins array
        public bool[] CollectedApples { get; set; } = new bool[7];

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
                        if (data.CollectedApples == null || data.CollectedApples.Length != 7)
                            data.CollectedApples = new bool[7];
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