using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPortsDesctop
{
    public class ProgressDataController : INotifyPropertyChanged
    {
        private static ProgressDataController _instance;

        public static ProgressDataController Instance => _instance ??= new ProgressDataController();

        private string id;
        public string SomeValue
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(SomeValue));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
