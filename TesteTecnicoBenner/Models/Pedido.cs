using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteTecnicoBenner.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public List<ProdutoPedido> Produtos { get; set; } = new List<ProdutoPedido>();
        public decimal ValorTotal => Produtos.Sum(p => p.Subtotal);
        public DateTime DataVenda { get; set; } = DateTime.Now;
        public string FormaPagamento { get; set; }
        public EnumStatusPedido Status { get; set; } = EnumStatusPedido.Pendente;
    }

    public enum EnumStatusPedido
    {
        Pendente,
        Pago,
        Enviado,
        Recebido
    }

}
