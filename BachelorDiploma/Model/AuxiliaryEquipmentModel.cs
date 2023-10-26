using System.ComponentModel;

namespace BachelorDiploma.Model
{
    public class AuxiliaryEquipmentModel : INotifyPropertyChanged
    {
        private string name;
        private string type;
        private string serialNumber;
        private string sertificateNumber;

        public string Name
        {
            get => name;
            set
            {
                if (name == value)
                {
                    return;
                }
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Type
        {
            get => type;
            set
            {
                if (type == value)
                {
                    return;
                }
                type = value;
                OnPropertyChanged("Type");
            }
        }
        public string SerialNumber
        {
            get => serialNumber;
            set
            {
                if (serialNumber == value)
                {
                    return;
                }
                serialNumber = value;
                OnPropertyChanged("SerialNumber");
            }
        }
        public string SertificateNumber
        {
            get => sertificateNumber;
            set
            {
                if (sertificateNumber == value)
                {
                    return;
                }
                sertificateNumber = value;
                OnPropertyChanged("SertificateNumber");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
