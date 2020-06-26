using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms
{
    public partial class MainForm : Form
    {
        BooksList list;

        public MainForm()
        {
            IsMdiContainer = true;
            InitializeComponent();
            list = new BooksList();
        }

        private void newViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewInterface view = new ViewForm(this);
            ((Form)view).MdiParent = this;
            ((Form)view).Show();
            foreach (Book book in list.Get()) view.Add(book);
        }

        public void GetAllBooks(ViewInterface view)
        {
            foreach (Book book in list.Get()) view.Add(book);
        }

        public bool Add(Book book)
        {
            if(list.Add(book))
            {
                foreach(ViewInterface view in MdiChildren)
                {
                    view.Add(book);
                }
                return true;
            }
            return false;
        }

        public bool Edit(Book old, Book edited)
        {
            if (list.Edit(old, edited))
            {
                foreach (ViewInterface view in MdiChildren)
                {
                    view.Edit(old, edited);
                }
                return true;
            }
            return false;
        }

        public void Remove(Book book)
        {
            if (list.Remove(book))
            {
                foreach (ViewInterface view in MdiChildren)
                {
                    view.Remove(book);
                }
            }
        }
    }
}
