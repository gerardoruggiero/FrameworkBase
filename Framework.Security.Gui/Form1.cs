using Framework.Security.Encryption;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Logicalis.Framework.Security.Gui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnEncriptar.Click += BtnEncriptar_Click;
            btnDesencriptar.Click += BtnDesencriptar_Click;
        }

        private void BtnDesencriptar_Click(object sender, EventArgs e)
        {
            string text = DataEncryptor.Decrypt(txtTexto.Text.Trim());
            txtTextoTransformado.Text = text;
        }

        private void BtnEncriptar_Click(object sender, EventArgs e)
        {
            string text = DataEncryptor.Encrypt(txtTexto.Text.Trim());
            txtTextoTransformado.Text = text;
        }
    }
}
