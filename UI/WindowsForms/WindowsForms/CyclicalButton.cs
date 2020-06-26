using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms
{
    public partial class CyclicalButton : UserControl
    {
        public enum Genre
        {
            Adventure,
            Romance,
            SF,
        }

        int idx = 0;
        List<Image> images;

        EventHandler onChangedHandler = null;

        public CyclicalButton()
        {
            images = new List<Image>();
            images.Add(WindowsForms.Properties.Resources.blue);
            images.Add(WindowsForms.Properties.Resources.red);
            images.Add(WindowsForms.Properties.Resources.green);

            InitializeComponent();
            UpdateImg();
        }

        public void onButtonChangedHandler(EventHandler e)
        {
            onChangedHandler = e;
            if (onChangedHandler != null)
            {
                EventArgs args = new EventArgs();
                onChangedHandler(this, args);
            }
        }

        [Category("Genre")]
        [Browsable(true)]
        public Genre Value
        {
            get { return (Genre)idx; }
            set
            {
                idx = (int)value;
                UpdateImg();
            }
        }

        public void UpdateImg()
        {
            if (button.Image != images[idx])
            {
                button.Image = images[idx];
                label.Text = ((Genre)idx).ToString();
                if (onChangedHandler != null)
                {
                    EventArgs e = new EventArgs();
                    onChangedHandler(this, e);
                }
            }
        }

        public void SetValue(string genre)
        {
            Genre g;
            Enum.TryParse(genre, out g);
            idx = (int)g;
            UpdateImg();
        }

        private void button_Click(object sender, EventArgs e)
        {
            idx = (idx + 1) % 3;
            UpdateImg();
        }
    }
}
