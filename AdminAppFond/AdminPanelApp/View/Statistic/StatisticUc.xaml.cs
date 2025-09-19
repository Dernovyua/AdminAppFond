using AdminPanelApp.Logic;
using AdminPanelApp.Models;
using AdminPanelApp.Requests;
using ClassControlsAndStyle.Dialogs;
using ETS.Resources;
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

namespace AdminPanelApp.View.Statistic
{
    /// <summary>
    /// Логика взаимодействия для StatisticUc.xaml
    /// </summary>
    public partial class StatisticUc : UserControl
    {
        public StatisticUc()
        {
            InitializeComponent();
        }
        AccountModel _acc;

        public void Load(AccountModel acc)
        {
            if (acc != null)
                DtgdStatDetail.ItemsSource = acc.Statistics;
            else
                DtgdStatDetail.ItemsSource = null;
            _acc = acc;
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
            if (_acc == null)
                return;

            StatisticModel statistic = new StatisticModel
            {
                Date = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            AddStatistic add = new AddStatistic(statistic, _acc.StatResult);
            add.ShowDialog();

            if (add.DialogResult == true)
            {
                StatisticRequests.AddStatistic(statistic);
                _acc.Statistics.Add( statistic);
                //LogicData.GetStatisticsAsync();
                //LogicData.Statistics.Add(statistic); // Предполагается, что у AccountModel есть коллекция Statistics
            }
        }

        private void EditStatistic()
        {
            if (DtgdStatDetail.SelectedItem is StatisticModel statistic)
            {
                AddStatistic edit = new AddStatistic(statistic, _acc.StatResult);
                edit.ShowDialog();

                if (edit.DialogResult == true)
                {
                    StatisticRequests.UpdateStatistic(statistic);
                    //LogicData.GetStatisticsAsync();
                }
            }
        }

        private void DeleteStatistic()
        {
            if (DtgdStatDetail.SelectedItem is StatisticModel statistic)
            {
                if (new DialogOkCancel("Вы действительно хотите удалить запись статистики?",
                    LanguageModel.GetString(LanguageDialogMessageKeys.CaptionAttentionKey))
                    .Result == MessageBoxResult.OK)
                {
                    StatisticRequests.DeleteStatistic(statistic.Id);
                    _acc.Statistics.Remove(statistic);
                    //LogicData.GetStatisticsAsync();
                    //LogicData.StatisticDisplay.Remove(statistic);
                }
            }
        }
    }
}
