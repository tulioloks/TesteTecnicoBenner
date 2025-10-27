using System.Windows;
using TesteTecnicoBenner.Views.Pedidos;
using TesteTecnicoBenner.Views.Pessoas;
using TesteTecnicoBenner.Views.Produtos;

namespace TesteTecnicoBenner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AbrirPessoa_Click(object sender, RoutedEventArgs e)
        {
            var pessoaView = new PessoaView();
            pessoaView.ShowDialog();
        }

        private void AbrirProduto_Click(object sender, RoutedEventArgs e)
        {
            var produtoView = new ProdutoView();
            produtoView.ShowDialog();
        }

        private void CadastrarPedido_Click(object sender, RoutedEventArgs e)
        {
            var pedidoView = new PedidoView();
            pedidoView.ShowDialog();
        }

        private void VisualizarPedidos_Click(object sender, RoutedEventArgs e)
        {
            var pedidosView = new VisualizarPedidosView();
            pedidosView.ShowDialog();
        }
    }
}
