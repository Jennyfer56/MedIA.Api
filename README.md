
# MedIA.Api – Global Solution 2025 (2TDS)

API REST em .NET 8 que implementa a parte de **pré-triagem digital** do sistema MedIA.  
O objetivo é permitir que o paciente faça uma triagem inicial, calculando a urgência e
direcionando para a unidade de saúde mais adequada **por localização**, gerando um hash
para QR Code que será usado na recepção do hospital.

---

## Arquitetura em camadas

Projeto organizado em **4 camadas**:

- **Domain**
  - Entidades de negócio:
    - `Paciente`
    - `UnidadeSaude`
    - `Triagem`
    - `Atendimento`
  - Enums:
    - `NivelUrgencia` (Baixa, Média, Alta, Crítica)
    - `StatusTriagem` (Aberta, EmAtendimento, Concluida, Cancelada)

- **Application**
  - DTOs:
    - `CriarTriagemRequest`
    - `TriagemResponse`
  - Serviços:
    - `TriagemService`
      - Calcula nível de urgência a partir da descrição dos sintomas.
      - Escolhe a **unidade de saúde mais próxima** do paciente (latitude/longitude).
      - Gera hash (`QrCodeHash`) para ser usado na geração do QR Code.
      - Implementa `/search` com **paginação, filtros e ordenação**.

- **Infrastructure**
  - `MedIaDbContext` (Entity Framework Core)
  - Mapeamento das entidades e índices (unique em Documento do Paciente)

- **Web (Presentation)**
  - Controllers:
    - `PacientesController`
    - `UnidadesSaudeController`
    - `TriagensController`

---

## Tecnologias utilizadas

- .NET 8  
- ASP.NET Core Web API  
- Entity Framework Core 8 (LocalDB)  
- Swagger / OpenAPI  

---

# Como executar o projeto

## Pré-requisitos
- .NET SDK 8 instalado  
- SQL Server LocalDB  

---

## Connection string

Arquivo `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MedIaDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
````

---

# Passo a passo

##  Aplicar as migrations

```bash
dotnet ef database update
```

##  Rodar a API

```bash
dotnet run
```

##  Acessar o Swagger

Abra no navegador:

```
https://localhost:{porta}/swagger
```

(Use a porta exibida no terminal ao rodar a API.)

---

# Endpoints principais

---

##  Pacientes

* `GET /api/Pacientes`
* `GET /api/Pacientes/{id}`
* `POST /api/Pacientes`
* `PUT /api/Pacientes/{id}`
* `DELETE /api/Pacientes/{id}`

### Exemplo de criação

```json
{
  "nomeCompleto": "Jennyfer Lee",
  "documento": "12345678900",
  "dataNascimento": "2003-05-04",
  "telefone": "11999998888",
  "endereco": "Rua Exemplo, 123",
  "latitude": -23.5895,
  "longitude": -46.6310
}
```

---

##  Unidades de Saúde

* `GET /api/UnidadesSaude`
* `GET /api/UnidadesSaude/{id}`
* `POST /api/UnidadesSaude`
* `PUT /api/UnidadesSaude/{id}`
* `PATCH /api/UnidadesSaude/{id}/ocupacao` (IoT/Node-RED)
* `DELETE /api/UnidadesSaude/{id}`

### Exemplo de criação

```json
{
  "nome": "UBS Vila Mariana",
  "endereco": "Rua das Flores, 123",
  "cidade": "São Paulo",
  "ocupacao": "baixa",
  "latitude": -23.5890,
  "longitude": -46.6320
}
```

---

##  Triagens

### Criar triagem

`POST /api/Triagens/analisar`

```json
{
  "pacienteId": 1,
  "sintomasDescricao": "Dor no peito e falta de ar"
}
```

### Resposta resumida

```json
{
  "id": 1,
  "pacienteId": 1,
  "unidadeSaudeId": 1,
  "sintomasDescricao": "Dor no peito e falta de ar",
  "nivelUrgencia": 3,
  "status": 1,
  "dataCriacao": "2025-11-17T23:03:25.4337948Z",
  "qrCodeBase64": "HASH_AQUI",
  "links": {
    "self": "/api/triagens/1",
    "cancelar": "/api/triagens/1/cancelar"
  }
}
```

---

##  Search – paginação + filtros + ordenação

`GET /api/Triagens/search`

Parâmetros:

* `page`
* `size`
* `pacienteId`
* `status`
* `sort` = `dataAsc` | `dataDesc`

### Exemplo

```
/api/Triagens/search?page=1&size=10&status=Aberta&sort=dataDesc
```

---

# Tratamento de erros

* Falhas de validação → **400 ValidationProblemDetails**
* Erros de negócio (ex.: paciente não existe) → **400 ProblemDetails**
* Triagem concluída e tentativa de cancelar → **400 ProblemDetails**
* Sucesso na criação → **201 Created** com header `Location`

---

# Integração com outras matérias

##  Mobile (React Native)

O app consome:

* Cadastro de pacientes
* Envio de sintomas
* Consulta de triagens
* QR Code gerado via hash da API

##  IoT (Node-RED)

O Node-RED atualiza:

* Ocupação das unidades em tempo real (`PATCH /ocupacao`)

Isso permite dashboards ao vivo durante a apresentação da GS.

---

#  Conclusão

A API .NET é o núcleo responsável por:

* processar triagens
* calcular urgência
* direcionar para unidade mais próxima
* gerar hash para QR Code
* integrar IoT + Mobile

* Integrantes do Grupo
Ivanildo Alfredo da Silva Filho - RM560049
Jennyfer Lee - RM561020
Letícia Sousa Prado Silva - RM559258


