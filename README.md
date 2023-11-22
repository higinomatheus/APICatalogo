# APICatalogo

Esta API foi desenvolvida como parte do curso "Web API ASP.NET Core Essencial", ministrado por Macoratti na Udemy.

## Sobre o Projeto

O APICatalogo é uma API construída utilizando o template ASP.NET Core Web API do .NET 6, com o intuito de exemplificar os principais conceitos relacionados à criação de serviços web. Durante o desenvolvimento do projeto, foram abordados os seguintes tópicos e práticas:

- Criação do arquivo de contexto de acesso a dados, modelos das entidades e registros de serviços na classe Program.
- Definição de relacionamentos entre as entidades do domínio para utilização do Entity Framework Migrations.
- Aplicação do recurso Migrations para a criação e atualização das tabelas.
- Utilização do Entity Framework Core como ferramenta de ORM.
- Implementação dos endpoints que expõem recursos para Categorias e Produtos.
- Adoção dos padrões Repository e Unity of Work (UoW).
- Paginação dos dados.
- Implementação de recursos de segurança e autenticação JWT.
- Criação de endpoints para a gestão de usuários e validação para a geração de tokens.
- Ajustes e configurações nos recursos do OpenApi (Swagger), utilizado para a documentação da API.

Este projeto visa oferecer uma compreensão abrangente sobre o desenvolvimento de APIs usando ASP.NET Core, abrangendo desde a modelagem de dados até a implementação de recursos de segurança.

## Como Utilizar

Para utilizar esta API, siga os passos abaixo:

1. Clone este repositório em sua máquina local.
2. Abra o projeto em sua IDE preferida.
3. Configure a string de conexão com o banco de dados no arquivo `appsettings.json`.
4. Execute as migrações para criar o banco de dados utilizando o Entity Framework Migrations.
5. Execute a aplicação e acesse a documentação gerada pelo Swagger para explorar os endpoints disponíveis.

Sinta-se à vontade para contribuir, relatar problemas ou sugerir melhorias. Estamos comprometidos em manter este projeto como uma referência de qualidade no desenvolvimento de APIs com ASP.NET Core.
