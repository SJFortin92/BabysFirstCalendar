using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.DatabaseModels
{
    public class MemoryDBModel
    {
        public int NoteID { get; set; }
        public int ChildID { get; set; }

        public DateTime DateOccurred { get; set; }

        public string Text { get; set; }

        public int HasPhoto { get; set; }

        public string PhotoLocation { get; set; }

        public int PhotoSize { get; set; }
    }
}