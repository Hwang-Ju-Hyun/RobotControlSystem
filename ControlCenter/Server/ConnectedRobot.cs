using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Core.Domain;

namespace ControlCenter.Server
{
    class ConnectedRobot
    {
        public string Ip_Address {  get; set; }
        public int RobotID { get; set; }
        public string IP_ID {  get; set; }

        public TcpClient Client { get; set; }

        public NetworkStream stream => Client.GetStream();        
    }
}
