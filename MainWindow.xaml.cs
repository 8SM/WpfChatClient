using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;



namespace WindowedChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int _port = 1333;
        const int _portForListen = 1334;
        
        string _ipServer = "192.168.1.66";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Login();
            WaitLoginAnswer();
            
        }

        private void Login()
        {

            TcpClient client = new TcpClient(_ipServer, _port);
            NetworkStream strm = client.GetStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(strm, LoginTextBox.Text);
            strm.Close();
            client.Close();
        }
        private async void WaitLoginAnswer()
        {
            IPAddress serverAddress = IPAddress.Parse(_ipServer);
            TcpListener tempServer = new TcpListener(serverAddress, _portForListen);
            tempServer.Start();
            TcpClient tcpClient = await tempServer.AcceptTcpClientAsync();
                /*
            string memberEndPoint = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).ToString();
            int indexOfPort = memberEndPoint.LastIndexOf(":");
            string serverIp = memberEndPoint.Remove(indexOfPort);
                */
            NetworkStream strm = tcpClient.GetStream();
            BinaryFormatter formatter = new BinaryFormatter();
            var answerFromServer = formatter.Deserialize(strm);
            if (answerFromServer is string)
            {
               string answer = answerFromServer as string;
                
                if (answer.Contains("success"))
                {
                    LoginInfoLabel.Content = answer;
                    //Смена окна
                    ChatWindow chatWin = new ChatWindow();
                    this.Title = "Connect Success!";
                    this.Close();
                    chatWin.Show();
                    
                    
                }
            }
            strm.Close();
            tcpClient.Close();
            tempServer.Stop();
        }
    }
}
