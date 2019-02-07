using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Playground.Nova.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Playground.Nova.Controllers
{
    [Route("api/[controller]")]
    public class MsItemsController : Controller
    {
        protected PlaygroundContext db = new PlaygroundContext();

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<ItemProxy> Get()
        {
            IEnumerable<ItemProxy> items = db.Items.OrderBy(x => x.Date).ToArray().Select(x => new ItemProxy(x));
            return items;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
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
        [HttpPost]
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

        // PUT api/<controller>/5
        [HttpPut]
        public async Task<ItemProxy> Put([FromBody]ItemProxy value)
        {
            await value.UpdateAsync(db);
            return value;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
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
