using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TesteTecnicoBenner.Services
{
    public static class ValidadoresServiceHelper
    {
        public static bool ValidarCPF(string cpf)
        {
            string somenteNumeros = new string(cpf.Where(char.IsDigit).ToArray());
            return somenteNumeros.Length == 11;
        }

        public static void PermitirApenasDigitos(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        public static void BloquearSenaoForDigitos(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(char.IsDigit))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

    }
}
