using Starcounter;

namespace Playground
{
    partial class ItemPage : Json
    {
        public bool ItemExist => this.Item.Data != null;

        protected void Handle(Input.SaveTrigger action)
        {
            action.Cancel();
            this.AttachedScope.Commit();
        }

        [ItemPage_json.Item]
        partial class ItemPageItem : Json, IBound<Database.Person>
        {
            public string ObjectId
            {
                get
                {
                    return this.Data?.GetObjectID();
                }
            }
        }
    }
}
