# 🤖 MLQuestionSimilarity

`MLQuestionSimilarity` é um serviço baseado em **Machine Learning** que calcula a similaridade entre uma questão alvo e uma lista de outras questões. Ele utiliza o algoritmo de **Similaridade de Cosseno** para comparar descrições de texto e determinar o quão semelhantes elas são. O projeto é desenvolvido usando **ML.NET** e integra-se com **RabbitMQ** para mensageria, permitindo um processamento eficiente em segundo plano.

## Funcionalidades ✨

- **Cálculo de Similaridade de Cosseno**: Utiliza ML.NET para calcular a pontuação de similaridade entre uma questão alvo e outras questões.
- **Integração com RabbitMQ**: Usa RabbitMQ para consumir mensagens e processá-las de forma assíncrona.
- **Processamento Assíncrono**: Utiliza um `Worker` em segundo plano para escutar a fila e processar as mensagens automaticamente.

## Estrutura do Projeto 🗂️

- **`Models`**: Contém os modelos de dados utilizados na aplicação.
- **`Services`**: Contém a lógica de negócios para o consumo de mensagens e o cálculo de similaridade.
- **`Worker`**: Executa o serviço em segundo plano que escuta a fila do RabbitMQ e processa as mensagens recebidas.

## Pré-Requisitos ⚙️

Antes de rodar o projeto, garanta que os seguintes componentes estejam instalados:

- [.NET 6.0 ou superior](https://dotnet.microsoft.com/download/dotnet) ⚡
- [RabbitMQ](https://www.rabbitmq.com/download.html) 🐰
- [ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet) 🧠

## Como Começar 🚀

1. Clone o repositório:

   ```bash
   git clone https://github.com/pedrinhoas7/MLQuestionSimilarity.git
   cd MLQuestionSimilarity
   ```

2. Instale as dependências:

   ```bash
   dotnet restore
   ```

3. Configure as definições do RabbitMQ:
   - Certifique-se de que o RabbitMQ está rodando localmente ou em um servidor remoto.
   - Se necessário, atualize a string de conexão do RabbitMQ na classe `MessagingService`.

4. Execute o projeto:

   ```bash
   dotnet run
   ```

## Como Funciona 🔍

1. O `Worker` escuta uma fila do RabbitMQ em busca de mensagens.
2. O `MessagingService` gerencia a conexão com o RabbitMQ e o consumo de mensagens.
3. Ao receber uma mensagem, o `SimilarityService` calcula a similaridade de cosseno entre a questão alvo e a lista de outras questões.
4. Os resultados são processados e podem ser registrados ou encaminhados para uso posterior (atualmente são mostrados somente no console).

## 🧠 Como a Similaridade é Calculada

A função principal responsável por calcular a similaridade entre duas questões é:

```csharp
public async Task<SimilarityResult> CalculateCosineSimilarityAsync(Question target, Question compare)
```

### Etapas do Processo

1. **Preparação dos Dados**
   ```csharp
   var questions = new List<Question> { target, compare };
   var data = _mlContext.Data.LoadFromEnumerable(questions);
   ```

2. **Extração de Características Textuais**
   ```csharp
   var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(Question.Description));
   ```

3. **Treinamento e Transformação**
   ```csharp
   var model = pipeline.Fit(data);
   var transformedData = model.Transform(data);
   ```

4. **Conversão para Vetores Numéricos**
   ```csharp
   var vectors = _mlContext.Data.CreateEnumerable<QuestionVector>(transformedData, reuseRowObject: false).ToList();
   ```

5. **Cálculo da Similaridade de Cosseno**
   ```csharp
   var score = CosineSimilarity(targetVector, compareVector);
   ```

---

### 🧮 Sobre o Algoritmo de Similaridade de Cosseno

```csharp
private double CosineSimilarity(float[] vectorA, float[] vectorB)
```

A similaridade de cosseno é calculada assim:
- **Produto escalar** entre os vetores.
- Dividido pelas **magnitudes** (normas) de cada vetor.

Se algum vetor tiver magnitude zero, a similaridade é considerada zero para evitar erros de divisão:

```csharp
var dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
var magnitudeA = Math.Sqrt(vectorA.Sum(x => x * x));
var magnitudeB = Math.Sqrt(vectorB.Sum(x => x * x));

if (magnitudeA == 0 || magnitudeB == 0)
    return 0;

return dotProduct / (magnitudeA * magnitudeB);
```

---

### ✅ Resultado Final

A função retorna um objeto `SimilarityResult` contendo a questão comparada e a pontuação da similaridade:

```csharp
return await Task.FromResult(new SimilarityResult
{
    Question = compare,
    Score = score
});
```

## Exemplo de Payload 📥

Exemplo de uma mensagem enviada para a fila para processamento:

```json
{
  "TargetQuestions": {
    "Id": "Q0",
    "Description": "Qual é a capital do Brasil?"
  },
  "AllQuestions": [
    {
      "Id": "Q1",
      "Description": "Qual cidade é a capital brasileira?"
    },
    {
      "Id": "Q2",
      "Description": "Quem descobriu o Brasil?"
    },
    {
      "Id": "Q3",
      "Description": "Onde fica Brasília?"
    },
    {
      "Id": "Q4",
      "Description": "Qual é a fórmula da água?"
    },
    {
      "Id": "Q5",
      "Description": "Qual é a capital do Brasil?."
    },
    {
      "Id": "Q6",
      "Description": "Qual é a capital do Brasil?"
    }
  ]
}

```

## Exemplo de Resposta 📥

Exemplo de como os dados são visualizados atualmente, futuramente eles poderão ser registrados ou encaminhados para uso:

![image](https://github.com/user-attachments/assets/d765547d-e4d3-43eb-9a91-69cf793410fb)


---

## 👨‍💻 Autor

Desenvolvido com 💙 por **Pedro Henrique**
