using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBMapReduce.Model
{
    public class Order
    {
        public ObjectId Id { get; set; }

        public int Number { get; set; }

        public int CustomerId { get; set; }

        public double Total { get; set; }
    }
}
