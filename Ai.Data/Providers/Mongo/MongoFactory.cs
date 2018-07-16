
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using MongoDB.Driver.Core.Bindings;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq.Expressions;
using Ai.Domain;
using Ai.Core;

namespace Ai.Data.Providers.Mongo
{

    public class MongoFactory
    {
        public static MongoClient _mongoClientAi;
        public static MongoClient _mongoClientChat;
        public MongoFactory(AppSettings appSettings)
        {
            if (_mongoClientAi == null)
                _mongoClientAi = new MongoClient(appSettings.MongoDB.ConnAi);
            if (_mongoClientChat == null)
                _mongoClientChat = new MongoClient(appSettings.MongoDB.ConnChat);
        }

        public async Task<List<BsonDocument>> excuteMongoSelect(string query, FindOptions<BsonDocument, BsonDocument> options,
           MongoClient mongoClient, string databaseName, string collectionName
           )
        {
            try
            {
                var db = mongoClient.GetDatabase(databaseName);
                var collection = db.GetCollection<BsonDocument>(collectionName);

                var rs = await collection.FindAsync(query, options);
                return rs.ToList();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<List<BsonDocument>> excuteMongoFullTextSearch(string query, int next,
          MongoClient mongoClient, string databaseName, string collectionName
          )
        {
            try
            {
                var db = mongoClient.GetDatabase(databaseName);
                var collection = db.GetCollection<BsonDocument>(collectionName);

                var rs = collection.Find(query).Project("{ score: { $meta: \"textScore\" }}")
                                .Sort("{ score: { $meta: \"textScore\" } }").Limit(50).Skip(next).ToList(); ;
                return rs.ToList();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<DeleteResult> excuteMongoDelete<T>(string query,
           MongoClient mongoClient, string databaseName, string collectionName
           )
        {
            try
            {
                var list = new List<T>();
                var db = mongoClient.GetDatabase(databaseName);
                var collection = db.GetCollection<T>(collectionName);

                var rs = await collection.DeleteManyAsync(query);
                return rs;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<ReplaceOneResult> excuteMongoUpdate(FilterDefinition<BsonDocument> filters, BsonDocument documents, UpdateOptions option,
            MongoClient mongoClient, string databaseName, string collectionName
           )
        {
            try
            {
                var db = mongoClient.GetDatabase(databaseName);
                var collection = db.GetCollection<BsonDocument>(collectionName);

                try
                {
                    return await collection.ReplaceOneAsync(filters, documents, option);
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<bool> excuteMongoInsert(BsonDocument documents,
            MongoClient mongoClient, string databaseName, string collectionName
           )
        {
            try
            {
                var db = mongoClient.GetDatabase(databaseName);
                var collection = db.GetCollection<BsonDocument>(collectionName);

                var rs = collection.InsertOneAsync(documents);
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public async Task<UpdateResult> excuteMongoUpdateColumns(FilterDefinition<BsonDocument> filters, UpdateDefinition<BsonDocument> documents,
            UpdateOptions option,
          MongoClient mongoClient, string databaseName, string collectionName
         )
        {
            try
            {
                var db = mongoClient.GetDatabase(databaseName);
                var collection = db.GetCollection<BsonDocument>(collectionName);

                try
                {
                    return await collection.UpdateOneAsync(filters, documents, option);
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

    }
}