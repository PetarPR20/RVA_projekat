using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFModelClient.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _testText;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string TestText
        {
            get
            {
                return _testText;
            }
            set
            {
                if (_testText != value)
                {
                    _testText = value;
                    OnPropertyChanged(nameof(TestText));
                }
            }
        }
        public MainWindowViewModel()
        {
            TestText = "Hello World";
        }
    }
}
