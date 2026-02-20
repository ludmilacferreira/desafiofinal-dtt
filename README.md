# Desafio Final

API RESTful para gerenciamento de equipamentos pesados de mineracao, desenvolvida como desafio final do programa DTT.

## Tecnologias

- **.NET 10** (ASP.NET Core Web API)
- **Entity Framework Core 10** (ORM)
- **PostgreSQL 16** (banco de dados)
- **Npgsql** (driver PostgreSQL para .NET)
- **Swagger / Swashbuckle** (documentacao interativa da API)
- **Docker & Docker Compose** (containerizacao)

## Estrutura do Projeto

```
desafiofinal-dtt/
├── desafiofinal-dtt.sln          # Solution file
└── ApiMina/
    ├── Controllers/
    │   └── EquipamentoControllers.cs   # Endpoints CRUD
    ├── Data/
    │   ├── AppDbContext.cs              # DbContext do EF Core
    │   └── criar-tabela-equipamentos-sql  # Script SQL de criacao da tabela
    ├── Dtos/
    │   ├── CreateEquipamentoDto.cs      # DTO de criacao
    │   ├── UpdateEquipamentoDto.cs      # DTO de atualizacao
    │   ├── EquipamentoResponseDto.cs    # DTO de resposta
    │   └── PageResultDto.cs             # DTO de paginacao
    ├── Models/
    │   ├── Equipamento.cs               # Entidade principal
    │   ├── TipoEquipamento.cs           # Enum de tipos
    │   └── StatusOperacional.cs         # Enum de status
    ├── Properties/
    │   └── launchSettings.json
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── docker-compose.yml
    ├── dockerfile
    ├── Program.cs                       # Ponto de entrada da aplicacao
    └── ApiMina.csproj
```

## Modelo de Dados

### Equipamento

| Campo              | Tipo           | Descricao                          |
|--------------------|----------------|------------------------------------|
| Id                 | int (PK)       | Identificador auto-incremento      |
| Codigo             | string (50)    | Codigo unico do equipamento        |
| Tipo               | enum (int)     | Tipo do equipamento                |
| Modelo             | string (120)   | Modelo/fabricante                  |
| Horimetro          | decimal (12,2) | Horas de uso acumuladas            |
| StatusOperacional  | enum (int)     | Status atual do equipamento        |
| DataAquisicao      | datetime       | Data de aquisicao                  |
| LocalizacaoAtual   | string (200)   | Localizacao atual na mina          |

### TipoEquipamento (Enum)

| Valor | Nome         |
|-------|--------------|
| 0     | Caminhao     |
| 1     | Escavadeira  |
| 2     | Perfuratriz  |
| 3     | Carregadeira |
| 4     | Trator       |

### StatusOperacional (Enum)

| Valor | Nome         |
|-------|--------------|
| 0     | Operacional  |
| 1     | EmManutencao |
| 2     | Parado       |

## Endpoints da API

Base URL: `http://localhost:5134/api/equipamentos`

| Metodo | Rota                       | Descricao                                  |
|--------|----------------------------|--------------------------------------------|
| POST   | `/api/equipamentos`        | Cria um novo equipamento                   |
| GET    | `/api/equipamentos`        | Lista equipamentos (paginado, com filtros)  |
| GET    | `/api/equipamentos/{id}`   | Busca equipamento por ID                   |
| PUT    | `/api/equipamentos/{id}`   | Atualiza um equipamento existente          |
| DELETE | `/api/equipamentos/{id}`   | Remove um equipamento                      |

### Parametros de Query (GET lista)

| Parametro  | Tipo   | Padrao | Descricao                                           |
|------------|--------|--------|------------------------------------------------------|
| page       | int    | 1      | Numero da pagina                                     |
| pageSize   | int    | 10     | Itens por pagina (max 50)                            |
| tipo       | string | null   | Filtro por tipo (Caminhao, Escavadeira, etc.)        |
| status     | string | null   | Filtro por status (Operacional, EmManutencao, Parado)|
| codigo     | string | null   | Filtro parcial por codigo (case insensitive)         |

## Pre-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://docs.docker.com/get-docker/) e [Docker Compose](https://docs.docker.com/compose/install/)
- (Opcional) [Visual Studio 2022+](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/) com extensao C#

## Como Executar

### Opcao 1: Docker Compose (recomendado)

1. Clone o repositorio:
   ```bash
   git clone https://github.com/ludmilacferreira/desafiofinal-dtt.git
   cd desafiofinal-dtt/ApiMina
   ```

2. Suba os containers (PostgreSQL + API):
   ```bash
   docker compose up -d
   ```

3. Acesse a API em `http://localhost:8080/api/equipamentos`

### Opcao 2: Execucao Local

1. Clone o repositorio:
   ```bash
   git clone https://github.com/ludmilacferreira/desafiofinal-dtt.git
   cd desafiofinal-dtt
   ```

2. Entre na pasta do projeto e suba somente o PostgreSQL via Docker:
   ```bash
   cd ApiMina
   docker compose up postgres -d
   ```
   > **Importante:** O arquivo `docker-compose.yml` esta dentro da pasta `ApiMina/`.
   > Voce precisa estar dentro dessa pasta para executar o `docker compose`.
   > Caso esteja na raiz do repositorio, use: `docker compose -f ApiMina/docker-compose.yml up postgres -d`

