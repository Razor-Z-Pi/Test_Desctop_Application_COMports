using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPortsDesctop
{
    internal class ControllerCOMPort
    {
        static string comPotrs = "";
        static int speed = 0;

        private SerialPort _serialPort;
        private Thread _thread;
        public void Init(string port, int s)
        {
            comPotrs = port;
            speed = s;

            _serialPort = new SerialPort(comPotrs, speed, Parity.None, 8, StopBits.One);
            _serialPort.Open();
        }

        public SerialPort serial()
        {
            return _serialPort;
        }

        public void setCom(string p, int s)
        {
            comPotrs = p;
            speed = s;
        }

        public string getCom()
        {
            return comPotrs;
        }

        public int getComSpeed()
        {
            return speed;
        }

        public void Close()
        {
            _serialPort?.Close();
            _thread?.Join(); // Дождаться завершения потока
        }
    }
}
