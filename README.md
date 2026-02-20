# Desafio Final

API RESTful para gerenciamento de equipamentos pesados de mineracao, desenvolvida como desafio final do programa DTT.

## Tecnologias

- **.NET 10** (ASP.NET Core Web API)
- **Entity Framework Core 10** (ORM)
- **PostgreSQL 16** (banco de dados)
- **Npgsql** (driver PostgreSQL para .NET)
- **Swagger / Swashbuckle** (documentacao interativa da API)
- **Docker & Docker Compose** (containerizacao)


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

## Como Executar

1. Clone o repositorio:
   bash
   git clone https://github.com/ludmilacferreira/desafiofinal-dtt.git
   cd desafiofinal-dtt/ApiMina


2. Suba os containers (PostgreSQL + API):
   bash
   docker compose up -d
  

## Como Testar

### 1. Via Swagger UI

Se o Swagger estiver habilitado no ambiente de desenvolvimento, acesse:

http://localhost:5134/swagger



    
