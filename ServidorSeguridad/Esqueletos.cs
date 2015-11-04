using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSeguridad
{
    class Esqueletos
    {
        KinectSensor miKinect;
        SocketComunicacion comunicacion = new SocketComunicacion();

        public void Cargar(KinectSensor miKinect)
        {
            this.miKinect = miKinect;

            miKinect.SkeletonStream.Enable();
            miKinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(miKinect_SkeletonFrameReady);
        }

        private void miKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }
            if (skeletons.Length != 0)
            {
                foreach (Skeleton skel in skeletons)
                {

                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        comunicacion.enviar("SKELETON DETECTADO");
                    }
                }
            }
        }
    }
}
