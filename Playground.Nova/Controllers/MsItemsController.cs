using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        private static int Counter = 0;
        private static ConcurrentDictionary<long, ItemProxy> Items = new ConcurrentDictionary<long, ItemProxy>();

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<ItemProxy> Get()
        {
            IEnumerable<ItemProxy> items = Items.Select(x => x.Value).OrderBy(x => x.Date);
            return items;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public ItemProxy Get(int id)
        {
            ItemProxy proxy = null;
            Items.TryGetValue(id, out proxy);

            return proxy;
        }

        // POST api/<controller>
        [HttpPost]
        public ItemProxy Post([FromBody]ItemProxy value)
        {
            if (value.Guid == null)
            {
                value.Guid = Guid.NewGuid().ToString();
            }

            long id = System.Threading.Interlocked.Increment(ref Counter);
            value.Id = id;
            Items.TryAdd(id, value);

            return value;
        }

        // PUT api/<controller>/5
        [HttpPut]
        public ItemProxy Put([FromBody]ItemProxy value)
        {
            if (Items.TryGetValue(value.Id, out ItemProxy proxy))
            {
                proxy.Guid = value.Guid;
                proxy.Date = value.Date;
                proxy.Thread = value.Thread;
                proxy.Index = value.Index;
            }

            return value;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            return Items.TryRemove(id, out ItemProxy _);
        }
    }
}
