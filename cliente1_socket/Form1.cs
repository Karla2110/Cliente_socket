using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SuperSocket.SocketBase;
using SuperWebSocket;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace cliente1_socket
{
    public partial class Form1 : Form
    {
       
        static private NetworkStream flujo;
        static private StreamWriter datos_escritos;
        static private StreamReader datos_leidos;
        static private TcpClient cliente = new TcpClient();
        static private string usuario = "Desconocio";

        private delegate void DAddItem(string s); //manda informacion entre procesos

        private void AddItem(string s)
        {
            listBox1.Items.Add(s); //agrega al istbox los elementos que los diferentes procesos conectados envian
        }

        public Form1()
        {
            InitializeComponent();
            //objetos que quiero hacer visibles al iniciar 
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            button1.Visible = true;

            //objetos que quieo hacer invisibles al iniciar
            textBox4.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            listBox1.Visible = false;
        }

        //metodo que se mantiene en ecucha de mensajes de otros clientes
        void escucha()
        {
            while (cliente.Connected)
            {
                try
                {
                    this.Invoke(new DAddItem(AddItem), datos_leidos.ReadLine());
                }
                catch
                {
                    MessageBox.Show("No se ha podido conectar con el servidor, lo siento", "ATENCION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit(); //si el mensaje no se puede enviar, es dbido a que el servidor esta desconectado, asi que cerrara el programa*
                }
            }

        }

        //Metodo que conecta con el servidor
        void conectar()
        {
            try
            {
                cliente.Connect("127.0.0.1", 8088); //se le asigna la direccion ip y el puerto por el cual se conectara el cliente para encontrar el servidor
                if (cliente.Connected)
                {
                   Thread hilo = new Thread(escucha); //se crea un nuevo hilo por cada proseso que se mantien en escucha

                    flujo = cliente.GetStream();
                    datos_escritos = new StreamWriter(flujo);
                    datos_leidos = new StreamReader(flujo);

                    datos_escritos.WriteLine(usuario);
                    datos_escritos.Flush();

                    hilo.Start();
                }
                else
                {
                    MessageBox.Show("Lo siento, tenemos fallas en el servidor o esta desconectado, prueba tener conexion antes", "ATENCION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lo siento, tenemos fallas en el servidor o esta desconectado, prueba tener conexion antes", "ATENCION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        //boton de conectar o inicio de sesion
        private void button1_Click(object sender, EventArgs e)
        {
            //pregunta si los campos de usuario, contraseña y confirmacion estan vacios, o diferente de null, si cumple la condicion...
            if ((textBox1.Text != "") && (textBox2.Text != "") && (textBox3.Text != ""))
            {
                //... ahora preguntara si los textos de contraseña son iguales, si es asi entrara 
                if (textBox2.Text == textBox3.Text)
                {
                    textBox1.Visible = false; textBox2.Visible = false; textBox3.Visible = false; textBox4.Visible = true;
                    button1.Visible = false; button2.Visible = true; button3.Visible = true;
                    label1.Visible = false; label2.Visible = false; label3.Visible = false; label4.Visible = false; label5.Visible = true; label6.Visible = true;
                    listBox1.Visible = true;
                    
                    //variable que guarda el nombre del usuario y lo manda al servidor para luego conectarlo
                    usuario = textBox1.Text;
                    conectar();

                    //asigna el nombre del usuario a un label, para darle la bienvenida
                    label5.Text = textBox1.Text;
                }
                else
                {
                    MessageBox.Show("Las contraseñas no coinciden, por favor intente de nuevo.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                }

            }
            else
            {
                MessageBox.Show("Por favor llene todos los campos antes de continuar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        //boton de salir
        private void button2_Click(object sender, EventArgs e)
        {
            //Si el boton de "salir" es precioonado mostrara un cuadro de dialogo en donde preguntara al usuario si esta segudo que quiere salir del chat
            DialogResult v = MessageBox.Show("¿Estas seguro que quieres salir?", "¡Espera!", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
            if (v == DialogResult.Yes) //si el usuario dice que si a la pregunta.
            { //cerrara la ventana de chat, y mostrara la de inicio de sesion
                textBox1.Visible = true;
                textBox2.Visible = true;
                textBox3.Visible = true;
                textBox4.Visible = false;
                button1.Visible = true;
                button2.Visible = false;
                button3.Visible = false;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = false;
                label6.Visible = false;
                listBox1.Visible = false;

                textBox2.Text = "";
                textBox3.Text = "";

                //Form1 mainMenu = new Form1();

                //mainMenu.Close();
                //this.Dispose();
            }
        }

        //boton de "Enviar" este boton envia el texto escrito en la caja de texto del chat y lo envia el servidor, luego lo muestra en el listbox
        

            //funcion quepermite enviar el mensaje ppor medio de la tecla "enter"
        private void sendMessage(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {

                datos_escritos.WriteLine(textBox4.Text);
                datos_escritos.Flush();
                textBox4.Clear();
            }
        }

        //boton enviar mensaje con el metodo clic
        private void button3_Click(object sender, EventArgs e)
        {
            datos_escritos.WriteLine(textBox4.Text);
            datos_escritos.Flush();
            textBox4.Clear();
        }
    }
}
