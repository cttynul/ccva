using MRJoinerWPF.Presenter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MRJoinerWPF;

namespace MRJoinerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            GestoreView.GetInstance().MostraView(typeof(MainWindow), typeof(WindowPresenter));

        }
     
    }
}
