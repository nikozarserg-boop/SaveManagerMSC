using System;
using System.IO;
using System.Collections.Generic;

namespace SaveManagerMSC
{
    public static class utils_Py
    {
        public static object log(object message, object level = null, params object[] extra)
        {
            try
            {
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC");
                if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
                string logFile = Path.Combine(dataDir, "programmLog.txt");
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string lvl = level != null ? level.ToString() : "INFO";
                string extras = (extra != null && extra.Length > 0) ? (" | " + string.Join(", ", extra)) : "";
                string line = $"{time} [{lvl}] {message}{extras}";
                Console.WriteLine(line);
                try { File.AppendAllText(logFile, line + Environment.NewLine); } catch { }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logger error: " + ex.Message);
                return null;
            }
        }

        public static object open_log(object app_root)
        {
            try
            {
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC");
                string logFile = Path.Combine(dataDir, "programmLog.txt");
                if (!File.Exists(logFile))
                {
                    Console.WriteLine("Log file not found: " + logFile);
                    return null;
                }
                string[] lines = File.ReadAllLines(logFile);
                int start = Math.Max(0, lines.Length - 200);
                for (int i = start; i < lines.Length; i++) Console.WriteLine(lines[i]);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("open_log error: " + ex.Message);
                return null;
            }
        }

