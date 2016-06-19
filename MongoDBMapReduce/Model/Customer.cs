using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBMapReduce.Model
{
    public class Customer
    {
        public ObjectId Id { get; set; }

        public int CustomerId { get; set; }

        public string Name { get; set; }
    }
}
