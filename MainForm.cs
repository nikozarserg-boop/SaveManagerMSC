using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;

namespace SaveManagerMSC
{
    public class MainForm : Form
    {
        private ListBox lstSaves;
        private Button btnCreate, btnDelete, btnRestore, btnRefresh, btnOpenLog, btnOpenReadme, btnDetails, btnBrowseSrc, btnBrowseTgt;
        private TextBox txtSrc, txtTgt, txtNewName;
        private Label lblSrc, lblTgt, lblNewName, lblStatus, lblExample;
        private ProgressBar progress;
        private ComboBox cmbLang;
        private GroupBox grpSource, grpTarget, grpActions, grpSaves;

        private Dictionary<string, string> texts = new Dictionary<string, string>();

        private const int MCI_OPEN = 0x803;
        private const int MCI_PLAY = 0x806;
        private const int MCI_STOP = 0x808;
        private const int MCI_CLOSE = 0x804;
        private const int MCI_STATUS = 0x814;

        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, IntPtr hwndCallback);

        private string musicFile;
        private System.Timers.Timer musicTimer;

        public MainForm()
        {
            InitializeComponent();
            try {
                var iconPath = Path.Combine(AppContext.BaseDirectory, "assets", "icon.ico");
                if (File.Exists(iconPath)) this.Icon = new Icon(iconPath);
            } catch { }

            InitializeMusic();
            LoadLocales();
            ApplyTexts();
            try {
            string defaultSaves = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC", "saves");
            if (!Directory.Exists(defaultSaves)) Directory.CreateDirectory(defaultSaves);
            txtTgt.Text = defaultSaves;
            utils_Py.log($"Default saves folder set to: {defaultSaves}", "INFO");
            } catch (Exception ex) {
            utils_Py.log("Error setting default saves folder: " + ex.Message, "ERROR");
            }
            try {
                string userName = Environment.UserName;
                string possiblePath = Path.Combine("C:", "Users", userName, "AppData", "LocalLow", "Amistech", "My Summer Car");
                if (Directory.Exists(possiblePath)) {
                    txtSrc.Text = possiblePath;
                    utils_Py.log($"Auto-detected source folder: {possiblePath}", "INFO");
                }
            } catch (Exception ex) {
                utils_Py.log("Error auto-detecting source folder: " + ex.Message, "ERROR");
            }
            PopulateSaves();
        }

        private void InitializeMusic()
        {
            try
            {
                string musicPath = Path.Combine(AppContext.BaseDirectory, "assets", "MainTheme.mp3");
                if (File.Exists(musicPath))
                {
                    musicFile = musicPath;
                    PlayMusicLoop();
                    musicTimer = new System.Timers.Timer(1000);
                    musicTimer.Elapsed += MusicTimer_Elapsed;
                    musicTimer.Start();
                }
            }
            catch (Exception ex)
            {
                utils_Py.log("Failed to initialize music: " + ex.Message, "ERROR");
            }
        }

