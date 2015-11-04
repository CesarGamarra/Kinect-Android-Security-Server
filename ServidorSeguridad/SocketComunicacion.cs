using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using System.Windows.Forms;
using Microsoft.Kinect;

namespace ServidorSeguridad
{
    class SocketComunicacion
    {
        static List<IWebSocketConnection> _sockets;

        KinectSensor miKinect = null;

        public SocketComunicacion()
        {
        }

        public SocketComunicacion(KinectSensor miKinect)
        {
            this.miKinect = miKinect;
        }

        public void InitializeSockets()
        {
            _sockets = new List<IWebSocketConnection>();
            int puerto = DatosConfiguracion.getPuerto() + 1;
            var server = new WebSocketServer("ws://localhost:" + puerto);

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    _sockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    _sockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    comprobarOrden(message);
                };
            });

        }

        private void comprobarOrden(String mensaje)
        {
            string [] split = mensaje.Split(' ');

            if (split[0] == "CONECTAR")
            {
                Form1.getInstance().inicializarKinect();
            }
            else if (split[0] == "DESCONECTAR")
            {
                Form1.getInstance().desconectarKinect();
            }
            else if (split[0] == "ANGULO")
            {
                try
                {
                    Angulo angulo = new Angulo(miKinect);
                    angulo.cambiarAngulo(Convert.ToInt32(split[1]));
                }
                catch (Exception)
                {
                }
            }
        }

        public void enviar(String mensaje)
        {
            foreach (var socket in _sockets)
            {
                socket.Send(mensaje);
            }
        }
    }
}
