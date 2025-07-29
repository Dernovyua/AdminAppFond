using AdminPanelApp.Logic;
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
    /// Логика взаимодействия для Transactions.xaml
    /// </summary>
    public partial class Transactions 
    {
        public Transactions()
        {
            InitializeComponent();

            DtgdTransaction.ItemsSource = LogicData.Transactions;
        }

        private void BtnAddTransaction_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelTransaction_Click(object sender, RoutedEventArgs e)
        {

        }


        private void MnitEditTransaction_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MnitAddTransaction_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MnitDelTransaction_Click(object sender, RoutedEventArgs e)
        {

        }


        private void DtgdTransaction_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
