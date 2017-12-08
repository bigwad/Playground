using Starcounter;

namespace B
{
    partial class link : Json
    {
        protected void Handle(Input.btn action)
        {
            action.Cancel();
            this.cp = Self.GET("/b/form");
        }
    }
}
