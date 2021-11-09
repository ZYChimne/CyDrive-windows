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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CyDrive.Models;

namespace CyDrive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileInfo[] fileInfoList = null;
        public MainWindow()
        {
            InitializeComponent();
            updateUsageAndCap();
            new Thread(() =>
            {
                fetchFileInfoList();
            }).Start();
        }
        private async void fetchFileInfoList()
        {
            fileInfoList = await Config.client.ListDirAsync("");
            foreach(FileInfo info in fileInfoList)
            {
                Debug.WriteLine(info.FilePath);
            }
        }
        private void updateUsageAndCap()
        {
            UsageButton.Content = "已使用：" + Config.client.Account.Usage + " / 总容量：" + Config.client.Account.Cap;
            UsageBar.Value = Config.client.Account.Usage;
            UsageBar.Maximum = Config.client.Account.Cap;
        }
        private void Usage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Clicked");
            updateUsageAndCap();
        }
    }
}
