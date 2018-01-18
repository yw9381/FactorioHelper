using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using MiniJSON;


namespace FactorioHelper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Version HelperVersion = new Version();
        ResourceDictionary LanguageResources = Application.Current.Resources;
        bool ChineseInputStatus = true;
        bool GameStatus = false;
        string GameDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Factorio\\";
        string ModsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Factorio\\mods\\";
        string MapsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Factorio\\saves\\";
        string TempPath = System.IO.Path.GetTempPath().Replace("\\\\", "\\");
        string SelfPath = Directory.GetCurrentDirectory();
        string version = "0.0.1 Alpha";
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow1_Loaded);
            this.MainWindow1.Title = LanguageResources["Windows_Title"] + " " + version;
        }
        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            // 检测游戏状态
            DispatcherTimer GameStatusTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            GameStatusTimer.Tick += new EventHandler(CheckGameStatus);
            GameStatusTimer.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void CheckGameStatus(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("factorio").Length != 0)
            {
                this.GameInfo_GameStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                this.GameInfo_GameStatus.Content = LanguageResources["GameInfo_GameStatusOn"];
                this.GameInfo_KillGame.Visibility = Visibility.Visible;
                this.GameStatus = true;
            }
            else
            {
                this.GameInfo_GameStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                this.GameInfo_GameStatus.Content = LanguageResources["GameInfoTable_Game_Status_Off"];
                this.GameInfo_KillGame.Visibility = Visibility.Hidden;
                this.GameStatus = false;
            }

        }

        private void LanguageSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string Language;
            switch (this.LanguageSelect.SelectedIndex)
            {
                case 0:
                    Language = "zh-cn";
                    break;
                case 1:
                    Language = "en-us";
                    break;
                case 2:
                    Language = "zh-cn";
                    break;
                default:
                    Language = "zh-cn";
                    break;

            }

            Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/Languages/" + Language + ".xaml", UriKind.RelativeOrAbsolute)
            };
            ResourceDictionary LanguageResources = Application.Current.Resources;
            this.MainWindow1.Title = LanguageResources["Windows_Title"] + " " + version;
        }

        private void GameInfoTable_OpenGameDataButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", GameDataPath);

        }

        private void GameInfoTa_OpenGameModsButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", ModsPath);
        }

        private void GameInfoTable_KillGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process[] p = Process.GetProcessesByName("factorio");
                p[0].Kill();
                MessageBox.Show("游戏已强制退出！");
                //GetProcess();
            }
            catch
            {
                MessageBox.Show("无法关闭游戏！");
            }
        }

        private void GameInfo_ChineseInputButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ChineseInputStatus)
            {
                if (GameStatus)
                {
                    GameInfo_ChineseInputButton.Content = LanguageResources["GameInfo_ChineseInputButtonOn"];
                    GameInfo_ChineseInputStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    GameInfo_ChineseInputStatus.Content = LanguageResources["GameInfo_ChineseInputStatusOn"];
                    ChineseInputStatus = false;
                }
                else
                {
                    MessageBox.Show((string)LanguageResources["GameInfoTable_Game_Status_Off"]);
                }
            }
            else
            {
                GameInfo_ChineseInputButton.Content = LanguageResources["GameInfo_ChineseInputButtonOff"];
                GameInfo_ChineseInputStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                GameInfo_ChineseInputStatus.Content = LanguageResources["GameInfo_ChineseInputStatusOn"];
                ChineseInputStatus = true;
            }
        }

        private void ServerHelper_ResetConfig_Click(object sender, RoutedEventArgs e)
        {
            ServerHelper_ServerNameTextBox.Text = "Factorio Server";
            ServerHelper_ServerTagTextBox.Text = "Server,CN,无Mod";
            ServerHelper_ServerUsernameTextBox.Text = "user";
            ServerHelper_ServerGamePasswordTextBox.Text = "pass";
            ServerHelper_ServerMaxUploadTextBox.Text = "1024";
            ServerHelper_ServerMinLatecyTextBox.Text = "0";
            ServerHelper_ServerAutoSaveTimeTextBox.Text = "10";
            ServerHelper_ServerAutoSaveSoltsComboBox.SelectedIndex = 2;
            ServerHelper_ServerAutoMaxAFKTimeTextBox.Text = "0";
            ServerHelper_ServerAdminPlayersTextBox.Text = "admin,hello,hehe";
            ServerHelper_ServerDescriptionTextBox.Text = "This is my factorio server\nwelcone join";
            ServerHelper_ServerMaxPlayersTextBox.Text = "64";
            ServerHelper_ServerPasswordTextBox.Text = "123456";
            ServerHelper_ServerTokenTextBox.Text = "";
            ServerHelper_ServerAdminPause.IsChecked = true;
            ServerHelper_ServerGenuineCheck.IsChecked = false;
            ServerHelper_ServerAllow_Commands.IsChecked = true;
            ServerHelper_ServerVisibilityLAN.IsChecked = true;
            ServerHelper_ServerVisibilityPuclic.IsChecked = true;
            ServerHelper_ServerIsMaxPlayers.IsChecked = true;
            ServerHelper_ServerAutoPause.IsChecked = true;
            ServerHelper_ServerAutoSave.IsChecked = true;
        }

        private void ServerHelperTable_GetHelp_Click(object sender, RoutedEventArgs e)
        {
            string help = LanguageResources["ServerHelperTable_HelpString"].ToString();
            MessageBox.Show(help, "Help");
        }

        private void ModManage_Flush_Click(object sender, RoutedEventArgs e)
        {
            ModManage_ListView.Items.Clear();
            DirectoryInfo folder = new DirectoryInfo(this.ModsPath);
            int Uid = 0;
            foreach (FileInfo file in folder.GetFiles("*.zip"))
            {
                string fileName = file.Name.Replace(".zip", "");
                string filePath = file.FullName;
                string modName = fileName.Substring(0, fileName.LastIndexOf('_'));
                string infoFileName = "info.json";
                string infoFilePath = this.unzip(filePath, infoFileName);
                Dictionary<string, object> modInfo = Json.Deserialize(readJSONFile(infoFilePath)) as Dictionary<string, object>;
                MessageBox.Show(modInfo["homepage"] as string);
                ModManage_List modItem = new ModManage_List
                {
                    Uid = Uid += 1,
                    name = modInfo["name"] as string,
                    title = modInfo["title"] as string,
                    version = modInfo["version"] as string,
                    chineseVersion = "0.2",
                    factorio_version = modInfo["factorio_version"] as string,
                    description = modInfo["description"] as string,
                };
                ModManage_ListView.Items.Add(modItem);
            }
        }
        private string readJSONFile(string fileName)
        {
            StreamReader sr = new StreamReader(fileName, Encoding.UTF8);
            String line;
            string JSONContent = "";
            while ((line = sr.ReadLine()) != null)
            {
                JSONContent = JSONContent + line.ToString();
            }
            return JSONContent;
        }
        private string unzip(string zipPath, string fileName)
        {
            // 
            Process checkFolderExec = new Process();
            checkFolderExec.StartInfo.FileName = @"unzip.exe";
            checkFolderExec.StartInfo.Arguments = " -l \"" + zipPath + "\"";
            checkFolderExec.StartInfo.CreateNoWindow = true;
            checkFolderExec.StartInfo.UseShellExecute = false;
            checkFolderExec.StartInfo.RedirectStandardOutput = true;
            checkFolderExec.StartInfo.RedirectStandardError = true;
            checkFolderExec.StartInfo.RedirectStandardInput = true;
            checkFolderExec.Start();

            string execResult = checkFolderExec.StandardOutput.ReadToEnd();
            string folderName = new Regex("^\\s.+\\s{2}(.+)\\/").Replace(execResult.Split('\n')[3], "$1");
            folderName = folderName.Replace("\r","\\");

            ProcessStartInfo exep = new ProcessStartInfo();
            exep.FileName = @"unzip.exe";
            exep.Arguments = " -n \"" + zipPath + "\" \"" + folderName + fileName + "\" -d " + this.TempPath;
            exep.CreateNoWindow = true;
            exep.UseShellExecute = false;
            Process.Start(exep);
            return this.TempPath + folderName + fileName;

        }
        public class ModManage_List
        {
            public int Uid { get; set; }
            public string name { get; set; }
            public string title { get; set; }
            public string version { get; set; }
            public string chineseVersion { get; set; }
            public string factorio_version { get; set; }
            public string description { get; set; }
        }

        private void ModManage_OpenModFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", this.ModsPath);
        }

        private void GameInfo_OpenGameDataButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", this.GameDataPath);
        }
    }
}