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
            set => SetProperty(ref _name, value);
        }

        public string Colors
        {
            get => _colors;
            set => SetProperty(ref _colors, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string Breed
        {
            get => _breed;
            set => SetProperty(ref _breed, value);
        }

        
        public event PropertyChangedEventHandler? PropertyChanged;

        private void SetProperty<T>(
            ref T backingField,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(
                    backingField,
                    value))
            {
                return;
            }

            backingField = value;

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}