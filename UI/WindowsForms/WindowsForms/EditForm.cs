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
    public partial class EditForm : Form
    {
        private Button cancelButton;
        private Button okButton;
        private TextBox authorTextBox;
        private TextBox titleTextBox;
        private DateTimePicker dateTimePicker;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;

        public enum Mode
        {
            Add,
            Update
        }

        MainForm mainForm;
        Book output = null;
        Book updated = null;
        private ErrorProvider errorProvider;
        private IContainer components;
        private CyclicalButton genreButton;
        Mode mode;

        public EditForm(MainForm mainForm, Mode mode = Mode.Add)
        {
            this.mainForm = mainForm;
            this.mode = mode;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.authorTextBox = new System.Windows.Forms.TextBox();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.genreButton = new WindowsForms.CyclicalButton();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(197, 126);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 19;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(87, 126);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 18;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // authorTextBox
            // 
            this.authorTextBox.Location = new System.Drawing.Point(87, 28);
            this.authorTextBox.Name = "authorTextBox";
            this.authorTextBox.Size = new System.Drawing.Size(185, 20);
            this.authorTextBox.TabIndex = 17;
            this.authorTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.authorTextBox_Validating);
            // 
            // titleTextBox
            // 
            this.titleTextBox.Location = new System.Drawing.Point(87, 2);
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(185, 20);
            this.titleTextBox.TabIndex = 16;
            this.titleTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.titleTextBox_Validating);
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.Location = new System.Drawing.Point(87, 80);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(185, 20);
            this.dateTimePicker.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Genre";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Author";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Title";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // genreButton
            // 
            this.genreButton.Location = new System.Drawing.Point(87, 53);
            this.genreButton.Name = "genreButton";
            this.genreButton.Size = new System.Drawing.Size(81, 23);
            this.genreButton.TabIndex = 21;
            this.genreButton.Value = WindowsForms.CyclicalButton.Genre.Romance;
            // 
            // EditForm
            // 
            this.ClientSize = new System.Drawing.Size(302, 161);
            this.Controls.Add(this.genreButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.authorTextBox);
            this.Controls.Add(this.titleTextBox);
            this.Controls.Add(this.dateTimePicker);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditForm";
            this.Text = "Book";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public void InitializeToUpdate(Book book)
        {
            updated = book;
            titleTextBox.Text = book.GetTitle();
            authorTextBox.Text = book.GetAuthor();
            genreButton.SetValue(book.GetGenre());
            dateTimePicker.Text = book.GetDate();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {
                output = new Book(titleTextBox.Text, authorTextBox.Text, 
                    genreButton.Value.ToString(), dateTimePicker.Value.ToShortDateString());

                switch (mode)
                {
                    case Mode.Add:
                        if(mainForm.Add(output)) DialogResult = DialogResult.OK;
                        else
                        {
                            errorProvider.SetError(titleTextBox, "Book already exists");
                        }
                        break;
                    case Mode.Update:
                        if (mainForm.Edit(updated, output)) DialogResult = DialogResult.OK;
                        else
                        {
                            errorProvider.SetError(titleTextBox, "Book already exists");
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void titleTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (titleTextBox.Text == "")
            {
                errorProvider.SetError(titleTextBox, "Book name cannot be blank");
                e.Cancel = true;
            }
        }

        private void authorTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (authorTextBox.Text == "")
            {
                errorProvider.SetError(authorTextBox, "Enter authors name");
                e.Cancel = true;
            }
        }
    }
}