        public static string ComputeSHA256(string path)
        {
            try
            {
                using var fs = File.OpenRead(path);
                using var sha = System.Security.Cryptography.SHA256.Create();
                var hash = sha.ComputeHash(fs);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            catch { return null; }
        }

        public static List<string> ScanFiles(string dir)
        {
            var files = new List<string>();
            if (!Directory.Exists(dir)) return files;
            foreach (var f in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
            {
                files.Add(Path.GetRelativePath(dir, f).Replace(Path.DirectorySeparatorChar, '/'));
            }
            return files;
        }

        public static object WriteMetadata(string metadir, string name, Dictionary<string, object> meta)
        {
            try
            {
                if (!Directory.Exists(metadir)) Directory.CreateDirectory(metadir);
                var path = Path.Combine(metadir, name + ".json");
                var json = System.Text.Json.JsonSerializer.Serialize(meta, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
                log($"Metadata written: {path}", "INFO");
                return null;
            }
            catch (Exception ex)
            {
                log("WriteMetadata error: " + ex.Message, "ERROR");
                Console.WriteLine("WriteMetadata error: " + ex.Message);
                return null;
            }
        }

        public static Dictionary<string, object> LoadMetadataByName(string metadir, string name)
        {
            try
            {
                var path = Path.Combine(metadir, name + ".json");
                if (!File.Exists(path)) return null;
                var txt = File.ReadAllText(path);
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(txt);
                return dict;
            }
            catch { return null; }
        }

        public static string NextSaveName(string dir, string baseName = "save")
        {
            int i = 1;
            string candidate;
            do
            {
                candidate = baseName + (i == 1 ? "" : "_" + i);
                i++;
            } while (Directory.Exists(Path.Combine(dir, candidate)));
            return candidate;
        }

        public static object CreateSave(string src, string targetDir, string name, object progress_callback = null, bool makeMeta = true)
        {
            try
            {
                log("CreateSave started", "INFO", $"src={src}, targetDir={targetDir}, name={name}");
                if (!Directory.Exists(src)) throw new Exception("Source folder not found: " + src);
                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
                string saveName = string.IsNullOrEmpty(name) ? NextSaveName(targetDir, "save") : name;
                string dest = Path.Combine(targetDir, saveName);
                if (Directory.Exists(dest)) throw new Exception("Save already exists: " + saveName);
                Directory.CreateDirectory(dest);
                log($"Created save directory: {dest}", "INFO");
                var files = Directory.EnumerateFiles(src, "*", SearchOption.AllDirectories);
                int total = 0; foreach (var _ in files) total++;
                int copied = 0;
                log($"Copying {total} files...", "INFO");
                foreach (var f in Directory.EnumerateFiles(src, "*", SearchOption.AllDirectories))
                {
                    var rel = Path.GetRelativePath(src, f);
                    var outPath = Path.Combine(dest, rel);
                    var outDir = Path.GetDirectoryName(outPath);
                    if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
                    File.Copy(f, outPath, true);
                    copied++;
                }
                log($"Copied {copied} files successfully", "INFO");
                var metaDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC", "metadata");
                if (!Directory.Exists(metaDir)) Directory.CreateDirectory(metaDir);
                var meta = new Dictionary<string, object>()
                {
                    {"name", saveName},
                    {"created", DateTime.UtcNow.ToString("o")},
                    {"source", src},
                    {"files", ScanFiles(dest)}
                };
                WriteMetadata(metaDir, saveName, meta);
                log($"CreateSave completed: {saveName}", "INFO");
                return saveName;
            }
            catch (Exception ex)
            {
                log("CreateSave error: " + ex.Message, "ERROR");
                Console.WriteLine("CreateSave error: " + ex.Message);
                throw;
            }
        }

        public static object DeleteSave(string baseDir, string name)
        {
            try
            {
                log($"DeleteSave started: {name}", "INFO");
                var savesRoot = baseDir;
                var dir = Path.Combine(savesRoot, name);
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                    log($"Deleted save directory: {dir}", "INFO");
                }
                var metaDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC", "metadata");
                var metaFile = Path.Combine(metaDir, name + ".json");
                if (File.Exists(metaFile))
                {
                    File.Delete(metaFile);
                    log($"Deleted metadata file: {metaFile}", "INFO");
                }
                log($"DeleteSave completed: {name}", "INFO");
                return null;
            }
            catch (Exception ex)
            {
                log("DeleteSave error: " + ex.Message, "ERROR");
                Console.WriteLine("DeleteSave error: " + ex.Message);
                throw;
            }
        }

        public static object RestoreSave(string baseDir, string name, string targetPath, bool make_backup = true, object progress_callback = null)
        {
            try
            {
                log($"RestoreSave started: {name} to {targetPath}", "INFO");
                var src = Path.Combine(baseDir, name);
                if (!Directory.Exists(src)) throw new Exception("Save not found: " + name);
                if (!Directory.Exists(targetPath)) throw new Exception("Target path not found: " + targetPath);
                string backupPath = null;
                if (make_backup)
                {
                    backupPath = Path.Combine(Path.GetDirectoryName(targetPath), "backup_before_restore_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                    Directory.CreateDirectory(backupPath);
                    log($"Creating backup at: {backupPath}", "INFO");
                    int backedUp = 0;
                    foreach (var f in Directory.EnumerateFiles(targetPath, "*", SearchOption.AllDirectories))
                    {
                        var rel = Path.GetRelativePath(targetPath, f);
                        var outp = Path.Combine(backupPath, rel);
                        var outd = Path.GetDirectoryName(outp);
                        if (!Directory.Exists(outd)) Directory.CreateDirectory(outd);
                        File.Copy(f, outp, true);
                        backedUp++;
                    }
                    log($"Backed up {backedUp} files", "INFO");
                    }
                    int restored = 0;
                foreach (var f in Directory.EnumerateFiles(src, "*", SearchOption.AllDirectories))
                {
                    var rel = Path.GetRelativePath(src, f);
                    var outp = Path.Combine(targetPath, rel);
                    var outd = Path.GetDirectoryName(outp);
                    if (!Directory.Exists(outd)) Directory.CreateDirectory(outd);
                    File.Copy(f, outp, true);
                    restored++;
                }
                log($"Restored {restored} files successfully", "INFO");
                log($"RestoreSave completed: {name}", "INFO");
                return backupPath;
            }
            catch (Exception ex)
            {
                log("RestoreSave error: " + ex.Message, "ERROR");
                Console.WriteLine("RestoreSave error: " + ex.Message);
                throw;
            }
        }
    }
}