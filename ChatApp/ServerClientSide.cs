using System.Net.Sockets;
using System.Text;

namespace ChatApp
{
  public class ServerClientSide
  {
    TcpClient _clientSocket;
    NetworkStream _serverStream;

    public void SetUpServer(string ip, string nickName, string password, TypeAuthentication typeAuth)
    {
      if (_clientSocket == null) _clientSocket = new TcpClient();
      _clientSocket.Connect(ip, 8888);
      _serverStream = _clientSocket.GetStream();
      byte[] outStream;
      if (typeAuth == TypeAuthentication.SignUp)
      {
        outStream = Encoding.ASCII.GetBytes(password + ConstantsActionVerbs.Registration + nickName + "$");
      }
      else if (typeAuth == TypeAuthentication.Login)
      {
        outStream = Encoding.ASCII.GetBytes(password + ConstantsActionVerbs.Login + nickName + "$");
      }
      else
      {
        outStream = Encoding.ASCII.GetBytes(nickName + "$");
      }
      _serverStream.Write(outStream, 0, outStream.Length);
      _serverStream.Flush();
    }

    public void LogOut()
    {
      byte[] outStream = Encoding.ASCII.GetBytes(ConstantsActionVerbs.Quite + "$");
      _serverStream.Write(outStream, 0, outStream.Length);
      _serverStream.Flush();
      _clientSocket = null;
    }

    public void PushMessage(string text)
    {
      if (_clientSocket != null)
      {
        byte[] outStream = Encoding.ASCII.GetBytes(text + "$");
        _serverStream.Write(outStream, 0, outStream.Length);
        _serverStream.Flush();       
      }
    }

    public string MessagesReceiver()
    {
      _serverStream = _clientSocket.GetStream();
      byte[] inStream = new byte[8192];
      _clientSocket.ReceiveBufferSize = 8192;
      _serverStream.Read(inStream, 0, _clientSocket.ReceiveBufferSize);
      return Encoding.ASCII.GetString(inStream);
    }
  }
}
