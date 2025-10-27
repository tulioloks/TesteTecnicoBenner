using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TesteTecnicoBenner.Models.Pedidos;
using TesteTecnicoBenner.Models.Pessoas;
using TesteTecnicoBenner.Services;
using TesteTecnicoBenner.Views.Pedidos;

namespace TesteTecnicoBenner.Views.Pessoas
{
    public partial class PessoaView : Window
    {
        private List<Pessoa> pessoas;
        private readonly JsonService<Pessoa> pessoaService = new JsonService<Pessoa>("Data/pessoas.json");
        private readonly JsonService<Pedido> pedidoService = new JsonService<Pedido>("Data/pedidos.json");

        public PessoaView()
        {
            InitializeComponent();
            pessoas = pessoaService.Carregar() ?? new List<Pessoa>();
            dgPessoas.ItemsSource = pessoas;
        }

        private void Filtrar_Click(object sender, RoutedEventArgs e)
        {
            var nomeFiltro = txtNomeFiltro.Text.Trim().ToLower();
            var cpfFiltro = txtCPFFiltro.Text.Trim();

            pessoas = pessoaService.Carregar() ?? new List<Pessoa>();

            var filtradas = pessoas.Where(p =>
                (string.IsNullOrEmpty(nomeFiltro) || p.Nome.ToLower().Contains(nomeFiltro)) &&
                (string.IsNullOrEmpty(cpfFiltro) || p.CPF == cpfFiltro)
            ).ToList();

            dgPessoas.ItemsSource = filtradas;
            dgPedidosPessoa.ItemsSource = null;
            btnEditar.IsEnabled = false;
        }

        private void Incluir_Click(object sender, RoutedEventArgs e)
        {
            var telaCadastro = new PessoaEdicaoView();
            telaCadastro.ShowDialog();
            RecarregarPessoas();
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            var pessoaSelecionada = dgPessoas.SelectedItem as Pessoa;
            if (pessoaSelecionada == null) return;

            var telaEdicao = new PessoaEdicaoView(pessoaSelecionada);
            telaEdicao.ShowDialog();
            RecarregarPessoas();
        }

        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            var pessoa = dgPessoas.SelectedItem as Pessoa;
            if (pessoa == null)
            {
                MessageBox.Show("Selecione uma pessoa para excluir.");
                return;
            }

            var pedidos = pedidoService.Carregar() ?? new List<Pedido>();
            if (pedidos.Any(p => p.PessoaId == pessoa.Id))
            {
                MessageBox.Show("Não é possível excluir esta pessoa porque ela possui pedidos vinculados.");
                return;
            }

            var resultado = MessageBox.Show(
                $"Deseja realmente excluir a pessoa \"{pessoa.Nome}\"?",
                "Confirmação de exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                pessoas.Remove(pessoa);
                pessoaService.Salvar(pessoas);
                dgPessoas.ItemsSource = pessoas;
                dgPedidosPessoa.ItemsSource = null;
                MessageBox.Show("Pessoa excluída com sucesso.");
            }
        }

        private void IncluirPedido_Click(object sender, RoutedEventArgs e)
        {
            var pessoa = dgPessoas.SelectedItem as Pessoa;
            if (pessoa == null)
            {
                MessageBox.Show("Selecione uma pessoa para vincular o pedido.");
                return;
            }

            var pedidoView = new PedidoView(pessoa);
            pedidoView.ShowDialog();

            pessoas = pessoaService.Carregar() ?? new List<Pessoa>();
            dgPessoas.ItemsSource = pessoas;

            CarregarPedidosDaPessoa(pessoa);
        }

        private void CarregarPedidosDaPessoa(Pessoa pessoa)
        {
            var todosPedidos = pedidoService.Carregar() ?? new List<Pedido>();
            var pedidosDaPessoa = todosPedidos.Where(p => p.PessoaId == pessoa.Id).ToList();

            var filtro = (cbFiltroStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(filtro) && filtro != "Todos" &&
                Enum.TryParse(filtro, out EnumStatusPedido statusSelecionado))
            {
                pedidosDaPessoa = pedidosDaPessoa.Where(p => p.Status == statusSelecionado).ToList();
            }

            dgPedidosPessoa.ItemsSource = pedidosDaPessoa;
        }

        private void cbFiltroStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pessoa = dgPessoas.SelectedItem as Pessoa;
            if (pessoa != null)
            {
                CarregarPedidosDaPessoa(pessoa);
            }
        }

        private void dgPessoas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEditar.IsEnabled = dgPessoas.SelectedItem != null;

            var pessoaSelecionada = dgPessoas.SelectedItem as Pessoa;
            if (pessoaSelecionada != null)
            {
                CarregarPedidosDaPessoa(pessoaSelecionada);
            }
            else
            {
                dgPedidosPessoa.ItemsSource = null;
            }
        }

        private void AtualizarStatusPedidoSelecionado(EnumStatusPedido novoStatus)
        {
            var pedidoSelecionado = dgPedidosPessoa.SelectedItem as Pedido;
            if (pedidoSelecionado == null)
            {
                MessageBox.Show("Selecione um pedido para atualizar o status.");
                return;
            }

            var pedidos = pedidoService.Carregar() ?? new List<Pedido>();
            var pedido = pedidos.FirstOrDefault(p => p.Id == pedidoSelecionado.Id);
            if (pedido == null)
            {
                MessageBox.Show("Pedido não encontrado.");
                return;
            }

            if (pedido.Status == novoStatus)
            {
                MessageBox.Show($"O pedido já está marcado como \"{novoStatus}\".");
                return;
            }

            pedido.Status = novoStatus;
            pedidoService.Salvar(pedidos);

            var pessoa = dgPessoas.SelectedItem as Pessoa;
            if (pessoa != null)
            {
                CarregarPedidosDaPessoa(pessoa);
            }

            MessageBox.Show($"Status do pedido #{pedido.Id} atualizado para \"{novoStatus}\".");
        }

        private void MarcarComoPago_Click(object sender, RoutedEventArgs e)
        {
            AtualizarStatusPedidoSelecionado(EnumStatusPedido.Pago);
        }

        private void MarcarComoEnviado_Click(object sender, RoutedEventArgs e)
        {
            AtualizarStatusPedidoSelecionado(EnumStatusPedido.Enviado);
        }

        private void MarcarComoRecebido_Click(object sender, RoutedEventArgs e)
        {
            AtualizarStatusPedidoSelecionado(EnumStatusPedido.Recebido);
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            pessoaService.Salvar(pessoas);
            MessageBox.Show("Pessoas salvas com sucesso!");
            dgPessoas.ItemsSource = pessoaService.Carregar();
        }

        private void RecarregarPessoas()
        {
            pessoas = pessoaService.Carregar() ?? new List<Pessoa>();
            dgPessoas.ItemsSource = pessoas;
            btnEditar.IsEnabled = false;
        }
        private void txtCPFFiltro_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidadoresServiceHelper.PermitirApenasDigitos(sender, e);
        }

        private void txtCPFFiltro_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            ValidadoresServiceHelper.BloquearSenaoForDigitos(sender, e);
        }
    }
}
