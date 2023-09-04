using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

namespace Runtime_Song_Converter
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int MajorV = 1;
        int MinorV = 0;
        int PatchV = 0;
        Boolean Preview = true;

        String IP = "";
        Boolean draggable = true;
        String exe = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 1);


        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists(exe + "\\tmp")) Directory.CreateDirectory(exe + "\\tmp");
            loadConfig();
            QuestIP();
            StartBMBF();
            txtbox.Text = "Output:";
            Update();

        }

        private void loadConfig()
        {
            if (!File.Exists(exe + "\\Config.json"))
            {
                return;
            }
            String txt = File.ReadAllText(exe + "\\Config.json");
            var json = JSON.Parse(txt);
            IP = json["IP"];
            Quest.Text = json["IP"];
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;


            if (mouseIsDown)
            {
                if (draggable)
                {
                    this.DragMove();
                }

            }

        }

        public void noDrag(object sender, MouseEventArgs e)
        {
            draggable = false;
        }

        public void doDrag(object sender, MouseEventArgs e)
        {
            draggable = true;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Directory.Exists(exe + "\\tmp"))
                {
                    Directory.Delete(exe + "\\tmp", true);
                }
            }
            catch
            {
            }
            String txt = "{}";
            if (File.Exists(exe + "\\Config.json"))
            {
                txt = File.ReadAllText(exe + "\\Config.json");
            }
            CheckIP();
            var json = JSON.Parse(txt);
            json["IP"] = IP;
            File.WriteAllText(exe + "\\Config.json", json.ToString());
            this.Close();
        }

        public void QuestIP()
        {
            String IPS = adbS("shell ifconfig wlan0");
            int Index = IPS.IndexOf("inet addr:");
            Boolean space = false;
            String FIP = "";
            for (int i = 0; i < IPS.Length; i++)
            {
                if (i > (Index + 9) && i < (Index + 9 + 16))
                {
                    if (IPS.Substring(i, 1) == " ")
                    {
                        space = true;
                    }
                    if (!space)
                    {
                        FIP = FIP + IPS.Substring(i, 1);
                    }
                }
            }

            if (FIP == "")
            {
                return;
            }
            IP = FIP;
            Quest.Text = FIP;
            if (IP == "")
            {
                Quest.Text = "Quest IP";
            }
        }

        public void StartBMBF()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                adb("shell am start -n com.weloveoculus.BMBF/com.weloveoculus.BMBF.MainActivity");
            }));
        }

        public Boolean adb(String Argument)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");
            ProcessStartInfo s = new ProcessStartInfo();
            s.CreateNoWindow = false;
            s.UseShellExecute = false;
            s.FileName = "adb.exe";
            s.WindowStyle = ProcessWindowStyle.Minimized;
            s.Arguments = Argument;
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(s))
                {
                    exeProcess.WaitForExit();
                    return true;
                }
            }
            catch
            {

                ProcessStartInfo se = new ProcessStartInfo();
                se.CreateNoWindow = false;
                se.UseShellExecute = false;
                se.FileName = User + "\\AppData\\Roaming\\SideQuest\\platform-tools\\adb.exe";
                se.WindowStyle = ProcessWindowStyle.Minimized;
                se.Arguments = Argument;
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(se))
                    {
                        exeProcess.WaitForExit();
                        return true;
                    }
                }
                catch
                {
                    // Log error.
                    txtbox.AppendText("\n\n\nAn error Occured (Code: ADB100). Check following");
                    txtbox.AppendText("\n\n- Your Quest is connected and USB Debugging enabled.");
                    txtbox.AppendText("\n\n- You have adb installed.");
                }
            }
            return false;
        }

        public String adbS(String Argument)
        {
            String User = System.Environment.GetEnvironmentVariable("USERPROFILE");
            ProcessStartInfo s = new ProcessStartInfo();
            s.CreateNoWindow = false;
            s.UseShellExecute = false;
            s.FileName = "adb.exe";
            s.WindowStyle = ProcessWindowStyle.Minimized;
            s.RedirectStandardOutput = true;
            s.Arguments = Argument;
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(s))
                {
                    String IPS = exeProcess.StandardOutput.ReadToEnd();
                    exeProcess.WaitForExit();
                    return IPS;
                }
            }
            catch
            {

                ProcessStartInfo se = new ProcessStartInfo();
                se.CreateNoWindow = false;
                se.UseShellExecute = false;
                se.FileName = User + "\\AppData\\Roaming\\SideQuest\\platform-tools\\adb.exe";
                se.WindowStyle = ProcessWindowStyle.Minimized;
                se.RedirectStandardOutput = true;
                se.Arguments = Argument;
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(se))
                    {
                        String IPS = exeProcess.StandardOutput.ReadToEnd();
                        exeProcess.WaitForExit();
                        return IPS;

                    }
                }
                catch
                {
                    // Log error.
                    txtbox.AppendText("\n\n\nAn error Occured (Code: ADB100). Check following");
                    txtbox.AppendText("\n\n- Your Quest is connected and USB Debugging enabled.");
                    txtbox.AppendText("\n\n- You have adb installed.");
                }
            }
            return "";
        }


        public Boolean CheckIP()
        {
            getQuestIP();
            if (IP == "Quest IP")
            {
                return false;
            }
            IP = IP.Replace(":5000000", "");
            IP = IP.Replace(":500000", "");
            IP = IP.Replace(":50000", "");
            IP = IP.Replace(":5000", "");
            IP = IP.Replace(":500", "");
            IP = IP.Replace(":50", "");
            IP = IP.Replace(":5", "");
            IP = IP.Replace(":", "");
            IP = IP.Replace("/", "");
            IP = IP.Replace("https", "");
            IP = IP.Replace("http", "");
            IP = IP.Replace("Http", "");
            IP = IP.Replace("Https", "");

            int count = 0;
            for (int i = 0; i < IP.Length; i++)
            {
                if (IP.Substring(i, 1) == ".")
                {
                    count++;
                }
            }
            if (count != 3)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                    Quest.Text = IP;
                }));
                return false;
            }
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate {
                Quest.Text = IP;
            }));
            IP = Quest.Text;
            return true;
        }

        public void Update()
        {
            try
            {
                //Download Update.txt
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile("https://raw.githubusercontent.com/ComputerElite/RSC/main/Update.txt", exe + "\\tmp\\Update.txt");
                    }
                    catch
                    {
                        txtbox.AppendText("\n\n\nAn error Occured (Code: UD100). Couldn't check for Updates. Check following");
                        txtbox.AppendText("\n\n- Your PC has internet.");
                    }
                }
                StreamReader VReader = new StreamReader(exe + "\\tmp\\Update.txt");

                String line;
                int l = 0;

                int MajorU = 0;
                int MinorU = 0;
                int PatchU = 0;
                while ((line = VReader.ReadLine()) != null)
                {
                    if (l == 0)
                    {
                        String URL = line;
                    }
                    if (l == 1)
                    {
                        MajorU = Convert.ToInt32(line);
                    }
                    if (l == 2)
                    {
                        MinorU = Convert.ToInt32(line);
                    }
                    if (l == 3)
                    {
                        PatchU = Convert.ToInt32(line);
                    }
                    l++;
                }

                if (MajorU > MajorV || MinorU > MinorV || PatchU > PatchV)
                {
                    //Newer Version available
                    UpdateB.Visibility = Visibility.Visible;
                }

                String MajorVS = Convert.ToString(MajorV);
                String MinorVS = Convert.ToString(MinorV);
                String PatchVS = Convert.ToString(PatchV);
                String MajorUS = Convert.ToString(MajorU);
                String MinorUS = Convert.ToString(MinorU);
                String PatchUS = Convert.ToString(PatchU);

                String VersionVS = MajorVS + MinorVS + PatchVS;
                int VersionV = Convert.ToInt32(VersionVS);
                String VersionUS = MajorUS + MinorUS + PatchUS + " ";
                int VersionU = Convert.ToInt32(VersionUS);
                if (VersionV > VersionU)
                {
                    //Newer Version that hasn't been released yet
                    txtbox.AppendText("\n\nLooks like you have a preview version. Downgrade now from " + MajorV + "." + MinorV + "." + PatchV + " to " + MajorU + "." + MinorU + "." + PatchU + " xD");
                    UpdateB.Visibility = Visibility.Visible;
                    UpdateB.Content = "Downgrade Now xD";
                }
                if (VersionV == VersionU && Preview)
                {
                    //User has Preview Version but a release Version has been released
                    txtbox.AppendText("\n\nLooks like you have a preview version. The release version has been released. Please Update now. ");
                    UpdateB.Visibility = Visibility.Visible;
                }
                VReader.Close();
            }
            catch
            {

            }
            try
            {
                File.Delete(exe + "\\tmp\\Update.txt");
            }
            catch
            {
            }
        }

        private void Start_Update(object sender, RoutedEventArgs e)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile("https://github.com/ComputerElite/RSC/raw/main/RSC_Update.exe", exe + "\\RSC_Update.exe");
            }
            //Process.Start(exe + "\\QSU_Update.exe");
            ProcessStartInfo s = new ProcessStartInfo();
            s.CreateNoWindow = false;
            s.UseShellExecute = false;
            s.FileName = exe + "\\RSC_Update.exe";
            try
            {
                using (Process exeProcess = Process.Start(s))
                {
                }
                this.Close();
            }
            catch
            {
                // Log error.
                txtbox.AppendText("\n\n\nAn error Occured (Code: UD200). Couldn't download Update.");
            }
        }

        private void Mini(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "Quest IP")
            {
                Quest.Text = "";
            }

        }

        public void getQuestIP()
        {
            IP = Quest.Text;
            return;
        }


        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = "Quest IP";
            }
        }

        private Boolean DownloadConfig()
        {
            CheckIP();
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile("http://" + IP + ":50000/host/beatsaber/config", exe + "\\tmp\\Config.json");
                }
                catch
                {
                    txtbox.AppendText("\n\n\nError (Code: BMBF100). Couldn't acces BMBF Web Interface. Check Following:");
                    txtbox.AppendText("\n\n- You've put in the right IP");
                    txtbox.AppendText("\n\n- BMBF is opened");
                    txtbox.AppendText("\n\n\nConverting Aborted. Please try again after you opened BMBF");
                    return false;
                }
                return true;
            }
        }

        private void StartConvert(object sender, RoutedEventArgs e)
        {
            StartBMBF();
            if (Directory.Exists(exe + "\\BeatSaberSongs")) Directory.Delete(exe + "\\BeatSaberSongs", true);
            if (Directory.Exists(exe + "\\CustomSongs")) Directory.Delete(exe + "\\CustomSongs", true);
            if (Directory.Exists(exe + "\\Playlists")) Directory.Delete(exe + "\\Playlists", true);
            //Get config and pull Songs
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\nPulling Songs");
                txtbox.ScrollToEnd();
            }));
            if(!DownloadConfig()) return;
            if (!adb("pull /sdcard/BMBFData/CustomSongs \"" + exe + "\"")) return;
            if (!adb("pull /sdcard/BMBFData/Playlists \"" + exe + "\"")) return;

            String config = File.ReadAllText(exe + "\\tmp\\Config.json");
            var json = JSON.Parse(config);

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile("https://raw.githubusercontent.com/BMBF/resources/master/assets/beatsaber-knowns.json", exe + "\\tmp\\beatsaber-knowns.json");
                }
                catch
                {
                    txtbox.AppendText("Couldn't check for new Song Packs. Aborting.");
                    return;
                }
            }
            var knowns = SimpleJSON.JSON.Parse(File.ReadAllText(exe + "\\tmp\\beatsaber-knowns.json"));
            String o;
            int index3 = 0;
            ArrayList BuiltInSongs = new ArrayList();

            while ((o = knowns["knownLevelPackIds"][index3]) != null)
            {
                BuiltInSongs.Add(o);
                index3++;
            }


            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\nMoving Songs to right Places.");
                txtbox.ScrollToEnd();
            }));

            Dictionary<String, int> Playlists = new Dictionary<string, int>();

            Directory.CreateDirectory(exe + "\\BeatSaberSongs");
            for (int i = 0; json["Config"]["Playlists"][i]["PlaylistName"] != null; i++)
            {
                Dictionary<String, int> Songs = new Dictionary<string, int>();
                Boolean know = false;
                for(int z = 0; z < BuiltInSongs.Count; z++)
                {
                    if (BuiltInSongs[z].ToString().Contains(json["Config"]["Playlists"][i]["PlaylistID"]))
                    {
                        know = true;
                        break;
                    }
                }
                if (know) continue;
                if (json["Config"]["Playlists"][i]["PlaylistID"] != "CustomSongs")
                {
                    String PlaylistN = json["Config"]["Playlists"][i]["PlaylistName"];
                    PlaylistN = PlaylistN.Replace("/", "");
                    PlaylistN = PlaylistN.Replace(":", "");
                    PlaylistN = PlaylistN.Replace("*", "");
                    PlaylistN = PlaylistN.Replace("?", "");
                    PlaylistN = PlaylistN.Replace("\"", "");
                    PlaylistN = PlaylistN.Replace("<", "");
                    PlaylistN = PlaylistN.Replace(">", "");
                    PlaylistN = PlaylistN.Replace("|", "");

                    for (int f = 0; f < PlaylistN.Length; f++)
                    {
                        if (PlaylistN.Substring(f, 1).Equals("\\"))
                        {
                            PlaylistN = PlaylistN.Substring(0, f - 1) + PlaylistN.Substring(f + 1, PlaylistN.Length - f - 1);
                        }
                    }

                    if (Playlists.ContainsKey(PlaylistN))
                    {
                        int PlaylistC = 0;
                        Playlists.TryGetValue(PlaylistN, out PlaylistC);
                        Playlists[PlaylistN] = PlaylistC + 1;
                        Directory.CreateDirectory(exe + "\\BeatSaberSongs\\" + PlaylistN + " " + (PlaylistC + 1));
                        if(!File.Exists(exe + "\\Playlists\\" + json["Config"]["Playlists"][i]["PlaylistID"] + ".png"))
                        {
                            txtbox.AppendText("\n\nCouldn't find Playlist cover for Playlist " + json["Config"]["Playlists"][i]["PlaylistID"] + ". Skipping.");
                        } else
                        {
                            File.Move(exe + "\\Playlists\\" + json["Config"]["Playlists"][i]["PlaylistID"] + ".png", exe + "\\BeatSaberSongs\\" + PlaylistN + " " + (PlaylistC + 1) + "\\playlistcover.png");
                        }
                    } else
                    {
                        Playlists.Add(PlaylistN, 0);
                        Directory.CreateDirectory(exe + "\\BeatSaberSongs\\" + PlaylistN);
                        if (!File.Exists(exe + "\\Playlists\\" + json["Config"]["Playlists"][i]["PlaylistID"] + ".png"))
                        {
                            txtbox.AppendText("\n\nCouldn't find Playlist cover for Playlist " + json["Config"]["Playlists"][i]["PlaylistID"] + ". Skipping.");
                        }
                        else
                        {
                            File.Move(exe + "\\Playlists\\" + json["Config"]["Playlists"][i]["PlaylistID"] + ".png", exe + "\\BeatSaberSongs\\" + PlaylistN + "\\playlistcover.png");
                        }
                    }
                    
                } else
                {
                    Directory.CreateDirectory(exe + "\\BeatSaberSongs");
                    if (!File.Exists(exe + "\\Playlists\\" + json["Config"]["Playlists"][i]["PlaylistID"] + ".png"))
                    {
                        txtbox.AppendText("\n\nCouldn't find Playlist cover for Playlist " + json["Config"]["Playlists"][i]["PlaylistID"] + ". Skipping.");
                    }
                    else
                    {
                        File.Move(exe + "\\Playlists\\" + json["Config"]["Playlists"][i]["PlaylistID"] + ".png", exe + "\\BeatSaberSongs\\playlistcover.png");
                    }
                }

                int SongNumber = 0;
                ArrayList alphabet = new ArrayList(){ "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
                
                for (int x = 0; json["Config"]["Playlists"][i]["SongList"][x]["SongID"] != null; x++)
                {
                    String dat = "";
                    if(File.Exists(exe + "\\CustomSongs\\" + json["Config"]["Playlists"][i]["SongList"][x]["SongID"] + "\\Info.dat"))
                    {
                        dat = File.ReadAllText(exe + "\\CustomSongs\\" + json["Config"]["Playlists"][i]["SongList"][x]["SongID"] + "\\Info.dat");
                    } else if (File.Exists(exe + "\\CustomSongs\\" + json["Config"]["Playlists"][i]["SongList"][x]["SongID"] + "\\info.dat"))
                    {
                        dat = File.ReadAllText(exe + "\\CustomSongs\\" + json["Config"]["Playlists"][i]["SongList"][x]["SongID"] + "\\info.dat");
                    } else
                    {
                        txtbox.AppendText("\n\nADB didn't copy everything. Skipping Song ID " + json["Config"]["Playlists"][i]["SongList"][x]["SongID"]);
                        SongNumber++;
                        continue;
                    }
                    var Info = JSON.Parse(dat);
                    String SongN = Info["_songName"].ToString();

                    SongN = SongN.Replace("/", "");
                    SongN = SongN.Replace(":", "");
                    SongN = SongN.Replace("*", "");
                    SongN = SongN.Replace("?", "");
                    SongN = SongN.Replace("\"", "");
                    SongN = SongN.Replace("<", "");
                    SongN = SongN.Replace(">", "");
                    SongN = SongN.Replace("|", "");

                    for (int f = 0; f < SongN.Length; f++)
                    {
                        if (SongN.Substring(f, 1).Equals("\\"))
                        {
                            SongN = SongN.Substring(0, f - 1) + SongN.Substring(f + 1, SongN.Length - f - 1);
                        }
                    }

                    SongN = SongNumber + "_" + SongN;

                    if (json["Config"]["Playlists"][i]["PlaylistID"] == "CustomSongs")
                    {
                        Directory.Move(exe + "\\CustomSongs\\" + json["Config"]["Playlists"][i]["SongList"][x]["SongID"], exe + "\\BeatSaberSongs\\" + SongN);
                    }
                    else
                    {
                        int PlaylistI = 0;
                        Playlists.TryGetValue(json["Config"]["Playlists"][i]["PlaylistName"], out PlaylistI);
                        if (PlaylistI == 0)
                        {
                            Directory.Move(exe + "\\CustomSongs\\" + json["Config"]["Playlists"][i]["SongList"][x]["SongID"], exe + "\\BeatSaberSongs\\" + json["Config"]["Playlists"][i]["PlaylistName"] + "\\" + SongN);
                        } else
                        {
                            Directory.Move(exe + "\\CustomSongs\\" + json["Config"]["Playlists"][i]["SongList"][x]["SongID"], exe + "\\BeatSaberSongs\\" + json["Config"]["Playlists"][i]["PlaylistName"] + " " + PlaylistI + "\\" + SongN);
                        }
                    }

                    SongNumber++;
                }
            }

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                txtbox.AppendText("\n\nPushing Sorted Songs to Quest.");
                txtbox.ScrollToEnd();
            }));
            if (!adb("shell rm -r /sdcard/BeatSaberSongs")) return;
            if (!adb("push \"" + exe + "\\BeatSaberSongs\" /sdcard/"))
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    txtbox.AppendText("\n\nYou need to copy the songs manually to the root of your Quest.");
                    txtbox.ScrollToEnd();
                }));
                return;
            }

            txtbox.AppendText("\n\nfinished");
        }

        private String getChars(ArrayList alphabet, int songNumber)
        {
            String songN = songNumber.ToString();
            String end = "";
            for(int i = 0; i < songN.Length; i++)
            {
                String current = songN.Substring(i, 1);
                end = end + (string)alphabet[Convert.ToInt32(current)];
            }
            return end;
        }
    }
}