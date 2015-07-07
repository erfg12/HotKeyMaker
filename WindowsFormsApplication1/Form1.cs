using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        protected override void WndProc(ref Message m) //hotbuttons
        {
            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();
                if (id == 1)
                    execBatch();
            }
            base.WndProc(ref m);
        }
        public Form1()
        {
            InitializeComponent();
        }

        private int textToInt(string text){
            if (text.Equals("TAB"))
                return 0x09;
            else
            {
                char[] myChar = hotkey.Text.ToCharArray();
                return (int)myChar[0];
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            hotkey.Text = Properties.Settings.Default.saved_key;
            filepath.Text = Properties.Settings.Default.saved_file;
            RegisterHotKey(this.Handle, 1, 0, textToInt(hotkey.Text)); // can do more if you want, or a modifier
        }

        private void execBatch()
        {
            System.Diagnostics.Process.Start(filepath.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Multiselect = true;
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                filepath.Text = file;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.saved_file = filepath.Text;
            Properties.Settings.Default.saved_key = hotkey.Text;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            this.Close();
            //Application.Restart(); // can restart if you want
        }

        private void notifyIcon1_MouseDoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
            notifyIcon1.Visible = true;
        }
    }
}
