using AdminPanelApp.Logic;
using ClassControlsAndStyle.Dialogs;
using ETS.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace AdminPanelApp.View
{
    /// <summary>
    /// Логика взаимодействия для SettingCRM.xaml
    /// </summary>
    public partial class SettingCRM
    {
        public SettingCRM()
        {
            InitializeComponent();

            TxbxTokenAdmin.Text = LogicData.SettingCrm.TgTokenCrm;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            LogicData.SettingCrm.TgTokenCrm= TxbxTokenAdmin.Text;

            DialogResult = true;
     
            Close();
        }

        private void BtnGide_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://etstrading.ru/bz_nastroyki_uvedomleniya") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                new DialogMessage(LanguageModel.GetString(LanguageCommonKeys.ErrorOpenKey) + " " + ex.Message, LanguageModel.GetString(LanguageDialogMessageKeys.CaptionAttentionKey));
            }
        }


    }
}
