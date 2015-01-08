using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO.Compression;
using System.Security.Cryptography;


namespace Zulu_Update
{
    public partial class MainForm : Form
    {

        public enum RunApplications
        {
            None,
            Client,
            Injection,
        }

        public enum UpdateStates
        {
            Ready,
            Updating,
            Downloading,
            FinishedDownloading,            
        }

        public const string CFG_FILENAME = "zulu_update.xml";
        public XmlDocument config;
        public Uri project_uri;

        public RunApplications runWhenFinished;

        public UpdateStates update_state;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!loadConfigFile())
            {
                MessageBox.Show("Can't read configuration file!");
                Application.Exit();
            }
            try
            {
                project_uri = new Uri(getCfgValue("projecturl"));
            }
            catch (UriFormatException)
            {
                MessageBox.Show("Malformed projecturl tag in the configuration file!");
                Application.Exit();
            }

            var value = new Uri(project_uri, getCfgValue("updates_page_url", "updates.html"));            

            webBrowser1.Navigate(value); 
            update_state = UpdateStates.Ready;

            this.Text = getCfgValue("application_title");

            runWhenFinished = (RunApplications)getCfgValueInt("runWhenFinished");

            workInBackground.DoWork += new DoWorkEventHandler(UpdateTheClient);
            workInBackground.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdateTheClientCompleted);
            workInBackground.ProgressChanged += new ProgressChangedEventHandler(UpdateTheClientProgressChanged);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            saveConfigFile();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            if (workInBackground.IsBusy && update_state != UpdateStates.Ready)
                return;          

            btnUpdate.Enabled = false;
            update_state = UpdateStates.Updating;

            workInBackground.RunWorkerAsync();

            return;

            /*
            if (update_state == UpdateStates.Ready || update_state == UpdateStates.Failed)
            {
                //System.Threading.Thread.Sleep(5000);
                updateInBackground.RunWorkerAsync();                
            }
             */

            /*
            if (update_state == UpdateStates.Ready || update_state == UpdateStates.Failed)
            {
                update_state = UpdateStates.Updating;
                ((Button)sender).Text = "Checking updates..";
                ((Button)sender).Enabled = false;

                lblStatus.Text = "Contacting Updates Site ..";

                updateInBackground.DoWork += updateInBackground_DoWork;

                var res = checkUpdates();
                if (res == true)
                {
                    lblStatus.Text = "All done...";                    
                    ((Button)sender).Text = "Play "+getCfgValue("shardname", "UO");
                    update_state = UpdateStates.Success;
                    if (getCfgValueInt("closeWhenFinished") == 1)
                    {
                        RunClientApplication();
                        Application.Exit();
                    }
                }
                else
                {
                    update_state = UpdateStates.Failed;
                    ((Button)sender).Text = "Check Updates ..";
                }
                ((Button)sender).Enabled = true;
            } else
                if (update_state == UpdateStates.Success)
                {
                    RunClientApplication();
                    Application.Exit();
                }
             * 
             */
        }

        private void UpdateTheClient(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Getting updates list ...");
            ((BackgroundWorker)sender).ReportProgress(1, ("Getting update list from: "+ project_uri ));
            string updatesListRaw = DownloadUpdatesListRaw();

            if (updatesListRaw == null)
            {
                var res = MessageBox.Show("Unable to read the updates URL!",
                   "Error updating!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var line in updatesListRaw.Split('\n'))
            {
                if (line.Trim().Length == 0)
                    continue;

                string[] fileInfoReceived = line.Trim().Split(',');
                Dictionary<string, string> remoteFileInfo =
                    new Dictionary<string, string>();

                remoteFileInfo.Add("filename", fileInfoReceived[0]);
                remoteFileInfo.Add("size", fileInfoReceived[1]);
                remoteFileInfo.Add("crc32b", fileInfoReceived[2]);
                remoteFileInfo.Add("md5_hash", fileInfoReceived[3]);
                remoteFileInfo.Add("local_filename", GetLocalFileName(remoteFileInfo["filename"]));                               

                ((BackgroundWorker)sender).ReportProgress(1, new string(("Checking " + Path.GetFileName(remoteFileInfo["local_filename"])).ToCharArray()));

                if (IsLocalFileNeedsUpdating(remoteFileInfo))
                {                    
                    Console.WriteLine("local file {0} needs to be updated", remoteFileInfo["filename"]);
                    update_state = UpdateStates.Downloading;
                    //
                    ((BackgroundWorker)sender).ReportProgress(1, remoteFileInfo);
                    while (update_state == UpdateStates.Downloading)
                    {
                        System.Threading.Thread.Sleep(150);
                        Console.WriteLine("Waiting for download to finish");
                    }

                    Console.WriteLine("Received {0}", remoteFileInfo["filename"]);
                    
                    if (File.Exists(remoteFileInfo["local_filename"]))
                    {
                        Console.WriteLine("Deleting the old file: {0}", remoteFileInfo["local_filename"]);
                        File.Delete(remoteFileInfo["local_filename"]);
                    }

                    var local_downloaded_filename = Path.Combine(getCfgValue("ultima_dir"), remoteFileInfo["filename"]);

                    Console.WriteLine("renaming {0} -> {1}", local_downloaded_filename + ".part", local_downloaded_filename);
                    File.Move(local_downloaded_filename + ".part", local_downloaded_filename);
                                        

                    if(isZip( remoteFileInfo["filename"] )) {
                        Console.WriteLine("Unzipping: {0}", local_downloaded_filename );
                        
                        unzipFile( local_downloaded_filename );
                        File.Delete( local_downloaded_filename );                                            
                    }            
          
                }                                             
            }                    
        }

        private void UpdateTheClientCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            update_state = UpdateStates.Ready;
            btnUpdate.Enabled = true;

            RunClientApplication();

            if(getCfgValueInt("closeWhenFinished")==1) {
                Application.Exit();
            }
        }

        private void UpdateTheClientProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            if (e.UserState is String)
            {
                lblStatus.Text = (string)e.UserState;
            } else if (e.UserState is IDictionary) {
                var remoteFileInfo = (Dictionary<string, string>)e.UserState;
                Console.WriteLine("Downloading {0}", remoteFileInfo["filename"]);
                downloadFile(remoteFileInfo["filename"]);
            }
        }

        private string DownloadUpdatesListRaw()
        {
            // create a new instance of WebClient
            var client = new WebClient();

            // set the user agent
            client.Headers.Add("user-agent", getCfgValue("user_agent"));
            try
            {
                // actually execute the GET request
                string ret = client.DownloadString(new Uri(project_uri, "/updates/list_updates.php"));
                return ret;
            }
            catch (WebException we)
            {
                // WebException.Status holds useful information
                Console.WriteLine(we.Message + "\n" + we.Status.ToString());
            }
            return null;
        }

        private string GetLocalFileName(string remote_filename)
        {
            string local_filename = null;

            if (Path.GetExtension(remote_filename) == ".zip") // remove .zip extention
            {
                local_filename = remote_filename.Substring(0, remote_filename.Length - 4);
            }

            local_filename = getCfgValue("ultima_dir") + @"\" + local_filename;

            return local_filename;
        }

        private string GetLocalMD5Hash(string local_flename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(local_flename))
                {
                 
                    var md5_hash_bytes = md5.ComputeHash(stream);

                    var res = BitConverter.ToString(md5_hash_bytes).Replace("-", "").ToLower();
                    return res;
                }
            }
        }

        bool IsLocalFileNeedsUpdating(Dictionary<string, string> remoteFileInfo)
        {

            string local_filename = remoteFileInfo["local_filename"];

            Console.WriteLine("Local filename: {0}", local_filename);

            if (!File.Exists(local_filename))
            {
                Console.WriteLine("Doesn't exist!");
                return true;
            }

            var localfile_size = (new FileInfo(local_filename)).Length;

            if ( /*(localfile_size < 1000000) || */ localfile_size != Int32.Parse(remoteFileInfo["size"]) )
            {
                Console.WriteLine("Size missmatch!");
                return true;
            }

            string local_crc32 = null;
            try
            {
                local_crc32 = getFileCrc32(local_filename);
            }
            catch (IOException)
            {
                lblStatus.Text = "File is locked.";

                var res = MessageBox.Show(string.Format("{0} is locked by another application!", System.IO.Path.GetFileName(local_filename)),
                    "Error updating!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                return false;
            }

            if (remoteFileInfo.ContainsKey("md5_hash"))
            {
                Console.WriteLine("Local md5: {0}, remote md5: {1}", new Object[] {GetLocalMD5Hash(local_filename), remoteFileInfo["md5_hash"]});

                if (GetLocalMD5Hash(local_filename) != remoteFileInfo["md5_hash"]) {
                    Console.WriteLine("MD5 hash missmatch!");
                    return false;
                }

            } else 
            if ((remoteFileInfo["crc32b"] != local_crc32) && (remoteFileInfo["crc32b"] != local_crc32.Substring(1) || local_crc32[0] != '0'))
            {
                Console.WriteLine("crc32b misssmatch!");
                return false;
            }

            return false;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm frmSettings = new SettingsForm();
            frmSettings.ShowDialog(this);
        }

        #region running client applicaition
        private void RunClientApplication()
        {
            switch (runWhenFinished)
            {
                case RunApplications.None:
                    break;
                case RunApplications.Client:
                    System.Diagnostics.Process.Start(
                        Path.Combine(getCfgValue("ultima_dir"), "client.exe" ));
                    break;
                case RunApplications.Injection:
                    foreach (string path in new string[] { "", @"injection\" })
                    {
                        foreach (string launcher_name in new string[] { "ilaunch.exe", "Launcher.exe" })
                        {
                            var app_path = Path.Combine(Path.Combine(getCfgValue("ultima_dir"), path), launcher_name);
                            Console.WriteLine("app_path: " + app_path);
                            if (File.Exists(app_path))
                            {
                                var start_info = new System.Diagnostics.ProcessStartInfo(Path.GetFullPath(app_path));
                                start_info.WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(app_path));

                                System.Diagnostics.Process.Start(start_info);
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        #endregion

        #region File Download Handling

        public void downloadFile(string remote_filename)
        {            

            lblStatus.Text = "Downloading " + remote_filename;            
            string local_path = Path.Combine( getCfgValue("ultima_dir"), remote_filename ) + ".part";

            Console.WriteLine("downloadFile: {0} to {1}", remote_filename, local_path);    

            var client = new WebClient();
            client.Headers.Add("user-agent", getCfgValue("user_agent"));            
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted += client_DownloadFileCompleted;            
            client.DownloadFileAsync(new Uri(project_uri, "updates/" + remote_filename), remote_filename + ".part");
            
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            update_state = UpdateStates.FinishedDownloading;
            return;           
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progBarDownload.Value = (int) ((e.BytesReceived * 100 ) / e.TotalBytesToReceive);
        }

        #endregion   

        #region zip hangling

        public bool isZip(string filename)
        {
            if (Path.GetExtension(filename).ToLower() == ".zip")
            {
                return true;
            }
            return false;
        }

        public void unzipFile(string archive_name)
        {
            // Open an existing zip file for reading
            ZipStorer zip = ZipStorer.Open(archive_name, FileAccess.Read);            

            string local_filename = null;

            if (Path.GetExtension(archive_name) == ".zip") // remove .zip extention
            {
                local_filename = Path.GetFileName(archive_name.Substring(0, archive_name.Length - 4));
            }

            // Read the central directory collection
            List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

            // Look for the desired file
            foreach (ZipStorer.ZipFileEntry entry in dir)
            {
                string zippedFilename = Path.GetFileName(entry.FilenameInZip);

                //Console.WriteLine("Unzip zippedFilename: {0}", zippedFilename);
                //Console.WriteLine("Unzip local_file: {0}", local_filename);

                if (!zippedFilename.Equals(local_filename, StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Unzip skipping file: {0}", zippedFilename);
                    continue;
                }

                zip.ExtractFile(entry, Path.Combine(Path.GetDirectoryName(archive_name), zippedFilename));

                if (zippedFilename == CFG_FILENAME)
                {
                    Console.WriteLine("Reloading config file ...");
                    loadConfigFile();
                }
            }

            zip.Dispose();           
        }

        #endregion

        #region Config File


        public string getCfgValue(string name, string default_value)
        {                        
            string value;
            try 
	        {	        
		        value = config["settings"][name].InnerText;
	        }
	        catch (NullReferenceException)
	        {
                return default_value;
	        }

            return value;
        }

        public string getCfgValue(string name)
        {
            return getCfgValue(name, "");
        }

        public int getCfgValueInt(string name)
        {
            var value = getCfgValue(name);
            if( value != "" )
                return Int32.Parse(config["settings"][name].InnerText);

            return 0;
        }

        public void setCfgValue(string name, string value)
        {
            updateCfgValue(name, value);
            saveConfigFile();
        }

        public void setCfgValue(string name, int value)
        {
            updateCfgValue(name, value.ToString());
            saveConfigFile();
        }

        public void updateCfgValue(string name, string value)
        {
            try
            {
                config["settings"][name].InnerText = value;
            }
            catch (NullReferenceException)
            {
                var new_element = config.CreateElement(name);               
                new_element.InnerText = value;
                config["settings"].AppendChild(new_element);                
            }            
        }

        public bool loadConfigFile()
        {
            config = new XmlDocument();
            try
            {
                config.Load(CFG_FILENAME);
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            return true;
        }

        public void saveConfigFile()
        {
            try
            {
                config.Save(CFG_FILENAME);
            }
            catch (Exception)
            {
                lblStatus.Text = "Error saving config file.";
            }            
        }

        #endregion

        public string getFileCrc32(string filename)
        {
            return getFileCrc32(filename, null);
        }

        public string getFileCrc32(string filename, BackgroundWorker sender)
        {
            Crc32 crc32 = new Crc32();
            String hash = String.Empty;

            using (FileStream fs = File.Open(filename, FileMode.Open))
                foreach (byte b in crc32.ComputeHash(fs)) hash += b.ToString("x2").ToLower();
            return hash;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
