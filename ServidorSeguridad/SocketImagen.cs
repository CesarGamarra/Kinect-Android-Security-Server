using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ServidorSeguridad
{
    class SocketImagen
    {
        static Socket socket;
        static Socket cliente;
        static List<Socket> clientes = new List<Socket>();
        byte[] colorPixels;
        public System.Threading.Timer timer;
        KinectSensor miKinect;
        Thread tConexion;
        Thread envio;

        //WriteableBitmap colorBitmap;

        private delegate void mydelegate();


        public void Cargar(KinectSensor miKinect)
        {
            this.miKinect = miKinect;
            miKinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            miKinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(miKinect_ColorFrameReady);

            colorPixels = new byte[miKinect.ColorStream.FramePixelDataLength];
            EncenderServidor();
        }

        void EncenderServidor()
        {
            string localComputerName = Dns.GetHostName();
            IPAddress direccion;

            NetworkInterface[] ni = NetworkInterface.GetAllNetworkInterfaces();

            direccion=ni[1].GetIPProperties().UnicastAddresses[1].Address;
            IPEndPoint ep = new IPEndPoint(direccion, DatosConfiguracion.getPuerto());
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ep);
            socket.Listen(10);
            tConexion = new Thread(new ThreadStart(ThreadConexion));
            tConexion.Start();
        }
        void ThreadConexion()
        {
            while (true)
            {
                cliente = socket.Accept();

                byte[] bytes = new byte[1024];
                int bytesRec = cliente.Receive(bytes);
                String datosRecibidos = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                string[] split = datosRecibidos.Split(',');

                if (split[0].ToUpper() == DatosConfiguracion.getUser().ToUpper() && split[1].ToUpper() == DatosConfiguracion.getPass().ToUpper())
                {
                    cliente.Send(Encoding.ASCII.GetBytes("IDENTIFICACION CORRECTA"));
                    Thread.Sleep(500);
                    clientes.Add(cliente);
                    if (clientes.Count == 1)
                    {
                        envio = new Thread(new ThreadStart(envioImagen));
                        envio.Start();
                    }
                }
                else
                {
                    cliente.Send(Encoding.ASCII.GetBytes("IDENTIFICACION INCORRECTA"));
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void enviar(Byte[] pixels)
        {
            if (clientes != null)
            {
                foreach (var client in clientes)
                {
                    try
                    {
                        client.Send(pixels);
                    }catch(Exception){
                        clientes.Remove(client);
                        client.Close();
                    }
                }
            }
        }

        private void envioImagen()
        {
            try
            {
                while (true)
                {
                    enviar(colorPixels);
                    Thread.Sleep(1600);
                }
            }
            catch (Exception)
            {
            }
        }

        public void cerrarConexiones(){
            try
            {
                socket.Close();
                tConexion.Abort();
                envio.Abort();
            }
            catch (Exception)
            {
            }
        }

        void miKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    colorFrame.CopyPixelDataTo(colorPixels);
                }
            }
        }
    }
}