3. Crie o banco de dados executando o script SQL:
   ```bash
   psql -h localhost -U postgres -d apimina_db -f Data/criar-tabela-equipamentos-sql
   ```
   A senha padrao e `postgres`.

4. Execute a API (ainda dentro da pasta `ApiMina/`):
   ```bash
   dotnet run
   ```

5. Acesse a API em `http://localhost:5134/api/equipamentos`

## Como Testar

### 1. Via Swagger UI

Se o Swagger estiver habilitado no ambiente de desenvolvimento, acesse:
```
http://localhost:5134/swagger
```
A interface interativa permite testar todos os endpoints diretamente pelo navegador.

### 2. Via curl (terminal)

**Criar um equipamento (POST):**
```bash
curl -X POST http://localhost:5134/api/equipamentos \
  -H "Content-Type: application/json" \
  -d '{
    "codigo": "CAT-950G-0001",
    "tipo": "Carregadeira",
    "modelo": "Caterpillar 950G",
    "horimetro": 3200.50,
    "statusOperacional": "Operacional",
    "dataAquisicao": "2023-06-15",
    "localizacaoAtual": "Mina Carajas N4E"
  }'
```

**Listar equipamentos (GET com paginacao):**
```bash
curl "http://localhost:5134/api/equipamentos?page=1&pageSize=5"
```

**Listar com filtro por tipo:**
```bash
curl "http://localhost:5134/api/equipamentos?tipo=Caminhao"
```

**Listar com filtro por status:**
```bash
curl "http://localhost:5134/api/equipamentos?status=Operacional"
```

**Buscar por ID (GET):**
```bash
curl http://localhost:5134/api/equipamentos/1
```

**Atualizar equipamento (PUT):**
```bash
curl -X PUT http://localhost:5134/api/equipamentos/1 \
  -H "Content-Type: application/json" \
  -d '{
    "codigo": "CAT-793F-000123",
    "tipo": "Caminhao",
    "modelo": "Caterpillar 793F",
    "horimetro": 18500.00,
    "statusOperacional": "EmManutencao",
    "localizacaoAtual": "Oficina Central"
  }'
```

**Deletar equipamento (DELETE):**
```bash
curl -X DELETE http://localhost:5134/api/equipamentos/1
```

### 3. Via Postman ou Insomnia

Importe a seguinte colecao de requisicoes:

- **Base URL:** `http://localhost:5134`
- Configure os endpoints listados na secao de Endpoints acima
- Use `Content-Type: application/json` no header para POST e PUT

### 4. Via arquivo .http (Visual Studio / VS Code REST Client)

O projeto inclui o arquivo `ApiMina.http` que pode ser usado com a extensao REST Client do VS Code ou diretamente no Visual Studio.

## Respostas da API

### Sucesso

- **201 Created** - Equipamento criado (retorna o objeto criado)
- **200 OK** - Listagem ou busca por ID (retorna dados)
- **204 No Content** - Atualizacao ou exclusao bem-sucedida

### Erros

- **400 Bad Request** - Dados invalidos (campo obrigatorio ausente, tipo/status invalido, horimetro negativo)
- **404 Not Found** - Equipamento nao encontrado
- **409 Conflict** - Codigo duplicado

Exemplo de resposta de erro:
```json
{
  "message": "Tipo invalido. Valores aceitos: Caminhao, Escavadeira, Perfuratriz, Carregadeira, Trator"
}
```

### Exemplo de Resposta Paginada (GET lista)

```json
{
  "page": 1,
  "pageSize": 10,
  "totalCount": 5,
  "totalPages": 1,
  "items": [
    {
      "id": 1,
      "codigo": "CAT-793F-000123",
      "tipo": "Caminhao",
      "modelo": "Caterpillar 793F",
      "horimetro": 18234.50,
      "statusOperacional": "Operacional",
      "dataAquisicao": "2019-03-15T00:00:00",
      "localizacaoAtual": "Mina Carajas N4E"
    }
  ]
}
```

## Configuracao do Banco de Dados

A connection string padrao esta em `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=apimina_db;Username=postgres;Password=postgres"
  }
}
```

Para alterar as credenciais do PostgreSQL no Docker Compose, edite as variaveis de ambiente no `docker-compose.yml` ou crie um arquivo `.env`.

## Dados de Exemplo

O script SQL em `Data/criar-tabela-equipamentos-sql` ja insere 5 registros de exemplo:

| Codigo           | Tipo        | Modelo              | Status       | Localizacao              |
|------------------|-------------|----------------------|--------------|--------------------------|
| CAT-793F-000123  | Caminhao    | Caterpillar 793F     | Operacional  | Mina Carajas N4E         |
| KOM-PC5500-0042  | Escavadeira | Komatsu PC5500       | Operacional  | Mina Carajas N5S         |
| ATL-D11T-0007    | Trator      | Caterpillar D11T     | EmManutencao | Oficina Central          |
| SAN-DT4000-0015  | Caminhao    | Sandvik DT4000       | Parado       | Patio de Estacionamento  |
| LIE-R9800-0003   | Escavadeira | Liebherr R 9800      | Operacional  | Mina Carajas N4E         |

## Autora

Ludmila Ferreira - [GitHub](https://github.com/ludmilacferreira)


