using AdminPanelApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Logic
{
    public static class LogicData
    {
        public static event Func<string, double> OnGetBalance;

        public static double Raise_OnGetBalance(string publicKey)
        {
            if (OnGetBalance != null)
            {
                // Вызов всех подписчиков, взять результат первого
                foreach (Func<string, double> handler in OnGetBalance.GetInvocationList())
                {
                    return handler(publicKey);
                }
            }
            return 0;
        }

        public static ObservableCollection<Client> Clients = new ObservableCollection<Client>();
    }
}
