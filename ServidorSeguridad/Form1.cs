using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServidorSeguridad
{
    public partial class Form1 : Form
    {
        private KinectSensor miKinect;
        SocketImagen socketImagen = new SocketImagen();
        SocketComunicacion socketComunicacion;
        Esqueletos esqueletos = new Esqueletos();

        public Form1()
        {
            InitializeComponent();
            form1 = this;
        }

        private static Form1 form1;
        public static Form1 getInstance()
        {
            return form1;
        }

        public void inicializarKinect()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    miKinect = potentialSensor;
                    KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
                    break;
                }
            }

            if (miKinect != null)
            {
                try
                {
                    miKinect.Start();
                    trayBar.BalloonTipTitle = "Servidor Seguridad";
                    trayBar.BalloonTipText = "Servidor funcionando";
                    trayBar.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
                    trayBar.ShowBalloonTip(1000);
                    socketImagen.Cargar(miKinect);
                    esqueletos.Cargar(miKinect);
                    socketComunicacion= new SocketComunicacion(miKinect);
                    socketComunicacion.InitializeSockets();
                }
                catch (Exception)
                {
                    miKinect = null;
                }
            }
            else
            {
                trayBar.BalloonTipTitle = "Servidor kinect";
                trayBar.BalloonTipText = "Kinect no encontrado";
                trayBar.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Error;
                trayBar.ShowBalloonTip(1000);
            }
        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case (KinectStatus.Connected):
                    if (miKinect == null)
                        inicializarKinect();
                    break;
                //En caso de que el status sea Disconnected se la variable miKinect se volvera nula e 
                //intentaremos buscar otro dispositivo Kinect cuyo estado sea Connected
                case (KinectStatus.Disconnected):
                    miKinect = null;
                    inicializarKinect();
                    break;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                this.Visible = false;
                trayBar.Visible = true;
            }
        }

        public void desconectarKinect()
        {
            if (miKinect != null)
            {
                miKinect.Stop();
                miKinect = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            desconectarKinect();
            socketImagen.cerrarConexiones();
        }

        private void salir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cambiarAngulo_Click(object sender, EventArgs e)
        {
            Angulo angulo = new Angulo(miKinect);
            angulo.Show();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                String user = txtUser.Text;
                String pass = txtPass.Text;
                int puerto = Convert.ToInt32(txtPuerto.Text);
                DatosConfiguracion.setUser(user);
                DatosConfiguracion.setPass(pass);
                DatosConfiguracion.setPuerto(puerto);

                inicializarKinect();

                WindowState = FormWindowState.Minimized;
                trayBar.Text = "Click derecho para mostrar menu";
            }
            catch (Exception)
            {
                MessageBox.Show("Error en los datos introducidos");
            }
            
        }
    }
}
