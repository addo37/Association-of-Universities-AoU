using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerData
{
    [Serializable]
    [ProtoContract]
    public class Packet
    {
        [ProtoMember(1, OverwriteList = true)]
        public List<string> data { get; set; }
        [ProtoMember(2)]
        public string senderID { get; set; }
        [ProtoMember(3)]
        public PacketType packetType { get; set; }

        public Packet()
        {
            data = new List<string>();
        }

        public Packet(PacketType type, string senderID)
        {
            data = new List<string>();
            this.senderID = senderID;
            this.packetType = type;
        }

        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<Packet>(ms, this);
                return ms.ToArray();
            }
        }

        public static string GetIP4Address()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

            return (from ip in ips
                    where ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                    select ip)
                   .FirstOrDefault()
                   .ToString();
        }
    }
}
