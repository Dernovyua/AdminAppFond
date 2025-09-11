using AdminPanelApp.Logic;
using AdminPanelApp.Models.Scenario;
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
using static DevExpress.Utils.Filtering.ExcelFilterOptions;

namespace AdminPanelApp.View.Scenario
{
    /// <summary>
    /// Логика взаимодействия для AddScenario.xaml
    /// </summary>
    public partial class AddScenario
    {
        private TgMenuModel _tgMenuModel;

        public AddScenario(TgMenuModel tgMenuModel)
        {
            InitializeComponent();
            _tgMenuModel = tgMenuModel;
            LoadTgMenuModelData();
        }

        private void LoadTgMenuModelData()
        {
            if (_tgMenuModel == null)
                return;

            TxbxName.Text = _tgMenuModel.Name ?? string.Empty;
            //TxbxType.Text = _tgMenuModel.Type ?? string.Empty;
            TxbxNameMenu.Text = _tgMenuModel.NameMenu ?? string.Empty;
            //TxbxIcon.Text = _tgMenuModel.Icon ?? string.Empty;
            TxbxColumn.Text = _tgMenuModel.Column.ToString();
            TxbxRow.Text = _tgMenuModel.Row.ToString();
            TxbxText.Text = _tgMenuModel.Text ?? string.Empty;
            //TxbxPathToDocument.Text = _tgMenuModel.PathToDocument ?? string.Empty;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TxbxName.Text) ||
                String.IsNullOrEmpty(TxbxNameMenu.Text) ||
                String.IsNullOrEmpty(TxbxText.Text))
                return;

            if (String.IsNullOrEmpty(_tgMenuModel.Type))
                LogicData.Scenarios.Add(_tgMenuModel);

            _tgMenuModel.Name = TxbxName.Text.Trim();
            _tgMenuModel.Type = "Меню ТГ";
            _tgMenuModel.NameMenu = TxbxNameMenu.Text.Trim();
            //_tgMenuModel.Icon = string.IsNullOrWhiteSpace(TxbxIcon.Text) ? null : TxbxIcon.Text.Trim();

            _tgMenuModel.Column = string.IsNullOrWhiteSpace(TxbxColumn.Text) ? 0 : Convert.ToInt32(TxbxColumn.Text.Trim());
            _tgMenuModel.Row = string.IsNullOrWhiteSpace(TxbxRow.Text) ? 0 : Convert.ToInt32(TxbxRow.Text.Trim());

            _tgMenuModel.Text = TxbxText.Text.Trim();
            //_tgMenuModel.PathToDocument = string.IsNullOrWhiteSpace(TxbxPathToDocument.Text) ? null : TxbxPathToDocument.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
