using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanelApp.Models.Scenario
{
    public interface  IScenarioModel 
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsRun { get; set; }

    }
}
