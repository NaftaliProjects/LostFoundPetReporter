using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class CreateAnimalDescription
    {
        private string _name = "";
        private string _colors = "";
        private string _type = "";
        private string _breed = "";

        private string _sex = "";
        private double? _age;
        private string _size = "";
        private double? _weightKg;

        private string _coatLength = "";
        private string _coatType = "";
        private string _pattern = "";

        private string _distinctiveMarkings = "";
        private string _eyeColor = "";
        private string _earDescription = "";
        private string _tailDescription = "";

        private bool? _collarPresent;
        private string _collarColor = "";
        private string _collarType = "";

        private bool? _harnessPresent;
        private string _harnessColor = "";

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

        public string Sex
        {
            get => _sex;
            set => SetProperty(ref _sex, value);
        }

        public double? Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        public string Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        public double? WeightKg
        {
            get => _weightKg;
            set => SetProperty(ref _weightKg, value);
        }

        public string CoatLength
        {
            get => _coatLength;
            set => SetProperty(ref _coatLength, value);
        }

        public string CoatType
        {
            get => _coatType;
            set => SetProperty(ref _coatType, value);
        }

        public string Pattern
        {
            get => _pattern;
            set => SetProperty(ref _pattern, value);
        }

        public string DistinctiveMarkings
        {
            get => _distinctiveMarkings;
            set => SetProperty(ref _distinctiveMarkings, value);
        }

        public string EyeColor
        {
            get => _eyeColor;
            set => SetProperty(ref _eyeColor, value);
        }

        public string EarDescription
        {
            get => _earDescription;
            set => SetProperty(ref _earDescription, value);
        }

        public string TailDescription
        {
            get => _tailDescription;
            set => SetProperty(ref _tailDescription, value);
        }

        public bool? CollarPresent
        {
            get => _collarPresent;
            set => SetProperty(ref _collarPresent, value);
        }

        public string CollarColor
        {
            get => _collarColor;
            set => SetProperty(ref _collarColor, value);
        }

        public string CollarType
        {
            get => _collarType;
            set => SetProperty(ref _collarType, value);
        }

        public bool? HarnessPresent
        {
            get => _harnessPresent;
            set => SetProperty(ref _harnessPresent, value);
        }

        public string HarnessColor
        {
            get => _harnessColor;
            set => SetProperty(ref _harnessColor, value);
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
