using DevExpress.Xpf.Docking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalETS.Model
{
    public class PanelClass
    {
        #region Обновляем заголовк в главном окне

        public delegate void UpdateTitile(string title);

        public event UpdateTitile OnUpdateTitile;

        public void Raise_UpdateTitile(string title)
        {
            if (null != OnUpdateTitile)
                OnUpdateTitile(title);
        }
        #endregion

        #region Поиск панели и возврщение ее

        public delegate DocumentPanel FindPanel(string title);

        public event FindPanel? OnFindPanel;

        public DocumentPanel? Raise_FindPanel(string title)
        {
            if (null != OnFindPanel)
                return OnFindPanel(title);

            return null;
        }

        #endregion


        #region Поиск панели и возврщение ее

        public delegate bool CheckOpenPanel(string title);

        public event CheckOpenPanel? OnCheckOpenPanel;

        public bool Raise_CheckOpenPanel(string title)
        {
            if (null != OnCheckOpenPanel)
                return OnCheckOpenPanel(title);

            return false;
        }

        #endregion

        #region Закрытие панели

        public delegate void ClosePanel(string title);

        public event ClosePanel? OnClosePanel;

        public void Raise_ClosePanel(string title)
        {
            if (null != OnClosePanel)
                OnClosePanel(title);

        }

        #endregion

        #region Добавление панели

        public delegate void AddPanel(DocumentPanel panel);

        public event AddPanel? OnAddPanel;

        public void Raise_AddPanel(DocumentPanel panel)
        {
            if (null != OnAddPanel)
                OnAddPanel(panel);
        }

        #endregion
    }
}
