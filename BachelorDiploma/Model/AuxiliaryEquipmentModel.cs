using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BachelorDiploma.Model
{
    public class AuxiliaryEquipmentModel :INotifyPropertyChanged
    {
        private string name;
        private string type;
        private string serialNumber;
        private string sertificateNumber;

        public string Name
        {
            get
            {
                return name;
            }
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
            get
            {
                return type;
            }
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
            get
            {
                return serialNumber;
            }
            set
            {
                if(serialNumber == value)
                {
                    return;
                }
                serialNumber = value;
                OnPropertyChanged("SerialNumber");
            }
        }
        public string SertificateNumber
        {
            get
            {
                return sertificateNumber;
            }
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
