using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;

namespace Subground
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());
        }
    }
}