using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appinno2.Models
{
    public class Event
    {
        public int pagenumber { get; set; }
        public long tagID { get; set; }
        public List<appinno2.ClassCollection.WebServiceModels.LatestEventsForList> Events { get; set; }
        public List<appinno2.ClassCollection.WebServiceModels.Tag> tags { get; set; }

    }
}
