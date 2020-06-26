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
    public partial class ViewForm : Form, ViewInterface
    {
        private ColumnHeader titleColumn;
        private ColumnHeader authorColumn;
        private ColumnHeader genreColumn;
        private ColumnHeader dateColumn;
        private IContainer components;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem removeToolStripMenuItem;
        private ListView list;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabelRecords;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem filterToolStripMenuItem;
        private ToolStripMenuItem allToolStripMenuItem;
        private ToolStripMenuItem before1990ToolStripMenuItem;
        private ToolStripMenuItem after1990ToolStripMenuItem;
        private MainForm mainForm;

        int mode = 0;
        private ToolStripStatusLabel toolStripStatusLabel2;
        string modeSub = "Filter: all";

        public ViewForm(MainForm mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.list = new System.Windows.Forms.ListView();
            this.titleColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.authorColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.genreColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dateColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelRecords = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.before1990ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.after1990ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // list
            // 
            this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.titleColumn,
            this.authorColumn,
            this.genreColumn,
            this.dateColumn});
            this.list.ContextMenuStrip = this.contextMenuStrip1;
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.FullRowSelect = true;
            this.list.HideSelection = false;
            this.list.Location = new System.Drawing.Point(0, 24);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(284, 215);
            this.list.TabIndex = 0;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.Details;
            // 
            // titleColumn
            // 
            this.titleColumn.Text = "Title";
            // 
            // authorColumn
            // 
            this.authorColumn.Text = "Author";
            // 
            // genreColumn
            // 
            this.genreColumn.Text = "Genre";
            // 
            // dateColumn
            // 
            this.dateColumn.Text = "Date";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.editToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(118, 70);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabelRecords,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 239);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(284, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(55, 17);
            this.toolStripStatusLabel1.Text = "Records: ";
            // 
            // toolStripStatusLabelRecords
            // 
            this.toolStripStatusLabelRecords.Name = "toolStripStatusLabelRecords";
            this.toolStripStatusLabelRecords.Size = new System.Drawing.Size(13, 17);
            this.toolStripStatusLabelRecords.Text = "0";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem,
            this.before1990ToolStripMenuItem,
            this.after1990ToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.filterToolStripMenuItem.Text = "Filter";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.allToolStripMenuItem.Text = "All";
            this.allToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // before1990ToolStripMenuItem
            // 
            this.before1990ToolStripMenuItem.Name = "before1990ToolStripMenuItem";
            this.before1990ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.before1990ToolStripMenuItem.Text = "Before 1990";
            this.before1990ToolStripMenuItem.Click += new System.EventHandler(this.before1990ToolStripMenuItem_Click);
            // 
            // after1990ToolStripMenuItem
            // 
            this.after1990ToolStripMenuItem.Name = "after1990ToolStripMenuItem";
            this.after1990ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.after1990ToolStripMenuItem.Text = "After 1990";
            this.after1990ToolStripMenuItem.Click += new System.EventHandler(this.after1990ToolStripMenuItem_Click);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(33, 17);
            // 
            // ViewForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.list);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "ViewForm";
            this.Text = "Catalog";
            this.Activated += new System.EventHandler(this.ViewForm_Activated);
            this.Deactivate += new System.EventHandler(this.ViewForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewForm_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ViewForm_FormClosing(Object sender, FormClosingEventArgs e)
        {
            e.Cancel = ((MdiParent.MdiChildren.Length <= 1) && (e.CloseReason == CloseReason.UserClosing));
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditForm form = new EditForm(mainForm, EditForm.Mode.Add);
            form.ShowDialog();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var items = list.SelectedItems;
            if(items.Count == 1)
            {
                EditForm form = new EditForm(mainForm, EditForm.Mode.Update);
                Book book = new Book(items[0].SubItems[0].Text, items[0].SubItems[1].Text,
                    items[0].SubItems[2].Text, items[0].SubItems[3].Text);
                form.InitializeToUpdate(book);
                form.ShowDialog();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var items = list.SelectedItems;
            if (items.Count == 1)
            {
                Book book = new Book(items[0].SubItems[0].Text, items[0].SubItems[1].Text,
                    items[0].SubItems[2].Text, items[0].SubItems[3].Text);
                mainForm.Remove(book);
            }
        }

        public void Add(Book book)
        {
            if (Filtered(book))
            {
                list.Items.Add(new ListViewItem(book.ToArray()));
                toolStripStatusLabelRecords.Text = list.Items.Count.ToString();
            }
        }

        public void Edit(Book old, Book edited)
        {
            Remove(old);
            Add(edited);
        }

        public void Remove(Book book)
        {
            for (int i = 0; i < list.Items.Count; ++i)
            {
                if (list.Items[i].SubItems[0].Text.Equals(book.GetTitle()) &&
                    list.Items[i].SubItems[1].Text.Equals(book.GetAuthor()) &&
                    list.Items[i].SubItems[2].Text.Equals(book.GetGenre()) &&
                    list.Items[i].SubItems[3].Text.Equals(book.GetDate()))
                {

                    list.Items[i].Remove();
                    break;
                }
            }
            toolStripStatusLabelRecords.Text = list.Items.Count.ToString();
        }

        private bool Filtered(Book book)
        {
            if(mode == 0)return true;
            if (mode == 1 && book.GetYear() <= 1990) return true;
            if (mode == 2 && book.GetYear() > 1990) return true;
            return false;
        }

        private void ViewForm_Activated(object sender, EventArgs e)
        {
            ToolStripManager.Merge(statusStrip1, ToolStripManager.FindToolStrip("mainStatus"));
            statusStrip1.Hide();
            menuStrip1.Hide();
        }

        private void ViewForm_Deactivate(object sender, EventArgs e)
        {
            ToolStripManager.RevertMerge(ToolStripManager.FindToolStrip("mainStatus"), statusStrip1);
            statusStrip1.Show();
            menuStrip1.Show();
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeSub = "Filter: all";
            ChangeFilter(0);
        }

        private void before1990ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeSub = "Filter: before 1990";
            ChangeFilter(1);
        }

        private void after1990ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeSub = "Filter: after 1990";
            ChangeFilter(2);
        }

        void ChangeFilter(int fMode)
        {
            mode = fMode;
            list.Items.Clear();
            mainForm.GetAllBooks(this);
            toolStripStatusLabelRecords.Text = list.Items.Count.ToString();
            toolStripStatusLabel2.Text = modeSub;
        }
    }
}
