using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TesteTecnicoBenner.Models.Produtos;
using TesteTecnicoBenner.Services;

namespace TesteTecnicoBenner.Views.Produtos
{
    public partial class ProdutoView : Window
    {
        private List<Produto> produtos;
        private readonly JsonService<Produto> produtoService = new JsonService<Produto>("Data/produtos.json");

        public ProdutoView()
        {
            InitializeComponent();
            produtos = produtoService.Carregar() ?? new List<Produto>();
            dgProdutos.ItemsSource = produtos;
        }

        private void Filtrar_Click(object sender, RoutedEventArgs e)
        {
            var nome = txtNomeFiltro.Text.Trim();
            var codigo = txtCodigoFiltro.Text.Trim();
            decimal.TryParse(txtValorMin.Text, out var min);
            decimal.TryParse(txtValorMax.Text, out var max);

            var filtrados = produtos.Where(p =>
                (string.IsNullOrEmpty(nome) || p.Nome.Contains(nome)) &&
                (string.IsNullOrEmpty(codigo) || p.Codigo.Contains(codigo)) &&
                (min == 0 || p.Valor >= min) &&
                (max == 0 || p.Valor <= max)
            ).ToList();

            dgProdutos.ItemsSource = filtrados;
        }

        private void Incluir_Click(object sender, RoutedEventArgs e)
        {
            var incluirView = new ProdutoIncluirView { Owner = this };

            if (incluirView.ShowDialog() != true) return;

            var novoProduto = incluirView.NovoProduto;
            produtos = produtoService.Carregar() ?? new List<Produto>();
            novoProduto.Id = produtos.Count > 0 ? produtos.Max(p => p.Id) + 1 : 1;
            produtos.Add(novoProduto);
            produtoService.Salvar(produtos);

            dgProdutos.ItemsSource = null;
            dgProdutos.ItemsSource = produtos;

            MessageBox.Show("Produto incluído com sucesso!");
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            var produtoSelecionado = dgProdutos.SelectedItem as Produto;
            if (produtoSelecionado == null)
            {
                MessageBox.Show("Selecione um produto para editar.");
                return;
            }

            var editarView = new ProdutoIncluirView(produtoSelecionado) { Owner = this };

            if (editarView.ShowDialog() != true) return;

            var produtoEditado = editarView.NovoProduto;
            produtos = produtoService.Carregar() ?? new List<Produto>();
            var index = produtos.FindIndex(p => p.Id == produtoSelecionado.Id);

            if (index >= 0)
            {
                produtoEditado.Id = produtoSelecionado.Id;
                produtos[index] = produtoEditado;
                produtoService.Salvar(produtos);

                dgProdutos.ItemsSource = null;
                dgProdutos.ItemsSource = produtos;

                MessageBox.Show("Produto editado com sucesso!");
            }
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            produtoService.Salvar(produtos);
            MessageBox.Show("Produtos salvos com sucesso!");
            dgProdutos.ItemsSource = null;
            dgProdutos.ItemsSource = produtoService.Carregar();
        }

        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            var selecionado = dgProdutos.SelectedItem as Produto;
            if (selecionado == null)
            {
                MessageBox.Show("Selecione um produto para excluir.");
                return;
            }

            produtos.Remove(selecionado);
            produtoService.Salvar(produtos);

            dgProdutos.ItemsSource = null;
            dgProdutos.ItemsSource = produtos;
        }
    }
}
