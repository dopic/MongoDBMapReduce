using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBMapReduce.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBMapReduce
{
    class Program
    {
        private const string CUSTOMER_COLLECTION = "Customers";
        private const string ORDER_COLLECTION = "Orders";

        static void Main(string[] args)
        {
            var client = new MongoDB.Driver.MongoClient("mongodb://localhost");
            var database = client.GetDatabase("MapReduce");

            if (!database.ListCollections().Any())
            {
                SeedOrders(database);
                SeedCustomers(database);
            }

            var map = GetFunction("CustomerMapFunction");
            var reduce = GetFunction("CustomerReduceFunction");
            var options = new MapReduceOptions<Order, BsonDocument>();
            options.Filter = Builders<Order>.Filter.Eq(o => o.CustomerId, 5000);
            options.Verbose = false;
            options.OutputOptions = MapReduceOutputOptions.Replace("Out");
            database.GetCollection<Order>(CUSTOMER_COLLECTION).MapReduce<BsonDocument>(new BsonJavaScript(map), new BsonJavaScript(reduce), options);


            var map2 = GetFunction("OrderMapFunction");
            var reduce2 = GetFunction("OrderReduceFunction");
            var options2 = new MapReduceOptions<Customer, BsonDocument>();
            options2.Filter = Builders<Customer>.Filter.Eq(o => o.CustomerId, 5000);
            options2.Verbose = false;
            options2.OutputOptions = MapReduceOutputOptions.Reduce("Out");
            var res = database.GetCollection<Customer>(ORDER_COLLECTION).MapReduce<BsonDocument>(new BsonJavaScript(map2), new BsonJavaScript(reduce2), options2);

            var output = database.GetCollection<BsonDocument>("Out");

            foreach (var doc in output.Find(b=> true).ToList())
            {
                Console.WriteLine();
                Console.WriteLine(doc.ToJson());
            }

            Console.Read();
        }

        private static void SeedCustomers(IMongoDatabase database)
        {
            database.DropCollection(CUSTOMER_COLLECTION);

            var collection = database.GetCollection<Customer>(CUSTOMER_COLLECTION);


            var cindexes = Builders<Customer>.IndexKeys.Ascending((o) => o.CustomerId);
            collection.Indexes.CreateOne(cindexes);

            var documents = new List<Customer>();
            for (var x = 1; x < 500000; x++)
            {
                documents.Add(new Customer
                {
                    Name = $"Douglas {x}",
                    CustomerId = x
                });
            }

            collection.InsertMany(documents);
        }

        private static void SeedOrders(IMongoDatabase database)
        {
            database.DropCollection(ORDER_COLLECTION);

            var collection = database.GetCollection<Order>(ORDER_COLLECTION);

            var indexes = Builders<Order>.IndexKeys.Ascending((o) => o.CustomerId);
            collection.Indexes.CreateOne(indexes);                    

            var documents = new List<Order>();

            for (var x = 1; x < 5000000; x++)
            {
                var customerId = x % 500000;

                documents.Add(new Order
                {
                    Number = x,
                    CustomerId = customerId,
                    Total = customerId
                });
            }

            collection.InsertMany(documents);
        }

        private static string GetFunction(string fileName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"MongoDBMapReduce.{fileName}.js"))
            {
                var streamReader = new StreamReader(stream);
                return streamReader.ReadToEnd();
            }
        }
    }
}
