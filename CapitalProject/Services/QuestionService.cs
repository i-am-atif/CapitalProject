using CapitalProject.DTOs;
using CapitalProject.Models;
using Microsoft.Azure.Cosmos;

namespace CapitalProject.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseName = "MyQuestionDatabase";
        private readonly string _containerName = "Questions";

        public QuestionService(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateContainerIfNotExistsAsync().Wait();
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);
            Console.WriteLine("Database created: {0}", database);
        }

        private async Task CreateContainerIfNotExistsAsync()
        {
            var database = _cosmosClient.GetDatabase(_databaseName);
            var container = await database.CreateContainerIfNotExistsAsync(_containerName, "/id");
            Console.WriteLine("Container created: {0}", container);
        }

        public async Task<Question> CreateQuestionAsync(QuestionDto question)
        {
            var container = _cosmosClient.GetContainer(_databaseName, _containerName);
            var newQuestion = new Question
            {
                Id = Guid.NewGuid().ToString(),
                Text = question.Text,
                Type = question.Type,
                Options = question.Options?.Select(o => new Answer { Text = o }).ToList()
            };
            await container.CreateItemAsync(newQuestion);
            return newQuestion;
        }

        public async Task<Question> UpdateQuestionAsync(string id, QuestionDto question)
        {
            var container = _cosmosClient.GetContainer(_databaseName, _containerName);
            //var existingQuestion = await container.ReadItemAsync<Question>(id, new PartitionKey(id));
            var response = await container.ReadItemAsync<Question>(id, new PartitionKey(id));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var existingQuestion = response.Resource;
                // Use the retrieved question object
                existingQuestion.Text = question.Text;
                existingQuestion.Type = question.Type;
                existingQuestion.Options = question.Options?.Select(o => new Answer { Text = o }).ToList();
                await container.ReplaceItemAsync(existingQuestion, existingQuestion.Id, new PartitionKey(existingQuestion.Id));
                return existingQuestion;
            }
            else
            {
                // Handle the read failure (e.g., log error, throw exception)
                return null;
            }
        }

        public async Task<List<Question>> GetQuestionsAsync()
        {
            var container = _cosmosClient.GetContainer(_databaseName, _containerName);
            var iterator = container.GetItemQueryIterator<Question>(new QueryDefinition("SELECT * FROM c"));
            var questions = new List<Question>();
            while (iterator.HasMoreResults)
            {
                foreach (var question in await iterator.ReadNextAsync())
                {
                    questions.Add(question);
                }
            }
            return questions;
        }

        public async Task<Question> GetQuestionByIdAsync(string id)
        {
            var container = _cosmosClient.GetContainer(_databaseName, _containerName);
            try
            {
                return await container.ReadItemAsync<Question>(id, new PartitionKey(id));
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ArgumentException("Question not found", nameof(id));
            }
        }
    }
}