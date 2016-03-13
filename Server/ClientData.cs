using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerData;

namespace Server
{
    class ClientData
    {
        public Thread clientThread;
        public string id;
        public Socket clientSocket;

        public ClientData()
        {
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_IN);
            clientThread.Start(clientSocket);
            sendConnectPacketToClient();
        }

        public ClientData(Socket clientSocket)
        {
            clientSocket.DontFragment = true;
            this.clientSocket = clientSocket;
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_IN);
            clientThread.Start(clientSocket);
            sendConnectPacketToClient();

        }
        
        public void sendConnectPacketToClient()
        {
            Packet p = new Packet(PacketType.Connect, "server");
            IPEndPoint IP = clientSocket.RemoteEndPoint as IPEndPoint;
            p.data.Add(id);
            clientSocket.Send(p.ToBytes());
        }
    }
}
