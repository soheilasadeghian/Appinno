using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appinno2.Models
{
    public class NewsDetail
    {
        public int Id { get; set; }
        public appinno2.ClassCollection.WebServiceModels.singleFullNews ListNewsDetail { get; set; }

    }
}
