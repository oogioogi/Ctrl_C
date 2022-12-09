using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyBoardHook_01
{
    public partial class Form1 : Form
    {
        private KeyBoardHooking hooking = new KeyBoardHooking();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            hooking.hook();
            hooking.KeyDown += Hooking_KeyDown;

        }

        private void Hooking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Thread.Sleep(400);
                listBox1.Items.Add(Clipboard.GetData(DataFormats.UnicodeText).ToString());
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex != -1)
            {
                Clipboard.SetData(DataFormats.UnicodeText, listBox1.SelectedItem.ToString());
            }
            
        }
    }
}
