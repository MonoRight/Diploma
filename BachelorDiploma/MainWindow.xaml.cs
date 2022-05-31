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
using System.ComponentModel;
using BachelorDiploma.Model;
using BLL.Enums;
using BLL.DTO;
using BLL;
using BLL.Exceptions;

namespace BachelorDiploma
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CalculationResult calculationResult;
        public MainWindow()
        {
            InitializeComponent();

            Notification.Show("Recommended to have 8GB of RAM",
                "Since this program performs a lot of mathematical operations, the amount of RAM affects the results.",
                NotificationType.Information,
                0, 0, 10);

            RAMManager.ShowNotificationRAMInformation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            T1_2DopomijneObladnannyaDataGrid.ItemsSource = AdditionalTablesModel.T1_2DopomijneObladnannya;
            AdditionalTablesModel.T1_2DopomijneObladnannya.ListChanged += T1_2DopomijneObladnannya_ListChanged;
        }

        private void T1_2DopomijneObladnannya_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemChanged)
            {

            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
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
                openFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
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

        private async void StartButtonClick(object sender, RoutedEventArgs e)
        {
            if (FileConnectionString.ConnectionString != null)
            {
                if (TankNameTextBox.Text != string.Empty && NominalVolumeTextBox.Text != string.Empty && FillingHeightTextBox.Text != string.Empty && DeathHeightTextBox.Text != string.Empty &&
                TemperatureTextBox.Text != string.Empty && LinearCoeffTempTextBox.Text != string.Empty && MaxDistBetweenPointsTextBox.Text != string.Empty && MaxDepthTextBox.Text != string.Empty &&
                ZeroPositionTextBox.Text != string.Empty && CorrectiveCoeffTextBox.Text != string.Empty && FromTextBox.Text != string.Empty && ToTextBox.Text != string.Empty)
                {
                    string name;
                    double nominalVolume, fillingHeight, deathHeight, temperature, linearCoeffTemp, maxDistBetweenPoints, maxDepth,
                        zeroPosition, correctiveCoeff, fromCorrectiveCoeff, toCorrectiveCoeff;
                    TankType tankType;
                    AlgorithmHullType algorithmHullType;
                    try
                    {
                        name = TankNameTextBox.Text;
                        nominalVolume = Convert.ToDouble(NominalVolumeTextBox.Text.Replace(".", ","));
                        fillingHeight = Convert.ToDouble(FillingHeightTextBox.Text.Replace(".", ","));
                        deathHeight = Convert.ToDouble(DeathHeightTextBox.Text.Replace(".", ","));
                        tankType = HorizontalTypeComboBoxItem.IsSelected ? TankType.Horizontal : TankType.Vertical;
                        temperature = Convert.ToDouble(TemperatureTextBox.Text.Replace(".", ","));
                        linearCoeffTemp = Convert.ToDouble(LinearCoeffTempTextBox.Text.Replace(".", ","));
                        maxDistBetweenPoints = Convert.ToDouble(MaxDistBetweenPointsTextBox.Text.Replace(".", ","));
                        maxDepth = Convert.ToDouble(MaxDepthTextBox.Text.Replace(".", ","));
                        zeroPosition = Convert.ToDouble(ZeroPositionTextBox.Text.Replace(".", ","));
                        correctiveCoeff = Convert.ToDouble(CorrectiveCoeffTextBox.Text.Replace(".", ","));
                        fromCorrectiveCoeff = Convert.ToDouble(FromTextBox.Text.Replace(".", ","));
                        toCorrectiveCoeff = Convert.ToDouble(ToTextBox.Text.Replace(".", ","));

                        if (AndrewHullTypeComboBoxItem.IsSelected)
                            algorithmHullType = AlgorithmHullType.Andrew;
                        else if (GrahamHullTypeComboBoxItem.IsSelected)
                            algorithmHullType = AlgorithmHullType.Graham;
                        else 
                            algorithmHullType = AlgorithmHullType.Andrew;
                    }
                    catch(OverflowException)
                    {
                        MessageWindow messageWindow = new MessageWindow("Введене число виходить за рамки діапазону типу даних! Зменшіть його розмір!");
                        messageWindow.Show();
                        return;
                    }
                    catch(FormatException)
                    {
                        MessageWindow messageWindow = new MessageWindow("Числові дані введені не вірно!");
                        messageWindow.Show();
                        return;
                    }
                    InformationModelDto infoModel = new InformationModelDto(name, nominalVolume, fillingHeight, deathHeight, tankType, algorithmHullType, temperature,
                                   linearCoeffTemp, maxDistBetweenPoints, maxDepth, zeroPosition, correctiveCoeff, fromCorrectiveCoeff, toCorrectiveCoeff);
                    MainCalculation mainCalculation = new MainCalculation(infoModel, FileConnectionString.ConnectionString);

                    await Task.Run(() =>
                    {
                        try
                        {
                            calculationResult = mainCalculation.Calculate();
                            GC.Collect();
                            MessageBox.Show(calculationResult.Volume);
                            Notification.Show("Розрахунки завершено",
                                            "Перевірте вкладку \"Результати\"",
                                            NotificationType.Success,
                                            0, 0, 60);
                        }
                        catch (WrongFileStructureException)
                        {
                            MessageWindow messageWindow = new MessageWindow("Структура файлу не вірна!");
                            messageWindow.Show();
                        }
                        catch (GrahamScanConvexHullException)
                        {
                            MessageWindow messageWindow = new MessageWindow("Помилка роботи алгоритму Грехема!");
                            messageWindow.Show();
                        }
                        catch(Exception exc)
                        {
                            MessageWindow messageWindow = new MessageWindow(exc.ToString());
                            messageWindow.Show();
                        }
                    });
                }
                else
                {
                    MessageWindow messageWindow = new MessageWindow("Всі поля повинні бути заповнені!");
                    messageWindow.Show();
                }
            }
            else
            {
                MessageWindow messageWindow = new MessageWindow("Файл відсутній!");
                messageWindow.Show();
            }
        }
    }
}
