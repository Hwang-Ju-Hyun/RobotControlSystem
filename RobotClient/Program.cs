using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Interop;
using Core.Core.Services;
using Core.Domain;
using Core_lib.Core.Domain;
using Core_lib.Core.Protocol;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    class NetworkClient
    {
        private TcpClient client;
        public NetworkStream stream;

        public async Task Connect(string ip, int port)
        {
            client = new TcpClient();
            await client.ConnectAsync(ip, port);
            stream = client.GetStream();
        }

        public async Task Send(RobotMessage msg)
        {
            Packet packet = ProtocolParser.ObjectToPacket(msg);
            byte[] bytes = packet.Serialize();
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
    static RobotController controller = null;
    static int RobotID = -1;
    static async Task Main()
    {
        NetworkClient nc = new NetworkClient();
        await nc.Connect("192.168.45.246", 5000);        

        Console.WriteLine("Connected to server");
        controller=new RobotController();
        ClientSession session = new ClientSession();


        // 1️ 최초 STATUS 전송
        RobotMessage status = new RobotMessage
        {
            //RobotId = 1,
            Type = MessageType.STATUS,
            State = RobotState.IDLE
        };

        //RobotMessage → Packet
        Packet statusPacket = ProtocolParser.ObjectToPacket(status);

        //Packet → byte
        byte[] statusBytes = statusPacket.Serialize();

        await nc.Send(status);
        Console.WriteLine("STATUS sent");


        _ = SendLoop(nc,status);

        // 2️ 서버 메시지 수신 루프
        byte[] buffer = new byte[1024];
        RobotMessage msg = null;
        while (true)
        {
            int bytesRead = await nc.stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                Console.WriteLine("Server disconnected");
                break;
            }

            byte[] received = buffer.Take(bytesRead).ToArray();
            session.AppendData(received);

            foreach (Packet packet in session.ExtractPackets())
            {
                status = ProtocolParser.PacketToObject(packet);

                Console.WriteLine($"[RECV From Server] Type = {status.Type}, RobotId={status.RobotId}");                
                
                if (status.Type == MessageType.ID_ASSIGN)
                {
                    RobotID = status.RobotId;
                    Console.WriteLine($"Assigned RobotId = {RobotID}");
                    await nc.Send(status);
                }
                // 3️ MOVE 명령을 받았을 경우
                if (status.Type == MessageType.MOVE)
                {
                    Console.WriteLine("MOVE Command received");
  
                    foreach (var p in status.Path)
                    {                                                                        
                        controller.CurrentPath.Enqueue(p);                        
                    }
                    // TODO: 여기서 실제 이동 로직 실행
                    // ex) 로컬 경로 큐에 추가                                                    
                }
            }
        }

        
    }

    static async Task SendLoop(NetworkClient nc,RobotMessage status)
    {       
        while (true)
        {                        
            if (controller.HasPath)
            {                
                PathNode next = controller.Tick();

                status = new RobotMessage
                {
                    Type = MessageType.STATUS,
                    RobotId = RobotID,
                    Row = next.Row,
                    Col = next.Col,
                    State = RobotState.MOVING
                };                
            }
            else if (!controller.HasPath&&status.State==RobotState.MOVING)
            {
                status = new RobotMessage
                {
                    Type = MessageType.STATUS,
                    RobotId = RobotID,
                    Row = -1,
                    Col = -1,
                    State = RobotState.END
                };
            }            
            await nc.Send(status);
            await Task.Delay(300);
        }
    }
    //Client Send Status message to Server 
    private static async Task SendClientStatus(RobotMessage msg,NetworkStream stream)
    {        
        Packet packet = ProtocolParser.ObjectToPacket(msg);
        byte[] bytes = packet.Serialize();
        await stream.WriteAsync(bytes,0, bytes.Length);
    }
}
