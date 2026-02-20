# Desafio Final Bootcamp Deloitte

API desenvolvida em ASP.NET Core (.NET 10) para gerenciamento de equipamentos pesados de mineracao, permitindo cadastro, consulta, atualizacao, remocao e filtros por tipo, status, codigo e modelo.

# Tecnologias Utilizadas

- .NET 10
- Entity Framework Core 10
- PostgreSQL 16
- Docker & Docker Compose
- Swagger / Swashbuckle
- xUnit (testes unitarios)
- Coverlet (cobertura de codigo)

## Funcionalidades da API

- CRUD completo de equipamentos
- Consulta por ID
- Listagem paginada com filtros (tipo, status, codigo, modelo)
- Validacao manual no controller
- Uso de DTOs para entrada e saida
- Testes unitarios com xUnit e banco InMemory

## Fluxo de Status Operacional

Os status validos sao:
```bash
Operacional | EmManutencao | Parado
```

## Endpoints

Base URL:
```bash
http://localhost:8080/api/equipamentos
```

*GET /api/equipamentos*
Lista todos os equipamentos (paginado, com filtros).

*GET /api/equipamentos/{id}*
Retorna equipamento por ID.

*POST /api/equipamentos*
Cria um novo equipamento.

*PUT /api/equipamentos/{id}*
Atualiza equipamento completo.

*DELETE /api/equipamentos/{id}*
Remove equipamento.

### Parametros de Query (GET lista)

| Parametro  | Tipo   | Padrao | Descricao                                            |
|------------|--------|--------|-------------------------------------------------------|
| page       | int    | 1      | Numero da pagina                                      |
| pageSize   | int    | 10     | Itens por pagina (max 50)                             |
| tipo       | string | null   | Filtro por tipo (Caminhao, Escavadeira, etc.)         |
| status     | string | null   | Filtro por status (Operacional, EmManutencao, Parado) |
| codigo     | string | null   | Filtro parcial por codigo (case insensitive)          |
| modelo     | string | null   | Filtro parcial por modelo (case insensitive)          |

## Como rodar o projeto localmente

## Rodando com Docker

### 1 - Clonar o Repositorio
```bash
git clone https://github.com/ludmilacferreira/desafiofinal-dtt.git
cd desafiofinal-dtt/ApiMina
```

### 2 - Subir os Containeres
```bash
docker-compose up -d
```

Isso ira subir:
- API .NET (porta 8080)
- PostgreSQL 16 (porta 5432)

### 3 - Verificar se esta rodando
```bash
docker ps
```

### 4 - Acessar a API

Swagger:
```bash
http://localhost:8080/swagger
```

Base API:
```bash
http://localhost:8080/api/equipamentos
```

## Criando a tabela manualmente (DBeaver ou outro cliente SQL)

### Conexao com PostgreSQL

Host:
```bash
localhost
```

Porta:
```bash
5432
```

Database:
```bash
apimina_db
```

Usuario:
```bash
postgres
```

Senha:
```bash
postgres
```

### Script de Criacao de tabela
```SQL
CREATE TABLE public.equipamentos (
    id                    SERIAL PRIMARY KEY,
    codigo                VARCHAR(50)    NOT NULL,
    tipo                  INTEGER        NOT NULL,
    modelo                VARCHAR(120)   NOT NULL,
    horimetro             NUMERIC(12,2)  NOT NULL DEFAULT 0,
    status_operacional    INTEGER        NOT NULL,
    data_aquisicao        TIMESTAMPTZ    NOT NULL,
    localizacao_atual     VARCHAR(200)   NOT NULL
);

CREATE UNIQUE INDEX ux_equipamentos_codigo ON public.equipamentos (codigo);

ALTER TABLE public.equipamentos
    ADD CONSTRAINT chk_horimetro_positivo CHECK (horimetro >= 0),
    ADD CONSTRAINT chk_tipo_valido CHECK (tipo IN (0, 1, 2, 3, 4)),
    ADD CONSTRAINT chk_status_valido CHECK (status_operacional IN (0, 1, 2));
```

## Testes Unitarios

O projeto inclui testes unitarios com xUnit e banco InMemory. Para rodar:
```bash
cd ApiMina.Tests
dotnet test
```

### Cobertura de Codigo
Para gerar o relatorio de cobertura:
```bash
dotnet test --collect:"XPlat Code Coverage"
```
O relatorio HTML esta disponivel em `ApiMina.Tests/coveragereport/index.html`.

## Regras de Negocio Implementadas

- Codigo unico por equipamento
- Horimetro deve ser positivo (>= 0)
- Tipo deve estar dentro dos valores permitidos: `Caminhao`, `Escavadeira`, `Perfuratriz`, `Carregadeira`, `Trator`
- Status deve estar dentro dos valores permitidos: `Operacional`, `EmManutencao`, `Parado`
- Validacao manual no controller
- Uso de DTOs para entrada e saida
- Paginacao com limite maximo de 50 itens por pagina
