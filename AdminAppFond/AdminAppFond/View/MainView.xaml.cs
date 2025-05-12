using DevExpress.CodeParser;
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
using TerminalETS.Model;


namespace AdminAppFond.View
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView
    {

        internal PanelClass _panelClass = new PanelClass();
        public MainView()
        {
            InitializeComponent();
        }


        internal void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(9);
        }

        private void OpenModuleView(object sender, RoutedEventArgs e)
        {

        }

        private void DlmMain_DockItemClosed(object sender, DevExpress.Xpf.Docking.Base.DockItemClosedEventArgs e)
        {

        }
    }
}
