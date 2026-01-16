using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;


/*
  [ Header ]   [ Length ]    [ Type ]     [ Payload ]
 패킷시작표시   Payload길이   메시지타입     실제데이터 
    2byte        4byte        1byte         N byte
*/

namespace Core_lib.Core.Protocol
{
    public class Packet
    {
        public const ushort HEADER = 0xAA55;

        public MessageType Type { get; }
        public byte[] Payload { get; }
        
        public Packet(MessageType type, byte[] payload)
        {
            Type = type;
            if(Array.Empty<byte>()==payload)
            {
                throw new ArgumentException("데이터가 존재하지 않습니다.");
            }
            Payload = payload;
        }
        
        //Packet -> byte[]
        public byte[] Serialize()
        {
            MemoryStream mem_stream = new MemoryStream();
            BinaryWriter bw=new BinaryWriter(mem_stream);

            bw.Write(HEADER);
            bw.Write(Payload.Length);
            bw.Write((byte)Type);
            bw.Write(Payload);

            return mem_stream.ToArray();
        }

        //byte[] -> Packet
        static public Packet Deserialize(byte[] data)
        {
            MemoryStream mem_stream = new MemoryStream(data);
            BinaryReader br = new BinaryReader(mem_stream);

            ushort header = br.ReadUInt16();
            int length = br.ReadInt32();
            MessageType type = (MessageType)br.ReadByte();
            byte[] payload= br.ReadBytes(length);

            return new Packet(type, payload);
        }
    }
}
