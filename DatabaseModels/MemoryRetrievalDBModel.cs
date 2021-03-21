using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.DatabaseModels
{
    public class MemoryRetrievalDBModel
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string StringDate { get; set; }
        public int HasPhoto { get; set; }
    }
}