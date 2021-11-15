using CyDrive.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CyDrive
{
    /// <summary>
    /// Message.xaml 的交互逻辑
    /// </summary>
    public enum DeviceType { Phone, PC };
    public partial class MessageWindow : Window
    {
        MainWindow mainWindow = null;
        string peerId;
        int msgCnt = 0;
        public MessageWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            Config.client.OnMessage += (sender, e) =>
            {
                peerId = e.Message.Sender;
                //Debug.WriteLine(e.Message.Sender);
                Dispatcher.Invoke(() => OnMessageReceived(e.Message));
            };
            Config.client.ConnectMessageService();
        }
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Send Clicked");
            Message res = await Config.client.SendText(MessageBox.Text, peerId);
            OnMessageSent(res);
        }
        private void OnMessageReceived(Message msg)
        {
            Debug.WriteLine("On Message Received");
            MessageGrid messageGrid = new MessageGrid(msg.Content, DeviceType.Phone, msgCnt);
            Grid.SetRow(messageGrid, msgCnt);
            Grid.SetColumn(messageGrid, 0);
            MsgDisplay.Children.Add(messageGrid);
            msgCnt++;
        }
        private void OnMessageSent(Message msg)
        {
            Debug.WriteLine("On Message Sent");
            MessageGrid messageGrid = new MessageGrid(msg.Content, DeviceType.PC, msgCnt);
            Grid.SetRow(messageGrid, msgCnt);
            Grid.SetColumn(messageGrid, 2);
            MsgDisplay.Children.Add(messageGrid);
            msgCnt++;
        }
    }
    public class MessageGrid: Grid
    {
        string msg;
        DeviceType deviceType;
        Image deviceImage = new Image();
        TextBlock messageTextBlock = new TextBlock();
        int msgCnt;
        public MessageGrid(string msg, DeviceType deviceType, int msgCnt)
        {
            this.msg= msg; 
            this.deviceType= deviceType;
            this.msgCnt = msgCnt;
            deviceImage.Width = 30;
            deviceImage.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "../../../../assets/Icons/" + (deviceType==DeviceType.PC?"pc":"phone") + ".png"));
            messageTextBlock.Text = msg;
            messageTextBlock.TextAlignment = TextAlignment.Center;
            messageTextBlock.TextWrapping = TextWrapping.Wrap;
            messageTextBlock.VerticalAlignment = VerticalAlignment.Center;
            messageTextBlock.Margin = new Thickness(6, 0, 0, 0);
            messageTextBlock.Background = new SolidColorBrush(Colors.SkyBlue);
            messageTextBlock.Foreground = new SolidColorBrush(Colors.White);
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            if (deviceType == DeviceType.PC)
            {
                columnDefinition1.Width = new GridLength(4, GridUnitType.Star);
                columnDefinition2.Width = new GridLength(1, GridUnitType.Star);
                SetColumn(messageTextBlock, 0);
                SetColumn(deviceImage, 1);
            }
            else
            {
                columnDefinition1.Width = new GridLength(1, GridUnitType.Star);
                columnDefinition2.Width = new GridLength(4, GridUnitType.Star);
                SetColumn(messageTextBlock, 1);
                SetColumn(deviceImage, 0);
            }
            ColumnDefinitions.Add(columnDefinition1);  
            ColumnDefinitions.Add(columnDefinition2);  
            Children.Add(deviceImage);
            Children.Add(messageTextBlock); 
        }
    }
}
