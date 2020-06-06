using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;
namespace Controller_Share
{
    class ControllerBridge
    {
        List<vJoy> controllers;
        ControllerBridge(List<vJoy> _controllers)
        {
            controllers = _controllers;
            InitializevJoyControllers();
        }
        void SendControllerDataToDriver()
        { }
        private void InitializevJoyControllers()
        {
            foreach (var controller in controllers)
            {

            }
        }
    }
}
