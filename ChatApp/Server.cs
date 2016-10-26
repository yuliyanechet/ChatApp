using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatApp
{
  public static class Server
  {
    public static Hashtable ClientsList = new Hashtable();
    public static Dictionary<string, string> ClientData = new Dictionary<string, string>();

    public static void StartServer()
    {
      TcpListener serverSocket = new TcpListener(IPAddress.Any, 8888);

      serverSocket.Start();

      while ((true))
      {
        var clientSocket = serverSocket.AcceptTcpClient();
        clientSocket.ReceiveBufferSize = 8192;

        byte[] bytesFrom = new byte[8192];

        NetworkStream networkStream = clientSocket.GetStream();
        networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);

        var dataFromClient = Encoding.ASCII.GetString(bytesFrom);
        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$", StringComparison.Ordinal));

        if (dataFromClient.Contains(ConstantsActionVerbs.Registration))
        {
          // registration
          var nick = dataFromClient.Substring(dataFromClient.IndexOf(ConstantsActionVerbs.Registration, StringComparison.Ordinal) + 32);
          var password = dataFromClient.Substring(0, dataFromClient.IndexOf(ConstantsActionVerbs.Registration, StringComparison.Ordinal));
          if (!ClientData.ContainsKey(nick))
          {
            ClientsList.Add(nick, clientSocket);
            ClientData.Add(nick, password);
            Broadcast(nick + " registered to chat ", nick, false);
            HandleClient client = new HandleClient();
            client.StartClient(clientSocket, nick, ClientsList);
          }
        }
        if (dataFromClient.Contains(ConstantsActionVerbs.Login))
        {
          // login
          var nick = dataFromClient.Substring(dataFromClient.IndexOf(ConstantsActionVerbs.Login, StringComparison.Ordinal) + 32);
          var password = dataFromClient.Substring(0, dataFromClient.IndexOf(ConstantsActionVerbs.Login, StringComparison.Ordinal));
          if (ClientData.ContainsKey(nick) && ClientData[nick] == password)
          {
            ClientsList.Add(nick, clientSocket);
            Broadcast(nick + " joined to chat ", nick, false);
            HandleClient client = new HandleClient();
            client.StartClient(clientSocket, nick, ClientsList);
          }
        }
      }
    }

    public static void Broadcast(string msg, string uName, bool flag)
    {
      if (msg == ConstantsActionVerbs.Quite)
      {
        // quite
        ClientsList.Remove(uName);
        msg = string.Format("{0} left the chat", uName);
        flag = false;
      }

      foreach (DictionaryEntry item in ClientsList)
      {
        var broadcastSocket = (TcpClient)item.Value;
        NetworkStream broadcastStream = broadcastSocket.GetStream();

        var broadcastBytes = flag ? Encoding.ASCII.GetBytes(uName + " says : " + msg) : Encoding.ASCII.GetBytes(msg);

        broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
        broadcastStream.Flush();
      }
    }
  }


  public class HandleClient
  {
    TcpClient _clientSocket;
    string _clNo;
    Hashtable _clientsList;

    public void StartClient(TcpClient inClientSocket, string clineNo, Hashtable cList)
    {
      _clientSocket = inClientSocket;
      _clNo = clineNo;
      _clientsList = cList;
      Thread ctThread = new Thread(DoChat);
      ctThread.Start();
    }

    private void DoChat()
    {
      int requestCount=0;
      byte[] bytesFrom = new byte[8192];

      while ((true))
      {
        try
        {
          requestCount = requestCount + 1;
          NetworkStream networkStream = _clientSocket.GetStream();
          networkStream.Read(bytesFrom, 0, (int)_clientSocket.ReceiveBufferSize);
          var dataFromClient = Encoding.ASCII.GetString(bytesFrom);
          dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$", StringComparison.Ordinal));

          Server.Broadcast(dataFromClient, _clNo, true);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
      }
    }

  }
}
