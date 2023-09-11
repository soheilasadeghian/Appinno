using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appinno2.Models
{
    public class Index
    {
        public List<appinno2.ClassCollection.WebServiceModels.LatestNewsForList> news { get; set; }
        public List<appinno2.ClassCollection.WebServiceModels.LatestDownloadForList> downloads { get; set; }
        public List<appinno2.ClassCollection.WebServiceModels.LatestEventsForList> events { get; set; }
        public List<appinno2.ClassCollection.WebServiceModels.LatestPublicationsForList> publications { get; set; }
        public List<appinno2.ClassCollection.WebServiceModels.LatestIoForList> organizations { get; set; }
        public List<appinno2.ClassCollection.WebServiceModels.LatestChartForList> charts { get; set; }
        public List<appinno2.ClassCollection.WebServiceModels.LatestNewsForList> ListNewsOnHomePage { get; set; }
    }
}
