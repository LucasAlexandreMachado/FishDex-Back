# 🧪 Guia de Testes - Fish Pokédex API

## Preparação

Certifique-se que a aplicação está rodando:

```bash
# Terminal 1: Iniciar banco de dados
cd /home/lucas/Documents/UFSC/UFSC-PROGRAMA/DesenvolvimentoWeb/TrabalhoFinal3
docker compose up -d

# Terminal 2: Rodar aplicação
cd FishPokedex
dotnet run
```

A API estará em: **http://localhost:5000**

## 🧪 Testes Com cURL

### 1. Listar Espécies (Vazio inicialmente)
```bash
curl http://localhost:5000/api/species
```
Esperado: `[]`

### 2. Criar uma Espécie
```bash
SPECIES_1=$(curl -s -X POST http://localhost:5000/api/species \
  -H "Content-Type: application/json" \
  -d '{
    "commonName": "Truta Arco-Íris",
    "scientificName": "Oncorhynchus mykiss"
  }' | jq -r '.id')

echo "Species ID: $SPECIES_1"
```

### 3. Criar Segunda Espécie
```bash
SPECIES_2=$(curl -s -X POST http://localhost:5000/api/species \
  -H "Content-Type: application/json" \
  -d '{
    "commonName": "Bagre",
    "scientificName": "Silurus glanis"
  }' | jq -r '.id')

echo "Species ID: $SPECIES_2"
```

### 4. Listar Espécies Criadas
```bash
curl http://localhost:5000/api/species | jq
```

### 5. Obter Espécie Específica
```bash
curl http://localhost:5000/api/species/1 | jq
```

### 6. Criar Captura
```bash
CATCH_1=$(curl -s -X POST http://localhost:5000/api/catches \
  -H "Content-Type: application/json" \
  -d '{
    "location": "Rio Uruguai",
    "speciesId": 1
  }' | jq -r '.id')

echo "Catch ID: $CATCH_1"
```

### 7. Criar Detalhe da Captura
```bash
curl -s -X POST http://localhost:5000/api/catchdetails \
  -H "Content-Type: application/json" \
  -d '{
    "weightKg": 2.5,
    "lengthCm": 35,
    "baitUsed": "Minhoca",
    "weatherCondition": "Ensolarado",
    "catchId": 1
  }' | jq
```

### 8. Obter Detalhe por Catch ID
```bash
curl http://localhost:5000/api/catchdetails/catch/1 | jq
```

### 9. Listar Todas as Capturas (com relacionamentos)
```bash
curl http://localhost:5000/api/catches | jq
```

### 10. Criar Segunda Captura
```bash
curl -s -X POST http://localhost:5000/api/catches \
  -H "Content-Type: application/json" \
  -d '{
    "location": "Lagoa Mirin",
    "speciesId": 2
  }' | jq
```

### 11. Tentar Deletar Espécie com Capturas (Deve falhar)
```bash
curl -X DELETE http://localhost:5000/api/species/1 \
  -H "Content-Type: application/json"
```
Esperado: `400 Bad Request` com mensagem "Cannot delete species because there are linked catches."

### 12. Deletar Captura
```bash
curl -X DELETE http://localhost:5000/api/catches/1 \
  -H "Content-Type: application/json"
```

### 13. Agora Deletar Espécie (Deve funcionar)
```bash
curl -X DELETE http://localhost:5000/api/species/1 \
  -H "Content-Type: application/json"
```

### 14. Validação: Tentar Criar Detalhe para Catch Inexistente
```bash
curl -s -X POST http://localhost:5000/api/catchdetails \
  -H "Content-Type: application/json" \
  -d '{
    "weightKg": 1.0,
    "lengthCm": 25,
    "baitUsed": "Minhoca",
    "weatherCondition": "Nublado",
    "catchId": 999
  }' | jq
```
Esperado: `400 Bad Request` - "The referenced Catch does not exist."

### 15. Validação: Tentar Criar Detalhe Duplicado
```bash
# Primeiro detalhe (sucesso)
curl -s -X POST http://localhost:5000/api/catchdetails \
  -H "Content-Type: application/json" \
  -d '{
    "weightKg": 3.0,
    "lengthCm": 40,
    "baitUsed": "Minhoca",
    "weatherCondition": "Ensolarado",
    "catchId": 2
  }' | jq

# Segundo detalhe para mesma captura (deve falhar)
curl -s -X POST http://localhost:5000/api/catchdetails \
  -H "Content-Type: application/json" \
  -d '{
    "weightKg": 2.0,
    "lengthCm": 30,
    "baitUsed": "Minhoca",
    "weatherCondition": "Chuvoso",
    "catchId": 2
  }' | jq
```
Esperado: `409 Conflict` - "This Catch already has a detail registered."

## 🔍 Teste com Swagger UI

Acesse: **http://localhost:5000/swagger**

- Explore todos os endpoints interativamente
- Teste diretamente pela UI
- Veja exemplos de request/response

## 📊 Verificar Banco em pgAdmin

1. Acesse: **http://localhost:8080**
2. Login: `admin@admin.com` / `admin123`
3. Conecte ao servidor PostgreSQL (password: `postgres123`)
4. Veja tabelas criadas:
   - `__EFMigrationsHistory`
   - `Species`
   - `Catches`
   - `CatchDetails`

## ✅ Checklist de Validação

- [ ] GET `/api/species` retorna lista vazia ou preenchida
- [ ] POST `/api/species` cria nova espécie com ID gerado
- [ ] GET `/api/species/{id}` retorna espécie com catches inclusos
- [ ] POST `/api/catches` cria captura com CatchDate automático (UTC)
- [ ] GET `/api/catches/{id}` retorna captura com Species e CatchDetail
- [ ] POST `/api/catchdetails` valida Catch existente
- [ ] POST `/api/catchdetails` previne duplicatas (409 Conflict)
- [ ] GET `/api/catchdetails/catch/{catchId}` retorna detalhe ou 404
- [ ] DELETE `/api/species/{id}` falha com catches relacionadas (400)
- [ ] DELETE `/api/catches/{id}` deleta cascade (remove CatchDetail)
- [ ] PUT endpoints atualizam corretamente
- [ ] CORS permite requisições cross-origin
- [ ] Swagger UI funciona em `/swagger`
- [ ] PostgreSQL contém dados após operações

## 🐛 Troubleshooting

**Erro: "Cannot connect to database"**
- Verificar se Docker está rodando: `docker ps`
- Verificar containers: `docker compose ps`

**Erro: "Migrations not applied"**
```bash
cd FishPokedex
dotnet ef database update
```

**Porta 5000 já em uso:**
```bash
# Mudar Program.cs ou usar:
dotnet run --urls "http://localhost:5001"
```

**Limpar tudo e começar do zero:**
```bash
# Parar containers
docker compose down -v

# Remover migrations geradas
rm -rf FishPokedex/Migrations

# Recrear
docker compose up -d
dotnet ef migrations add Initial
dotnet ef database update
```
