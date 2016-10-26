using System;
using System.Windows;

namespace ChatApp
{
  public partial class AuthWindow : Window
  {
    private EventsArguments _evArgs;
        public event EventHandler<EventsArguments> MyEvent;

        protected void OnMyEvent()
        {
            if (MyEvent != null)
            {
                MyEvent(this, _evArgs);
            }     
        }

        public AuthWindow()
        {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            // signUp
            _evArgs = new EventsArguments();
            _evArgs.RunServer = (bool)ClientRadioButtonPlusServerRadioButton1.IsChecked;
            _evArgs.TypeAuthentication = (bool)RegistartionRadioButton.IsChecked 
              ? TypeAuthentication.SignUp : TypeAuthentication.Login;
            _evArgs.NickName = NickNameTextBox.Text;
            _evArgs.Password = PasswordTextBox.Text;
            _evArgs.IpAdress = IpAdress.Text;
            OnMyEvent();
        }
    }

    public class EventsArguments : EventArgs
    {
        public bool RunServer;
        public string IpAdress;
        public string NickName;
        public string Password;
        public TypeAuthentication TypeAuthentication;
     }

  public enum TypeAuthentication
  {
    SignUp, Login
  }
 }

