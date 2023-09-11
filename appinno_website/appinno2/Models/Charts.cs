using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appinno2.Models
{
    public class Chart
    {
        public int pagenumber { get; set; }
        public long tagID { get; set; }
        public List<appinno2.ClassCollection.WebServiceModels.LatestChartForList> Charts { get; set; }
        public List<appinno2.ClassCollection.WebServiceModels.Tag> tags { get; set; }

    }
}
