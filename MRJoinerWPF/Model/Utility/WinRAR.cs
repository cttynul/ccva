using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MRJoinerWPF
{
    class WinRAR
    {

        public static string doesWinRarExist()
        {
            string winrarPath = String.Empty;
            if (File.Exists("C:\\Program Files\\WinRAR\\WinRar.exe"))
            {
                winrarPath = "\"C:\\Program Files\\WinRAR\\WinRar.exe\"";
            }
            else if (File.Exists("C:\\Program Files (x86)\\WinRAR\\WinRar.exe"))
            {
                winrarPath = "\"C:\\Program Files (x86)\\WinRAR\\WinRar.exe\"";
            }
            else
            {
                MessageBox.Show("Seems you havent WinRar installed on your system, you can't use without it", "Cant find WinRar", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return winrarPath;
        }

        public static void COMPRESS(string destname,string dir)
        {
            Process process = new Process();
            process.StartInfo.FileName = @"" + doesWinRarExist();
            process.StartInfo.Arguments = @"a -ep " + "\"" + destname + "\"" + " " + "\"" + dir+ "\"";
            process.Start();
            process.WaitForExit();
            

        }

        public static void EXTRACT(string sourceRAR,string dest)
        {
            Process process = new Process();
            process.StartInfo.FileName = @"" + doesWinRarExist();
            process.StartInfo.Arguments = @"e -o+ " + "\"" + sourceRAR + "\"" + " *.* " + "\"" + dest+"\"";
            process.Start();
            process.WaitForExit();
        }



    }
}
