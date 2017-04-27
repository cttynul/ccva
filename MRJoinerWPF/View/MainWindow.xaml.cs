using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MRJoinerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        public TextBox FileOverride
        {
            get
            {
                return this.textFileToOverride;
            }
            set
            {
                this.textFileToOverride = value;
            }
        }
        public Button FileOverrideButton
        {
            get
            {
                return this.buttonFileToOverride;
            }
        }
        public TextBox FileZip
        {
            get
            {
                return this.textFileToZip;
            }
            set
            {
                this.textFileToZip = value;
            }
        }
        public Button FileZipButton
        {
            get
            {
                return this.buttonFilesToZip;
            }
        }
        public RadioButton NoEncrypt
        {
            get
            {
                return this.noEnc;
            }
        }
        public RadioButton YesEncrypt
        {
            get
            {
                return this.yesEnc;
            }
        }
        public PasswordBox PasswordEncrypt
        {
            get
            {
                return this.textPass;
            }
        }
        public TextBox OutputFile
        {
            get
            {
                return this.textOutputFile;
            }
            set
            {
                this.textOutputFile = value;
            }
        }
        public Button StartJoin
        {
            get
            {
                return this.buttonStartJoin;
            }
        }
        public ComboBox AlgorithmListEn
        {
            get
            {
                return this.crypterListAlgorithm;
            }
        }
        public TextBox OpenFile
        {
            get
            {
                return this.textBoxOpenFile;
            }
            set
            {
                this.textBoxOpenFile = value;
            }
        }
        public Button OpenFileButton
        {
            get
            {
                return this.buttonFileToOpen;
            }
        }
        public Button StartOpenFile
        {
            get
            {
                return this.buttonStartOpenFile;
            }
        }
        public TextBox FileToDecrypt
        {
            get
            {
                return this.textFileToDecrypt;
            }
            set
            {
                this.textFileToDecrypt = value;
            }
        }
        public Button FileToDecryptButton
        {
            get
            {
                return this.buttonFileToDecrypt;
            }
        }
        public PasswordBox PasswordDecrypt
        {
            get
            {
                return this.textPassDecrypt;
            }
        }
        public TextBox OutputDecryptFile
        {
            get
            {
                return this.textOutFile;
            }
            set
            {
                this.textOutFile = value;
            }
        }
        public Button StartDecrypt
        {
            get
            {
                return this.buttonStartDecrypt;
            }
        }
       

        public TabControl TabChooser
        {
            get
            {
                return this.tabControl;
            }
            set
            {
                this.tabControl = value;
            }

        }
        public ComboBox AlgorithmListDec
        {
            get
            {
                return this.algorithmListDec;
            }
        }

        public ProgressBar ProgressEncryptBar
        {
            get
            {
                return progressEncrypt;
            }
        }

        public ProgressBar ProgressDecryptBar
        {
            get
            {
                return progressDecrypt;
            }
        }

        public ProgressBar ProgressWithoutDecryptBar
        {
            get
            {
                return progressBarWithoutDecrypt;
            }
        }
    }
}