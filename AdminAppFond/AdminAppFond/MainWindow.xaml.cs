using AdminAppFond.View;
using System.Text;
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

namespace AdminAppFond
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainView View => MainView;

        public MainWindow()
        {
            InitializeComponent();

            Title = "Панель управления " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            View._panelClass.OnUpdateTitile += _panelClass_OnUpdateTitile;
        }

        /// <summary>
        /// Creat list documentContent for save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            View.OnClosing(e);
        }

        private void _panelClass_OnUpdateTitile(string title)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Title = "Панель управления " +
                                       System.Reflection.Assembly.GetExecutingAssembly().GetName().Version +
                                       title;
                }
                ), DispatcherPriority.Normal, null);
            }
            else
            {
                Title = "Панель управления " +
                                   System.Reflection.Assembly.GetExecutingAssembly().GetName().Version +
                                   title;
            }
        }



    }
}