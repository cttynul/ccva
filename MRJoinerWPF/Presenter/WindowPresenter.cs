using Microsoft.Win32;
using MRJoinerWPF.Model.CryptAlgorithm;
using MRJoinerWPF.utility;
using MRJoinerWPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MRJoinerWPF.Presenter
{
    class WindowPresenter : Presenter
    {
        private class ComboData
        {
            public string name { get; set; }
            public Type type { get; set; }
        }
        private bool encryption = false;
        private string fileToBeOpened = String.Empty;
        private string fileToOverride = String.Empty;
        private string fileToDecrypt = String.Empty;
        private string[] filesToEncrypt;
        private string currentDir = String.Empty;
        private string JoinedFolder = String.Empty;

        private Thread encrypting;
        private Thread decrypting;


        public MainWindow View
        {
            get { return (MainWindow)Element; }
        }


        public WindowPresenter(MainWindow view) : base(view)
        {
            Load();
            //select file buttons
            View.FileOverrideButton.Click += onFileToOverrideClick;
            View.FileZipButton.Click += onFileToZipClick;
            View.FileToDecryptButton.Click += onFileToDecryptClick;
            View.OpenFileButton.Click += onOpenFileClick;

            //start cose button
            View.StartDecrypt.Click += onStartDecryptClick;
            View.StartJoin.Click += onStartJoinClick;
            View.StartOpenFile.Click += onStartOpenFileClick;

            //radioGroup
            View.YesEncrypt.Checked += onYesEncryptChanged;
            View.NoEncrypt.Checked += onNoEncryptChanged;

        }

        private void Load()
        {

            if (String.IsNullOrWhiteSpace(WinRAR.doesWinRarExist()))
            {
                Application.Current.Shutdown();
            }


            List<ComboData> ListData = new List<ComboData>();

            var type = typeof(IAlgorithm);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => type.IsAssignableFrom(p) && !p.IsInterface);

            foreach (Type t in types)
            {
                ListData.Add(new ComboData { name = t.Name, type = t });
            }

            View.AlgorithmListEn.ItemsSource = ListData;
            View.AlgorithmListEn.DisplayMemberPath = "name";
            View.AlgorithmListEn.SelectedValuePath = "type";

            View.AlgorithmListEn.SelectedIndex = 0;

            View.AlgorithmListDec.ItemsSource = ListData;
            View.AlgorithmListDec.DisplayMemberPath = "name";
            View.AlgorithmListDec.SelectedValuePath = "type";

            View.AlgorithmListDec.SelectedIndex = 0;
        }


        private void onNoEncryptChanged(object sender, EventArgs e)
        {
            View.PasswordEncrypt.IsEnabled = false;
            encryption = false;
            View.AlgorithmListEn.IsEnabled = false;
        }

        private void onYesEncryptChanged(object sender, EventArgs e)
        {
            View.PasswordEncrypt.IsEnabled = true;
            encryption = true;
            View.AlgorithmListEn.IsEnabled = true;
        }


        //seleziona il file da aprire senza Crypt
        private void onOpenFileClick(object sender, EventArgs e)
        {
            OpenFileDialog result = new OpenFileDialog();
            result.Multiselect = false;
            if (result.ShowDialog() == true)
            {
                fileToBeOpened = result.FileName;
                View.OpenFile.Text = "";
                View.OpenFile.Text += "\"" + Path.GetFileName(fileToBeOpened) + "\" ";
            }
        }

        private void onFileToDecryptClick(object sender, EventArgs e)
        {
            OpenFileDialog result = new OpenFileDialog();
            result.Multiselect = false;
            if (result.ShowDialog() == true)
            {
                View.PasswordDecrypt.IsEnabled = true;

                fileToDecrypt = result.FileName;
                View.FileToDecrypt.Text = "";

                View.FileToDecrypt.Text += "\"" + Path.GetFileName(fileToDecrypt) + "\" ";


                currentDir = Path.GetDirectoryName(fileToDecrypt);
                View.OutputDecryptFile.Text = currentDir + "\\Decrypted";

            }
        }

        private void onFileToZipClick(object sender, EventArgs e)
        {
            OpenFileDialog result = new OpenFileDialog();
            result.Multiselect = true;

            if (result.ShowDialog() == true)
            {
                filesToEncrypt = result.FileNames;
                if (checkFilesDim())
                {
                    View.FileZip.Text = "";
                    foreach (string s in filesToEncrypt)
                    {

                        View.FileZip.AppendText("\"" + Path.GetFileName(s) + "\" ");
                    }
                }
            }
            result.Reset();
        }

        private bool checkFilesDim()
        {
            foreach (string s in filesToEncrypt)
            {
                FileInfo singleFile = new FileInfo(s);

                if (singleFile.Length > 2147483648)
                {
                    MessageBox.Show("Attenzione, il file " + singleFile.Name + " supera i 2 gb, selezionare file minori di 2 gb", Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        private void onFileToOverrideClick(object sender, EventArgs e)
        {
            OpenFileDialog result = new OpenFileDialog();
            result.Multiselect = false;
            if (result.ShowDialog() == true)
            {
                fileToOverride = result.FileName;
                View.FileOverride.Text = "";
                View.FileOverride.Text = fileToOverride;
                currentDir = Path.GetDirectoryName(fileToOverride);
                JoinedFolder = Path.GetDirectoryName(fileToOverride) + "\\Joined";
                View.OutputFile.Text = JoinedFolder;
            }
        }



        //Start openFile No Encrypt buttons Event
        private void onStartOpenFileClick(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(View.OpenFile.Text))
            {
                if (decrypting != null && decrypting.IsAlive)
                {
                    MessageBox.Show("Waiting decryption...", Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    Action StartLoop;
                    StartLoop = () => CryptoCore.OpenWithoutCrypt(fileToBeOpened, View.ProgressWithoutDecryptBar);

                    decrypting = new Thread(StartLoop.Invoke);

                    decrypting.Start();
                }
            }
            else
            {
                MessageBox.Show("Missing File to Open !", Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private void onStartJoinClick(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(View.FileOverride.Text) && !String.IsNullOrWhiteSpace(View.FileZip.Text))
            {
                if (encrypting != null && encrypting.IsAlive)
                {
                    MessageBox.Show("Waiting encryption...", Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    //lancio view
                    if (encryption)
                    {
                        string passwordEN = View.PasswordEncrypt.Password;
                        IAlgorithm selected = getAlgorithmFromComboEn();

                        Action StartLoop;
                        StartLoop = () => CryptoCore.EncryptAndAppend(fileToOverride, filesToEncrypt, passwordEN, selected, View.ProgressEncryptBar);

                        encrypting = new Thread(StartLoop.Invoke);

                        encrypting.Start();

                        View.PasswordEncrypt.Password = String.Empty;
                    }
                    else
                    {
                        Action StartLoop;
                        StartLoop = () => CryptoCore.EncryptAndAppend(fileToOverride, filesToEncrypt, View.progressEncrypt);

                        encrypting = new Thread(StartLoop.Invoke);

                        encrypting.Start();

                    }
                }

            }
            else MessageBox.Show("Something is missed!", Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //TODO:gestire caso di fallimento dato da res=false

        }

        private void onStartDecryptClick(object sender, EventArgs e)
        {

            if (!String.IsNullOrWhiteSpace(View.FileToDecrypt.Text) && !String.IsNullOrWhiteSpace(View.PasswordDecrypt.Password))
            {
                if (decrypting != null && decrypting.IsAlive)
                {
                    MessageBox.Show("Waiting decryption...", Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    string passwordDEC = View.PasswordDecrypt.Password;

                    try
                    {
                        IAlgorithm selected = getAlgorithmFromComboDec();

                        Action StartLoop;
                        StartLoop = () => CryptoCore.ExtractAndDecrypt(fileToDecrypt, passwordDEC, selected, View.ProgressDecryptBar);

                        decrypting = new Thread(StartLoop.Invoke);

                        decrypting.Start();
                        
                    }
                    catch (UnauthorizedAccessException e1)
                    {
                        MessageBox.Show("Incorrect password" + e1.Message);
                        View.PasswordDecrypt.Password = String.Empty;
                        if (Directory.Exists(Path.GetDirectoryName(fileToDecrypt) + "\\Decrypted")) Directory.Delete(Path.GetDirectoryName(fileToDecrypt) + "\\Decrypted", true);
                        return;
                    }

                    View.PasswordDecrypt.Password = String.Empty;
                }
            }
            else MessageBox.Show("Select a file first! OR Insert a password !", Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }


        private IAlgorithm getAlgorithmFromComboDec()
        {
            Type selected = Type.GetType(View.AlgorithmListDec.SelectedValue.ToString());
            IAlgorithm res = (IAlgorithm)Activator.CreateInstance(selected);
            return res;
        }

        private IAlgorithm getAlgorithmFromComboEn()
        {
            Type selected = Type.GetType(View.AlgorithmListEn.SelectedValue.ToString());
            IAlgorithm res = (IAlgorithm)Activator.CreateInstance(selected);
            return res;
        }
    }
}
