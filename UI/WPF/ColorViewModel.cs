using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPicker
{
    class ColorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Color color;
        CopyCommand command;

        public ColorViewModel()
        {
            color = new Color();
            command = new CopyCommand();
        }

        public CopyCommand Command
        {
            get
            {
                return command;
            }
        }

        public int R
        {
            get
            {
                return color.R;
            }

            set
            {
                RGB = new int[3] { value, G, B };
            }
        }

        public int G
        {
            get
            {
                return color.G;
            }

            set
            {
                RGB = new int[3] { R, value, B };
            }
        }

        public int B
        {
            get
            {
                return color.B;
            }

            set
            {
                RGB = new int[3] { R, G, value };
            }
        }

        public int[] RGB
        {
            get
            {
                return color.RGB;
            }

            set
            {
                color.RGB = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("RGB"));
            }
        }
    }
}
