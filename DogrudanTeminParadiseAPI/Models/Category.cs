﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid CreatedByAdminId { get; set; }
    }
}
