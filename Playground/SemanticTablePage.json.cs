using Starcounter;

namespace Playground
{
    partial class SemanticTablePage : Json
    {
        public SemanticTablePage()
        {
            this.TableItems = Self.GET("/playground/table-items");
        }
    }
}
