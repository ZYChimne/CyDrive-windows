using CyDrive.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CyDrive
{
    /// <summary>
    /// Task.xaml 的交互逻辑
    /// </summary>
    enum TaskType { DOWNLOAD, UPLOAD, COMPLETE };
    public partial class Tasks : Window
    {
        TaskType taskType = TaskType.DOWNLOAD;
        MainWindow mainWindow=null;
        List<DataTask> downloadTasks=new List<DataTask>();
        List<DataTask> uploadTasks = new List<DataTask>();
        List<DataTask> completeTasks = new List<DataTask>();
        public Tasks(MainWindow mainWindow, List<DataTask> downloadTasks, List<DataTask> uploadTasks, List<DataTask> completeTasks)
        {
            this.mainWindow=mainWindow;
            this.downloadTasks=downloadTasks;
            this.uploadTasks=uploadTasks;
            this.completeTasks=completeTasks;
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Tick;
            timer.Start();
        }
        private void ShowTasks(List<DataTask> tasks)
        {
            TaskDisplay.Children.Clear();
            for (int i = 0; i < tasks.Count; i++)
            {
                TaskGrid taskGrid = new TaskGrid(tasks[i]);
                Grid.SetRow(taskGrid, i%6);
                TaskDisplay.Children.Add(taskGrid);
            }
        }
        private void Tick(object sender, EventArgs e)
        {
            switch (taskType)
            {
                case TaskType.DOWNLOAD:ShowTasks(downloadTasks);break;
                case TaskType.UPLOAD: ShowTasks(uploadTasks); break;
                case TaskType.COMPLETE: ShowTasks(completeTasks); break;
            }
        }

        private void BackToMainWindow(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            Close();
        }
    }

    public class TaskGrid:Grid
    {
        Image fileImage = null;
        string suffix;
        string filetype;
        string filename;
        long filesize;
        TextBlock filenameTextblock = null;
        ProgressBar progressbar = null;
        TextBlock percentageBlock = null;
        Button startButton = null;
        public TaskGrid(DataTask task)
        {
            int splitIndex = task.FileInfo.FilePath.LastIndexOf('.');
            if (splitIndex > 0)
            {
                filename = task.FileInfo.FilePath.Substring(1, splitIndex - 1);
                suffix = task.FileInfo.FilePath.Substring(splitIndex + 1);
                filetype = Config.getFileTypeIcon(suffix);
                //Debug.WriteLine(filetype);
            }
            else
            {
                filename = task.FileInfo.FilePath.Substring(1);
                suffix = "";
                filetype = "unknown";
            }
            filesize = task.FileInfo.Size;
            fileImage = new Image
            {
                Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "../../../../assets/Icons/" + filetype + ".png"))
            };
            filenameTextblock = new TextBlock
            {
                Text = filename,
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };
            progressbar = new ProgressBar
            {
                Width = 60,
                Height = 20,
                Foreground = new SolidColorBrush(Colors.SkyBlue),
                Maximum = filesize,
                Value = task.Offset,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            percentageBlock = new TextBlock
            {
                Text = (task.Offset * 100 / filesize).ToString(),
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };
            startButton = new Button
            {
                Content = "暂停",
                Background = new SolidColorBrush(Colors.SkyBlue),
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 12
            };
            SetColumn(fileImage, 0);
            SetColumn(filenameTextblock, 1);
            SetColumn(progressbar, 3);
            SetColumn(percentageBlock, 4);
            SetColumn(startButton, 5);
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            ColumnDefinition columnDefinition3 = new ColumnDefinition();
            ColumnDefinition columnDefinition4 = new ColumnDefinition();
            ColumnDefinition columnDefinition5 = new ColumnDefinition();
            ColumnDefinition columnDefinition6 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(1, GridUnitType.Star); //fileimage
            columnDefinition2.Width = new GridLength(2, GridUnitType.Star); //filename
            columnDefinition3.Width = new GridLength(5, GridUnitType.Star); //blank
            columnDefinition4.Width = new GridLength(2, GridUnitType.Star); //progress bar
            columnDefinition5.Width = new GridLength(1, GridUnitType.Star); //percentage
            columnDefinition6.Width = new GridLength(1, GridUnitType.Star); //start button
            ColumnDefinitions.Add(columnDefinition1);
            ColumnDefinitions.Add(columnDefinition2);
            ColumnDefinitions.Add(columnDefinition3);
            ColumnDefinitions.Add(columnDefinition4);
            ColumnDefinitions.Add(columnDefinition5);
            ColumnDefinitions.Add(columnDefinition6);
            Children.Add(fileImage);
            Children.Add(filenameTextblock);
            Children.Add(progressbar);
            Children.Add(percentageBlock);
            Children.Add(startButton);
        }
    }
}
