using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ServerData;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Client.Menu;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Timers;

namespace Client
{
    public enum PasswordScore
    {
        Blank = 0,
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }

    public class Advisor
    {
        public static PasswordScore CheckPassStrength(string password)
        {
            int score = 1;

            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 8)
                score++;
            if (password.Length >= 12)
                score++;
            if (Regex.Match(password, @"/\d+/", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"/[a-z]/", RegexOptions.ECMAScript).Success &&
              Regex.Match(password, @"/[A-Z]/", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"/.[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]/", RegexOptions.ECMAScript).Success)
                score++;

            return (PasswordScore)score;
        }

        public static bool CheckEmail(string email)
        {
            var foo = new EmailAddressAttribute();
            return foo.IsValid(email);
        }
    }

    public static class Switcher
    {
        static System.Timers.Timer aTimer;
        static Socket socket;
        static IPAddress ipAddress;
        static Boolean isConnected = false;
        static Boolean ServerDC = false;
        public static Thread thread;
        public static MainWindow pageSwitcher;
        public static bool ThreadRunning = false;
        private static string Username;
        private static string Password;
        public static string Rank;
        public static string Name;
        public static Thread Logint;
        private static string ID = "none";

        public static List<Contribution> Contributions;
        public static List<Exchange> Exchanges;
        public static List<Scholarship> Scholarships;
        public static List<Fund> Funds;
        public static List<User> Users;
        public static List<Event> Events;

        public static System.Timers.Timer myTimer;
        public static void Switch(UserControl newPage)
        {
            pageSwitcher.Navigate(newPage);
        }

        public static void Switch(UserControl newPage, object state)
        {
            pageSwitcher.Navigate(newPage, state);
        }
        
        private static void Data_IN()
        {
            byte[] buffer = new byte[1];
            // byte [] bufferb;
            int readBytes = 0;
            int extraBytes = 0;
            NetworkStream ns = new NetworkStream(socket);
            while (true)
            {
                try
                {
                    buffer = new byte[socket.SendBufferSize];
                    readBytes = socket.Receive(buffer);
                    //bufferb = buffer.Take(readBytes).ToArray();
                    if (readBytes > 0)
                    {
                        using (MemoryStream ms = new MemoryStream(buffer, 0, readBytes))
                        {
                            Packet p = ProtoBuf.Serializer.Deserialize<Packet>(ms);
                            DataManager(p);
                        }
                    }
                }
                catch (SocketException ex)
                {
                    ConnectionToServerLost();
                    break;
                }
                catch (EndOfStreamException ex)
                {
                    while (true)
                        try
                        {
                            byte[] tempbuffer = new byte[socket.SendBufferSize];
                            extraBytes = socket.Receive(tempbuffer);
                            for (int i = readBytes; i < readBytes + extraBytes; i++)
                            {
                                buffer[i] = tempbuffer[i - readBytes];
                            }
                            readBytes += extraBytes;
                            using (MemoryStream ms = new MemoryStream(buffer, 0, readBytes))
                            {
                                Packet p = ProtoBuf.Serializer.Deserialize<Packet>(ms);
                                DataManager(p);
                                break;
                            }
                        }
                        catch (Exception ex2)
                        {
                            continue;
                        }
                }
            }
        }

        private static void ConnectionToServerLost()
        {
            socket.Close();

            if (isConnected)
            {
                ServerDC = true;
                if (pageSwitcher != null)
                    pageSwitcher.Dispatcher.Invoke(new Action(() =>
                    {
                        pageSwitcher.Close();
                    }));
                isConnected = false;
            }
            thread.Abort();
        }

        private static void DataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Connect:
                    Connect(p);
                    break;
                case PacketType.Login:
                    Login(p);
                    break;
                case PacketType.Scholarships:
                    readScholarships(p);
                    break;
                case PacketType.Funds:
                    readFunds(p);
                    break;
                case PacketType.Exchanges:
                    readExchanges(p);
                    break;
                case PacketType.Contributions:
                    readContributions(p);
                    break;
                case PacketType.Users:
                    readUsers(p);
                    break;
                case PacketType.Events:
                    readEvents(p);
                    break;
                case PacketType.Chat:
                    addMessage(p);
                    break;
                case PacketType.GetLogs:
                    getLogs(p);
                    break;
                case PacketType.TokenMismatch:
                    mismatchToken(p);
                    break;
                case PacketType.ResetSuccess:
                    resetSuccess(p);
                    break;
                default:
                    break;
            }
        }

        private static void addMessage(Packet p)
        {
            pageSwitcher.Dispatcher.Invoke(new Action(() =>
            {
                if (pageSwitcher.Content is Menu.MainMenu)
                {
                    ((Menu.MainMenu)pageSwitcher.Content).txtChat.Document.Blocks.Add(new Paragraph(new Run("\n<" + DateTime.Now.ToString("HH:mm:ss") + "> " + p.data[0] + ": " + p.data[1])));
                    ((Menu.MainMenu)pageSwitcher.Content).txtChat.ScrollToEnd();
                }
        }));
        }

        private static void mismatchToken(Packet p)
        {
            pageSwitcher.Dispatcher.Invoke(new Action(() =>
            {
                if (pageSwitcher.Content is Menu.Help)
                {
                    ((Menu.Help)pageSwitcher.Content).lblError.Content = "Token is invalid!";
                }
            }));
        }

        private static void resetSuccess(Packet p)
        {
            pageSwitcher.Dispatcher.Invoke(new Action(() =>
            {
                if (pageSwitcher.Content is Menu.Help)
                {
                    ((Menu.Help)pageSwitcher.Content).lblError.Content = "Password has been changed successfully!";
                }
            }));
        }

        private static void getLogs(Packet p)
        {
            List<String> filelist = new List<String>();
            for (int i = 0; i < p.data.Count; i++)
            {
                filelist.Add(p.data[i]);
                Minutes.filemap.Add(p.data[i], p.data[++i]);
            }
            pageSwitcher.Dispatcher.Invoke(new Action(() =>
            {
                if (pageSwitcher.Content is Menu.Minutes)
                    {
                        ((Menu.Minutes)pageSwitcher.Content).lstFiles.ItemsSource = filelist;
                    }
            }));
        }

        private static void readUsers(Packet p)
        {
            User temp;
            for (int i = 0; i < p.data.Count; i++)
            {
                temp = new User();
                temp.id = Int32.Parse(p.data[i]);
                temp.username = p.data[i + 1];
                temp.email = p.data[i + 2];
                temp.name = p.data[i + 3];
                temp.attribute = p.data[i + 4];
                temp.university = p.data[i + 5];

                i += 6;

                Users.Add(temp);
            }
        }

        private static void readEvents(Packet p)
        {
            Event temp;
            for (int i = 0; i < p.data.Count; i++)
            {
                temp = new Event();
                temp.id = Int32.Parse(p.data[i]);
                temp.name = p.data[i + 1];
                temp.datestart = p.data[i + 2];
                temp.dateend = p.data[i + 3];
                temp.desc = p.data[i + 4];

                i += 5;

                Events.Add(temp);
            }

            pageSwitcher.Dispatcher.Invoke(new Action(() =>
            {
                if (pageSwitcher.Content is Menu.MainMenu)
                {
                    ((Menu.MainMenu)pageSwitcher.Content).lstEvents.Items.Refresh();
                }
            }));
        }

        private static void readScholarships(Packet p)
        {
            Scholarship temp;
            for (int i = 0; i < p.data.Count; i++)
            {
                temp = new Scholarship();
                temp.application_id = Int32.Parse(p.data[i]);
                temp.date_submission = p.data[i + 1];
                temp.valid_until = p.data[i + 2];
                temp.username = p.data[i + 3];
                temp.name = p.data[i + 4];
                temp.university = p.data[i + 5];
                temp.father_name = p.data[i + 6];
                temp.father_worktype = p.data[i + 7];
                temp.father_workplace = p.data[i + 8];
                temp.father_workaddress = p.data[i + 9];
                temp.father_startdate = p.data[i + 10];
                temp.father_enddate = p.data[i + 11];
                temp.mother_name = p.data[i + 12];
                temp.mother_worktype = p.data[i + 13];
                temp.mother_workplace = p.data[i + 14];
                temp.mother_workaddress = p.data[i + 15];
                temp.mother_startdate = p.data[i + 16];
                temp.mother_enddate = p.data[i + 17];
                temp.siblings_number = p.data[i + 18];
                temp.siblings_position = p.data[i + 19];
                temp.father_income = Double.Parse(p.data[i + 20]);
                temp.mother_income = Double.Parse(p.data[i + 21]);
                temp.other_income = Double.Parse(p.data[i + 22]);
                temp.expenses = Double.Parse(p.data[i + 23]);
                temp.status = p.data[i + 24];
                temp.percentage = Int32.Parse(p.data[i + 25]);

                i += 26;

                Scholarships.Add(temp);
            }
        }

        private static void readFunds(Packet p)
        {
            Fund temp;
            for (int i = 0; i < p.data.Count; i++)
            {
                temp = new Fund();
                temp.application_id = Int32.Parse(p.data[i]);
                temp.date_submission = p.data[i + 1];
                temp.valid_until = p.data[i + 2];
                temp.username = p.data[i + 3];
                temp.name = p.data[i + 4];
                temp.university = p.data[i + 5];
                temp.reasons = p.data[i + 6];
                temp.amount = Double.Parse(p.data[i + 7]);
                temp.itemized_expenses = p.data[i + 8];
                temp.planned_term = p.data[i + 9];
                temp.other_info = p.data[i + 10];
                temp.status = p.data[i + 11];
                temp.amount_given = Int32.Parse(p.data[i + 12]);
                i += 13;

                Funds.Add(temp);
            }
        }

        private static void readExchanges(Packet p)
        {
            Exchange temp;
            for (int i = 0; i < p.data.Count; i++)
            {
                temp = new Exchange();
                temp.application_id = Int32.Parse(p.data[i]);
                temp.date_submission = p.data[i + 1];
                temp.valid_until = p.data[i + 2];
                temp.username = p.data[i + 3];
                temp.name = p.data[i + 4];
                temp.university = p.data[i + 5];
                temp.subjects = p.data[i + 6];
                temp.term = p.data[i + 7];
                temp.class_hours = p.data[i + 8];
                temp.other_info = p.data[i + 9];
                temp.status = p.data[i + 10];
                i += 11;

                Exchanges.Add(temp);
            }
        }

        private static void readContributions(Packet p)
        {
            Contribution temp;
            for (int i = 0; i < p.data.Count; i++)
            {
                temp = new Contribution();
                temp.application_id = Int32.Parse(p.data[i]);
                temp.date_submission = p.data[i + 1];
                temp.username = p.data[i + 2];
                temp.name = p.data[i + 3];
                temp.university = p.data[i + 4];;
                temp.attribute = p.data[i + 5];
                temp.amount = Int32.Parse(p.data[i + 6]);
                i += 7;

                Contributions.Add(temp);
            }
        }

        /// ////////////////////////////////////////////////////////

        public static void changePass(String user, String token, String pass)
        {
            Packet p = new Packet(PacketType.Reset, ID);
            p.data.Add(user);
            p.data.Add(token);
            p.data.Add(pass);
            socket.Send(p.ToBytes());
        }

        public static void requestToken(String user)
        {
            Packet p = new Packet(PacketType.Token, ID);
            p.data.Add(user);
            socket.Send(p.ToBytes());
        }

        public static void addEvent(String name, String start, String end, String desc)
        {
            Packet p = new Packet(PacketType.AddEvent, ID);
            p.data.Add(name);
            p.data.Add("" + start);
            p.data.Add("" + end);
            p.data.Add(desc);
            socket.Send(p.ToBytes());
        }

        public static void getLogs()
        {
            Packet p = new Packet(PacketType.GetLogs, ID);
            socket.Send(p.ToBytes());
        }

        public static void demoteUser(User user)
        {
            Packet p = new Packet(PacketType.DemoteUser, ID);
            p.data.Add(user.id + "");
            socket.Send(p.ToBytes());
        }

        public static void promoteUser(User user)
        {
            Packet p = new Packet(PacketType.PromoteUser, ID);
            p.data.Add(user.id + "");
            socket.Send(p.ToBytes());
        }

        public static void deleteUser(User user)
        {
            Packet p = new Packet(PacketType.DeleteUser, ID);
            p.data.Add(user.id + "");
            socket.Send(p.ToBytes());
        }

        public static void updateUserList(object source, ElapsedEventArgs e)
        {
            Packet p = new Packet(PacketType.UpdateLists, ID);
            socket.Send(p.ToBytes());
        }

        public static void addMember(String user, String pass, String email, String name, String attribute, String university)
        {
            Packet p = new Packet(PacketType.AddUser, ID);
            p.data.Add(user);
            p.data.Add(pass);
            p.data.Add(email);
            p.data.Add(name);
            p.data.Add(attribute);
            p.data.Add(university);
            socket.Send(p.ToBytes());
        }

        public static void sendChat(String msg)
        {
            Packet p = new Packet(PacketType.Chat, ID);
            p.data.Add(Name);
            p.data.Add(msg);
            socket.Send(p.ToBytes());
        }

        public static void acceptFund(Fund fund, int amount)
        {
            Packet p = new Packet(PacketType.AcceptFund, ID);
            p.data.Add("" + fund.application_id);
            p.data.Add("" + amount);
            socket.Send(p.ToBytes());
        }

        public static void rejectFund(Fund fund)
        {
            Packet p = new Packet(PacketType.RejectFund, ID);
            p.data.Add("" + fund.application_id);
            socket.Send(p.ToBytes());
        }

        public static void acceptExchange(Exchange exchange)
        {
            Packet p = new Packet(PacketType.AcceptExchange, ID);
            p.data.Add("" + exchange.application_id);
            socket.Send(p.ToBytes());
        }

        public static void rejectExchange(Exchange exchange)
        {
            Packet p = new Packet(PacketType.RejectExchange, ID);
            p.data.Add("" + exchange.application_id);
            socket.Send(p.ToBytes());
        }

        public static void acceptScholarship(Scholarship scho, int percent)
        {
            Packet p = new Packet(PacketType.AcceptScholarship, ID);
            p.data.Add("" + scho.application_id);
            p.data.Add("" + percent);
            socket.Send(p.ToBytes());
        }

        public static void rejectScholarship(Scholarship scho)
        {
            Packet p = new Packet(PacketType.RejectScholarship, ID);
            p.data.Add("" + scho.application_id);
            socket.Send(p.ToBytes());
        }

        private static void Login(Packet p)
        {
            try {
                if (p.data[0].Equals("Success"))
                {
                    Name = p.data[1];
                    Rank = p.data[2];

                    pageSwitcher.Dispatcher.Invoke(new Action(() =>
                    {
                        Switch(new MainMenu());
                    }
                    ));
                }
                else
                {
                    writeError("Invalid username or password.");
                }
            } catch (Exception ex)
            {
                writeError("Error while verifying Login data.");
            }
        }

        private static void Connect (Packet p)
        {
            ID = p.data[0];
            isConnected = true;

            Scholarships = new List<Scholarship>();
            Contributions = new List<Contribution>();
            Funds = new List<Fund>();
            Exchanges = new List<Exchange>();
            Users = new List<User>();
            Events = new List<Event>();
        }
        
        public static void tryConnect()
        {
            try
            {
                if (!isConnected)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPAddress.TryParse(Packet.GetIP4Address(), out ipAddress);
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 4242);
                    socket.Connect(ipEndPoint);
                    
                    isConnected = true;

                    thread = new Thread(Data_IN);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
            }
            catch (SocketException ex)
            {
                writeError("Unable to connect to server.");
            }
        }

        private static void writeError(String error)
        {
            pageSwitcher.Dispatcher.Invoke(new Action(() =>
               {
                   if (pageSwitcher.Content is Menu.Login)
                       ((Menu.Login)pageSwitcher.Content).lblError.Content = error;
               }));
        }

        public static void Login(String user, String pass)
        {
            Username = user;
            Password = pass;
            tryConnect();
            if (isConnected && !ThreadRunning)
            {
                Logint = new Thread(loginHelper);
                ThreadRunning = true;
                Logint.Start();
            }
        }

        public static void loginHelper()
        {
           while (true)
                if (!ID.Equals("none"))
                {
                    Packet login = new Packet(PacketType.Login, ID);
                    login.data.Add(Username);
                    login.data.Add(Password);
                    socket.Send(login.ToBytes());
                    ThreadRunning = false;
                    Logint.Abort();
                }
        }
    }
}
