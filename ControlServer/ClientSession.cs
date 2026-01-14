//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Core_lib.Core.Protocol;

//namespace ControlServer
//{
//    public class ClientSession
//    {
//        private readonly List<byte> buffer= new List<byte>();

//        public void AppendData(byte[] data)
//        {
//            buffer.AddRange(data);
//        }

//        public IEnumerable<Packet> ExtractPackets()
//        {
//            List<Packet> packets = new List<Packet>();

//            while(true)
//            {
//                if(buffer.Count<7)
//                {
//                    //[Header] : 2byte
//                    //[Length] : 4byte
//                    //[Type]   : 1byte                    
//                    break;
//                }
//                ushort header = BitConverter.ToUInt16(buffer.ToArray(), 0);
//                if(header!=Packet.HEADER)
//                {
//                    buffer.RemoveAt(0);
//                    continue;
//                }

//                int length = BitConverter.ToInt32(buffer.ToArray(), 2);
//                int packetSize = 7 + length;

//                if (buffer.Count < packetSize)
//                    break;
                
//                byte[] packetBytes=buffer.Take(packetSize).ToArray();

//                buffer.RemoveRange(0, packetSize);

//                packets.Add(Packet.Deserialize(packetBytes));
//            }
//            return packets;
//        }
//    }
//}