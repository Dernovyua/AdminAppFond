using AdminPanelApp.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Logic
{
    public class AdminMain
    {
        //LogicData logicData = new LogicData();

        public void Execute(string path)
        {
            LogicDb.Main(path);

            LogicData.Clients = ClientsRequests.GetCliensOnLoad();
        }


    }
}
