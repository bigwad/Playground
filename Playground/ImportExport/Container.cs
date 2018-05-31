using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Starcounter;
using Starcounter.Linq;
using Database.Tree;

namespace Playground.ImportExport
{
    public class Container
    {
        public List<Dictionary<string, object>> Parents { get; set; }
        public List<Dictionary<string, object>> Children { get; set; }
        public List<Dictionary<string, object>> Siblings { get; set; }

        protected virtual string ObjectNoPropertyName { get; set; } = "ObjectNo";

        public Container()
        {
        }

        public static Container FromJson(string json)
        {
            JsonSerializerSettings settings = GetJsonSerializerSettings();
            Container container = JsonConvert.DeserializeObject<Container>(json, settings);

            return container;
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                Converters = new List<JsonConverter> { new JsonDecimalConverter(), new JsonIntConverter() }
            };

            return settings;
        }

        public void PopulateFromDatabase()
        {
            this.Parents = new List<Dictionary<string, object>>();
            this.Children = new List<Dictionary<string, object>>();
            this.Siblings = new List<Dictionary<string, object>>();

            this.Populate<Parent>(DbLinq.Objects<Parent>().ToArray().Where(x => x.GetType() == typeof(Parent)), this.Parents);
            this.Populate<Child>(DbLinq.Objects<Child>(), this.Children);
            this.Populate<Sibling>(DbLinq.Objects<Sibling>(), this.Siblings);
        }

        public void PopulateToDatabase()
        {
            Dictionary<ulong, ulong> objectNoMap = new Dictionary<ulong, ulong>();
            List<ReferenceCandidate> referenceCandidates = new List<ReferenceCandidate>();

            this.Populate<Parent>(this.Parents, objectNoMap, referenceCandidates);
            this.Populate<Child>(this.Children, objectNoMap, referenceCandidates);
            this.Populate<Sibling>(this.Siblings, objectNoMap, referenceCandidates);

            foreach (ReferenceCandidate candidate in referenceCandidates)
            {
                ulong newObjectNo = objectNoMap[candidate.OldObjectNo];
                object value = Db.FromId(newObjectNo);

                candidate.Property.SetValue(candidate.Entity, value);
            }
        }

        public string ToJson()
        {
            JsonSerializerSettings settings = GetJsonSerializerSettings();
            string json = JsonConvert.SerializeObject(this, settings);

            return json;
        }

        protected void Populate<TSource>(IEnumerable<TSource> sources, List<Dictionary<string, object>> destinations)
            where TSource : class
        {
            Type type = typeof(TSource);
            PropertyInfo[] properties = type.GetProperties().Where(x => x.CanRead && x.CanWrite).ToArray();

            foreach (TSource source in sources)
            {
                Dictionary<string, object> destination = new Dictionary<string, object>();

                destination.Add(this.ObjectNoPropertyName, source.GetObjectNo());

                foreach (PropertyInfo pi in properties)
                {
                    object value = pi.GetValue(source);

                    if (value != null && pi.PropertyType.GetTypeInfo().IsClass && pi.PropertyType != typeof(string))
                    {
                        value = value.GetObjectNo();
                    }

                    destination.Add(pi.Name, value);
                }

                destinations.Add(destination);
            }
        }

        protected void Populate<TDestination>(IEnumerable<Dictionary<string, object>> sources, Dictionary<ulong, ulong> objectNoMap, List<ReferenceCandidate> referenceCandidates)
            where TDestination : class, new()
        {
            Type type = typeof(TDestination);
            PropertyInfo[] properties = type.GetProperties().Where(x => x.CanRead && x.CanWrite).ToArray();

            foreach (Dictionary<string, object> source in sources)
            {
                TDestination destination = new TDestination();
                ulong oldObjectNo = (ulong)source[this.ObjectNoPropertyName];

                objectNoMap.Add(oldObjectNo, destination.GetObjectNo());

                foreach (PropertyInfo pi in properties)
                {
                    object value = source[pi.Name];

                    if (value != null && pi.PropertyType.GetTypeInfo().IsClass && pi.PropertyType != typeof(string))
                    {
                        referenceCandidates.Add(new ReferenceCandidate()
                        {
                            Entity = destination,
                            Property = pi,
                            OldObjectNo = (ulong)value
                        });
                    }
                    else
                    {
                        pi.SetValue(destination, value);
                    }
                }
            }
        }

        protected class ReferenceCandidate
        {
            public object Entity { get; set; }
            public PropertyInfo Property { get; set; }
            public ulong OldObjectNo { get; set; }
        }
    }
}
