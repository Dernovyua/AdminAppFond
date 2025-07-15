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
        public static ObservableCollection<Client> Clients = new ObservableCollection<Client>();
    }
}
