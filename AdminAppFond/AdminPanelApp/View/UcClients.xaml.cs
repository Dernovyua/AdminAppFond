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

namespace AdminPanelApp.View
{
    /// <summary>
    /// Логика взаимодействия для UcClients.xaml
    /// </summary>
    public partial class UcClients
    {
        public UcClients()
        {
            InitializeComponent();
        }

        private void DtgdClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void DtgdClients_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClient win = new AddClient();
            win.ShowDialog();
        }

        private void BtnCopyClient_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelClient_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MnitAddClient_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MnitCopyClient_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MnitEditClient_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Редактировать клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnitDelClient_Click(object sender, RoutedEventArgs e)
        {
                
        }

        private void DtgdAccounts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButtonAddAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDeleteAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MnitAddAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MnitEditAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MnitDelAccount_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
