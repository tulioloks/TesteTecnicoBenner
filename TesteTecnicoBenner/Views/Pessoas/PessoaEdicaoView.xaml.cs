using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TesteTecnicoBenner.Models.Pessoas;
using TesteTecnicoBenner.Services;

namespace TesteTecnicoBenner.Views.Pessoas
{
    public partial class PessoaEdicaoView : Window
    {
        private readonly JsonService<Pessoa> pessoaService = new JsonService<Pessoa>("Data/pessoas.json");
        private Pessoa pessoaEditada;

        public PessoaEdicaoView()
        {
            InitializeComponent();
        }

        public PessoaEdicaoView(Pessoa pessoa) : this()
        {
            pessoaEditada = pessoa;
            if (pessoaEditada == null) return;

            txtNome.Text = pessoaEditada.Nome;
            txtCPF.Text = pessoaEditada.CPF;
            txtEndereco.Text = pessoaEditada.Endereco;
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            var nome = txtNome.Text.Trim();
            var cpf = txtCPF.Text.Trim();
            var endereco = txtEndereco.Text.Trim();

            if (!ValidadoresServiceHelper.ValidarCPF(txtCPF.Text))
            {
                MessageBox.Show("O CPF deve conter exatamente 11 dígitos numéricos.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCPF.Focus();
                return;
            }

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cpf))
            {
                MessageBox.Show("Preencha todos os campos.");
                return;
            }

            var pessoas = pessoaService.Carregar() ?? new List<Pessoa>();
            var cpfDuplicado = pessoas.Any(p => p.CPF == cpf && p.Id != pessoaEditada?.Id);

            if (cpfDuplicado)
            {
                MessageBox.Show("Este CPF já está cadastrado.");
                return;
            }

            if (pessoaEditada == null)
            {
                var novaPessoa = new Pessoa
                {
                    Id = pessoas.Count > 0 ? pessoas.Max(p => p.Id) + 1 : 1,
                    Nome = nome,
                    CPF = cpf,
                    Endereco = endereco
                };

                pessoas.Add(novaPessoa);
            }
            else
            {
                var index = pessoas.FindIndex(p => p.Id == pessoaEditada.Id);
                if (index >= 0)
                {
                    pessoaEditada.Nome = nome;
                    pessoaEditada.CPF = cpf;
                    pessoaEditada.Endereco = endereco;
                    pessoas[index] = pessoaEditada;
                }
            }

            pessoaService.Salvar(pessoas);
            MessageBox.Show("Pessoa salva com sucesso!");
            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtCPF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidadoresServiceHelper.PermitirApenasDigitos(sender, e);
        }

        private void txtCPF_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            ValidadoresServiceHelper.BloquearSenaoForDigitos(sender, e);
        }
    }
}
