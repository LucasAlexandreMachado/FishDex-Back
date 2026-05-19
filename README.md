# Fish Pokédex - Web API

Uma API REST completa desenvolvida em **.NET 8** com **Entity Framework Core** e **PostgreSQL** para gerenciar um registro de peixes (Fish Pokédex) com arquitetura profissional.

## 🚀 Tecnologias

- **.NET 8** - Framework Web
- **ASP.NET Core** - API REST
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL 16** - Banco de dados
- **Npgsql** - Driver PostgreSQL
- **Swagger/OpenAPI** - Documentação interativa
- **Docker** - Containerização
- **pgAdmin** - Gerenciador de banco visual

## 📋 Estrutura do Projeto

```
FishPokedex/
├── Models/
│   ├── Species.cs          # Modelo de espécie de peixe
│   ├── Catch.cs            # Modelo de captura
│   └── CatchDetail.cs      # Detalhes da captura
├── Controllers/
│   ├── SpeciesController.cs
│   ├── CatchesController.cs
│   └── CatchDetailsController.cs
├── Data/
│   └── AppDbContext.cs     # Contexto Entity Framework
├── Migrations/
│   └── Initial/            # Geradas automaticamente
└── Program.cs              # Configuração da aplicação
```

## 🗄️ Modelo de Dados

### Species (Espécie)
- `Id` (PK)
- `CommonName` (obrigatório)
- `ScientificName` (opcional)
- `Catches` (Coleção de capturas)

### Catch (Captura)
- `Id` (PK)
- `Location` (obrigatório)
- `CatchDate` (DateTime UTC)
- `SpeciesId` (FK → Species) **DELETE RESTRICT**
- `Species` (Navegação)
- `CatchDetail` (Navegação 1:1)

### CatchDetail (Detalhe da Captura)
- `Id` (PK)
- `WeightKg` (decimal, opcional)
- `LengthCm` (decimal, opcional)
- `BaitUsed` (string, opcional)
- `WeatherCondition` (string, opcional)
- `CatchId` (FK → Catch, UNIQUE) **DELETE CASCADE**
- `Catch` (Navegação)

## 🔗 Relacionamentos

- **1:N** - Um peixe tem muitas capturas (com restrição de deleção)
- **1:1** - Uma captura tem um detalhe (com cascata de deleção)

## 📍 Endpoints da API

### Species (Espécies)
```
GET    /api/species              # Listar todas as espécies
GET    /api/species/{id}         # Obter espécie específica (com catches)
POST   /api/species              # Criar nova espécie
PUT    /api/species/{id}         # Atualizar espécie
DELETE /api/species/{id}         # Deletar espécie (com validação)
```

### Catches (Capturas)
```
GET    /api/catches              # Listar todas as capturas (com Species e Detail)
GET    /api/catches/{id}         # Obter captura específica
POST   /api/catches              # Criar captura (CatchDate = UtcNow automaticamente)
PUT    /api/catches/{id}         # Atualizar captura
DELETE /api/catches/{id}         # Deletar captura
```

### CatchDetails (Detalhes)
```
GET    /api/catchdetails              # Listar todos os detalhes
GET    /api/catchdetails/{id}         # Obter detalhe por ID
GET    /api/catchdetails/catch/{catchId}  # Obter detalhe por Catch ID (404 se não encontrado)
POST   /api/catchdetails              # Criar detalhe (valida Catch existente + duplicata)
PUT    /api/catchdetails/{id}         # Atualizar detalhe
DELETE /api/catchdetails/{id}         # Deletar detalhe
```

## ⚙️ Configuração

### Banco de Dados

Connection string (appsettings.json):
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=fish_pokedex;Username=postgres;Password=postgres123"
}
```

### Docker

Arquivo [docker-compose.yml](docker-compose.yml) inclui:
- **PostgreSQL 16 Alpine** (porta 5432)
- **pgAdmin 4** (porta 8080)

Credenciais:
- Postgres: `postgres` / `postgres123`
- pgAdmin: `admin@admin.com` / `admin123`

## 🚀 Como Executar

### 1. Pré-requisitos
- .NET 8 SDK instalado
- Docker e Docker Compose

### 2. Iniciar Banco de Dados

```bash
cd /home/lucas/Documents/UFSC/UFSC-PROGRAMA/DesenvolvimentoWeb/TrabalhoFinal3
docker compose up -d
```

### 3. Executar Migrations (se necessário)

```bash
cd FishPokedex
dotnet ef migrations add ModelName     # Criar nova migração
dotnet ef database update              # Aplicar migrations
```

### 4. Rodar a Aplicação

```bash
dotnet run
```

A API estará disponível em:
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **pgAdmin**: http://localhost:8080

## 🛡️ Features de Segurança

✅ **CORS Policy Aberta** - Aceita requisições de qualquer origem
✅ **ReferenceHandler.IgnoreCycles** - Previne loops de serialização JSON
✅ **Entity Framework Foreign Keys** - Relacionamentos bem definidos
✅ **Delete Restrictions** - Validação para evitar orfandade de dados
✅ **Unique Constraints** - Índice único em CatchDetail.CatchId

## 📝 Validações

### SpeciesController
- **DELETE**: Captura erro `DbUpdateException` se houver catches relacionadas
- Retorna `400 Bad Request` com mensagem amigável

### CatchesController
- **POST**: Define `CatchDate = DateTime.UtcNow` automaticamente
- Aguarda requisição com `{ "location": "...", "speciesId": ... }`

### CatchDetailsController
- **POST**: Valida se `Catch` existe (retorna `400` se não)
- **POST**: Valida se já existe detalhe para esse Catch (retorna `409 Conflict`)
- **GET /catch/{catchId}**: Endpoint customizado com suporte a 404

## 📦 NuGet Packages

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.0.0" />
```

## 🧪 Exemplo de Uso

### Criar Espécie
```bash
curl -X POST http://localhost:5000/api/species \
  -H "Content-Type: application/json" \
  -d '{"commonName": "Truta", "scientificName": "Salmo trutta"}'
```

### Criar Captura
```bash
curl -X POST http://localhost:5000/api/catches \
  -H "Content-Type: application/json" \
  -d '{"location": "Rio Uruguai", "speciesId": 1}'
```

### Criar Detalhe de Captura
```bash
curl -X POST http://localhost:5000/api/catchdetails \
  -H "Content-Type: application/json" \
  -d '{"weightKg": 2.5, "lengthCm": 35, "baitUsed": "Minhoca", "weatherCondition": "Ensolarado", "catchId": 1}'
```

## 📄 Licença

Projeto acadêmico - UFSC

## 👥 Desenvolvido por

Lucas - Desenvolvimento Web 2026
