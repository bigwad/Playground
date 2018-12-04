using Starcounter;

namespace Playground
{
    partial class TableRowItemJson : Json, IBound<Database.Person>
    {
        public TableRowItemJson(Database.Person item)
        {
            this.Data = item;
        }

        public TableRowItemJson(string objectId)
        {
            this.Data = Db.FromId<Database.Person>(objectId);
        }

        public string ObjectId => this.Data?.GetObjectID();
    }
}
