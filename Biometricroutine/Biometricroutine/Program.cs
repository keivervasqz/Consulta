using System;
using System.Threading;

namespace Biometricroutine
{
    class Program
    {
        static void Main(string[] args)
        {
            BiometricSoft method = new BiometricSoft();
            string option = "", ip = "", target = "";

            try
            {
                if (args.Length > 0)
                {
                    option = args[0];

                    if (args.Length > 1)
                    {
                        ip = args[1];
                    }
                    if (args.Length > 2)
                    {
                        target = args[2];
                    }
                    method.main(option, ip, target);
                }
                else
                {
                    return;
                }
                
            }
            catch (Exception ex)
            {
                method.LogsERROR("Verificación de Argumentos", ex.Message, "");
            }
            
            Thread.Sleep(100);
        }
    }
}