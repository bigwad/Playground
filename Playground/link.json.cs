using Starcounter;

namespace A
{
    partial class link : Json
    {
        protected void Handle(Input.btn action)
        {
            action.Cancel();
            this.cp = Self.GET("/a/form");
        }
    }
}
