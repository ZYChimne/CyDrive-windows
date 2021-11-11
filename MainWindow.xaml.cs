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
            Debug.WriteLine(Config.client.Account.Name);
            Username.Content = Config.client.Account.Name;
            updateUsageAndCap();
            new Thread(() =>
            {
                fetchFileInfoList();
            }).Start();
            
        }
        private async void fetchFileInfoList()
        {
            fileInfoList = await Config.client.ListDirAsync("");
            foreach (FileInfo info in fileInfoList)
            {
                Debug.WriteLine(info.FilePath);
            }
            Dispatcher.Invoke(() => loadFileGridList());
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
        private void loadFileGridList()
        {
            FileGridList.Children.Clear();
            foreach (FileInfo info in fileInfoList)
            {
                FileGridList.Children.Add(new FileGrid(info));
                break;
            }
        }
    }

    public class FileGrid :Grid
    {
        string filename;
        string suffix;
        string filetype;
        Image fileImage = new Image();
        TextBlock textBlock = new TextBlock();
        public FileGrid(FileInfo info)
        {
            
            int splitIndex = info.FilePath.LastIndexOf('.');
            if(splitIndex > 0)
            {
                filename = info.FilePath.Substring(1, splitIndex-1);
                suffix = info.FilePath.Substring(splitIndex + 1);
                filetype = Config.getFileTypeIcon(suffix);
                Debug.WriteLine(filetype);
            }
            else
            {
                filename = info.FilePath;
                suffix = "";
                filetype = "unknown";
            }
            fileImage.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "../../../../assets/Icons/"+filetype+".png"));
            textBlock.Text = filename;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            SetRow(fileImage, 0);
            SetRow(textBlock, 1);
            RowDefinitions.Add(rowDefinition1);
            RowDefinitions.Add(rowDefinition2);
            Children.Add(fileImage);
            Children.Add(textBlock);
        }
    }

}
