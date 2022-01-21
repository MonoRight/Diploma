using BachelorDiploma.FileConnection;
using BachelorDiploma.NotificationManagement;
using BachelorDiploma.RAMManagement;
using Microsoft.Win32;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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

namespace BachelorDiploma
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {      
        public MainWindow()
        {
            InitializeComponent();

            Notification.Show("Recommended to have 8GB of RAM",
                "Since this program performs a lot of mathematical operations, the amount of RAM affects the results.",
                NotificationType.Information,
                0, 0, 10);

            RAMManager.ShowNotificationRAMInformation();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void PackIconEntypo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PackIconBootstrapIcons_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void StackPanelDragDrop(object sender, DragEventArgs e)
        {
            string[] vs = (string[])e.Data.GetData(DataFormats.FileDrop);
            FileConnectionString.ConnectionString = @"" + vs[0];
            FileImage.Source = new BitmapImage(new Uri(@"/BachelorDiploma;component/Images/file1.png", UriKind.RelativeOrAbsolute));
        }

        private void StackPanelDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                FileImage.Source = new BitmapImage(new Uri(@"/BachelorDiploma;component/Images/file1.png", UriKind.RelativeOrAbsolute));
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void StackPanelDragLeave(object sender, DragEventArgs e)
        {
            if (FileConnectionString.ConnectionString == null)
            {
                FileImage.Source = new BitmapImage(new Uri(@"/BachelorDiploma;component/Images/file.png", UriKind.RelativeOrAbsolute));
            }
        }

        private async Task FileDialog()
        {
            await Task.Run(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Оберіть файл";
                openFileDialog.InitialDirectory = @"C:\Users";
                if (openFileDialog.ShowDialog() == true)
                {
                    FileConnectionString.ConnectionString = openFileDialog.FileName.ToString();
                }
            });
        }

        private async void StackPanelMouseDown(object sender, MouseButtonEventArgs e)
        {
            await FileDialog();
            if (FileConnectionString.ConnectionString != null)
            {
                FileImage.Source = new BitmapImage(new Uri(@"/BachelorDiploma;component/Images/file1.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                FileImage.Source = new BitmapImage(new Uri(@"/BachelorDiploma;component/Images/file.png", UriKind.RelativeOrAbsolute));
            }
        }
    }
}
