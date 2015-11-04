using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSeguridad
{
    class DatosConfiguracion
    {
        private static String user;
        private static String pass;
        private static int puerto;

        public static String getUser(){
            return user;
        }

        public static void setUser(String usuario)
        {
            user = usuario;
        }

        public static String getPass()
        {
            return pass;
        }

        public static void setPass(String password)
        {
            pass = password;
        }

        public static int getPuerto()
        {
            return puerto;
        }

        public static void setPuerto(int numeroPuerto)
        {
            puerto = numeroPuerto;
        }
    }
}
