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
        #region DllImports
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, uint vlc);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

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

        Dictionary<string, uint> dictionary = new Dictionary<string, uint>();

        private uint textToVKey(string text){
            if (dictionary.ContainsKey(text))
                return dictionary[text];
            else
                return 0x00; // I don't know.
        }

        private void loadDictionary()
        {
            int i = 0;
            uint ui = 0x00;

            dictionary.Add("BACKSPACE", 0x08);
            dictionary.Add("TAB", 0x09);
            dictionary.Add("RETURN", 0x0D);
            dictionary.Add("PAUSE/BREAK", 0x13);
            dictionary.Add("CAPS LOCK", 0x14);
            dictionary.Add("ESC", 0x1B);
            dictionary.Add("SPACEBAR", 0x20);
            dictionary.Add("PAGE UP", 0x21);
            dictionary.Add("PAGE DOWN", 0x22);
            dictionary.Add("END", 0x23);
            dictionary.Add("HOME", 0x24);
            dictionary.Add("LEFT ARROW", 0x25);
            dictionary.Add("UP ARROW", 0x26);
            dictionary.Add("RIGHT ARROW", 0x27);
            dictionary.Add("DOWN ARROW", 0x28);
            dictionary.Add("PRINT SCREEN", 0x2C);
            dictionary.Add("INSERT", 0x2D);
            dictionary.Add("DELETE", 0x2E);

            //numbers
            for (ui = 0x30; ui <= 0x39; ui++)
            {
                dictionary.Add(i.ToString(), ui);
                i++;
            }

            //letters
            ui = 0x41;
            for (char c = 'A'; c <= 'Z'; c++)
            {
                dictionary.Add(c.ToString(), ui);
                ui++;
            }

            dictionary.Add("WIN LEFT", 0x5B);
            dictionary.Add("WIN RIGHT", 0x5C);

            //num pad
            i = 0;
            for (ui = 0x60; ui <= 0x69; ui++)
            {
                dictionary.Add("NUMPAD " + i.ToString(), ui);
                i++;
            }

            dictionary.Add("NUMPAD ASTERISK", 0x6A);
            dictionary.Add("NUMPAD ADD", 0x6B);
            dictionary.Add("NUMPAD SUBTRACT", 0x6D);
            dictionary.Add("NUMPAD DECIMAL", 0x6E);
            dictionary.Add("NUMPAD DIVIDE", 0x6F);

            //functions
            i = 1;
            for (ui = 0x70; ui <= 0x7B; ui++)
            {
                dictionary.Add("F" + i.ToString(), ui);
                i++;
            }

            dictionary.Add("NUMPAD NUMLOCK", 0x90); // weird uint placement, on US keyboards its on the numpad.
            dictionary.Add("SCROLL LOCK", 0x91);

            dictionary.Add("LEFT SHIFT", 0xA0);
            dictionary.Add("RIGHT SHIFT", 0xA1);
            dictionary.Add("LEFT CTRL", 0xA2);
            dictionary.Add("RIGHT CTRL", 0xA3);

            hotkey.DataSource = new BindingSource(dictionary, null);
            hotkey.DisplayMember = "Key";
            hotkey.ValueMember = "Value";
        }

        private int holdKey(string key)
        {
            if (key.Equals("ALT"))
                return 1;
            else if (key.Equals("CTRL"))
                return 2;
            else if (key.Equals("SHIFT"))
                return 4;
            else if (key.Equals("WIN"))
                return 8;
            else
                return 0; // none
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadDictionary();
            hotkey.Text = Properties.Settings.Default.saved_key;
            filepath.Text = Properties.Settings.Default.saved_file;
            holdkey.Text = Properties.Settings.Default.saved_holdkey;
            RegisterHotKey(this.Handle, 1, holdKey(holdkey.Text), textToVKey(hotkey.Text)); // can do more if you want, or a modifier
        }

        private void execBatch()
        {
            try
            {
                System.Diagnostics.Process.Start(filepath.Text);
            }
            catch
            {
                MessageBox.Show("ERROR: Missing file to execute.");
            }
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
            notifyIcon1.ShowBalloonTip(2000, "HotKeyMaker", "I'm hiding down here.",ToolTipIcon.Info);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void hotkey_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnregisterHotKey(this.Handle, 1);
            RegisterHotKey(this.Handle, 1, holdKey(holdkey.Text), textToVKey(hotkey.Text));
        }

        private void holdkey_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnregisterHotKey(this.Handle, 1);
            RegisterHotKey(this.Handle, 1, holdKey(holdkey.Text), textToVKey(hotkey.Text));
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.saved_file = filepath.Text;
            Properties.Settings.Default.saved_key = hotkey.Text;
            Properties.Settings.Default.saved_holdkey = holdkey.Text;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by The New Age Soldier ( VISIT: http://newagesoldier.com ) Created for my brother Kyle.","About HotKeyMaker", MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://newagesoldier.com");
        }
    }
}
