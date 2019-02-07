using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Playground.Mvc.Models;
using System.Net.Http;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Playground.Mvc.Controllers
{
    public class BulkController : ApiController
    {
        // GET api/<controller>/Empty
        [Route("api/bulk/empty")]
        [HttpGet]
        public HttpResponseMessage Empty()
        {
            return PerformAction(() =>
            {
                using (var db = new PlaygroundEntities())
                {
                    try
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.Database.ExecuteSqlCommand("DELETE FROM Items");
                    }
                    finally
                    {
                        db.Configuration.AutoDetectChangesEnabled = true;
                    }
                    
                }
            });
        }

        // GET api/<controller>/Generate
        [Route("api/bulk/generate/{count}")]
        [HttpGet]
        public HttpResponseMessage Generate(int count)
        {
            return PerformAction(() =>
            {
                using (var db = new PlaygroundEntities())
                {
                    try
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        for (int i = 0; i < count; i++)
                        {
                            db.Items.Add(new Item
                            {
                                Guid = Guid.NewGuid().ToString(),
                                Date = new DateTime(),
                                Index = i,
                                Thread = i
                            });
                        }
                        db.SaveChanges();
                    }
                    finally
                    {
                        db.Configuration.AutoDetectChangesEnabled = true;
                        db.Configuration.ValidateOnSaveEnabled = true;
                    }
                }
            });
        }

        internal class TimeDuration
        {
            public string ElapsedTime;
        }

        static HttpResponseMessage PerformAction(Action operation)
        {
            var watch = Stopwatch.StartNew();
            operation();
            watch.Stop();

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(
                new TimeDuration
                {
                    ElapsedTime = $"{watch.ElapsedMilliseconds} miliseconds"
                }))
            };
        }
    }
}