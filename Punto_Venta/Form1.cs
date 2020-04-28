using System;
using System.Drawing;
using System.IO;
using java.net;
using java.io;
using System.Windows.Forms;
using Console = System.Console;
using java.lang;
using File = System.IO.File;

namespace Punto_Venta
{
    public partial class Form1 : Form
    {
        //variable de clase(en el mismo nivel del metodo)
        public static int num_venta;
        public const string USER_AGENT = "Mozilla/5.0";
        private string[,] productos; 
        /*private string[,] productos =
        {
            {"1","Burro percheron","100"},
            {"2","Hot dogs de la uni","60"},
            {"3","Ensalada","130"},
            {"4","Enchiladas","80"},
            {"5","Torta de civil","100"}
        };*/

        public Form1()
        {
            InitializeComponent();
            try
            {
                string numero = sendPOST("https://aloapq.000webhostapp.com/ventas_last_id.php");
                string numero2 = numero.Replace("\t", "");
                int num_venta2 = int.Parse(numero2) + 1;
                num_venta = num_venta2;
            }
            catch (System.Exception e)
            {
                Console.WriteLine("No se pudo conectar con la base de datos. Problemas con el Server Intentar otra vez");
            }

            //Codigo para jalar los datos del archivo php de la base de datos
            string resultado = sendPOST("https://aloapq.000webhostapp.com/productos_lista.php");
            string[] productos_array;
            productos_array = resultado.Split(',');
            productos_array[0] = productos_array[0].Replace("\t", "");
            
            productos = new string[productos_array.Length / 3, 3];
            //Console.WriteLine(productos);
            //Console.WriteLine(productos_array.Length);
            int i = 0;
            int j = 0;
            while (i < productos_array.Length-1)
            {
                productos[j, 0] = productos_array[i];
                
                productos[j, 1] = productos_array[i+1];
                
                productos[j, 2] = productos_array[i+2];
                Console.WriteLine(i);
                i = i + 3;
                j++;
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Location = new Point((this.Width / 2) - (label1.Width / 2), 0);
            label2.Text = DateTime.Now.ToLongTimeString() + DateTime.Now.ToLongDateString();
            label2.Location = new Point((this.Width / 2) - (label2.Width / 2), label1.Height + 1);
            dataGridView1.Width = this.Width - 10;
            dataGridView1.Height = this.Height * 3 / 4;
            dataGridView1.Location = new Point(5, label1.Height + label2.Height + 1);
            //textBox1 = this.Width - 10;
            textBox1.Location = new Point(0, this.Height - textBox1.Height - 3);
            textBox1.Width = this.Width;
            label3.Location = new Point(this.Width - dataGridView1.Columns[3].Width, label1.Height + dataGridView1.Height + 30);
            button1.Location = new Point(this.Width - dataGridView1.Columns[3].Width - label3.Width - 3, label1.Height + dataGridView1.Height + 30);
            button2.Location = new Point(this.Width - dataGridView1.Columns[2].Width - label2.Width - 3, label1.Height + dataGridView1.Height + 30);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToLongTimeString() + DateTime.Now.ToLongDateString();
        }

        private void total()
        {
            float total = 0.0f;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                total += float.Parse(dataGridView1[3, i].Value.ToString());
            }
            label3.Text = "Total = " + total;
            textBox1.Clear();
            textBox1.Focus();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void buscarProductos()
        {
            if (textBox1.Text.IndexOf('*') != -1)
            {
                string[] ej = textBox1.Text.Split('*');
                for (int i = 0; i < 5; i++)
                {
                    if (ej[1] == productos[i,0])
                    {
                        dataGridView1.Rows.Add(productos[i,2], productos[i,1], ej[0], float.Parse(productos[i,2]) * float.Parse(ej[0]));
                        total();
                    }
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (textBox1.Text == productos[i,0])
                    {
                        dataGridView1.Rows.Add(productos[i,2], productos[i,1], "1");
                        total();
                    }
                }
            }

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Form1_Load(sender, e);

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }



        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buscarProductos();
            }
            if (e.KeyCode == Keys.Escape)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
                total();
            }
            if (e.KeyCode == Keys.P)
            {
                guardar_bd(); //guarda base datos
                MessageBox.Show("Imprimiendo Ticket", "Abarrotes Covid");
                imprimirTicket();
            }

            if (e.KeyCode == Keys.Delete)
            {
                MessageBox.Show("Adios", "Abarrotes Covid");

                this.Close();
            }

            if (e.KeyCode == Keys.R)
            {
                RecargaCel();
            }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        public static string Recarga = "";
        public static string monto = "";
        private void button1_Click(object sender, EventArgs e)
        {
            RecargaCel();
        }

        private void RecargaCel()
        {
            Form2 recarga = new Form2();
            recarga.Owner = this;
            recarga.ShowDialog();
            agregarrecarga();
        }


        private void agregarrecarga()
        {
            dataGridView1.Rows.Add(monto, Recarga, 1, monto);
            total();
        }

        private void button2_Click(object sender, EventArgs e) //IMPRIMIR TICKET
        {
            MessageBox.Show("Imprimiendo Ticket", "Abarrotes Covid");
            imprimirTicket();
        }

