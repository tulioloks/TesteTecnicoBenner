using System.Collections.Generic;
using TesteTecnicoBenner.Models.Pedidos;

namespace TesteTecnicoBenner.Models.Pessoas
{
    public class Pessoa
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Endereco { get; set; }
        public List<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
