using System.Net.Sockets;
using Core_lib.Core.Domain;
using Core_lib.Core.Protocol;

class Program
{
    static async Task Main()
    {
        TcpClient client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", 5000);

        NetworkStream stream = client.GetStream();

        RobotMessage message = new RobotMessage { RobotId = 1, Type = MessageType.STATUS, State = RobotState.IDLE};

        Packet packet = ProtocolParser.ToPacket(message);
        byte[] bytes = packet.Serialize();

        await stream.WriteAsync(bytes, 0, bytes.Length);

        Console.WriteLine("Message Sent");
        
        while (true)
        {
            string a = Console.ReadLine();
            if(int.Parse(a)==1)
            {
                break;
            }
        }
        
    }
}