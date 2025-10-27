using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TesteTecnicoBenner.Models;
using TesteTecnicoBenner.Services;

namespace TesteTecnicoBenner.Views.Pedidos
{
    public partial class VisualizarPedidosView : Window
    {
        private readonly JsonService<Pedido> pedidoService = new JsonService<Pedido>("Data/pedidos.json");
        private readonly JsonService<Pessoa> pessoaService = new JsonService<Pessoa>("Data/pessoas.json");
        private List<Pedido> pedidosOriginais;

        public VisualizarPedidosView()
        {
            InitializeComponent();
            dgPedidos.SelectionChanged += DgPedidos_SelectionChanged;
            CarregarPedidos();
        }

        private void CarregarPedidos()
        {
            pedidosOriginais = pedidoService.Carregar() ?? new List<Pedido>();
            var pessoas = pessoaService.Carregar() ?? new List<Pessoa>();

            var listaExibicao = pedidosOriginais.Select(p => new PedidoExibicao
            {
                Id = p.Id,
                NomePessoa = pessoas.FirstOrDefault(c => c.Id == p.PessoaId)?.Nome ?? "Desconhecido",
                DataVenda = p.DataVenda,
                ValorTotal = p.Produtos?.Sum(i => i.Subtotal) ?? 0,
                FormaPagamento = p.FormaPagamento,
                Status = p.Status
            }).ToList();

            dgPedidos.ItemsSource = listaExibicao;
            dgProdutosDoPedido.ItemsSource = null;
        }

        private void NovoPedido_Click(object sender, RoutedEventArgs e)
        {
            var pedidoView = new PedidoView();
            pedidoView.ShowDialog();
            CarregarPedidos();
        }

        private void Atualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarPedidos();
        }

        private void DgPedidos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pedidoExibicao = dgPedidos.SelectedItem as PedidoExibicao;
            if (pedidoExibicao == null)
            {
                dgProdutosDoPedido.ItemsSource = null;
                return;
            }

            var pedidoCompleto = pedidosOriginais.FirstOrDefault(p => p.Id == pedidoExibicao.Id);
            dgProdutosDoPedido.ItemsSource = pedidoCompleto?.Produtos ?? null;
        }
    }
}
