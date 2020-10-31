using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace cliente1_socket
{
    
    public partial class Chat : Form
    {
        Form1 inicio;

        static public NetworkStream stream;
        static public StreamWriter streamW;
        static public StreamReader streamR;
        static public TcpClient cliente = new TcpClient();
        static public string dato = "Desconocio ";
        public delegate void DAddItem(String s); //manda informacion entre procesos


        public Chat()
        {
            InitializeComponent();
        }
        
        private void AddItem(String s)
        {
            listBox1.Items.Add(s);
        }

        public void escucha()
        {
            while (cliente.Connected)
            {
                try
                {
                    this.Invoke(new DAddItem(AddItem), streamR.ReadLine());
                }
                catch
                {
                    MessageBox.Show("No se ha podido conectar con el servidor, lo siento");
                    Application.Exit();
                }
            }

        }

        public void conectar()
        {
            try
            {
                cliente.Connect("127.0.0.1", 8088);
                if (cliente.Connected)
                {
                    Thread hilo = new Thread(escucha);

                    stream = cliente.GetStream();
                    streamW = new StreamWriter(stream);
                    streamR = new StreamReader(stream);

                    streamW.WriteLine(dato);
                    streamW.Flush();

                    hilo.Start();
                }
                else
                {
                    MessageBox.Show("Lo siento, tenemos fallas en el servidor, prueba tener conexion antes");
                    Application.Exit();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lo siento, tenemos fallas en el servidor, prueba tener conexion antes");
                Application.Exit();
            }
        }
        private void Chat_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult v = MessageBox.Show("¿Estas seguro que quieres salir?", "¡Espera!", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
            if (v == DialogResult.Yes)
            {
                inicio = new Form1();
                inicio.Show();
                this.Hide();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //streamW.WriteLine(label2.Text);
            //streamW.Flush();

            dato = label2.Text;
            conectar();
        }
    }
}
