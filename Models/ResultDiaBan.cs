using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyInfo.Models
{
    public class ResultDiaBan
    {
        public List<DiaBan> LtsItem { get; set; }
        public int TotalDoanhNghiep { get; set; }
        public int TotalItem { get; set; }
    }
    public class DiaBan
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string SolrID { get; set; }
        public int TotalDoanhNghiep { get; set; }
    }
}
