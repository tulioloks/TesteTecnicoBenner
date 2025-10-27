using System;
using System.Windows;
using System.Windows.Controls;
using TesteTecnicoBenner.Models.Produtos;

namespace TesteTecnicoBenner.Views.Produtos
{
    public partial class ProdutoIncluirView : Window
    {
        public Produto NovoProduto { get; private set; }

        public ProdutoIncluirView()
        {
            InitializeComponent();
        }

        public ProdutoIncluirView(Produto produtoExistente) : this()
        {
            if (produtoExistente == null) return;

            txtNome.Text = produtoExistente.Nome;
            txtValor.Text = produtoExistente.Valor.ToString("F2");
            txtCodigo.Text = produtoExistente.Codigo;

            NovoProduto = new Produto
            {
                Id = produtoExistente.Id,
                Nome = produtoExistente.Nome,
                Valor = produtoExistente.Valor,
                Codigo = produtoExistente.Codigo
            };
        }

        private void txtValor_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !decimal.TryParse(((TextBox)sender).Text + e.Text, out _);
        }


        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            var nome = txtNome.Text.Trim();
            var codigo = txtCodigo.Text.Trim();
            var valorValido = decimal.TryParse(txtValor.Text, out var valor);

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(codigo) || !valorValido)
            {
                MessageBox.Show("Preencha todos os campos corretamente.");
                return;
            }

            NovoProduto = new Produto
            {
                Nome = nome,
                Valor = valor,
                Codigo = codigo
            };

            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
