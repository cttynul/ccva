using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MRJoinerWPF.utility;
using MRJoinerWPF.Model.CryptAlgorithm;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls;

namespace MRJoinerWPF
{

    public class CryptoCore
    {
        private static void UpdateProgressBar(ProgressBar progress, double value)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { progress.SetValue(ProgressBar.ValueProperty, value); }, null);
        }
        private static bool tmpFiles(string fileToOver, string[] fileToZip)
        {
            string currentDir = Path.GetDirectoryName(fileToOver);
            string dest = Path.GetDirectoryName(fileToOver) + "\\Joined";
            bool res = false;
            try
            {
                Directory.Delete(currentDir + "\\temp_zip", true);
            }
            catch (IOException)
            {
                res = false;
            }

            try
            {
                Directory.Delete(dest, true);
            }
            catch (IOException)
            {
                res = false;
            }

            //zip files
            try
            {
                Directory.CreateDirectory(currentDir + "\\temp_zip");
                res = true;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Errore nella creazione di Temp_zip" + ex.Message);
                return false;
            }
            string copiedFilesPath = currentDir + "\\temp_zip";

            //cartella zip del fileToOver
            foreach (string s in fileToZip)
            {
                try
                {
                    File.Copy(s, copiedFilesPath + "\\" + Path.GetFileName(s));
                }
                catch (IOException e2)
                {
                    MessageBox.Show("Errore nella copia dei files" + e2.Message);
                    return false;
                }
            }
            return res;
        }
        private static void joinFiles(string fileToOver, ProgressBar progress)
        {
            string dest = Path.GetDirectoryName(fileToOver) + "\\Joined";
            string currentDir = Path.GetDirectoryName(fileToOver);
            string copiedFilesPath = currentDir + "\\temp_zip";
            //join files e cazzo cancella i file temporanei
            string rarFile = Path.ChangeExtension(fileToOver, ".rar");
            try
            {
                Directory.CreateDirectory(dest);
                UpdateProgressBar(progress, 85.0);
            }
            catch (IOException)
            {
                MessageBox.Show("fallito a creare la cartella di destinazione: " + dest);

            }

            cmd.runCommand("copy /b \"" + fileToOver + "\"+" + "\"" + rarFile + "\" \"" + dest + "\\" + Path.GetFileName(fileToOver) + "\"");
            UpdateProgressBar(progress, 90.0);
            File.Delete(rarFile);
            UpdateProgressBar(progress, 95.0);
            cmd.runCommand("rmdir /s /q" + " " + "\"" + copiedFilesPath + "\"");
            UpdateProgressBar(progress, 100.0);
            MessageBox.Show("File(s) joined!", Assembly.GetEntryAssembly().GetName().Name);
            UpdateProgressBar(progress, 0.0);
        }
        private static void Append(string fileToOver, string[] fileToZip, ProgressBar progress)
        {
            bool res = false;
            string currentDir = Path.GetDirectoryName(fileToOver);
            string copiedFilesPath = currentDir + "\\temp_zip";

            res = tmpFiles(fileToOver, fileToZip);
            UpdateProgressBar(progress, 40.0);
            //compressione FILES & Join
            if (res)
            {
                WinRAR.COMPRESS(Path.ChangeExtension(fileToOver, ".rar"), copiedFilesPath);
                UpdateProgressBar(progress, 70.0);
                joinFiles(fileToOver, progress);
            }
        }
        public static void EncryptAndAppend(string fileToOver, string[] fileToZip, ProgressBar progress)
        {
            Append(fileToOver, fileToZip, progress);
        }
        public static void EncryptAndAppend(string fileToOver, string[] fileToZip, string password, IAlgorithm encrypter, ProgressBar progress)
        {

            bool res = false;
            string currentDir = Path.GetDirectoryName(fileToOver);
            string copiedFilesPath = currentDir + "\\temp_zip";

            if (String.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Encryption Password Missing! Please retry!", "Missing Password!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            UpdateProgressBar(progress, 10.0);

            res = tmpFiles(fileToOver, fileToZip);

            UpdateProgressBar(progress, 20.0);

            int allFile = Directory.GetFiles(copiedFilesPath).Length;
            int i = 1;

            if (!String.IsNullOrWhiteSpace(password) && res)
            {
                foreach (string s in Directory.GetFiles(copiedFilesPath))
                {
                    EncryptFile(s, password, encrypter);
                    UpdateProgressBar(progress, (20 + ((i / (double)allFile) * 40)));
                    i++;
                }

            }
            else
            {
                MessageBox.Show("Encryption Password Missing! Please retry!", "Missing Password!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            //compressione
            WinRAR.COMPRESS(Path.ChangeExtension(fileToOver, ".rar"), copiedFilesPath);
            UpdateProgressBar(progress, 80.0);
            joinFiles(fileToOver, progress);
        }
        public static void ExtractAndDecrypt(string file, string password, IAlgorithm decrypter, ProgressBar progress)
        {
            UpdateProgressBar(progress, 10.0);
            Directory.CreateDirectory(Path.GetDirectoryName(file) + "\\EncryptedFiles");
            string rarFile = Path.ChangeExtension(file, ".rar");
            File.Copy(file, rarFile, true);
            UpdateProgressBar(progress, 20.0);

            WinRAR.EXTRACT(rarFile, Path.GetDirectoryName(rarFile) + "\\EncryptedFiles");
            UpdateProgressBar(progress, 40.0);

            int allFiles = (Directory.GetFiles(Path.GetDirectoryName(file) + "\\EncryptedFiles")).Length;
            int i = 1;

            foreach (string f in Directory.GetFiles(Path.GetDirectoryName(file) + "\\EncryptedFiles"))
            {
                DecryptFile(f, password, Path.GetDirectoryName(file), decrypter);
                UpdateProgressBar(progress, (40 + (i / (double)allFiles) * 30));
                i++;
            }

            cmd.runCommand("rmdir /s /q" + " " + "\"" + Path.GetDirectoryName(rarFile) + "\\EncryptedFiles" + "\"");
            UpdateProgressBar(progress, 85.0);
            File.Delete(rarFile);
            UpdateProgressBar(progress, 100.0);
            MessageBox.Show("File decrypted!", Assembly.GetEntryAssembly().GetName().Name);
            UpdateProgressBar(progress, 0.0);

        }
        public static bool EncryptFile(string source, string password, IAlgorithm encrypter)
        {
            string path = source;
            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show("No Such File");
                return false;
            }

            byte[] bytes = File.ReadAllBytes(path);
            byte[] encrypted_bytes = encrypter.encrypt(bytes, password);


            if (System.IO.File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllBytes(path, encrypted_bytes);
            return true;
        }
        public static bool DecryptFile(string file, string password, string destFolder, IAlgorithm decrypter)
        {
            string path = file;


            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show("No Such File");
                return false;
            }

            byte[] bytes = File.ReadAllBytes(file);
            byte[] decrypted_bytes = null;
            try
            {
                decrypted_bytes = decrypter.decrypt(bytes, password);
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show("Incorrect Password: {0}", e.Message);
                return false;
            }


            string tempF = destFolder + "\\Decrypted";
            if (!Directory.Exists(tempF)) Directory.CreateDirectory(tempF);

            path = tempF + "\\" + Path.GetFileName(file);


            if (!System.IO.File.Exists(path))
            {
                FileStream fs = System.IO.File.Create(path);
                fs.Close();
            }

            File.WriteAllBytes(path, decrypted_bytes);

            return true;
        }

        public static void OpenWithoutCrypt(string fileToOpen,ProgressBar progress)
        {
            UpdateProgressBar(progress, 10.0);
            Directory.CreateDirectory(Path.GetDirectoryName(fileToOpen) + "\\Files");
            string rarFile = Path.ChangeExtension(fileToOpen, ".rar");
            File.Copy(fileToOpen, rarFile, true);
            UpdateProgressBar(progress, 40.0);
            //estrazione
            WinRAR.EXTRACT(rarFile, Path.GetDirectoryName(rarFile) + "\\Files");
            UpdateProgressBar(progress, 80.0);
            //eliminazione archivio temporaneo
            File.Delete(rarFile);
            UpdateProgressBar(progress, 100.0);
            MessageBox.Show("File extracted! :D", Assembly.GetEntryAssembly().GetName().Name);
            UpdateProgressBar(progress, 0.0);
        }
    }
}
