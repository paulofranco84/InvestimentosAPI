# Investimentos API

API desenvolvida para fins de produção temática do PSI 14931 - ASSISTENTE SENIOR (2058), com foco em backend C#.

---

## Tecnologias
- **ASP.NET Core 8** 
- **SQLite + EF Core**
- **OpenTelemetry**
- **JWT Auth com Identity**
- **xUnit + Moq**
- **Docker**

---

## Introdução
Esta API foi desenvolvida em **.NET 8** seguindo o padrão **DDD**. Na primeira execução, é realizado o *seed* de produtos, clientes e investimentos para testes iniciais.

Foi disponibilizado um CRUD básico utilizando repositório genérico para inclusão de novos clientes e produtos, além da possibilidade de realizar investimentos.

Para acesso ao banco e persistência dos dados, é utilizado o padrão **Unit of Work**, que minimiza problemas de concorrência e melhora a performance da aplicação, reduzindo o custo de I/O. Esse padrão também facilita a criação de testes unitários, permitindo mockar a camada de persistência sem depender do banco real.

---

## Autenticação
A API utiliza autenticação **JWT** implementada com **Microsoft ASP.NET Core Identity**.  
Na primeira execução, é criado um usuário de testes com:
- **Login:** `admin`
- **Senha:** `admin`

Também é possível cadastrar novos usuários.

---

## Banco de Dados
Foi escolhido o banco **SQLite**, utilizando o padrão **Code First**, onde o banco é criado pelo EF Core com os comandos:

```bash
dotnet ef migrations add CriacaoDoBanco
dotnet ef database update
```

---

## Middlewares
A API conta com:
- **Middleware de exceção global** para tratamento de erros, eliminando a necessidade de blocos `try/catch` nos métodos.
- **Middleware para telemetria** utilizando **OpenTelemetry**.

---

## Testes
Os testes foram desenvolvidos com **xUnit** e **Moq**.  
Devido ao tempo limitado, foram priorizados testes para controladores e serviços essenciais, deixando de fora controladores auxiliares (inserção de clientes/produtos), autenticação e telemetria, que não sofrem alterações frequentes.

---

## Principais Endpoints

### **Investimentos**
- **GET** `/api/Investimentos/ObterPorCliente/{clienteId}`
  - **Descrição:** Retorna os investimentos do cliente.
  - **Parâmetro:** Id do cliente.
  - **Modelo de resposta:**
    ```json
    {
      "id": 1,
      "clienteId": 123,
      "tipo": "CDB",
      "valor": 5000,
      "prazoMeses": 0,
      "rentabilidade": 0.12,
      "data": "2025-01-15T00:00:00"
    }
    ```

- **POST** `/api/Investimentos/Investir`
  - **Descrição:** Realiza um novo investimento para o cliente.
  - **Corpo da requisição:**
    ```json
    {
      "clienteId": 0,
      "valor": 0,
      "prazoMeses": 0,
      "produtoId": 0
    }
    ```
  - **Modelo de resposta:**
    ```json
    {
      "id": 16,
      "clienteId": 125,
      "tipo": "Tesouro",
      "valor": 10000,
      "prazoMeses": 12,
      "rentabilidade": 0.1,
      "data": "2025-11-19T18:16:00.6451462Z"
    }
    ```

- **GET** `/api/Investimentos/{investimentoId}`
  - **Descrição:** Retorna os dados de um investimento pelo seu Id.
  - **Parâmetro:** Id do investimento.
  - **Modelo de resposta:**
    ```json
    {
      "id": 10,
      "clienteId": 123,
      "tipo": "Fundo",
      "valor": 50000,
      "prazoMeses": 12,
      "rentabilidade": 0.2,
      "data": "2025-11-18T17:36:55.3617764"
    }
    ```

---

### **Perfil de Risco**
- **GET** `/api/PerfilRisco/{clienteId}`
  - **Descrição:** Retorna o perfil de risco do cliente.
  - **Parâmetro:** Id do cliente.
  - **Modelo de resposta:**
    ```json
    {
      "clienteId": 123,
      "perfil": "Agressivo",
      "pontuacao": 89,
      "descricao": "Perfil voltado para alta rentabilidade e maior risco."
    }
    ```

---

### **Simulações**
- **GET** `/api/Simulacoes/simular-investimento`
  - **Descrição:** Faz uma nova simulação de investimento.
  - **Corpo da requisição:**
    ```json
    {
      "clienteId": 0,
      "valor": 0,
      "prazoMeses": 0,
      "tipoProduto": "string"
    }
    ```
  - **Modelo de resposta:**
    ```json
    {
      "produtoValidado": {
        "id": 104,
        "nome": "LCA Agro Caixa 2026",
        "tipo": "LCA",
        "rentabilidade": 0.14,
        "risco": "Baixo"
      },
      "resultadoSimulacao": {
        "valorFinal": "67579.20",
        "rentabilidadeEfetiva": 0.14,
        "prazoMeses": 24
      },
      "dataSimulacao": "2025-11-19T18:23:53.2447401Z"
    }
    ```

---

## Docker
A API está habilitada para execução via **Docker**.

### Executando com Docker
```bash
docker-compose up --build
```

---
