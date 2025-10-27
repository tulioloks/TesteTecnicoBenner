using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TesteTecnicoBenner.Models.Pedidos;
using TesteTecnicoBenner.Models.Pessoas;
using TesteTecnicoBenner.Models.Produtos;
using TesteTecnicoBenner.Services;

namespace TesteTecnicoBenner.Views.Pedidos
{
    public partial class PedidoView : Window
    {
        private readonly JsonService<Pessoa> pessoaService = new JsonService<Pessoa>("Data/pessoas.json");
        private readonly JsonService<Produto> produtoService = new JsonService<Produto>("Data/produtos.json");
        private readonly JsonService<Pedido> pedidoService = new JsonService<Pedido>("Data/pedidos.json");

        private List<Pessoa> pessoas;
        private List<Produto> produtos;
        private List<ProdutoPedido> produtoPedido = new List<ProdutoPedido>();
        private Pedido pedidoAtual;
        private bool pedidoFinalizado = false;

        public PedidoView()
        {
            InitializeComponent();
            pessoas = pessoaService.Carregar() ?? new List<Pessoa>();
            produtos = produtoService.Carregar() ?? new List<Produto>();

            cbClientes.ItemsSource = pessoas;
            cbProdutos.ItemsSource = produtos;
        }

        public PedidoView(Pessoa pessoa) : this()
        {
            cbClientes.SelectedItem = pessoas.FirstOrDefault(c => c.Id == pessoa.Id);
            cbClientes.IsEnabled = false;
        }

        private void AdicionarProduto_Click(object sender, RoutedEventArgs e)
        {
            var produto = cbProdutos.SelectedItem as Produto;
            if (produto == null)
            {
                MessageBox.Show("Selecione um produto válido.");
                return;
            }

            if (!int.TryParse(txtQuantidade.Text, out int quantidade) || quantidade <= 0)
            {
                MessageBox.Show("Informe uma quantidade válida (maior que zero).");
                return;
            }

            var cliente = cbClientes.SelectedItem as Pessoa;
            if (cliente == null)
            {
                MessageBox.Show("Selecione o cliente antes de adicionar produtos.");
                return;
            }

            var formaPagamentoItem = cbFormaPagamento.SelectedItem as ComboBoxItem;
            var formaPagamento = formaPagamentoItem?.Content?.ToString();
            if (string.IsNullOrEmpty(formaPagamento))
            {
                MessageBox.Show("Selecione a forma de pagamento antes de adicionar produtos.");
                return;
            }

            var item = new ProdutoPedido
            {
                Produto = produto,
                Quantidade = quantidade,
                Nome = produto.Nome,
                ValorUnitario = produto.Valor
            };

            produtoPedido.Add(item);
            dgItensPedido.ItemsSource = null;
            dgItensPedido.ItemsSource = produtoPedido;

            AtualizarTotal();
            AtualizarPedido(cliente, formaPagamento);
        }

        private void AtualizarTotal()
        {
            var total = produtoPedido.Sum(i => i.Subtotal);
            txtValorTotal.Text = $"Total: R$ {total:F2}";
        }

        private void AtualizarPedido(Pessoa cliente, string formaPagamento)
        {
            var pedidos = pedidoService.Carregar() ?? new List<Pedido>();

            if (pedidoAtual == null)
            {
                pedidoAtual = new Pedido
                {
                    Id = pedidos.Count > 0 ? pedidos.Max(p => p.Id) + 1 : 1,
                    PessoaId = cliente.Id,
                    Produtos = new List<ProdutoPedido>(produtoPedido),
                    FormaPagamento = formaPagamento,
                    Status = EnumStatusPedido.Pendente,
                    DataVenda = DateTime.Now
                };

                pedidos.Add(pedidoAtual);
            }
            else
            {
                pedidoAtual.PessoaId = cliente.Id;
                pedidoAtual.FormaPagamento = formaPagamento;
                pedidoAtual.Produtos = new List<ProdutoPedido>(produtoPedido);

                var index = pedidos.FindIndex(p => p.Id == pedidoAtual.Id);
                if (index >= 0)
                {
                    pedidos[index] = pedidoAtual;
                }
            }

            pedidoService.Salvar(pedidos);
        }

        private void SalvarPedido_Click(object sender, RoutedEventArgs e)
        {
            var cliente = cbClientes.SelectedItem as Pessoa;
            var formaPagamento = (cbFormaPagamento.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (cliente == null || string.IsNullOrEmpty(formaPagamento) || produtoPedido.Count == 0)
            {
                MessageBox.Show("Preencha todos os campos e adicione ao menos um produto.");
                return;
            }

            AtualizarPedido(cliente, formaPagamento);
            MessageBox.Show("Pedido salvo como pendente.");
        }

        private void FinalizarPedido_Click(object sender, RoutedEventArgs e)
        {
            pedidoFinalizado = true;
            Close();
        }

        private void LimparProdutos_Click(object sender, RoutedEventArgs e)
        {
            produtoPedido.Clear();
            dgItensPedido.ItemsSource = null;
            txtValorTotal.Text = "Total: R$ 0.00";
        }

        private void PedidoView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!pedidoFinalizado && pedidoAtual != null && pedidoAtual.Status == EnumStatusPedido.Pendente)
            {
                var resultado = MessageBox.Show(
                    "O pedido ainda está pendente. Deseja realmente fechar e excluir este pedido?",
                    "Confirmação de fechamento",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    var pedidos = pedidoService.Carregar() ?? new List<Pedido>();
                    pedidos = pedidos.Where(p => p.Id != pedidoAtual.Id).ToList();
                    pedidoService.Salvar(pedidos);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void cbClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var clienteSelecionado = cbClientes.SelectedItem as Pessoa;

            if (clienteSelecionado != null && pedidoAtual != null)
            {
                pedidoAtual.PessoaId = clienteSelecionado.Id;

                var pedidos = pedidoService.Carregar() ?? new List<Pedido>();
                var index = pedidos.FindIndex(p => p.Id == pedidoAtual.Id);
                if (index >= 0)
                {
                    pedidos[index] = pedidoAtual;
                    pedidoService.Salvar(pedidos);
                }
            }
        }
    }
}
