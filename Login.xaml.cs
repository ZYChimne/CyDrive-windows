using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using CyDrive.Models;

namespace CyDrive
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        Account account = new Account();
        MainWindow mainWindow = null;
        public Login()
        {
            InitializeComponent();
            if (Config.DEBUG)
            {
                Email.Text = Config.DEFAULT_EMAIL;
                Password.Password = Config.DEFAULT_PASSWORD;
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Login Clicked");
            account.Email = Email.Text;
            account.Password = Utils.PasswordHash(Password.Password);
            bool res = await Config.client.LoginAsync(account);
            Debug.WriteLine(res);
            if (res)
            {
                if (mainWindow == null)
                {
                    mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                Hide();
            }
            else
            {
                MessageBox.Show("登录失败，请检查账号和密码是否输入正确");
            }
        }
    }
}
