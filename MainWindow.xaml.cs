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
using System.Windows.Threading;
using CyDrive.Models;
using Microsoft.Win32;

namespace CyDrive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileInfo[] fileInfoList = null;
        private HashSet<string> dirlist = new HashSet<string>();
        private List<DataTask> downloadTasks = new List<DataTask>();
        List<DataTask> uploadTasks = new List<DataTask>();
        List<DataTask> completeTasks = new List<DataTask>();
        Tasks taskWindow = null;
        Settings settingsWindow = null;
        private double dirBtnPosEnd = 6;
        private int dirCnt = 0;
        private MessageWindow messageWindow = null;
        private string shortDir;
        private string longDir;
        public MainWindow()
        {
            InitializeComponent();
            //Debug.WriteLine(Config.client.Account.Name);
            Username.Content = Config.client.Account.Name;
            UpdateUsageAndCap();
            LoadFiles("", "Root");
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(15); //定时刷新文件
            timer.Tick += Tick;
            timer.Start();
        }
        private void Tick(object sender, EventArgs e)
        {
            LoadFiles(longDir, shortDir);
        }
        private async void FetchFileInfoList(string dir)
        {
            fileInfoList = await Config.client.ListDirAsync(dir);
            foreach (FileInfo info in fileInfoList)
            {
                Debug.WriteLine(info.FilePath);
            }
            Dispatcher.Invoke(() => LoadFileGridList(fileInfoList.Take(12).ToArray()));
        }
        private void UpdateUsageAndCap()
        {
            string usage = FormatSize(Config.client.Account.Usage);
            string cap = FormatSize(Config.client.Account.Cap);
            UsageButton.Content = "已使用：" + usage + " / 总容量：" + cap;
            UsageBar.Value = Config.client.Account.Usage;
            UsageBar.Maximum = Config.client.Account.Cap;
        }
        private void Usage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Usage Clicked");
            UpdateUsageAndCap();
        }
        private void LoadFileGridList(FileInfo[] fileInfoList)
        {
            FileGridList.Children.Clear();
            for (int i = 0; i < fileInfoList.Length; i++)
            {
                FileGrid fileGrid = new FileGrid(fileInfoList[i]);
                Grid.SetColumn(fileGrid, i % 4);
                Grid.SetRow(fileGrid, i / 4);
                fileGrid.MouseDown += OnFileGridMouseDown;
                FileGridList.Children.Add(fileGrid);
            }
        }
        private string FormatSize(long size)
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
                case 1: suffix = "KB"; break;
                case 2: suffix = "MB"; break;
                case 3: suffix = "GB"; break;
                case 4: suffix = "TB"; break;
            }
            return size.ToString() + suffix;
        }

        private void AddDirButton(int cnt, string name)
        {
            if(dirlist.Contains(name)) return;
            dirlist.Add(name);
            Debug.WriteLine("Add Dir Button");
            Button btn = new Button();
            if (cnt == 0)
            {
                btn.Content = name;
            }
            else
            {
                btn.Content = "> " + name;
            }
            btn.BorderThickness = new Thickness(0);
            btn.FontSize = 12;
            btn.Background = new SolidColorBrush(Colors.White);
            btn.Foreground = new SolidColorBrush(Colors.Black);
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.Margin = new Thickness(dirBtnPosEnd, 0, 0, 0);
            //这里根据路径字符串长度计算大小，按照英文字母计算，如果出现中文路径可能出现显示不完整
            btn.Width = btn.Content.ToString().Length * 8; 
            dirBtnPosEnd = 6 + btn.Width;
            DirList.Children.Add(btn);
        }
        private void LoadFiles(string longDir, string shortDir)
        {
            this.longDir = longDir;
            this.shortDir = shortDir;
            new Thread(() =>
            {
                FetchFileInfoList(longDir);
            }).Start();
            AddDirButton(dirCnt, shortDir);
        }
        private void TaskWindow_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Task Window Clicked");
            taskWindow = new Tasks(this, downloadTasks, uploadTasks, completeTasks);
            if(!taskWindow.IsVisible) taskWindow.Show();
            Hide();
        }
        private void SettingsWindow_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Settings Window Clicked");
            settingsWindow = new Settings(this);
            if(!settingsWindow.IsVisible) settingsWindow.Show();
            Hide();
        }
        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Upload Clicked");
            OpenFileDialog fileDialog = new OpenFileDialog();
            bool? res = fileDialog.ShowDialog();
            if(res == true)
            {
                Debug.WriteLine("Start Upload");
                Config.client.UploadAsync(fileDialog.FileName, "/").ContinueWith(async (task) =>
                {
                    uploadTasks.Add(await task);
                });
            }
            
        }
        private void OnFileGridMouseDown(object sender, MouseEventArgs e)
        {
            FileGrid fileGrid = (FileGrid)sender;
            Debug.WriteLine("On File Grid Mouse Down Clicked");
            if (fileGrid.filetype == "folder")
            {
                dirCnt++;
                LoadFiles(fileGrid.longFilename, fileGrid.shortFilename);
            }
            else
            {

                Debug.WriteLine("Start Downloading " + fileGrid.shortFilename);
                string downloadPath = "C:/Users/ZYC/Downloads/" + fileGrid.shortFilename + "." + fileGrid.suffix;
                //string downloadPath = "~/Downloads/" + fileGrid.shortFilename + "." + fileGrid.suffix;
                Config.client.DownloadAsync(fileGrid.longFilename, downloadPath).ContinueWith(async (task) =>
                {
                    downloadTasks.Add(await task);
                });
            }
        }
        private void MessageWindow_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MessageWindow Clicked");
            if (messageWindow == null)
            {
                messageWindow = new MessageWindow(this);
                messageWindow.Show();
            }
            Hide();
        }
    }
    public class FileGrid : Grid
    {
        public string shortFilename;
        public string longFilename;
        public string suffix;
        public string filetype;
        Image fileImage = new Image();
        TextBlock textBlock = new TextBlock();
        public FileGrid(FileInfo info)
        {
            longFilename = info.FilePath;
            int splitIndex = info.FilePath.LastIndexOf('.');
            if (info.IsDir)
            {
                shortFilename = info.FilePath.Substring(1);
                suffix = "";
                filetype = "folder";
            }
            else
            {
                if (splitIndex > 0)
                {
                    shortFilename = info.FilePath.Substring(1, splitIndex - 1);
                    suffix = info.FilePath.Substring(splitIndex + 1);
                    filetype = Config.getFileTypeIcon(suffix);
                    //Debug.WriteLine(filetype);
                }
                else
                {
                    shortFilename = info.FilePath.Substring(1);
                    suffix = "";
                    filetype = "unknown";
                }
            }
            fileImage.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "../../../../assets/Icons/" + filetype + ".png"));
            textBlock.Text = shortFilename;
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
        }
    }
}
