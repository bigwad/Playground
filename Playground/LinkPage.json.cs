using Starcounter;

namespace Playground
{
    partial class LinkPage : Json
    {
        protected void Handle(Input.LoadTrigger action)
        {
            action.Cancel();
            this.CurrentPage = Self.GET("/a/form");
        }
    }
}
