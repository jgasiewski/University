using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPicker
{
    class Color
    {
        int[] rgb;

        public Color(int r = 0, int g = 0, int b = 0)
        {
            rgb = new int[3];
            rgb[0] = r;
            rgb[1] = g;
            rgb[2] = b;
        }

        public int R
        {
            get
            {
                return rgb[0];
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
                return rgb[1];
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
                return rgb[2];
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
                return rgb;
            }

            set
            {
                if (value.Length == 3)
                {
                    for(int i = 0; i < 3; ++i)
                    {
                        if (value[i] < 0 || value[i] > 255) return;
                    }

                    rgb = value;
                }
            }
        }
    }
}
