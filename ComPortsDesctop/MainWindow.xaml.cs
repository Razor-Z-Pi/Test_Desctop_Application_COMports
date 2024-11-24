using System;
using System.IO;
using System.IO.Ports;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComPortsDesctop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;
        private Thread _thread;

        int ind = 0;
        string request;

        int bufR;
        int errorRed;
        int id1;
        int pl1;

        private int zeroIndexColor = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ProgressDataController.Instance;

            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < AdresId.GetIds().Count; i++)
            {
                ControllerComPotrMain.cOMPort.serial().WriteLine($"${AdresId.GetIds()[i]} 3 0 0 0;");
                ControllerComPotrMain.cOMPort.serial().ReadLine();
            }
            ControllerComPotrMain.cOMPort.serial().Close();
        }

        private void ScanComPorts_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем список перед новым сканированием
            ComPortsList.Items.Clear();

            // Получаем доступные COM порты
            string[] ports = SerialPort.GetPortNames();

            // Добавляем порты в ListBox
            if (ports.Length > 0)
            {
                foreach (string port in ports)
                {
                    ComPortsList.Items.Add(port);
                }
            }
            else
            {
                ComPortsList.Items.Add("COM порты не найдены");
            }
        }

        private void ComPortsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ComPortsList.SelectedItem == null)
                return;

            string selectedPort = ComPortsList.SelectedItem.ToString();

            ControllerComPotrMain.cOMPort = new ControllerCOMPort();

            ControllerComPotrMain.cOMPort.Init(selectedPort, 9600);
            ControllerComPotrMain.cOMPort.serial().ReadTimeout = 500; // Таймаут чтения 2 секунды

            if (selectedPort.Contains("COM"))
            {
                try
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        ind = 0;
                        try
                        {
                            // Отправляем строку
                            ControllerComPotrMain.cOMPort.serial().WriteLine($"${i} 0;");
                            MessageBox.Show(i.ToString());
                            // Ждем ответа
                            string response = ControllerComPotrMain.cOMPort.serial().ReadLine();
                            request = response;
                            ind += 1;
                        }
                        catch (Exception ex)
                        {
                            //Для ошибок
                        }

                        if (ind > 0)
                        {
                            MessageBox.Show(i.ToString());
                            AdresId.AddId(i);
                            ProgressDataController.Instance.SomeValue = $"Найдено устройство с id - {i.ToString()}";
                            Thread.Sleep(1000);

                        }
                        else
                        {
                            ProgressDataController.Instance.SomeValue = $"Поиск устройств по id - {i.ToString()}";
                        }
                    }


                    if (AdresId.GetIds().Count == 1)
                    {
                        MessageBox.Show("У вас рабочий один щит, что повлияет на корректность работы приложения",
                                        "Внимание",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                        AdresId.AddId(0);
                    }

                    if (AdresId.GetIds().Count == 0)
                    {
                        MessageBox.Show("У вас проблемы с подключением устройств",
                                        "Внимание",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка работы с портом: {ex.Message}");
                }

            }
        }

        private void Color_Quer_Click(object sender, RoutedEventArgs e)
        {
            // Запускаем работу в отдельном потоке
            ControllerColorRGB.thread = new Thread(ProcessData);
            ControllerColorRGB.thread.Start();
        }

        private async void ProcessData()
        {
            zeroIndexColor = 0;
            try
            {
                while (true)
                {
                    for (int i = 0; i < AdresId.GetIds().Count; i++)
                    {
                        ControllerComPotrMain.cOMPort.serial().WriteLine($"${AdresId.GetIds()[i]} 3 255 0 0 1000;");
                        ControllerComPotrMain.cOMPort.serial().ReadLine();
                        await Task.Delay(1000);
                        ControllerComPotrMain.cOMPort.serial().WriteLine($"${AdresId.GetIds()[i]} 3 0 0 255 1000;");
                        ControllerComPotrMain.cOMPort.serial().ReadLine();
                        await Task.Delay(1000);
                        ControllerComPotrMain.cOMPort.serial().WriteLine($"${AdresId.GetIds()[i]} 3 255 255 0 1000;");
                        ControllerComPotrMain.cOMPort.serial().ReadLine();
                        await Task.Delay(1000);
                        ControllerComPotrMain.cOMPort.serial().WriteLine($"${AdresId.GetIds()[i]} 3 0 128 0 1000;");
                        ControllerComPotrMain.cOMPort.serial().ReadLine();
                        await Task.Delay(1000);
                    }

                    if (zeroIndexColor == 1)
                    {
                        break;
                    }
                }

            }
            catch
            {
                //error
            }

        }

        private void Points_Click(object sender, RoutedEventArgs e)
        {
            // Запускаем работу в отдельном потоке
            _thread = new Thread(ProcessData1);
            _thread.Start();
        }

        private void ProcessData1()
        {
            if (AdresId.GetIds().Count > 1)
            {
                try
                {
                    id1 = AdresId.GetIds()[0];
                }
                catch
                {
                    MessageBox.Show("Аварийное закрытие программы, проверьте устройства.",
                                    "Ошибка",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            string res;
            DateTime currentTime = DateTime.Now;
            while (true)
            {
                try
                {

                    for (int i = 0; i <= AdresId.GetIds().Count; i++)
                    {
                        ControllerComPotrMain.cOMPort.serial().WriteLine($"${id1} 3 255 0 0;");
                        this.Dispatcher.Invoke(() =>
                        {
                            MsPush.Text = $"${id1} 3 255 0 0;";
                        });
                        ControllerComPotrMain.cOMPort.serial().ReadTimeout = 2000;
                        Task.Delay(2000);
                        ControllerComPotrMain.cOMPort.serial().WriteLine($"${id1} 2;");
                        this.Dispatcher.Invoke(() =>
                        {
                            MsPush.Text = $"${AdresId.GetIds()[i]} 2;";
                        });
                        string user1 = ControllerComPotrMain.cOMPort.serial().ReadLine();
                        this.Dispatcher.Invoke(() =>
                        {
                            MsReturn.Text = user1.ToString();
                        });
                        MessageBox.Show($"Запрос ${id1} 2; Ответ {user1}; Системное время {currentTime.ToString("HH:mm:ss")}");
                        string[] parts = user1.Split(' '); // Разделение по пробелу
                        string[] ch = parts[2].Split(";");
                        if ((user1 != null) && (int.Parse(ch[0]) > 0) && (int.Parse(ch[0]) > bufR))
                        {
                            bufR = int.Parse(ch[0]);
                            pl1 = int.Parse(ch[0]);
                            this.Dispatcher.Invoke(() =>
                            {
                                Point.Text = pl1.ToString();
                            });
                            ControllerComPotrMain.cOMPort.serial().WriteLine($"${id1} 3 0 0 0;");
                        }
                        MessageBox.Show($"Запрос ${id1} 2; Ответ {user1}; Системное время {currentTime.ToString("HH:mm:ss")}");
                    }
                }
                catch
                {
                    errorRed += 1;
                    if (errorRed > 5)
                    {
                        MessageBox.Show($"Щит, не работает");
                        break;
                    }
                }
            }
        }

        private void Stop_color_Click(object sender, RoutedEventArgs e)
        {
            zeroIndexColor = 1;
            try
            {
                for (int i = 0; i < AdresId.GetIds().Count; i++)
                {
                    ControllerComPotrMain.cOMPort.serial().WriteLine($"${AdresId.GetIds()[i]} 3 0 0 0;");
                    ControllerComPotrMain.cOMPort.serial().ReadLine();
                }
            }
            catch
            {

            }
        }

        private void Stop_point_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListId_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show(AdresId.GetIds().Count.ToString());
                for (int i = 0; i < AdresId.GetIds().Count; i++)
                {
                    MessageBox.Show(AdresId.GetIds()[i].ToString());
                }
            }
            catch
            {
                MessageBox.Show("error");
            }
            
        }
    }
}