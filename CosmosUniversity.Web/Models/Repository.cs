using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace CosmosUniversity.Web.Models
{
    public static class Repository<T> where T : class
    {
        private static readonly string _endPoint = ConfigurationManager.AppSettings["CosmosDBEndPoint"];
        private static readonly string _authKey = ConfigurationManager.AppSettings["CosmosDBAuthKey"];
        private static readonly string _dbName = "cosmosuniversity";
        private static readonly string _collectionName = "student";
        private static ConnectionPolicy _connectionPolicy = new ConnectionPolicy { EnableEndpointDiscovery = false };
        private static DocumentClient client = new DocumentClient(new Uri(_endPoint), _authKey, _connectionPolicy);

        public static async Task<IEnumerable<T>> GetStudentsAsync(Expression<Func<T, bool>> where)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_dbName, _collectionName);
            FeedOptions feedOptions = new FeedOptions { MaxItemCount = -1 };

            IDocumentQuery<T> students;
            
            if (where == null)
            {
                students = client.CreateDocumentQuery<T>(collectionUri, feedOptions)
                                               .AsDocumentQuery();
            }
            else
            {
                students = client.CreateDocumentQuery<T>(collectionUri, feedOptions)
                                               .Where(where)
                                               .AsDocumentQuery();
            }
                                    
            List<T> listOfStudents = new List<T>();
            while (students.HasMoreResults)
            {
                listOfStudents.AddRange(await students.ExecuteNextAsync<T>());
            }

            return listOfStudents;
        }

        public static async Task<T> GetStudentAsync(string id, int partitionKey)
        {
            if (string.IsNullOrEmpty(id))
                throw new ApplicationException("No student id specified");

            Uri documentUri = UriFactory.CreateDocumentUri(_dbName, _collectionName, id);
            try
            {
                RequestOptions requestOptions = new RequestOptions {
                    PartitionKey = new PartitionKey(partitionKey)
                };
                Document student = await client.ReadDocumentAsync(documentUri, requestOptions);
                return (T)(dynamic)student;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                throw;
            }
        }

        public static async Task<Document> CreateStudentAsync(T student)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_dbName, _collectionName);
            return await client.CreateDocumentAsync(collectionUri, student);
        }

        public static async Task<Document> ReplaceStudentAsync(T student, string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ApplicationException("No student id specified");

            Uri documentUri = UriFactory.CreateDocumentUri(_dbName, _collectionName, id);
            return await client.ReplaceDocumentAsync(documentUri, student);
        }

        public static async Task<Document> DeleteStudentAsync(string id, int partitionKey)
        {
            if (string.IsNullOrEmpty(id))
                throw new ApplicationException("No student id specified");

            RequestOptions requestOptions = new RequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            };
            Uri documentUri = UriFactory.CreateDocumentUri(_dbName, _collectionName, id);
            return await client.DeleteDocumentAsync(documentUri, requestOptions);
        }
    }
}