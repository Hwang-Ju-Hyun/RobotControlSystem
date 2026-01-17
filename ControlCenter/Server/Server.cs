using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Xml.Linq;
using ControlCenter;
using ControlCenter.Server;
using Core.Core.Services;
using Core.Domain;
using Core_lib.Core.Domain;
using Core_lib.Core.Protocol;

namespace ControlServer
{
    class Server
    {
        public ObservableCollection<ConnectedRobot> ConnectedClient {get;}        
        private int nextRobotId = 1;
        private readonly object idLock = new object();
        static int RobotId = 0;
        public Server()
        {
            ConnectedClient = new ObservableCollection<ConnectedRobot>();
        }


        Dictionary<int, ConnectedRobot> sessions = new Dictionary<int, ConnectedRobot>();
        public ConnectedRobot RegisterSession(int robotId,ConnectedRobot cr)
        {
            return (sessions[robotId]= cr);
        }

        public ConnectedRobot GetSession(int robotId)
        {
            return sessions[robotId];
        }

        public async Task Start()
        {
            const int port = 5000;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Debug.WriteLine("Control Server Started on port : 5000");


            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();                   
                IPEndPoint clientIpPoint = ((IPEndPoint)client.Client.RemoteEndPoint);
                string ip_str = clientIpPoint.ToString();

                ConnectedRobot cr = new ConnectedRobot();
                cr.Ip_Address = ip_str;
                cr.Client = client;
                cr.IP_ID = ip_str;

                lock (idLock)
                {
                    cr.RobotID = nextRobotId;
                    cr.IP_ID += (" : "+cr.RobotID);
                    nextRobotId++;
                }

                RobotMessage assign = new RobotMessage
                {
                    Type = MessageType.ID_ASSIGN,
                    RobotId = cr.RobotID
                };

                Send(ProtocolParser.ObjectToPacket(assign), cr);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ConnectedClient.Add(cr);
                });                  
                
                Debug.WriteLine($"Success Client Connected ID : {cr.RobotID}");

                _ = HandleClientAsync(client);
            }
        }

        async Task HandleClientAsync(TcpClient client)
        {
            ClientSession session = new ClientSession();
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            try
            {
                while (true)
                {
                    /*ReadAsync 
                     return : length of buffer
                    */
                    int bytesRead_length = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead_length == 0)
                        break;

                    byte[] received = buffer.Take(bytesRead_length).ToArray();
                    session.AppendData(received);

                    foreach (Packet packet in session.ExtractPackets())
                    {
                        RobotMessage robot_msg = ProtocolParser.PacketToObject(packet);
                        if (robot_msg.Type == MessageType.ID_ASSIGN)
                        {
                            Debug.WriteLine($"[RECV from Client] IP :{((IPEndPoint)client.Client.RemoteEndPoint).ToString()}, ID : {robot_msg.RobotId}, Type : {robot_msg.Type}, State : {robot_msg.State}");
                            RobotManager.GetInstance.UpdateStatus(robot_msg.RobotId, robot_msg.State);
                        }                        
                       
                        if (robot_msg.Type == MessageType.STATUS)
                        {
                            RobotManager.GetInstance.UpdateStatus(robot_msg.RobotId, robot_msg.State);
                        }
                        if(robot_msg.State== RobotState.MOVING)
                        {
                            Debug.WriteLine($"Row : {robot_msg.Row}, Col : {robot_msg.Col}");
                        }
                        else if(robot_msg.State == RobotState.END)
                        {
                            MessageBox.Show($"Robot : {robot_msg.RobotId}  Arrived");                            
                        }
                    }
                    //ReceivedData(buffer, bytesRead_length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
                Debug.WriteLine("Client Disconnected");
            }
        }

        //byte[] → Packet → RobotMessage
        static void ReceivedData(byte[] buffer, int length)
        {
            byte[] data = new byte[length];
            Array.Copy(buffer, data, length);

            Packet packet = Packet.Deserialize(data);
            RobotMessage robot_msg = ProtocolParser.PacketToObject(packet);

            Console.WriteLine($"[RECV] Robot_ID : {robot_msg.RobotId}, Type : {robot_msg.Type}, Data : {robot_msg.State}");
        }

        public void  Send(Packet packet,ConnectedRobot robot)
        {
            byte[] bytes = packet.Serialize();
            robot.stream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
