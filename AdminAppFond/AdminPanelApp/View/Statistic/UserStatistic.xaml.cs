using AdminPanelApp.Logic;
using AdminPanelApp.Models;
using AdminPanelApp.Requests;
using ClassControlsAndStyle.Dialogs;
using ETS.Resources;
using Ex.UI.Kit;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для UserStatistic.xaml
    /// </summary>
    public partial class UserStatistic
    {
        public UserStatistic()
        {
            InitializeComponent();

            //DtgdStat.ItemsSource = LogicData.StatisticDisplay;
        }

        private void DtgdStat_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditStatistic();
        }

        private void BtnAddStat_Click(object sender, RoutedEventArgs e)
        {
            AddStatistic();
        }

        private void BtnDelStat_Click(object sender, RoutedEventArgs e)
        {
            DeleteStatistic();
        }

        private void MnitAddStat_Click(object sender, RoutedEventArgs e)
        {
            AddStatistic();
        }

        private void MnitEditStat_Click(object sender, RoutedEventArgs e)
        {
            EditStatistic();
        }

        private void MnitDelStat_Click(object sender, RoutedEventArgs e)
        {
            DeleteStatistic();
        }

        private void AddStatistic()
        {

            StatisticModel statistic = new StatisticModel
            {
                Date = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            if (DtgdStat.SelectedItem is StatisticDisplayModel stat)
            {
                //AddStatistic add = new AddStatistic(statistic, stat);
                //add.ShowDialog();

                //if (add.DialogResult == true)
                //{
                //    StatisticRequests.AddStatistic(statistic);
                //    LogicData.GetStatisticsAsync();
                //    //LogicData.Statistics.Add(statistic); // Предполагается, что у AccountModel есть коллекция Statistics
                //}
            }
        }

        private void EditStatistic()
        {
            if (DtgdStatDetail.SelectedItem is StatisticModel statistic &&
                DtgdStat.SelectedItem is StatisticDisplayModel stat)
            {
                //AddStatistic edit = new AddStatistic(statistic, stat);
                //edit.ShowDialog();

                //if (edit.DialogResult == true)
                //{
                //    StatisticRequests.UpdateStatistic(statistic);
                //    //LogicData.GetStatisticsAsync();
                //}
            }
        }

        private void DeleteStatistic()
        {
            if (DtgdStatDetail.SelectedItem is StatisticModel statistic)
            {
                //if (new DialogOkCancel("Вы действительно хотите удалить запись статистики?",
                //    LanguageModel.GetString(LanguageDialogMessageKeys.CaptionAttentionKey))
                //    .Result == MessageBoxResult.OK)
                //{
                //    StatisticRequests.DeleteStatistic(statistic.Id);
                //    LogicData.GetStatisticsAsync();
                //    //LogicData.StatisticDisplay.Remove(statistic);
                //}
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LogicData.GetStatisticsAsync();
        }


      
    }


}
