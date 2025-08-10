# Integração API Rick And Morty

Este projeto implementa uma API em .NET 9 (Minimal APIs) com SQLite, seguindo princípios de **Clean Architecture**.  
Inclui funcionalidades como upload de arquivo CSV, validação, processamento assíncrono, persistência em SQLite e integração com a API pública do Rick and Morty.

---

## Índice

- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Como executar (Docker)](#como-executar-docker)
- [Formato do arquivo CSV](#formato-do-arquivo-csv)
- [Endpoints / Rotas](#endpoints--rotas)
  - [POST /upload](#post-upload)
  - [GET /status/{processId}](#get-statusprocessid)
  - [GET /upload/{processId}](#get-uploadprocessid)
- [Regras de paginação](#regras-de-paginação)
- [Códigos de status do processamento](#códigos-de-status-do-processamento)
- [Estrutura de pastas importante](#estrutura-de-pastas-importante)
---

## Funcionalidades

- Upload de arquivos CSV contendo referências a episódios, personagens e localizações.
- Validação do arquivo e das linhas (formato e ordem dos campos).
- Processamento **assíncrono** (fila/worker) — a API retorna imediatamente um `processId` para consulta de status.
- Armazenamento dos dados processados em **SQLite** (pasta `/data` no container).
- Endpoints para consultar status e recuperar os dados processados (com paginação).

---

## Tecnologias

- .NET 9 (Minimal APIs)
- ASP.NET Core
- Entity Framework Core (SQLite)
- Serilog (logging)
- Docker

---

## Como executar (Docker)

### Build da imagem
```bash
docker build -t rickandmorty-app .
```

### Executar a imagem (mapear porta e montar volume para persistência do SQLite)
> Observação: a aplicação por padrão no `Program.cs` do projeto usa a porta **5164** (ex.: `options.ListenAnyIP(5164)`).

Exemplo (mapeando porta 5164):
```bash
docker run -d --name rickandmorty \
  -p 5164:5164 \
  -v $(pwd)/data:/data \
  rickandmorty-app
```

## Formato do arquivo CSV

O arquivo **deve obrigatoriamente** seguir este formato (primeira linha é o cabeçalho):

```csv
episode_id,character_id,character_name,location_id
12,5,Jerry Smith,20
```

Regras:
- A primeira linha **obrigatoriamente** deve ser o cabeçalho com exatamente essas colunas e nessa ordem.
- Valores separados por **vírgula**.
- Se o arquivo **tiver alguma linha inválida**, a requisição **será ignorada** (conforme regra do projeto).
- Formatos inválidos, colunas faltando ou tipos errados invalidam o upload.

---

## Endpoints / Rotas

### POST /upload
Faz upload do arquivo CSV (multipart/form-data).

**Request (curl - exemplo Windows)**
```bash
curl --request POST \
  --url http://localhost:5164/upload/ \
  --header 'content-type: multipart/form-data' \
  --form 'file=@/caminho/para/arquivo.csv'
```

**Request (curl - exemplo Linux/macOS)**
```bash
curl -X POST "http://localhost:5164/upload/" -F "file=@/caminho/para/arquivo.csv"
```

**Resposta de sucesso (200 OK)**
```json
{
  "message": "Upload realizado com sucesso!",
  "processId": "0227fe39-e271-4c4a-b21f-7ddd52bc48ca",
  "statusUrl": "/status/0227fe39-e271-4c4a-b21f-7ddd52bc48ca"
}
```

> Importante: se o arquivo for inválido (linha inválida, cabeçalho errado, etc.), a requisição é ignorada conforme comportamento implementado.

---

### GET /status/{processId}
Consulta o status do processamento do arquivo.

**Request (curl)**
```bash
curl --request GET \
  --url http://localhost:5164/status/0227fe39-e271-4c4a-b21f-7ddd52bc48ca
```

**Response (200 OK) — Exemplo**
```json
{
  "id": "0227fe39-e271-4c4a-b21f-7ddd52bc48ca",
  "filePath": "Uploads/0227fe39-e271-4c4a-b21f-7ddd52bc48ca",
  "createdTimestamp": "2025-08-09T03:07:52.1225374",
  "startTime": "2025-08-09T03:07:52.1225374",
  "endTime": "2025-08-09T03:08:14.120834",
  "status": "CONCLUÍDO COM SUCESSO"
}
```

**Status possíveis:**
- `RECEBIDO`
- `EM PROCESSAMENTO`
- `CONCLUÍDO COM SUCESSO`

**Status de erro (encerramento com falha):**
- `ERRO - FALHA AO SALVAR DADOS`
- `ERRO - FALHA AO OBTER DADOS`
- `ERRO INTERNO DURANTE O PROCESSAMENTO`

---

### GET /upload/{processId}
Retorna os dados processados referentes ao `processId`. Suporta paginação via query string (`pageNumber` e `pageSize`).

**Request (curl)**
```bash
curl --request GET \
  --url 'http://localhost:5164/upload/0227fe39-e271-4c4a-b21f-7ddd52bc48ca?pageNumber=1&pageSize=1'
```

**Resposta (200 OK) — Exemplo**
```json
{
    "episodes": [
        {
            "id": "int",
            "name": "string",
            "air_date": "string",
            "episode": "string",
            "characters": [
                {
                    "id": "int",
                    "name": "string",
                    "status": "string",
                    "species": "string",
                    "type": "string",
                    "gender": "string",
                    "origin": {
                        "id": "int",
                        "name": "string",
                        "type": "string",
                        "dimension": "string"
                    },
                    "location": {
                        "id": "int",
                        "name": "string",
                        "type": "string",
                        "dimension": "string"
                    }
                }
            ]
        }
    ],
    "totalLocations": "int",
    "totalFemaleCharacters": "int",
    "totalMaleCharacters": "int",
    "totalGenderlessCharacters": "int",
    "totalGenderUnknownCharacters": "int",
    "uploadeFilePath": "string"
}
```

> Nota: `uploadeFilePath` segue o formato implementado — ajuste se preferir outro campo (`uploadedFilePath`, `uploadFilePath`, etc.).

---

## Regras de paginação

- Se **`pageNumber`** não vier, mas **`pageSize`** vier → assume `pageNumber = 1`.
- Se **`pageNumber`** vier, mas **`pageSize`** não vier → assume `pageSize = 10`.
- Ambos podem ser usados juntos: `?pageNumber=2&pageSize=20`.

---

## Códigos de status do processamento

- `RECEBIDO` — upload aceito e enfileirado.
- `EM PROCESSAMENTO` — worker consumindo o arquivo e populando dados.
- `CONCLUÍDO COM SUCESSO` — processamento finalizado sem erros.
- `ERRO - FALHA AO SALVAR DADOS` — falha ao persistir dados no DB.
- `ERRO - FALHA AO OBTER DADOS` — falha ao recuperar dados externos (API Rick and Morty).
- `ERRO INTERNO DURANTE O PROCESSAMENTO` — exceptions ou problemas não categorizados.

---

## Estrutura de pastas importante

- `./data` — pasta mapeada para o SQLite (host ↔ container).
- `./Uploads` — local (no container) onde os arquivos CSV são armazenados temporariamente.
- `./src` — código-fonte (UseCases, Ports, Adapters, Controllers/Endpoints).
- `Dockerfile` — imagem multi-stage (build + runtime).

---