using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace Core_lib.Core.Protocol
{
    public static class ProtocolParser
    {
        //RobotMessage → Packet
        public static Packet ToPacket(RobotMessage message)
        {
            string json = JsonSerializer.Serialize(message);
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(json);

            return new Packet(message.Type, payload);
        }

        //Packet → RobotMessage
        public static RobotMessage FromPacket(Packet packet)
        {
            //GetString : byte to Json format
            string json = System.Text.Encoding.UTF8.GetString(packet.Payload);
            /*Deseialize : Json format conver to Object*/
            return JsonSerializer.Deserialize<RobotMessage>(json);
        }
    }
}
