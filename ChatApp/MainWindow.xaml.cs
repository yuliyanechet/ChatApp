using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace ChatApp
{
  public partial class MainWindow : Window
  {
    delegate void SetTextCallback();

    private Thread _clientChatThread;

    private AuthWindow _authWindow;
    private ServerClientSide _serverClientSide;
    private string _readData;

    public MainWindow()
    {
      InitializeComponent();
      Closing += MainWindow_Closing;
      
    }

    private void SendMessageButton_Click(object sender, EventArgs e)
    {
      // Send Message
      if (_serverClientSide != null)
      _serverClientSide.PushMessage(TextEditor.Text);     
      TextEditor.Text = "";
    }

    private void ConnectToServerButton_Click(object sender, EventArgs e)
    {
      // Open Authenticate Window
      _authWindow = new AuthWindow();
      _authWindow.SubmitForm += ConnectOrCreateChat;
      _authWindow.Show();
    }

    private void ConnectOrCreateChat(object sender, EventsArguments e)
    {
      if (e.RunServer)
      {
        Thread ctThread = new Thread(Server.StartServer);
        ctThread.Start();
      }

      _serverClientSide = new ServerClientSide();
      _serverClientSide.SetUpServer(e.IpAdress, e.NickName, e.Password, e.TypeAuthentication);

      _clientChatThread = new Thread(GetMessage);
      _clientChatThread.Start();
      NickNamelabel.Content = e.NickName;

      _authWindow.SubmitForm -= ConnectOrCreateChat;
      _authWindow.Close();
    }

    private void QuiteButton_Click(object sender, RoutedEventArgs e)
    {
      // quite
      if(_serverClientSide != null)
      _serverClientSide.LogOut();
      _clientChatThread.Abort();
      NickNamelabel.Content = "Not authorized";
    }

    private void GetMessage()
    {
      while (true)
      {   
        _readData = "" + _serverClientSide.MessagesReceiver();
        Msg();
      }
    }

    private void Msg()
    {
      if (_readData.Length > 0)
      {
        TextFeed.Dispatcher.Invoke(new SetTextCallback(AddDataToTextFeed));
      }
    }

    private void AddDataToTextFeed()
    {
      TextFeed.AppendText(Environment.NewLine + " >> " + _readData.Replace("\0", string.Empty));
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
      Environment.Exit(0);
    }
  }
}
