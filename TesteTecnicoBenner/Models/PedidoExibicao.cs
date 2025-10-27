using System;

namespace TesteTecnicoBenner.Models
{
    public class PedidoExibicao
    {
        public int Id { get; set; }
        public string NomePessoa { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public string FormaPagamento { get; set; }
        public EnumStatusPedido Status { get; set; }
    }

}
