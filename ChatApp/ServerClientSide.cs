using System.Net.Sockets;
using System.Text;

namespace ChatApp
{
  public class ServerClientSide
  {
    TcpClient _clientSocket;
    NetworkStream _serverStream;

    public void SetUpServer(EventsArguments e)
    {
      if (_clientSocket == null) _clientSocket = new TcpClient();
      _clientSocket.Connect(e.IpAdress, 8888);
      _serverStream = _clientSocket.GetStream();
      byte[] outStream;
      if (e.TypeAuthentication == TypeAuthentication.SignUp)
      {
        outStream = Encoding.ASCII.GetBytes(e.Password + ConstantsActionVerbs.Registration + e.NickName + "$");
      }
      else if (e.TypeAuthentication == TypeAuthentication.Login)
      {
        outStream = Encoding.ASCII.GetBytes(e.Password + ConstantsActionVerbs.Login + e.NickName + "$");
      }
      else
      {
        outStream = Encoding.ASCII.GetBytes(e.NickName + "$");
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
