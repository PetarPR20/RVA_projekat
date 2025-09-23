using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPFApplication
{
    public class NewModelWrapper : INotifyPropertyChanged
    {
        private int? _id;
        public int? Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        private string _modelName;
        public string ModelName
        {
            get => _modelName;
            set { _modelName = value; OnPropertyChanged(nameof(ModelName)); }
        }

        private string _brandName;
        public string BrandName
        {
            get => _brandName;
            set { _brandName = value; OnPropertyChanged(nameof(BrandName)); }
        }

        private BodyType _bodyType;
        public BodyType BodyType
        {
            get => _bodyType;
            set { _bodyType = value; OnPropertyChanged(nameof(BodyType)); }
        }

        private int? _numberOfDoors;
        public int? NumberOfDoors
        {
            get => _numberOfDoors;
            set { _numberOfDoors = value; OnPropertyChanged(nameof(NumberOfDoors)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void Clear()
        {
            Id = null;
            ModelName = string.Empty;
            BrandName = string.Empty;
            BodyType = BodyType.HATCHBACK;
            NumberOfDoors = null;
        }
    }

}
