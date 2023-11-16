using ChildImmunizationCare_Parent.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChildImmunizationCare_Child;
using ChildImmunizationCare_Child.Models;

namespace ChildImmunizationCare
{
    public class Parent
    {
        private readonly IMongoCollection<ParentDetails> _mongoParents;

        public Parent()
        {
            var dbClient = new MongoClient("mongodb+srv://dhivakar:dhivakar@cluster0.rmwyf47.mongodb.net/cluster0");
            IMongoDatabase db = dbClient.GetDatabase("ChildImmunizationCare");
            _mongoParents = db.GetCollection<ParentDetails>("Parent");
        }


        [FunctionName("InsertParent")]
        public async Task<IActionResult> InsertParent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Parents")]
            ParentRequest parentRequest,
            HttpRequest req,
            ILogger log)
        {          

            var parentToInsert = new ParentDetails
            {
                Name = parentRequest.Name,
                Email = parentRequest.Email,
                Address = parentRequest.Address,
                Phone = parentRequest.Phone,
            };

            await _mongoParents.InsertOneAsync(parentToInsert);

            return new OkResult();
        }

        [FunctionName("UpdateParent")]
        public async Task<IActionResult> UpdateParent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Parents/{parentId}")]
            ParentRequest parentRequest,
            HttpRequest req,
            string parentId,
            ILogger log)
        {

            var fields = new Dictionary<string, Object>();

            if (parentRequest.Phone is not null)
                fields.Add(nameof(ParentDetails.Phone), parentRequest.Phone);

            if (parentRequest.Name is not null)
                fields.Add(nameof(ParentDetails.Name), parentRequest.Name);

            if (parentRequest.Address is not null)
                fields.Add(nameof(ParentDetails.Address), parentRequest.Address);

            if (parentRequest.Email is not null)
                fields.Add(nameof(ParentDetails.Email), parentRequest.Email);

            var filter = Builders<ParentDetails>.Filter.Eq(e => e.Id, parentId);
            var updates = fields.Select(f => Builders<ParentDetails>.Update.Set(f.Key, f.Value));
            var update = Builders<ParentDetails>.Update.Combine(updates);
            var result = await _mongoParents.UpdateOneAsync(filter, update);

            return new OkObjectResult(result);
        }

        [FunctionName("GetAllParents")]
        public async Task<IActionResult> GetAllParents(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Parents")]
            HttpRequest req,
           ILogger log)
        {

            var cursor = await _mongoParents.FindAsync(Builders<ParentDetails>.Filter.Empty);
            var list = await cursor.ToListAsync();

            return new OkObjectResult(list);
        }

        [FunctionName("DeleteParent")]
        public async Task<IActionResult> DeleteParent(
         [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "Parents/{parentId}")]
         HttpRequest req,
         string parentId,
         ILogger log)
        {
            var filter = Builders<ParentDetails>.Filter
                        .Eq(r => r.Id, parentId);

             await _mongoParents.DeleteOneAsync(filter);

            return new OkResult();
        }

        [FunctionName("CreateChild")]
        public async Task<IActionResult> CreateChild(
       [HttpTrigger(AuthorizationLevel.Anonymous, "Post", Route = "Parents/CreateChild")]
       ChildRequest childRequest,
         HttpRequest req,
       ILogger log)
        {

            var childObj = new Child();
            await childObj.InsertChild(childRequest);

            return new OkResult();
        }


    }
}
