using AdminPanelApp.Logic;
using AdminPanelApp.Models;
using AdminPanelApp.Models.Scenario;
using AdminPanelApp.Requests;
using ClassControlsAndStyle.Dialogs;
using DevExpress.Utils.Extensions;
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
using System.Windows.Shapes;

namespace AdminPanelApp.View.Scenario
{
    /// <summary>
    /// Логика взаимодействия для ScenarioView.xaml
    /// </summary>
    public partial class ScenarioView 
    {
        public ScenarioView()
        {
            InitializeComponent();


            DtgdScenario.ItemsSource = LogicData.Scenarios;
        }

        private void BtnAddTgMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DtgdTgMenu_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnDelTgMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MnitAddTgMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MnitEditTgMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MnitDelTgMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddScenario_Click(object sender, RoutedEventArgs e)
        {
            AddScenario();
        }

        private void BtnDelScenario_Click(object sender, RoutedEventArgs e)
        {
            Del();
        }

        private void DtgdScenario_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Edit();
        }

        private void MnitAddScenario_Click(object sender, RoutedEventArgs e)
        {
            AddScenario();
        }

        private void MnitEditScenario_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        private void MnitDelScenario_Click(object sender, RoutedEventArgs e)
        {
            Del();
        }

        private void Del()
        {
            if (DtgdScenario.SelectedItem is TgMenuModel tgMenuModel)
            {
                try
                {
                    if (new DialogOkCancel("Вы действительно хотите удалить?",
                    LanguageModel.GetString(LanguageDialogMessageKeys.CaptionAttentionKey))
                    .Result == MessageBoxResult.OK)
                    {
                        //TransactionRequests.DeleteTransaction(transaction.Id);
                        LogicData.Scenarios.Remove(tgMenuModel);
                    }
                }
                catch (Exception ex)
                {
                    new DialogMessage(ex.Message, "Ошибка");
                }
            }

        }

        private void Edit()
        {
            if (DtgdScenario.SelectedItem is TgMenuModel tgMenuModel)
            {
                try
                {
                    AddScenario edit = new AddScenario(tgMenuModel);
                    edit.ShowDialog();
                }
                catch (Exception ex)
                {
                    new DialogMessage(ex.Message, "Ошибка");
                }
            }
        }

        private void AddScenario()
        {
            TgMenuModel tgMenuModel = new TgMenuModel();

            AddScenario add = new AddScenario(tgMenuModel);
            add.ShowDialog();
        }
    }
}
