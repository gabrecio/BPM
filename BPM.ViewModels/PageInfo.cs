using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPM.ViewModels
{
    public class PageInfo
    {
        public int page  { get; set; }
        public int itemsPerPage { get; set; }
        public string sortBy { get; set; }
        public bool reverse { get; set; }
        public string  search  { get; set; }
        public int totalItems { get; set; }
    }
}