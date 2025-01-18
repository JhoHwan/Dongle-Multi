using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DummyClientWrapper;

namespace DummyClientCore
{
    public partial class MainForm : Form
    {
        private DummyManager _manager = new DummyManager(4);
        private bool _start;
        public MainForm()
        {
            InitializeComponent();

            _manager.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 50; i++)
            {
                _manager.Connect("127.0.0.1", 7777);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _manager.DisconnectAll();

        }

        private void TestProtocol()
        {
            _start = true;
            while (_start)
            {
                for (int i = 0; i < 100; i++)
                {
                    _manager.Connect("127.0.0.1", 7777);
                }

                Thread.Sleep(2000);

                _manager.DisconnectAll();

                Thread.Sleep(2000);

            }
        }
    }
}
