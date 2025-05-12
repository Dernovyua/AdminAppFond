using AdminPanelApp.View;
using DevExpress.CodeParser;
using DevExpress.Xpf.Docking;
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
using System.Windows.Threading;
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
            CheckOpenDocument(((MenuItem)sender).Header.ToString());
        }

        /// <summary>
        /// Проверяем открытые DocumentContent и в случае отсутствия добавляем
        /// </summary>
        /// <param name="sender">name DocumentContent</param>
        public void CheckOpenDocument(string namePanel)
        {
            var panel = FindPanel(namePanel);

            if (panel != null)
            {
                panel.IsActive = true;
                return;
            }

            PublishNewDocument(namePanel);
        }

        /// <summary>
        /// Проверка открытых окон для модулей
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public bool CheckOpenDocumentBool(string namePanel)
        {
            bool result = true;
            var panel = FindPanel(namePanel);

            if (panel != null)
            {
                panel.IsActive = true;
                return false;
            }

            return result;
        }

        /// <summary>
        /// Поиск панели
        /// </summary>
        /// <param name="name"></param>
        internal DocumentPanel FindPanel(string name)
        {
            if (DlmMain.LayoutRoot.Items.Count > 0)
            {
                foreach (var item in DlmMain.LayoutRoot.Items)
                {
                    var itemGroup = item as DocumentGroup;
                    if (itemGroup != null)
                        foreach (var itempanel in itemGroup.Items)
                        {
                            if (itempanel.Caption != null)
                            {
                                if (itempanel.Caption.ToString() == name)
                                    return itempanel as DocumentPanel;
                            }
                        }
                }

                foreach (var item in DlmMain.FloatGroups)
                {
                    foreach (var itempanel in item.Items)
                    {
                        if (itempanel.Caption != null)
                        {
                            if (itempanel.Caption.ToString() == name)
                                return itempanel as DocumentPanel;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Поиск панели
        /// </summary>
        /// <param name="name"></param>
        internal void ClosePanel(string name)
        {
            if (DlmMain.LayoutRoot.Items.Count > 0)
            {
                foreach (var item in DlmMain.LayoutRoot.Items)
                {
                    var itemGroup = item as DocumentGroup;
                    if (itemGroup != null)
                        foreach (var itempanel in itemGroup.Items)
                        {
                            if (itempanel.Caption != null)
                            {
                                if (itempanel.Caption.ToString() == name)
                                {
                                    itemGroup.Remove(itempanel);
                                    break;
                                }
                            }
                        }
                }
            }
        }

        public void AddDockPanelModuls(DocumentPanel panel)
        {
            if (panel.Caption == null)
                return;

            var panl = FindPanel(panel.Caption.ToString());
            if (panl == null)
            {
                if (DlmMain.LayoutRoot.Items.Count > 0)
                {
                    foreach (var item in DlmMain.LayoutRoot.Items)
                    {
                        var itemGroup = item as DocumentGroup;
                        if (itemGroup != null)
                            itemGroup.Add(panel);
                    }
                }
                else
                {
                    DlmMain.LayoutRoot.Items.Add(new DocumentGroup());
                    foreach (var item in DlmMain.LayoutRoot.Items)
                    {
                        var itemGroup = item as DocumentGroup;
                        if (itemGroup != null)
                            itemGroup.Add(panel);
                    }
                }
            }
            else
            {
            }
            panel.IsActive = true;

        }

        /// <summary>
        /// Open DocumentConent
        /// </summary>
        /// <param name="baseDocTitle">name DocumentContent</param>
        private void PublishNewDocument(string baseDocTitle)
        {

            if (baseDocTitle == "Клиенты")
            {
                UcClients uc = new UcClients();
                AddDockPanelModuls(uc);
                return;
            }

      

        }


        private void DlmMain_DockItemClosed(object sender, DevExpress.Xpf.Docking.Base.DockItemClosedEventArgs e)
        {
            for (int i = DlmMain.ClosedPanels.Count - 1; i >= 0; i--)
            {
                DlmMain.ClosedPanels.RemoveAt(i);
            }
        }
    }
}
