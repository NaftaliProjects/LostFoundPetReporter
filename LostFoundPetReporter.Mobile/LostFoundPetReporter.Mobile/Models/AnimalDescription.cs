using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LostFoundPetReporter.Mobile.Models
{
    public class AnimalDescription : INotifyPropertyChanged
    {
        private string _name = "";
        private string _colors = "";
        private string _type = "";
        private string _breed = "";

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged();
            }
        }

        public string Colors
        {
            get => _colors;
            set
            {
                if (_colors == value)
                    return;

                _colors = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                if (_type == value)
                    return;

                _type = value;
                OnPropertyChanged();
            }
        }

        public string Breed
        {
            get => _breed;
            set
            {
                if (_breed == value)
                    return;

                _breed = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}