using Starcounter;

namespace Subground
{
    partial class LinkPage : Json
    {
        protected void Handle(Input.LoadTrigger action)
        {
            action.Cancel();
            this.CurrentPage = Self.GET("/b/form");
        }
    }
}
