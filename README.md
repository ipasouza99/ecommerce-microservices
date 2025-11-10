# 🛒  ecommerce-microservices – Gestão de Estoque de produtos e Vendas

Este projeto implementa uma arquitetura de microserviços para gerenciamento de estoque de produtos e vendas em uma plataforma de e-commerce.
O sistema é dividido em dois microserviços principais — Gestão de Estoque de produtos e Gestão de Vendas — que se comunicam entre si via RabbitMQ e são acessados por meio de um API Gateway.

A solução foi desenvolvida em .**NET Core (C#)**, utilizando **Entity Framework Core** e **JWT** para autenticação.

# 🧱 Arquitetura da Solução
                              ┌────────────────────┐
                        │      CLIENTE      │
                        └────────┬──────────┘
                                 │
                                 ▼
                       ┌────────────────────┐
                       │    API GATEWAY     │
                       │ (Ocelot / YARP)    │
                       └────────┬───────────┘
                    ┌───────────┴──────────────┐
                    │                          │
                    ▼                          ▼
      ┌────────────────────┐        ┌────────────────────┐
      │ Microserviço       │        │ Microserviço       │
      │ Gestão de Estoque  │        │ Gestão de Vendas   │
      │ (.NET Core API)    │        │ (.NET Core API)    │
      └────────┬───────────┘        └────────┬───────────┘
               │                             │
               │                             │
               ▼                             ▼
        ┌─────────────┐               ┌─────────────┐
        │ SQL Server  │               │ SQL Server  │
        └─────────────┘               └─────────────┘

               ▲                             │
               │                             │
               │       Comunicação via        │
               └───────► RabbitMQ ◄───────────┘


 # 🚀 Tecnologias Utilizadas

**.NET Core 8 / ASP.NET Core Web API**

**C#**

**Entity Framework Core** (ORM)

**SQL Server** (banco de dados relacional)

**RabbitMQ** (mensageria entre microserviços)

**JWT** (JSON Web Token) (autenticação)

**API Gateway** (roteamento centralizado)

**Docker Compose** (para orquestração opcional)

**Swagger** (documentação da API)

**xUnit** (testes unitários)

**Serilog**  (Logging estruturado)

**Oceltot** (API Gateway)

# Teste Unitário

**xUnit** (Framework de testes)


 # 🧩 Estrutura dos Microserviços
**🧮 Microserviço 1 – Gestão de Estoque**

Responsável por:

Cadastrar produtos (POST /api/produtos)

Consultar lista de produtos (GET /api/produtos)

Atualizar estoque automaticamente após uma venda (mensagem RabbitMQ)

Validar quantidades disponíveis

**💰 Microserviço 2 – Gestão de Vendas**

Responsável por:

Criar pedidos de venda (POST /api/pedidos)

Validar estoque antes da confirmação

Consultar pedidos (GET /api/pedidos)

Notificar o microserviço de estoque via RabbitMQ sobre a venda confirmada

# 🧠 API Gateway

Atua como ponto único de entrada para todas as requisições, roteando chamadas para o microserviço correto.

Pode ser implementado com Ocelot (biblioteca de gateway em .NET).

# 📨 Comunicação Assíncrona com RabbitMQ

A comunicação entre microserviços ocorre de forma assíncrona via RabbitMQ.
Quando uma venda é confirmada:

O MS Vendas publica uma mensagem no tópico venda_confirmada.

O MS Estoque consome essa mensagem.

O estoque do produto é automaticamente reduzido.

 # 🔐 Autenticação JWT

Usuários devem autenticar-se via /api/auth/login.

O token JWT deve ser enviado no cabeçalho Authorization: Bearer <token>.

Apenas usuários autenticados podem acessar endpoints protegidos (como /api/sales e /api/inventory).

# ⚙️ Como Executar o Projeto

**✅ Pré-requisitos**

**.NET 8 SDK**

**Docker**
 (para RabbitMQ e SQL Server)

**RabbitMQ Management**
 (usuário: guest / senha: guest)

 **Visual Studio Code** (Recomendado)

# 📦 Configuração via Docker Compose

**Um docker-compose.yml pode orquestrar:**

SQL Server

RabbitMQ

API Gateway

Microserviço de Estoque

Microserviço de Vendas

Comando:

docker-compose up -d

# 🧰 Rodando Localmente (sem Docker)

**Clone o repositório:**

git clone https://github.com/seuusuario/ecommerce-microservices.git

cd ecommerce-microservices


Configure o banco de dados nos arquivos appsettings.json de cada microserviço.

Execute as migrações:

dotnet ef database update


Execute os microserviços:

dotnet run --project src/EstoqueService
dotnet run --project src/VendasService
dotnet run --project src/ApiGateway

# 📖 Endpoints Principais

**🔹 Microserviço de Estoque**
Método	Endpoint	Descrição
POST	/api/produtos	Cadastrar novo produto
GET	/api/produtos	Listar todos os produtos
GET	/api/produtos/{id}	Consultar produto por ID

**🔹 Microserviço de Vendas**
Método	Endpoint	Descrição
POST	/api/pedidos	Criar um novo pedido
GET	/api/pedidos	Consultar todos os pedidos
GET	/api/pedidos/{id}	Detalhar um pedido específico

# 🧪 Testes Unitários

Testes desenvolvidos com xUnit e Moq, cobrindo:

Cadastro e consulta de produtos

Criação de pedidos e validação de estoque

Integração com RabbitMQ simulada

**Execute os testes:**

dotnet test

# 🩺 Monitoramento e Logs

Logs configurados com Serilog

Cada microserviço grava logs em arquivo e console

Possível integração futura com ELK Stack ou Application Insights

# ⚡ Escalabilidade

A arquitetura foi planejada para permitir:

Escalonamento horizontal de microserviços (Kubernetes ou Docker Swarm)

Adição de novos serviços (pagamentos, entrega, usuários)

Resiliência e isolamento de falhas

# 🧑‍💻 Contribuindo

**Contribuições são bem-vindas!**

Siga os passos:

Faça um fork do projeto

Crie uma branch: git checkout -b feature/nova-funcionalidade

Commit suas mudanças: git commit -m "feat: nova funcionalidade"

Faça push: git push origin feature/nova-funcionalidade

Crie um Pull Request 🎉


# 👨‍💻 Autor

[Ícaro de Souza Passos (ipasouza99)]

📧 [ipasouza99@gmail.com
]
🌐 linkedin.com/in/ipasouza99
