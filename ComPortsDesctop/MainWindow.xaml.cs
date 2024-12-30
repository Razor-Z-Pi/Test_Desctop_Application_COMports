using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO.Ports;
using System.Numerics;
using System.Text;
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
        private string nameCom = "";
        private int numberComDevices = 1;
        private string messageCom = "";

        static SerialPort master;
        static SerialPort slave;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ComName_Click(object sender, RoutedEventArgs e)
        {
            nameCom = ComInputName.Text;

            NumberComSave.Text = nameCom;

            if (messageCom != "" && nameCom != "")
            {
                Start.IsEnabled = true;
            }
            else
            {
                Start.IsEnabled = false;
            }
        }

        private void NumberDevics_Click(object sender, RoutedEventArgs e)
        {
            numberComDevices = Convert.ToInt32(ComDevices.Text);

            DevicesSave.Text = numberComDevices.ToString();

            if (messageCom != "" && nameCom != "")
            {
                Start.IsEnabled = true;
            }
            else
            {
                Start.IsEnabled = false;
            }
        }

        private void ComBtnMessage_Click(object sender, RoutedEventArgs e)
        {
            messageCom = ComMessage.Text;

            MessageSave.Text = messageCom;

            if (messageCom != "" && nameCom != "")
            {
                Start.IsEnabled = true;
            }
            else
            {
                Start.IsEnabled = false;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                COMcreate();
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void COMcreate()
        {
            string portName = $"COM{nameCom}";


            // Создайте два экземпляра SerialPort для отправки и приема
            slave = new SerialPort(portName, 9600);

            string quit = "-";
            string dataSender = "-";

            slave.Open();
            MessageBox.Show($"{portName}  успешно открыт!");


            int shtuk = numberComDevices;

            List<int> objects = new List<int>();

            for (int i = 1; i <= shtuk; i++)
            {
                objects.Add(i);
            }

            while (quit != "q")
            {
                dataSender = slave.ReadLine();

                if (dataSender != "")
                {


                    // Чтение данных

                    MessageBox.Show($"Получено через {slave.PortName}: {dataSender}");
                    string str = ProcessCommand(dataSender);
                    MessageBox.Show($"Отправлен через {slave.PortName}: {str}");
                    slave.WriteLine(str);
                }
            }

            master.Close();
            slave.Close();
            MessageBox.Show("Порты  закрыты ");
        }

        static string ProcessCommand(string input)
        {
            // Проверяем, начинается ли строка с символа "$" и содержит ли завершение ";"
            if (string.IsNullOrEmpty(input) || !input.StartsWith("$") || !input.EndsWith(";"))
            {
                return "Invalid Command";
            }

            // Извлекаем команду (убираем "$" и ";")
            string command = input.Trim('$', ';');

            // Разбиваем строку на части
            string[] parts = command.Split(' ');

            // Проверяем, есть ли необходимое количество частей
            if (parts.Length >= 2)
            {
                switch (parts[1])
                {
                    case "0":
                        return $"${parts[0]} ok;";
                        break;
                    case "1":
                        return $"${parts[0]} ok;";
                        break;
                    case "2":
                        return $"${parts[0]} 10;";
                        break;
                    case "3":
                        return $"${parts[0]} ok;";
                        break;

                }

            }
            // Формируем ответ на команду
            return $"add new command for " + input;
        }

    }
}