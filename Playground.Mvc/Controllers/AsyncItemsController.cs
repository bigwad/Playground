using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Playground.Mvc.Models;

namespace Playground.Mvc.Controllers
{
    public class AsyncItemsController : ApiController
    {
        protected PlaygroundEntities db = new PlaygroundEntities();

        // GET api/<controller>
        public IEnumerable<ItemProxy> Get()
        {
            IEnumerable<ItemProxy> items = db.Items.OrderBy(x => x.Date).ToArray().Select(x => new ItemProxy(x));
            return items;
        }

        // GET api/<controller>/5
        public ItemProxy Get(int id)
        {
            Item item = db.Items.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return new ItemProxy();
            }

            ItemProxy proxy = new ItemProxy(item);

            return proxy;
        }

        // POST api/<controller>
        public async Task<ItemProxy> Post([FromBody]ItemProxy value)
        {
            if (value.Guid == null)
            {
                value.Guid = Guid.NewGuid().ToString();
            }

            value.Date = DateTime.Now;
            await value.InsertAsync(db);

            return value;
        }

        // PUT api/<controller>
        public async Task<ItemProxy> Put([FromBody]ItemProxy value)
        {
            await value.UpdateAsync(db);
            return value;
        }

        // DELETE api/<controller>/5
        public async Task<bool> Delete(int id)
        {
            Item item = db.Items.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return false;
            }

            ItemProxy proxy = new ItemProxy(item);
            await proxy.DeleteAsync(db);

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}