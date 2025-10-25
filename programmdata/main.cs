using System;
using System.IO;
using System.Collections.Generic;

namespace SaveManagerMSC
{
    public static class main_Py
    {
        public static object ensure_app_root()
        {
            string baseDir = AppContext.BaseDirectory;
            string dataDir = Path.Combine(baseDir, "programmdata");
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            return dataDir;
        }

        public static object read_config()
        {
            try
            {
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC");
                if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
                string cfgPath = Path.Combine(dataDir, "config.json");
                if (!File.Exists(cfgPath))
                {
                    var defaultCfg = new Dictionary<string, object>() { { "lang", "en" }, { "version", "1.0" }, { "hide_log_by_default", true } };
                    File.WriteAllText(cfgPath, System.Text.Json.JsonSerializer.Serialize(defaultCfg));
                    return defaultCfg;
                }
                var txt = File.ReadAllText(cfgPath);
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(txt);
                return dict ?? new Dictionary<string, object>();
            }
            catch { return new Dictionary<string, object>(); }
        }

        public static object write_config(object cfg_obj)
        {
            try
            {
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC");
                if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
                string cfgPath = Path.Combine(dataDir, "config.json");
                string json = System.Text.Json.JsonSerializer.Serialize(cfg_obj);
                File.WriteAllText(cfgPath, json);
                return null;
            }
            catch { return null; }
        }

        public static object load_locale(object lang)
        {
            try
            {
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC");
                string localesDir = Path.Combine(dataDir, "locales");
                string langCode = lang != null ? lang.ToString() : "en";
                string path = Path.Combine(localesDir, langCode + ".json");
                if (!File.Exists(path))
                {
                    path = Path.Combine(localesDir, "en.json");
                    if (!File.Exists(path)) return new Dictionary<string, object>();
                }
                var txt = File.ReadAllText(path);
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(txt);
                return dict ?? new Dictionary<string, object>();
            }
            catch { return new Dictionary<string, object>(); }
        }

        public static object populate_saves()
        {
            try
            {
                string baseDir = AppContext.BaseDirectory;
                string savesRoot = Path.Combine(baseDir, "programmdata", "saves");
                if (!Directory.Exists(savesRoot)) Directory.CreateDirectory(savesRoot);
                var dirs = Directory.GetDirectories(savesRoot);
                Console.WriteLine("Available saves:");
                foreach (var d in dirs)
                {
                    Console.WriteLine("- " + Path.GetFileName(d));
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("populate_saves error: " + ex.Message);
                return null;
            }
        }

        public static object create_action()
        {
            try
            {
                string baseDir = AppContext.BaseDirectory;
                string defaultSrc = Path.Combine(baseDir, "game_src"); // user should adjust
                string savesRoot = Path.Combine(baseDir, "programmdata", "saves");
                Directory.CreateDirectory(savesRoot);
                var saveName = SaveManagerMSC.utils_Py.CreateSave(defaultSrc, savesRoot, null);
                Console.WriteLine("Created save: " + saveName);
                return saveName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("create_action error: " + ex.Message);
                return null;
            }
        }

        public static object delete_action()
        {
            try
            {
                string baseDir = AppContext.BaseDirectory;
                string savesRoot = Path.Combine(baseDir, "programmdata", "saves");
                var dirs = Directory.GetDirectories(savesRoot);
                if (dirs.Length == 0) { Console.WriteLine("No saves to delete"); return null; }
                var name = Path.GetFileName(dirs[0]);
                SaveManagerMSC.utils_Py.DeleteSave(savesRoot, name);
                Console.WriteLine("Deleted save: " + name);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("delete_action error: " + ex.Message);
                return null;
            }
        }

        public static object restore_action()
        {
            try
            {
                string baseDir = AppContext.BaseDirectory;
                string savesRoot = Path.Combine(baseDir, "programmdata", "saves");
                var dirs = Directory.GetDirectories(savesRoot);
                if (dirs.Length == 0) { Console.WriteLine("No saves to restore"); return null; }
                var name = Path.GetFileName(dirs[0]);
                string gamePath = Path.Combine(baseDir, "game_src"); // adjust as needed
                var backup = SaveManagerMSC.utils_Py.RestoreSave(savesRoot, name, gamePath, true);
                Console.WriteLine("Restored save: " + name + ". Backup: " + (backup ?? "none"));
                return backup;
            }
            catch (Exception ex)
            {
                Console.WriteLine("restore_action error: " + ex.Message);
                return null;
            }
        }

        public static object open_log(object app_root)
        {
            return SaveManagerMSC.utils_Py.open_log(null);
        }

        public static object main()
        {
            try
            {
                string baseDir = AppContext.BaseDirectory;
                var cfgObj = read_config();
                Dictionary<string, object> cfg = cfgObj as Dictionary<string, object> ?? new Dictionary<string, object>();
                string lang = cfg.ContainsKey("lang") ? cfg["lang"].ToString() : "en";
                var localeObj = load_locale(lang);
                Dictionary<string, object> locale = localeObj as Dictionary<string, object> ?? new Dictionary<string, object>();
                string title = locale.ContainsKey("app_title") ? locale["app_title"].ToString() : "App";
                Console.WriteLine($"=== {title} ===");
                Console.WriteLine("1) Show locales\n2) Change language\n3) Open log\n4) Exit");
                while (true)
                {
                    Console.Write("Choice: ");
                    var c = Console.ReadLine();
                    if (c == "1")
                    {
                        var files = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC", "locales"), "*.json");
                        for (int i = 0; i < files.Length; i++) Console.WriteLine($"{i + 1}) {Path.GetFileNameWithoutExtension(files[i])}");
                    }
                    else if (c == "2")
                    {
                        var files = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC", "locales"), "*.json");
                        for (int i = 0; i < files.Length; i++) Console.WriteLine($"{i + 1}) {Path.GetFileNameWithoutExtension(files[i])}");
                        Console.Write("Select lang number: ");
                        var s = Console.ReadLine();
                        if (int.TryParse(s, out int idx) && idx >= 1 && idx <= files.Length)
                        {
                            var selected = Path.GetFileNameWithoutExtension(files[idx - 1]);
                            cfg["lang"] = selected;
                            write_config(cfg);
                            Console.WriteLine("Language saved. Restart to apply.");
                        }
                    }
                    else if (c == "3")
                    {
                        SaveManagerMSC.utils_Py.open_log(null);
                    }
                    else if (c == "4" || string.IsNullOrEmpty(c))
                    {
                        break;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main error: " + ex.Message);
                return null;
            }
        }
    }
}