using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BoneEditor.Annotations;

namespace BoneEditor
{
    public class Bone : INotifyPropertyChanged
    {
        private double _z;
        private double _b;
        private string _group;
        private string _name;
        private double _x;
        private double _y;
        public int Id { get; set; }
        public string Code { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged1();
            }
        }

        public bool Enabled => !(Math.Abs(Math.Abs(X - 1)) < 0.01 && Math.Abs(Y - 1) < 0.01 &&
                                 Math.Abs(Z - 1) < 0.01 && Math.Abs(B - 1) < 0.01);

        public string Group
        {
            get => _group;
            set
            {
                if (value == _group) return;
                _group = value;
                OnPropertyChanged1();
            }
        }

        public double X
        {
            get => _x;
            set
            {
                if (value.Equals(_x)) return;
                _x = value;
                OnPropertyChanged1();
                OnPropertyChanged1(nameof(Enabled));
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (value.Equals(_y)) return;
                _y = value;
                OnPropertyChanged1();
                OnPropertyChanged1(nameof(Enabled));
            }
        }

        public double Z
        {
            get => _z;
            set
            {
                if (value.Equals(_z)) return;
                _z = value;
                OnPropertyChanged1();
                OnPropertyChanged1(nameof(Enabled));
            }
        }

        public double B
        {
            get => _b;
            set
            {
                if (value.Equals(_b)) return;
                _b = value;
                OnPropertyChanged1();
                OnPropertyChanged1(nameof(Enabled));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged1([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Id},{Code},{Enabled},{X:0.##},{Y:0.##},{Z:0.##},{B:0.##}";
        }
    }
}