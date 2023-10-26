using BachelorDiploma.FileConnection;
using BachelorDiploma.NotificationManagement;
using BachelorDiploma.RAMManagement;
using Microsoft.Win32;
using Notifications.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
        public MainWindow()
        {
            InitializeComponent();

            Notification.Show("Рекомендується мати 8 Гб оперативної пам'яті",
                "Оскільки ця програма виконує багато математичних операцій і зберігає результати, обсяг оперативної пам'яті впливає на результати.",
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
                        MessageWindow messageWindow = new MessageWindow("Помилка", "Введене число виходить за рамки діапазону типу даних! Зменшіть його розмір!");
                        messageWindow.Show();
                        return;
                    }
                    catch(FormatException)
                    {
                        MessageWindow messageWindow = new MessageWindow("Помилка",  "Числові дані введені не вірно!");
                        messageWindow.Show();
                        return;
                    }
                    InformationModelDto infoModel = new InformationModelDto(name, nominalVolume, fillingHeight, deathHeight, tankType, algorithmHullType, temperature,
                                   linearCoeffTemp, maxDistBetweenPoints, maxDepth, zeroPosition, correctiveCoeff, fromCorrectiveCoeff, toCorrectiveCoeff);
                    AdditionalTablesModelDto additionalTablesModelDto = FillAdditionalTables();
                    MainCalculation mainCalculation = new MainCalculation(infoModel, additionalTablesModelDto, FileConnectionString.ConnectionString);

                    await Task.Run(() =>
                    {
                        try
                        {
                            mainCalculation.Calculate();
                            GC.Collect();
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageWindow messageWindow = new MessageWindow("Результат обчислення", "Об'єм резервуару: " + mainCalculation.CalculationResult.Volume + "м³");
                                messageWindow.Show();
                            });
                            
                            Notification.Show("Розрахунки завершено",
                                            "Перевірте вкладку \"Результати\" і згенерований документ повірки",
                                            NotificationType.Success,
                                            0, 0, 60);
                        }
                        catch (WrongFileStructureException)
                        {
                            MessageWindow messageWindow = new MessageWindow("Помилка", "Структура файлу не вірна!");
                            messageWindow.Show();
                        }
                        catch (GrahamScanConvexHullException)
                        {
                            MessageWindow messageWindow = new MessageWindow("Помилка", "Помилка роботи алгоритму Грехема!");
                            messageWindow.Show();
                        }
                        catch(Exception exc)
                        {
                            MessageWindow messageWindow = new MessageWindow("Помилка", exc.ToString());
                            messageWindow.Show();
                        }
                    });
                }
                else
                {
                    MessageWindow messageWindow = new MessageWindow("Помилка", "Всі поля повинні бути заповнені!");
                    messageWindow.Show();
                }
            }
            else
            {
                MessageWindow messageWindow = new MessageWindow("Помилка", "Файл відсутній!");
                messageWindow.Show();
            }
        }

        private AdditionalTablesModelDto FillAdditionalTables()
        {
            AdditionalTablesModelDto tablesModelDto = new AdditionalTablesModelDto();

            tablesModelDto.T1RegNumDoc = T1RegNumDocTextBox.Text;
            tablesModelDto.T1RegDate = T1RegDateTextBox.Text;
            tablesModelDto.T1CalibrateDate = T1CalibrateDateTextBox.Text;
            tablesModelDto.T1RezType = T1RezTypeTextBox.Text;
            tablesModelDto.T1RezNumber = T1RezNumberTextBox.Text;
            tablesModelDto.T1Temperature = T1TemperatureTextBox.Text;
            tablesModelDto.T1AtmPressure = T1AtmPressureTextBox.Text;
            tablesModelDto.T1MethodPoznaka = T1MethodPoznakaTextBox.Text;
            tablesModelDto.T1MethodName = T1MethodNameTextBox.Text;
            tablesModelDto.T1MethodOrganization = T1MethodOrganizationTextBox.Text;

            tablesModelDto.T1_1Name = T1_1NameTextBox.Text;
            tablesModelDto.T1_1Type = T1_1TypeTextBox.Text;
            tablesModelDto.T1_1ServiceNum = T1_1ServiceNumTextBox.Text;
            tablesModelDto.T1_1NumSvidocDovidka = T1_1NumSvidocDovidkaTextBox.Text;
            tablesModelDto.T1_1CalibrateDate = T1_1CalibrateDateTextBox.Text;
            tablesModelDto.T1_1MainParameters = T1_1MainParametersTextBox.Text;

            tablesModelDto.T2_1Xb = T2_1XbTextBox.Text;
            tablesModelDto.T2_1Yb = T2_1YbTextBox.Text;
            tablesModelDto.T2_1BaseHeightRez = T2_1BaseHeightRezTextBox.Text;
            tablesModelDto.T2_1BaseHeightRivnemera = T2_1BaseHeightRivnemeraTextBox.Text;

            tablesModelDto.T2_2Name = T2_2NameTextBox.Text;
            tablesModelDto.T2_2Gustina = T2_2GustinaTextBox.Text;
            tablesModelDto.T2_2Riven = T2_2RivenTextBox.Text;
            tablesModelDto.T2_2Tisk = T2_2TiskTextBox.Text;
            tablesModelDto.T2_2SeverdnyaGustina = T2_2SeverdnyaGustinaTextBox.Text;

            tablesModelDto.T2_3NekontrPorojnini = T2_3NekontrPorojniniTextBox.Text;
            tablesModelDto.T2_3NizyVerhy = T2_3NizyVerhyTextBox.Text;
            tablesModelDto.T2_3Granichna = T2_3GranichnaTextBox.Text;
            tablesModelDto.T2_3Temperature = T2_3TemperatureTextBox.Text;

            tablesModelDto.T2_4CilindrTovshinaStinkiZnachenya = T2_4CilindrTovshinaStinkiZnachenyaTextBox.Text;
            tablesModelDto.T2_4CilindrTovshinaStinkiGranici = T2_4CilindrTovshinaStinkiGraniciTextBox.Text;
            tablesModelDto.T2_4CilindrTovshinaSharuFarbiZnachenya = T2_4CilindrTovshinaSharuFarbiZnachenyaTextBox.Text;
            tablesModelDto.T2_4CilindrTovshinaSharuFarbiGranici = T2_4CilindrTovshinaSharuFarbiGraniciTextBox.Text;
            tablesModelDto.T2_4CilindrFormElement = T2_4CilindrFormElementTextBox.Text;

            tablesModelDto.T2_4PerednyeDnisheTovshinaStinkiZnachenya = T2_4PerednyeDnisheTovshinaStinkiZnachenyaTextBox.Text;
            tablesModelDto.T2_4PerednyeDnisheTovshinaStinkiGranici = T2_4PerednyeDnisheTovshinaStinkiGraniciTextBox.Text;
            tablesModelDto.T2_4PerednyeDnisheTovshinaSharuFarbiZnachenya = T2_4PerednyeDnisheTovshinaSharuFarbiZnachenyaTextBox.Text;
            tablesModelDto.T2_4PerednyeDnisheTovshinaSharuFarbiGranici = T2_4PerednyeDnisheTovshinaSharuFarbiGraniciTextBox.Text;
            tablesModelDto.T2_4PerednyeDnisheFormElement = T2_4PerednyeDnisheFormElementTextBox.Text;

            tablesModelDto.T2_4ZadnyeDnisheTovshinaStinkiZnachenya = T2_4ZadnyeDnisheTovshinaStinkiZnachenyaTextBox.Text;
            tablesModelDto.T2_4ZadnyeDnisheTovshinaStinkiGranici = T2_4ZadnyeDnisheTovshinaStinkiGraniciTextBox.Text;
            tablesModelDto.T2_4ZadnyeDnisheTovshinaSharuFarbiZnachenya = T2_4ZadnyeDnisheTovshinaSharuFarbiZnachenyaTextBox.Text;
            tablesModelDto.T2_4ZadnyeDnisheTovshinaSharuFarbiGranici = T2_4ZadnyeDnisheTovshinaSharuFarbiGraniciTextBox.Text;
            tablesModelDto.T2_4ZadnyeDnisheFormElement = T2_4ZadnyeDnisheFormElementTextBox.Text;

            tablesModelDto.T2_5_1Name = T2_5_1NameTextBox.Text;
            tablesModelDto.T2_5_1GustinaRidini = T2_5_1GustinaRidiniTextBox.Text;
            tablesModelDto.T2_5_1KoeffObem = T2_5_1KoeffObemTextBox.Text;
            tablesModelDto.T2_5_1KoeffStisnennya = T2_5_1KoeffStisnennyaTextBox.Text;
            tablesModelDto.T2_5_1KoeffLiniynogo = T2_5_1KoeffLiniynogoTextBox.Text;
            
            tablesModelDto.T2_5_2Number = T2_5_2NumberTextBox.Text;
            tablesModelDto.T2_5_2DozovaMist = T2_5_2DozovaMistTextBox.Text;
            tablesModelDto.T2_5_2TemperatureInRez = T2_5_2TemperatureInRezTextBox.Text;
            tablesModelDto.T2_5_2TemperatureLichilnick = T2_5_2TemperatureLichilnickTextBox.Text;
            tablesModelDto.T2_5_2RivenRidini = T2_5_2RivenRidiniTextBox.Text;
            tablesModelDto.T2_5_2NadlishkoviyTisk = T2_5_2NadlishkoviyTiskTextBox.Text;
            
            tablesModelDto.T2_6Type = T2_6TypeTextBox.Text;
            tablesModelDto.T2_6Height = T2_6HeightTextBox.Text;
            tablesModelDto.T2_6Lenght = T2_6LenghtTextBox.Text;
            tablesModelDto.T2_6Diameter = T2_6DiameterTextBox.Text;
            tablesModelDto.T2_6KutNahily = T2_6KutNahilyTextBox.Text;
            tablesModelDto.T2_6Obem = T2_6ObemTextBox.Text;
            tablesModelDto.T2_6AbsoluteNijnyaMeja = T2_6AbsoluteNijnyaMejaTextBox.Text;
            tablesModelDto.T2_6AbsoluteVerhnyaMeja = T2_6AbsoluteVerhnyaMejaTextBox.Text;
            
            tablesModelDto.T3_2KilkistShariv = T3_2KilkistSharivTextBox.Text;
            tablesModelDto.T3_2KilkistVerticalPeretiniv = T3_2KilkistVerticalPeretinivTextBox.Text;
            
            tablesModelDto.GradPriznachenya = GradPriznachenyaTextBox.Text;
            tablesModelDto.GradOrganizaciaVlasnik = GradOrganizaciaVlasnikTextBox.Text;
            tablesModelDto.GradMisceVstanovlenya = GradMisceVstanovlenyaTextBox.Text;
            tablesModelDto.GradTypeRez = GradTypeRezTextBox.Text;
            tablesModelDto.GradNominalMist = GradNominalMistTextBox.Text;
            tablesModelDto.GradGraniciDopustimoiPohibki = GradGraniciDopustimoiPohibkiTextBox.Text;
            tablesModelDto.GradBasovaVisota = GradBasovaVisotaTextBox.Text;
            tablesModelDto.GradGranichnaVisotaNapovnenya = GradGranichnaVisotaNapovnenyaTextBox.Text;
            tablesModelDto.GradMistkistNaGranichnyVisoty = GradMistkistNaGranichnyVisotyTextBox.Text;
            tablesModelDto.GradDilyankyNizche = GradDilyankyNizcheTextBox.Text;
            tablesModelDto.GradMistkistMertvoiPoroznini = GradMistkistMertvoiPorozniniTextBox.Text;
            tablesModelDto.GradDataProvedenyaPovirki = GradDataProvedenyaPovirkiTextBox.Text;
            tablesModelDto.GradDataChergovoiPovirki = GradDataChergovoiPovirkiTextBox.Text;
            tablesModelDto.GradVsogoArkushiv = GradVsogoArkushivTextBox.Text;

            foreach(AuxiliaryEquipmentModel auxEq in AdditionalTablesModel.T1_2DopomijneObladnannya)
            {
                tablesModelDto.T1_2DopomijneObladnannya.Add(new AuxiliaryEquipmentModelDto()
                {
                    Name = auxEq.Name,
                    Type = auxEq.Type,
                    SerialNumber = auxEq.SerialNumber,
                    SertificateNumber = auxEq.SertificateNumber
                });
            }

            return tablesModelDto;
        }
    }
}
