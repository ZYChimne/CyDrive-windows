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
            Dispatcher.Invoke(() => loadFileGridList(fileInfoList.Take(12).ToArray()));
        }
        private void updateUsageAndCap()
        {
            string usage = formatSize(Config.client.Account.Usage);
            string cap = formatSize(Config.client.Account.Cap);
            UsageButton.Content = "已使用：" + usage + " / 总容量：" + cap;
            UsageBar.Value = Config.client.Account.Usage;
            UsageBar.Maximum = Config.client.Account.Cap;
        }
        private void Usage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Usage Clicked");
            updateUsageAndCap();
        }
        private void loadFileGridList(FileInfo[] fileInfoList)
        {
            FileGridList.Children.Clear();
            for (int i=0;i<fileInfoList.Length;i++)
            {
                FileGrid fileGrid = new FileGrid(fileInfoList[i]);
                Grid.SetColumn(fileGrid, i%4);
                Grid.SetRow(fileGrid, i/4);
                FileGridList.Children.Add(fileGrid);
            }
        }
        private string formatSize(long size)
        {
            int cnt = 0;
            string suffix = "";
            while (size / 1000 > 0)
            {
                size /= 1000;
                cnt++;
            }
            switch (cnt)
            {
                case 0: suffix = "B"; break;
                case 1:suffix = "KB";break;
                case 2:suffix = "MB";break;
                case 3: suffix = "GB"; break;
                case 4: suffix = "TB"; break;
            }
            return size.ToString()+suffix;
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
            if (info.IsDir)
            {
                filename = info.FilePath.Substring(1);
                suffix = "";
                filetype = "folder";
            }
            else
            {
                if (splitIndex > 0)
                {
                    filename = info.FilePath.Substring(1, splitIndex - 1);
                    suffix = info.FilePath.Substring(splitIndex + 1);
                    filetype = Config.getFileTypeIcon(suffix);
                    Debug.WriteLine(filetype);
                }
                else
                {
                    filename = info.FilePath.Substring(1);
                    suffix = "";
                    filetype = "unknown";
                }
            }
            fileImage.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "../../../../assets/Icons/" + filetype + ".png"));
            textBlock.Text = filename;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.Margin = new Thickness(12, 0, 12, 0);
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            SetRow(fileImage, 0);
            SetRow(textBlock, 1);
            RowDefinitions.Add(rowDefinition1);
            RowDefinitions.Add(rowDefinition2);
            Children.Add(fileImage);
            Children.Add(textBlock);
            MouseDown+=OnFileGridMouseDown;
        }
        private void OnFileGridMouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("On File Grid Mouse Down Clicked");
        }
    }
    
}
