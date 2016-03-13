using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.IO;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Net.Mail;

namespace Server
{
    class Server
    {
        static DBConnection db;
        static Socket listenerSocket;
        public static List<ClientData> _clients;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server...");
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<ClientData>();

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Packet.GetIP4Address()), 4242);
            listenerSocket.Bind(ip);

            db = DBConnection.Instance();
            db.IsConnect();
            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();

            Console.WriteLine("Success... Listening IP: " + Packet.GetIP4Address() + ":4242");
        }

        public static void Data_IN(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;
            byte[] buffer;
            int counter = 0;
            int readBytes;

            while (true)
            {
                try
                {
                    if (clientSocket.Connected)
                    {
                        buffer = new byte[clientSocket.SendBufferSize];
                        readBytes = clientSocket.Receive(buffer);

                        if (readBytes > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(buffer, 0, readBytes))
                            {
                                DataManager(ProtoBuf.Serializer.Deserialize<Packet>(ms));
                                continue;
                            }
                        }

                        if (readBytes == 0)
                        {
                            counter++;
                            if (counter >= 20)
                            {
                                break;
                            }
                        }
                    }
                    else
                        break;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Client Disconnected.");
                    
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("General Exception." + " " + ex.Message);
                    continue;
                }

            }
        }

        public static void ClearClient(Packet p)
        {
            var exitClient = GetClientByID(p);
            try
            {
                CloseClientConnection(exitClient);
                RemoveClientFromList(exitClient);
                Console.WriteLine("Client cleared.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong while clearing client.");
            }
        }

        static void ListenThread()
        {
            while (true)
            {
                listenerSocket.Listen(0);
                _clients.Add(new ClientData(listenerSocket.Accept()));
            }
        }

        public static void DataManager(Packet p)
        {
            Console.WriteLine("Processing packet...");
            switch (p.packetType)
            {
                case PacketType.Login:
                    Login(p);
                    break;
                case PacketType.AcceptScholarship:
                    acceptScholarship(p);
                    break;
                case PacketType.RejectScholarship:
                    rejectScholarship(p);
                    break;
                case PacketType.AcceptExchange:
                    acceptExchange(p);
                    break;
                case PacketType.RejectExchange:
                    rejectExchange(p);
                    break;
                case PacketType.AcceptFund:
                    acceptFund(p);
                    break;
                case PacketType.RejectFund:
                    rejectFund(p);
                    break;
                case PacketType.Chat:
                    Chat(p);
                    break;
                case PacketType.AddUser:
                    AddUser(p);
                    break;
                case PacketType.DemoteUser:
                    demoteUser(p);
                    break;
                case PacketType.PromoteUser:
                    promoteUser(p);
                    break;
                case PacketType.DeleteUser:
                    deleteUser(p);
                    break;
                case PacketType.GetLogs:
                    getLogs(p);
                    break;
                case PacketType.AddEvent:
                    addEvent(p);
                    break;
                case PacketType.Token:
                    genToken(p);
                    break;
                case PacketType.Reset:
                    resetUser(p);
                    break;
                default:
                    break;
            }
        }

        private static void genToken(Packet p)
        {
            MySqlDataReader dataReader = null;
            try
            {
                string query = "SELECT * FROM users_table where username='" + p.data[0] +"';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                dataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
            }

            string emailTo = "someone@domain.com";

            if (dataReader.Read())
            {
                if (!("" + dataReader["token"]).Equals(""))
                    return;

                emailTo = dataReader["email"] + "";

            }
            
            dataReader.Close();
            try
            {
                string token = Guid.NewGuid().ToString();
                string query = "UPDATE users_table SET token='" + token + "' WHERE username='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();


                string smtpAddress = "smtp.gmail.com";
                int portNumber = 587;
                bool enableSSL = true;

                string emailFrom = "ed2l.bot@gmail.com";
                string password = "aou123321aou";
                string subject = "User Token - Association of Universities";
                string body = "Dear " + p.data[0] + "<br/><br/>" + "We've received your request to change your password. Please use the following token:<br/><br/>" + token + "<br/>You may reply to this e-mail if you're facing any issues.<br/><br/>Kind regards,<br/>Association of Universities";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid user ID.");
            }

        }

        private static void resetUser(Packet p)
        {
            MySqlDataReader dataReader = null;
            try
            {
                string query = "SELECT * FROM users_table where username='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                dataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
            }

            if (dataReader.Read())
            {
                if (!("" + dataReader["token"]).Equals(p.data[1]))
                {
                    Packet p1 = new Packet(PacketType.TokenMismatch, "server");
                    GetClientByID(p).clientSocket.Send(p1.ToBytes());
                    return;
                }
            }
            else
                return;

            dataReader.Close();
            try
            {
                string query = "UPDATE users_table SET password='" + p.data[2] + "', token='' WHERE username='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();

                Packet p1 = new Packet(PacketType.ResetSuccess, "server");
                GetClientByID(p).clientSocket.Send(p1.ToBytes());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid user ID.");
            }
        }

        private static void addEvent(Packet p)
        {
            try
            {
                string query = "INSERT INTO events(name, date_start, date_end, description) VALUES ('" + p.data[0] + "','" + p.data[1].Replace('/','-') + "','" + p.data[2].Replace('/', '-') + "','" + p.data[3] + "');";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid event data.");
            }
        }

        private static void getLogs(Packet p)
        {
            StreamReader reader;

            Packet p1 = new Packet(PacketType.GetLogs, "server");
            String[] CurFiles = Directory.GetFiles(Directory.GetCurrentDirectory());
            String name = "";
            foreach (String s in CurFiles)
            {
                if (s.Length > 4)
                    if (s.Substring(s.Length - 4, 4).Equals(".txt"))
                    {
                        int i = s.Length - 5;
                        while (s.ElementAt(i) != '\\')
                        {
                            name = s.ElementAt(i) + name;
                            i--;
                        }
                        p1.data.Add(name);
                        reader = new StreamReader(@s);
                        p1.data.Add(reader.ReadToEnd());
                    }
                name = "";
            }

            GetClientByID(p).clientSocket.Send(p1.ToBytes());
        }

        private static void demoteUser(Packet p)
        {
            try
            {
                string query = "UPDATE users_table SET attribute='Member' WHERE user_id='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid user ID.");
            }
        }

        private static void promoteUser(Packet p)
        {
            try
            {
                string query = "UPDATE users_table SET attribute='Manager' WHERE user_id='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid user ID.");
            }
        }

        private static void deleteUser(Packet p)
        {
            try
            {
                string query = "Delete from users_table WHERE user_id='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid user ID.");
            }
        }

        private static void AddUser(Packet p)
        {
            try
            {
                string query = "INSERT INTO users_table(username, password, email, name, attribute, university) VALUES ('" + p.data[0] + "','" + p.data[1] + "','" + p.data[2] + "','" + p.data[3] + "','" + p.data[4] + "','" + p.data[5] + "');";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid user data.");
            }
        }

        private static void Chat(Packet p)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].clientSocket.Send(p.ToBytes());
            }
        }

        private static void acceptFund(Packet p)
        {
            try
            {
                string query = "UPDATE fund_request SET status='Accepted', amount_given='" + p.data[1] + "' WHERE application_id='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid ID.");
            }
        }

        private static void rejectFund(Packet p)
        {
            try
            {
                string query = "UPDATE fund_request SET status='Rejected', amount_given='' WHERE application_id='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid ID.");
            }
        }

        private static void acceptExchange (Packet p)
        {
            try
            {
                string query = "UPDATE exchange SET status='Accepted' WHERE application_id='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid ID.");
            }
        }

        private static void rejectExchange(Packet p)
        {
            try
            {
                string query = "UPDATE exchange SET status='Rejected' WHERE application_id='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid ID.");
            }
        }

        private static void acceptScholarship(Packet p)
        {
            try
            {
                string query = "UPDATE scholarships SET status='Accepted', percentage='" + p.data[1] + "' WHERE application_id='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid ID.");
            }
        }

        private static void rejectScholarship(Packet p)
        {
            try
            {
                string query = "UPDATE scholarships SET Status='Rejected' WHERE application_id='" + p.data[0] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid ID.");
            }
        }

        private static void Login (Packet p)
        {
            MySqlDataReader dataReader = null;
            try
            {
                string query = "SELECT * FROM users_table where username='" + p.data[0]+ "' and password='" + p.data[1] + "';";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                dataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
            }
            if (dataReader.Read())
            {
                p.data.Clear();
                p.data.Add("Success");
                p.data.Add((string)dataReader["name"]);
                p.data.Add((string)dataReader["attribute"]);
                GetClientByID(p).clientSocket.Send(p.ToBytes());
                dataReader.Close();
                sendLists(p);
            }
            else
            {
                p.data.Clear();
                p.data.Add("Failure");
                GetClientByID(p).clientSocket.Send(p.ToBytes());
            }

            dataReader.Close();
        }

        private static void sendLists(Packet p)
        {
            MySqlDataReader dataReader = null;
            Packet p1 = new Packet(PacketType.Scholarships, "server");
            try
            {
                string query = "SELECT * FROM scholarships;";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                dataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
            }
            
            while (dataReader.Read())
            {
                p1.data.Add("" + dataReader["application_id"]);
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("date_submission")));
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("valid_until")));
                p1.data.Add((string)dataReader["username"]);
                p1.data.Add((string)dataReader["name"]);
                p1.data.Add((string)dataReader["university"]);
                p1.data.Add((string)dataReader["father_name"]);
                p1.data.Add((string)dataReader["father_worktype"]);
                p1.data.Add((string)dataReader["father_workplace"]);
                p1.data.Add((string)dataReader["father_workaddress"]);
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("father_startdate")));
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("father_enddate")));
                p1.data.Add((string)dataReader["mother_name"]);
                p1.data.Add((string)dataReader["mother_worktype"]);
                p1.data.Add((string)dataReader["mother_workplace"]);
                p1.data.Add((string)dataReader["mother_workaddress"]);
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("mother_startdate")));
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("mother_enddate")));
                p1.data.Add("" + dataReader["siblings_number"]);
                p1.data.Add((string)dataReader["siblings_position"]);
                p1.data.Add("" + dataReader["father_income"]);
                p1.data.Add("" + dataReader["mother_income"]);
                p1.data.Add("" + dataReader["other_income"]);
                p1.data.Add("" + dataReader["expenses"]);
                p1.data.Add("" + dataReader["status"]);
                p1.data.Add("" + dataReader["percentage"]);
                p1.data.Add("END");
            }

            Thread.Sleep(100);
            GetClientByID(p).clientSocket.Send(p1.ToBytes());
            dataReader.Close();

            p1 = new Packet(PacketType.Funds, "server");
            try
            {
                string query = "SELECT * FROM fund_request;";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                dataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
            }

            while (dataReader.Read())
            {
                p1.data.Add("" + dataReader["application_id"]);
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("date_submission")));
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("valid_until")));
                p1.data.Add((string)dataReader["username"]);
                p1.data.Add((string)dataReader["name"]);
                p1.data.Add((string)dataReader["university"]);
                p1.data.Add((string)dataReader["reasons"]);
                p1.data.Add("" + dataReader["amount"]);
                p1.data.Add((string)dataReader["itemized_expenses"]);
                p1.data.Add("" + dataReader["planned_term"]);
                p1.data.Add("" + dataReader["other_info"]);
                p1.data.Add("" + dataReader["status"]);
                p1.data.Add("" + dataReader["amount_given"]);
                p1.data.Add("END");
            }

            Thread.Sleep(100);
            GetClientByID(p).clientSocket.Send(p1.ToBytes());
            dataReader.Close();

            p1 = new Packet(PacketType.Exchanges, "server");
            try
            {
                string query = "SELECT * FROM exchange;";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                dataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
            }

            while (dataReader.Read())
            {
                p1.data.Add("" + dataReader["application_id"]);
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("date_submission")));
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("valid_until")));
                p1.data.Add((string)dataReader["username"]);
                p1.data.Add((string)dataReader["name"]);
                p1.data.Add((string)dataReader["university"]);
                p1.data.Add((string)dataReader["subjects"]);
                p1.data.Add((string)dataReader["term"]);
                p1.data.Add("" + dataReader["class_hours"]);
                p1.data.Add("" + dataReader["other_info"]);
                p1.data.Add("" + dataReader["status"]);
                p1.data.Add("END");
            }

            Thread.Sleep(100);
            GetClientByID(p).clientSocket.Send(p1.ToBytes());
            dataReader.Close();

            p1 = new Packet(PacketType.Contributions, "server");
            try
            {
                string query = "SELECT * FROM contributions;";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                dataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
            }

            while (dataReader.Read())
            {
                p1.data.Add("" + dataReader["application_id"]);
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("date_submission")));
                p1.data.Add((string)dataReader["username"]);
                p1.data.Add((string)dataReader["name"]);
                p1.data.Add((string)dataReader["university"]);
                p1.data.Add("" + dataReader["attribute"]);
                p1.data.Add("" + dataReader["amount"]);
                p1.data.Add("END");
            }

            Thread.Sleep(100);
            GetClientByID(p).clientSocket.Send(p1.ToBytes());
            dataReader.Close();

            p1 = new Packet(PacketType.Users, "server");
            try
            {
                string query = "SELECT * FROM users_table;";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                dataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
            }

            while (dataReader.Read())
            {
                p1.data.Add("" + dataReader["user_id"]);
                p1.data.Add((string)dataReader["username"]);
                p1.data.Add((string)dataReader["email"]);
                p1.data.Add((string)dataReader["name"]);
                p1.data.Add("" + dataReader["attribute"]);
                p1.data.Add((string)dataReader["university"]);
                p1.data.Add("END");
            }

            Thread.Sleep(100);

            GetClientByID(p).clientSocket.Send(p1.ToBytes());

            dataReader.Close(); p1 = new Packet(PacketType.Events, "server");
            try
            {
                string query = "SELECT * FROM events;";
                MySqlCommand cmd = new MySqlCommand(query, db.GetConnection());
                dataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
            }

            while (dataReader.Read())
            {
                p1.data.Add("" + dataReader["id"]);
                p1.data.Add((string)dataReader["name"]);
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("date_start")));
                p1.data.Add("" + dataReader.GetDateTime(dataReader.GetOrdinal("date_end")));
                p1.data.Add("" + dataReader["description"]);
                p1.data.Add("END");
            }

            Thread.Sleep(100);
            GetClientByID(p).clientSocket.Send(p1.ToBytes());

            dataReader.Close();
        }
        private static ClientData GetClientByID(Packet p)
        {
            try
            {
                return (from client in _clients
                        where client.id == p.senderID
                        select client)
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GETCLIENTBYID.");
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        private static void CloseClientConnection(ClientData c)
        {
            try
            {
                c.clientSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("CLOSECLIENTCONNECTION.");
                Console.WriteLine(ex.Message);

            }
        }

        private static void RemoveClientFromList(ClientData c)
        {
            try
            {
                _clients.Remove(c);
            }
            catch (Exception ex)
            {
                Console.WriteLine("REMOVECLIENTFROMLIST.");
                Console.WriteLine(ex.Message);

            }
        }

        private static void AbortClientThread(ClientData c)
        {
            try
            {
                c.clientThread.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ABORTCLIENTTHREAD.");
                Console.WriteLine(ex.Message);

            }
        }
    }
}
