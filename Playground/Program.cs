using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Starcounter;

namespace Playground
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