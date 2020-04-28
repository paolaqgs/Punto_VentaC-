using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Punto_Venta
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            
            if (textBox2.TextLength != 0 && comboBox1.SelectedItem != null && comboBox2.SelectedItem != null )
            {
                string Recarga_Cel = "Recarga:" + comboBox1.SelectedItem.ToString();
                string monto = comboBox2.SelectedItem.ToString();
                Form1.monto = monto;
                Form1.Recarga = Recarga_Cel;
                MessageBox.Show(Recarga_Cel+"\nMonto:"+ monto, "Confirmado");
            }
            else
            {
                MessageBox.Show("Ingresa todos los campos", "Abarrotes Covid");
            }
            
            
            
            
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)//acepte puro numero
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true; 
            }

            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
    }
}
