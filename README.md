Sistema de Cadastro (WPF)
🚀 Como executar o projeto

Pré-requisitos
.NET Framework 4.6 instalado
Visual Studio 2017 ou superior com suporte ao .NET Framework 4.6
Sistema operacional Windows
Clonar o repositório

Abrir o projeto

Abra o arquivo .sln no Visual Studio
Aguarde o carregamento das dependências
Executar
Pressione F5 ou clique em Iniciar Depuração
A tela inicial será exibida com os módulos disponíveis
📦 Persistência de Dados Os dados são armazenados em arquivos .json na pasta Data/:

pessoas.json
produtos.json
pedidos.json
Esses arquivos são carregados e salvos automaticamente pelo JsonService.

✅ Funcionalidades

Validação de CPF: aceita apenas números
Validação de valor: aceita apenas números decimais
Filtros dinâmicos por nome, CPF, status e valores
Interface responsiva e padronizada com GroupBox e DataGrid
Separação clara entre seções e botões de ação
🛠️ Dicas de uso

Para cadastrar um pedido, selecione uma pessoa e clique em Incluir Pedido
Para editar ou excluir, selecione uma linha na grid e clique no botão correspondente
Os campos CPF e Valor possuem validação para evitar entrada inválida