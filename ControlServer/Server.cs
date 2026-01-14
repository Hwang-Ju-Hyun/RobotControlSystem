//using System.Net;
//using System.Net.Sockets;
//using ControlServer;
//using Core.Core.Services;
//using Core_lib.Core.Protocol;


class Server
{
    static async Task Main(string[] args)
    {
        return;
    }
}

//    static async Task HandleClientAsync(TcpClient client)
//    {
//        ClientSession session = new ClientSession();
//        NetworkStream stream = client.GetStream();
//        byte[] buffer = new byte[1024];

//        try
//        {                        
//            while (true)
//            {
//                /*ReadAsync 
//                 return : length of buffer
//                */
//                int bytesRead_length = await stream.ReadAsync(buffer, 0, buffer.Length);
//                if (bytesRead_length == 0)
//                    break;

//                byte[] received = buffer.Take(bytesRead_length).ToArray();
//                session.AppendData(received);

//                foreach(Packet packet in session.ExtractPackets())
//                {
//                    RobotMessage robot_msg=ProtocolParser.FromPacket(packet);
//                    Console.WriteLine($"[RECV] Robot_ID : {robot_msg.RobotId}, Type : {robot_msg.Type}, State : {robot_msg.State}");

//                    if(robot_msg.Type==MessageType.STATUS)
//                    {                        
//                        RobotManager.GetInstance.UpdateStatus(robot_msg.RobotId, robot_msg.State);
//                    }
//                }
//                //ReceivedData(buffer, bytesRead_length);
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex.Message);
//        }
//        finally
//        {
//            client.Close();
//            Console.WriteLine("Client Disconnected");
//        }
//    }

//    //byte[] → Packet → RobotMessage
//    static void ReceivedData(byte[] buffer,int length)
//    {
//        byte[] data = new byte[length];
//        Array.Copy(buffer, data, length);

//        Packet packet = Packet.Deserialize(data);
//        RobotMessage robot_msg=ProtocolParser.FromPacket(packet);

//        Console.WriteLine($"[RECV] Robot_ID : {robot_msg.RobotId}, Type : {robot_msg.Type}, Data : {robot_msg.State}");
//    }
//}