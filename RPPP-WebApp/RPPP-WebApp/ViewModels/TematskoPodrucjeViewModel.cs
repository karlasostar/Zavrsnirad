using System.Collections.Generic;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class TematskoPodrucjeViewModel
    {
        public IEnumerable<TematskoPodrucje> TematskoPodrucje { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
