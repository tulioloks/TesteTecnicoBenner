namespace TesteTecnicoBenner.Models
{
    public class ProdutoPedido
    {
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }
        public string Nome { get; set; }        
        public decimal ValorUnitario { get; set; } 
        public decimal Subtotal => Produto != null ? Produto.Valor * Quantidade : 0;
    }
}