        private void PlayMusicLoop()
        {
            try
            {
                StopMusic();
                mciSendString($"open \"{musicFile}\" type mpegvideo alias MainTheme", "", 0, IntPtr.Zero);
                mciSendString("seek MainTheme to 19000", "", 0, IntPtr.Zero);
                mciSendString("play MainTheme", "", 0, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                utils_Py.log("Failed to play music: " + ex.Message, "ERROR");
            }
        }

        private void StopMusic()
        {
            try
            {
                mciSendString("stop MainTheme", "", 0, IntPtr.Zero);
                mciSendString("close MainTheme", "", 0, IntPtr.Zero);
            }
            catch { }
        }

        private void MusicTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                string positionStr = new string(' ', 255);
                if (mciSendString("status MainTheme position", positionStr, 255, IntPtr.Zero) == 0)
                {
                    int pos = int.Parse(positionStr.Trim());
                    if (pos >= 45000)
                    {
                        PlayMusicLoop();
                    }
                }
            }
            catch { }
        }

        private void InitializeComponent()
        {
            this.Text = "SaveManagerMSC";
            this.Width = 850;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 248, 255); // AliceBlue
            this.Font = new Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Prevents resizing and maximizing
            this.AutoScaleMode = AutoScaleMode.Font;

            var picLogo = new PictureBox() { Left = 10, Top = 10, Width = 64, Height = 64, SizeMode = PictureBoxSizeMode.StretchImage };
            try { picLogo.Image = Image.FromFile(Path.Combine(AppContext.BaseDirectory, "assets", "icon.ico")); } catch { }

            grpSource = new GroupBox() { Text = "Source Folder", Left = 84, Top = 10, Width = 726, Height = 80, BackColor = Color.WhiteSmoke };
            lblSrc = new Label() { Left = 10, Top = 20, Width = 120, Text = "Game folder (source):", Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            txtSrc = new TextBox() { Left = 140, Top = 18, Width = 460, BackColor = Color.White };
            btnBrowseSrc = new Button() { Left = 610, Top = 16, Width = 100, Text = "Browse...", BackColor = Color.LightSteelBlue, FlatStyle = FlatStyle.Flat };
            btnBrowseSrc.Click += (s,e) => { using var d = new FolderBrowserDialog(); if (d.ShowDialog()==DialogResult.OK) txtSrc.Text = d.SelectedPath; };
            lblExample = new Label() { Left = 10, Top = 45, Width = 690, Text = "Example: C:\\Users\\<Name>\\AppData\\LocalLow\\Amistech\\My Summer Car", ForeColor = Color.Gray, Font = new Font("Segoe UI", 7F) };
            grpSource.Controls.AddRange(new Control[] { lblSrc, txtSrc, btnBrowseSrc, lblExample });

            grpTarget = new GroupBox() { Text = "Target Folder", Left = 10, Top = 100, Width = 800, Height = 60, BackColor = Color.WhiteSmoke };
            lblTgt = new Label() { Left = 10, Top = 20, Width = 130, Text = "Saves folder (destination):", Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            txtTgt = new TextBox() { Left = 150, Top = 18, Width = 450, BackColor = Color.White };
            btnBrowseTgt = new Button() { Left = 610, Top = 16, Width = 100, Text = "Browse...", BackColor = Color.LightSteelBlue, FlatStyle = FlatStyle.Flat };
            btnBrowseTgt.Click += (s,e) => { using var d = new FolderBrowserDialog(); if (d.ShowDialog()==DialogResult.OK) txtTgt.Text = d.SelectedPath; };
            grpTarget.Controls.AddRange(new Control[] { lblTgt, txtTgt, btnBrowseTgt });

            grpActions = new GroupBox() { Text = "Actions", Left = 10, Top = 170, Width = 800, Height = 80, BackColor = Color.WhiteSmoke };
            lblNewName = new Label() { Left = 10, Top = 20, Width = 140, Text = "New save name (optional):", Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            txtNewName = new TextBox() { Left = 160, Top = 18, Width = 200, BackColor = Color.White };
            btnCreate = new Button() { Left = 380, Top = 16, Width = 100, Text = "Create", BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnCreate.Click += async (s,e) => await BtnCreate_Click();
            btnDelete = new Button() { Left = 490, Top = 16, Width = 100, Text = "Delete", BackColor = Color.LightCoral, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnDelete.Click += async (s,e) => await BtnDelete_Click();
            btnRestore = new Button() { Left = 600, Top = 16, Width = 100, Text = "Restore", BackColor = Color.LightBlue, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnRestore.Click += async (s,e) => await BtnRestore_Click();
            grpActions.Controls.AddRange(new Control[] { lblNewName, txtNewName, btnCreate, btnDelete, btnRestore });

            grpSaves = new GroupBox() { Text = "Existing Saves", Left = 10, Top = 260, Width = 800, Height = 220, BackColor = Color.WhiteSmoke };
            lstSaves = new ListBox() { Left = 10, Top = 20, Width = 700, Height = 160, BackColor = Color.White, Font = new Font("Segoe UI", 9F) };
            lstSaves.DoubleClick += async (s,e) => await BtnRestore_Click();
            btnDetails = new Button() { Left = 720, Top = 20, Width = 70, Height = 40, Text = "Details", BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 7F) };
            btnDetails.Click += (s,e) => ShowDetails();
            btnRefresh = new Button() { Left = 720, Top = 70, Width = 70, Height = 40, Text = "Refresh", BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 7F) };
            btnRefresh.Click += (s,e) => PopulateSaves();
            grpSaves.Controls.AddRange(new Control[] { lstSaves, btnDetails, btnRefresh });

            var pnlBottom = new Panel() { Left = 10, Top = 490, Width = 800, Height = 60, BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle };
            btnOpenLog = new Button() { Left = 10, Top = 10, Width = 100, Text = "Open Log", BackColor = Color.LightSteelBlue, FlatStyle = FlatStyle.Flat };
            btnOpenLog.Click += (s,e) => OpenLog();
            btnOpenReadme = new Button() { Left = 120, Top = 10, Width = 100, Text = "ReadMe", BackColor = Color.LightSteelBlue, FlatStyle = FlatStyle.Flat };
            btnOpenReadme.Click += (s,e) => OpenReadme();
            cmbLang = new ComboBox() { Left = 680, Top = 10, Width = 100, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.White };
            cmbLang.SelectedIndexChanged += (s,e) => { if (cmbLang.SelectedItem!=null) SetLang(cmbLang.SelectedItem.ToString()); };
            progress = new ProgressBar() { Left = 10, Top = 35, Width = 550, Height = 20, Visible = false, ForeColor = Color.DodgerBlue };
            lblStatus = new Label() { Left = 230, Top = 12, Width = 300, Text = "Ready", Font = new Font("Segoe UI", 9F, FontStyle.Italic), ForeColor = Color.DarkGreen };
            pnlBottom.Controls.AddRange(new Control[] { btnOpenLog, btnOpenReadme, cmbLang, progress, lblStatus });

            this.Controls.AddRange(new Control[] { picLogo, grpSource, grpTarget, grpActions, grpSaves, pnlBottom });
        }

        private void LoadLocales()
        {
            try
            {
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC");
                string localesDir = Path.Combine(dataDir, "locales");
                if (!Directory.Exists(localesDir)) Directory.CreateDirectory(localesDir);
                var files = Directory.GetFiles(localesDir, "*.json");
                utils_Py.log($"Loading locales from: {localesDir}, found {files.Length} files", "INFO");
                foreach (var f in files)
                {
                    var key = Path.GetFileNameWithoutExtension(f);
                    cmbLang.Items.Add(key);
                }
                var cfgObj = SaveManagerMSC.main_Py.read_config();
                var cfg = cfgObj as Dictionary<string, object> ?? new Dictionary<string, object>();
                string lang = cfg.ContainsKey("lang") ? cfg["lang"].ToString() : (cmbLang.Items.Count>0?cmbLang.Items[0].ToString():"en");
                cmbLang.SelectedIndexChanged -= (s,e) => { if (cmbLang.SelectedItem!=null) SetLang(cmbLang.SelectedItem.ToString()); };
                cmbLang.SelectedItem = lang;
                cmbLang.SelectedIndexChanged += (s,e) => { if (cmbLang.SelectedItem!=null) SetLang(cmbLang.SelectedItem.ToString()); };
                LoadLocale(lang);
            }
            catch (Exception ex)
            {
                utils_Py.log("LoadLocales error: " + ex.Message, "ERROR");
            }
        }

        private void LoadLocale(string lang)
        {
            try
            {
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC");
                string localesDir = Path.Combine(dataDir, "locales");
                string path = Path.Combine(localesDir, lang + ".json");
                if (!File.Exists(path)) return;
                var txt = File.ReadAllText(path);
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(txt);
                texts = dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? "");
                ApplyTexts();
            }
            catch { }
        }

        private void ApplyTexts()
        {
        if (texts.Count==0) return;
        this.Text = texts.GetValueOrDefault("app_title","SaveManagerMSC");
        grpSource.Text = texts.GetValueOrDefault("source_folder_group","Source Folder");
        lblSrc.Text = texts.GetValueOrDefault("choose_source","Game folder (source):");
        lblExample.Text = texts.GetValueOrDefault("example_path","Example: C:\\Users\\<Name>\\AppData\\LocalLow\\Amistech\\My Summer Car");
        grpTarget.Text = texts.GetValueOrDefault("target_folder_group","Target Folder");
        lblTgt.Text = texts.GetValueOrDefault("choose_target","Saves folder (destination):");
        grpActions.Text = texts.GetValueOrDefault("actions_group","Actions");
        lblNewName.Text = texts.GetValueOrDefault("new_save_name","New save name (optional):");
        btnCreate.Text = texts.GetValueOrDefault("create_save","Create");
        btnDelete.Text = texts.GetValueOrDefault("delete_save","Delete");
        btnRestore.Text = texts.GetValueOrDefault("restore_save","Restore");
        grpSaves.Text = texts.GetValueOrDefault("existing_saves_group","Existing Saves");
        btnDetails.Text = texts.GetValueOrDefault("save_details","Details");
        btnOpenLog.Text = texts.GetValueOrDefault("open_log","Open Log");
        btnOpenReadme.Text = texts.GetValueOrDefault("open_readme","ReadMe");
        btnRefresh.Text = texts.GetValueOrDefault("refresh","Refresh");
        btnBrowseSrc.Text = texts.GetValueOrDefault("browse_button","Browse...");
            btnBrowseTgt.Text = texts.GetValueOrDefault("browse_button","Browse...");
            lblStatus.Text = "";
        }

        private void SetLang(string lang)
        {
            try
            {
                var cfg = SaveManagerMSC.main_Py.read_config() as Dictionary<string, object> ?? new Dictionary<string, object>();
                cfg["lang"] = lang;
                SaveManagerMSC.main_Py.write_config(cfg);
                LoadLocale(lang);
                }
            catch { }
        }

        private void PopulateSaves()
        {
            try
            {
                lstSaves.Items.Clear();
                string savesRoot = txtTgt.Text;
                if (string.IsNullOrEmpty(savesRoot))
                {
                savesRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC", "saves");
                }
                if (!Directory.Exists(savesRoot)) Directory.CreateDirectory(savesRoot);
                utils_Py.log($"Populating saves from: {savesRoot}", "INFO");
                foreach (var d in Directory.GetDirectories(savesRoot))
                {
                    lstSaves.Items.Add(Path.GetFileName(d));
                }
                utils_Py.log($"Found {lstSaves.Items.Count} saves", "INFO");
                lblStatus.Text = "";
            }
            catch (Exception ex)
            {
                utils_Py.log("populate_saves error: " + ex.Message, "ERROR");
                MessageBox.Show("populate_saves error: " + ex.Message);
            }
        }

        private async Task BtnCreate_Click()
        {
            try
            {
                progress.Style = ProgressBarStyle.Marquee;
                progress.Visible = true;
                lblStatus.Text = texts.GetValueOrDefault("status_creating","Creating...");
                await Task.Run(() =>
                {
                    string src = txtSrc.Text;
                    string tgt = txtTgt.Text;
                    if (string.IsNullOrEmpty(src)) src = Path.Combine(AppContext.BaseDirectory, "game_src");
                    if (string.IsNullOrEmpty(tgt)) tgt = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC", "saves");
                    SaveManagerMSC.utils_Py.CreateSave(src, tgt, txtNewName.Text);
                });
                PopulateSaves();
                lblStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Create error: " + ex.Message);
            }
            finally
            {
                progress.Visible = false;
            }
        }

        private async Task BtnDelete_Click()
        {
            try
            {
                if (lstSaves.SelectedItem==null) { MessageBox.Show(texts.GetValueOrDefault("error_no_selection","Select a save first")); return; }
                var name = lstSaves.SelectedItem.ToString();
                string msg = texts.GetValueOrDefault("confirm_delete","Delete save {name} ?").Replace("{name}", name);
                if (MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                string savesRoot = txtTgt.Text;
                if (string.IsNullOrEmpty(savesRoot))
                {
                    savesRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC", "saves");
                }
                await Task.Run(() => {
                    SaveManagerMSC.utils_Py.DeleteSave(savesRoot, name);
                });
                PopulateSaves();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete error: " + ex.Message);
            }
        }

        private async Task BtnRestore_Click()
        {
            try
            {
                if (lstSaves.SelectedItem==null) { MessageBox.Show(texts.GetValueOrDefault("error_no_selection","Select a save first")); return; }
                var name = lstSaves.SelectedItem.ToString();
                var dlg = new FolderBrowserDialog();
                dlg.Description = texts.GetValueOrDefault("choose_restore_destination","Select target folder to restore into (game folder)");
                if (dlg.ShowDialog()!=DialogResult.OK) return;
                string target = dlg.SelectedPath;
                string backupMsg = texts.GetValueOrDefault("backup_prompt","Make a backup of the current files in the target folder before restoring?");
                bool makeBackup = MessageBox.Show(backupMsg, "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes;
                string savesRoot = txtTgt.Text;
                if (string.IsNullOrEmpty(savesRoot))
                {
                    savesRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC", "saves");
                }
                progress.Style = ProgressBarStyle.Marquee;
                progress.Visible = true;
                lblStatus.Text = texts.GetValueOrDefault("status_restoring","Restoring...");
                await Task.Run(() => {
                    SaveManagerMSC.utils_Py.RestoreSave(savesRoot, name, target, makeBackup);
                });
                lblStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Restore error: " + ex.Message);
            }
            finally
            {
                progress.Visible = false;
            }
        }

        private void OpenLog()
        {
            try
            {
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC");
                string logFile = Path.Combine(dataDir, "programmLog.txt");
                if (!File.Exists(logFile))
                {
                    MessageBox.Show("Log not found: " + logFile);
                    return;
                }
                ProcessStartInfo psi = new ProcessStartInfo(logFile) { UseShellExecute = true };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Open log error: " + ex.Message);
            }
        }

        private void ShowDetails()
        {
            try
            {
                if (lstSaves.SelectedItem == null) { MessageBox.Show(texts.GetValueOrDefault("error_no_selection", "No save selected.")); return; }
                var name = lstSaves.SelectedItem.ToString();
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveManagerMSC");
                string metadir = Path.Combine(dataDir, "metadata");
                var meta = SaveManagerMSC.utils_Py.LoadMetadataByName(metadir, name);
                if (meta == null) { MessageBox.Show(texts.GetValueOrDefault("error_no_metadata", "Failed to find metadata for selected save.")); return; }
                
                int filesCount = 0;
                if (meta.ContainsKey("files") && meta["files"] != null)
                {
                    if (meta["files"] is System.Text.Json.JsonElement jsonElement && jsonElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        filesCount = jsonElement.GetArrayLength();
                    }
                }
                
                string created = meta.GetValueOrDefault("created", "Unknown")?.ToString() ?? "Unknown";
                if (DateTime.TryParse(created, out DateTime createdDate))
                {
                    created = createdDate.ToString("dd.MM.yyyy HH:mm:ss");
                }
                
                string details = $"Name: {meta.GetValueOrDefault("name", "")}\nCreated: {created}\nSource: {meta.GetValueOrDefault("source", "")}\nFiles: {filesCount}";
                MessageBox.Show(details, texts.GetValueOrDefault("save_details", "Details"));
            }
            catch (Exception ex)
            {
                utils_Py.log("Show details error: " + ex.Message, "ERROR");
                MessageBox.Show("Show details error: " + ex.Message);
            }
        }

        private void OpenReadme()
        {
            try
            {
                string path = Path.Combine(AppContext.BaseDirectory, "ReadMe.txt");
                if (!File.Exists(path)) { MessageBox.Show("ReadMe not found: " + path); return; }
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Open ReadMe error: " + ex.Message);
            }
        }
    }
}
