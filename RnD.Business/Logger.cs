using System;

namespace RnD.Business
{
    public class Logger
    {
        public static void Logg(string mesage)
        {
            Console.WriteLine(DateTime.Now + " " + mesage);
        }

        public static void Logg(string mesage, params object[] args)
        {
            Console.WriteLine(DateTime.Now + " " + string.Format(mesage, args));
        }
    }
}