        private void imprimirTicket()
        {
            //CAMBIAR RUTA
            
            
           
            // Create a file to write to.
            try
            {
                string path = @"C:\\Users\\pao_q\\OneDrive\\Escritorio\\Punto_Venta\\Punto_Venta\\ticket.txt";
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("\t\t\tAbarrotes Covid\n\t\t\t\tTicket\n");
                    sw.WriteLine(DateTime.Now.ToLongDateString() + "\t\t\t\t\t" + DateTime.Now.ToLongTimeString() + "\n");
                    sw.WriteLine("\tPrecio\t\tNombre\t\t\tCantidad\tTotal");


                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            sw.Write("\t" + dataGridView1.Rows[i].Cells[j].Value + "\t" + "|");
                        }
                        sw.WriteLine("");
                        sw.WriteLine("--------------------------------------------------------------------------\n");
                    }
                    sw.WriteLine("\t\t\t\t" + label3.Text);

                }

                
                MessageBox.Show("Ticket Impreso", "Abarrotes Covid");
            }   
            

            catch (System.Exception e)
            {
                e.Message.ToString();
            }
            
            limpiar(); //limpia gridview & total


        }

        private void limpiar()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            textBox1.Clear();

        }

        private void guardar_bd()
        {
            int rows = dataGridView1.Rows.Count;

            string numero = sendPOST("https://aloapq.000webhostapp.com/ventas_last_id.php");
            string numero2 = numero.Replace("\t", "");
            int num_venta2 = int.Parse(numero2) + 1;
            num_venta = num_venta2;
            int x = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < (productos.Length-1)/3 ; j++)
                {
                    
                    
                    if (productos[j,1].Equals(dataGridView1.Rows[i].Cells[1].Value))
                    {
                        
                        sendGET("https://aloapq.000webhostapp.com/Insert_Ventas.php?"
                                + "num_venta=" + num_venta + "&id_producto_venta=" + productos[j,0] + "&cantidad_venta="
                                + dataGridView1.Rows[i].Cells[2].Value);
                    }


                }
                if (dataGridView1.Rows[i].Cells[1].Value.Equals("Recarga:Telcel")
                    || dataGridView1.Rows[i].Cells[1].Value.Equals("Recarga:Movistar")
                    || dataGridView1.Rows[i].Cells[1].Value.Equals("Recarga:AT&T"))
                {
                    Console.WriteLine(dataGridView1.Rows[i].Cells[3].Value);
                    
                    if (dataGridView1.Rows[i].Cells[3].Value.Equals("100.0"))
                    {
                        x = 10;
                    }
                    else if (dataGridView1.Rows[i].Cells[3].Value.Equals("200.0")) 
                    {
                        x = 11;
                    }
                    else if (dataGridView1.Rows[i].Cells[3].Value.Equals("500.0"))
                    {
                        x = 12;
                    }
                   
                    sendGET("https://aloapq.000webhostapp.com/Insert_Ventas.php?"
                            + "num_venta=" + num_venta + "&id_producto_venta=" + x + "&cantidad_venta="
                            + dataGridView1.Rows[i].Cells[2].Value);
                }

            }

        }

        private void sendGET(string GET_URL)
        {
            var url = new URL(GET_URL);
            HttpURLConnection con = (HttpURLConnection)url.openConnection();
            con.setRequestMethod("GET");
            con.setRequestProperty("User-Agent", USER_AGENT);

            int responseCode = con.getResponseCode();
            
            if (responseCode == HttpURLConnection.HTTP_OK)
            {
                BufferedReader br = new BufferedReader(new InputStreamReader(con.getInputStream()));
                string inputLine;
                StringBuffer response = new StringBuffer();

                while ((inputLine = br.readLine()) != null)
                {
                    response.append(inputLine);
                }
                br.close();
            }
            else
            {
                Console.WriteLine("GET request not worked");
            }
            
        }

        private string sendPOST(string POST_URL)
        {
            string result = "";
            var url = new java.net.URL(POST_URL);
            HttpURLConnection con = (HttpURLConnection)url.openConnection();
            con.setRequestMethod("GET");
            con.setRequestProperty("User-Agent", USER_AGENT);
            //for POST only -START
            con.setDoOutput(true);
            OutputStream os = con.getOutputStream();
            //os.write(POST_PARAMS.getBytes());
            //os.write(Encoding.UTF8.GetBytes(POST_PARAMS)); //??
            os.flush();
            os.close();
            //for POST only -END
            int responseCode = con.getResponseCode();

            if (responseCode == HttpURLConnection.HTTP_OK)
            { //success
                BufferedReader br = new BufferedReader(new InputStreamReader(
                        con.getInputStream()));
                string inputLine;
                StringBuffer response = new StringBuffer();

                while ((inputLine = br.readLine()) != null)
                {
                    response.append(inputLine);
                }

                br.close();

                // print result
                result = response.toString();
            }
            else
            {
                Console.WriteLine("POST request not worked");
            }
            
            return result;
        }



    }
}


