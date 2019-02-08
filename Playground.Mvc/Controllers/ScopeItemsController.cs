using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using System.Transactions;
using Newtonsoft.Json;
using Playground.Mvc.Models;

namespace Playground.Mvc.Controllers
{
    public class ScopeItemsController : ApiController
    {
        protected IsolationLevel DefaultIsolationLevel = IsolationLevel.Snapshot;

        // GET api/<controller>
        public IEnumerable<ItemProxy> Get()
        {
            List<ItemProxy> items;

            using (var db = new PlaygroundEntities())
            {
                items = db.Items.OrderBy(x => x.Date).AsEnumerable().Select(x => new ItemProxy(x)).ToList();
            }

            return items;
        }

        // GET api/<controller>/5
        public ItemProxy Get(int id)
        {
            ItemProxy proxy;

            using (var db = new PlaygroundEntities())
            {
                Item item = db.Items.FirstOrDefault(x => x.Id == id);

                if (item == null)
                {
                    return new ItemProxy();
                }

                proxy = new ItemProxy(item);
            }

            return proxy;
        }

        // POST api/<controller>
        public ItemProxy Post([FromBody]ItemProxy value)
        {
            if (value.Guid == null)
            {
                value.Guid = Guid.NewGuid().ToString();
            }

            value.Date = DateTime.Now;

            using (var db = new PlaygroundEntities())
            {
                using (TransactionScope scope = this.CreateTransactionScope())
                {
                    value.Insert(db);
                    scope.Complete();
                }
            }

            return value;
        }

        // PUT api/<controller>
        public ItemProxy Put([FromBody]ItemProxy value)
        {
            using (var db = new PlaygroundEntities())
            {
                using (TransactionScope scope = this.CreateTransactionScope())
                {
                    value.Update(db);
                    scope.Complete();
                }
            }

            return value;
        }

        // DELETE api/<controller>/5
        public bool Delete(int id)
        {
            using (var db = new PlaygroundEntities())
            {
                using (TransactionScope scope = this.CreateTransactionScope())
                {
                    Item item = db.Items.FirstOrDefault(x => x.Id == id);

                    if (item == null)
                    {
                        return false;
                    }

                    ItemProxy proxy = new ItemProxy(item);
                    proxy.Delete(db);
                    scope.Complete();
                }
            }

            return true;
        }

        protected TransactionScope CreateTransactionScope()
        {
            TransactionOptions options = new TransactionOptions() { IsolationLevel = this.DefaultIsolationLevel };
            TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options);

            return scope;
        }
    }
}