using CompanyInfo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CompanyInfo
{
    public static class Library
    {
        public static List<DiaBan> LayDSTinhThanh()
        {
            string jsonTinh = GetReleases(APIUrl.APITinhThanh);
            ResultDiaBan lstTinh = JsonConvert.DeserializeObject<ResultDiaBan>(jsonTinh);
            return lstTinh.LtsItem.Select(x => new DiaBan {
                ID = x.ID,
                Title = x.Title,
                SubTitle = x.Title+" ("+x.TotalDoanhNghiep+")",
                TotalDoanhNghiep = x.TotalDoanhNghiep,
                SolrID = x.SolrID.Substring(1,x.SolrID.Length-1)
            }).ToList();
        }

        public static List<DiaBan> LayDSQuanHuyen(int idTinh)
        {
            string jsonHuyen = GetReleases(String.Format(APIUrl.APIQuanHuyen, idTinh));
            List<DiaBan> lstHuyen = JsonConvert.DeserializeObject<List<DiaBan>>(jsonHuyen);
            return lstHuyen.Select(x => new DiaBan
            {
                ID = x.ID,
                Title = x.Title,
                TotalDoanhNghiep = x.TotalDoanhNghiep,
                SolrID = x.SolrID.Substring(1, x.SolrID.Length - 1)
            }).ToList();
        }

        public static List<DiaBan> LayDSPhuongXa(int idHuyen)
        {
            string jsonXa = GetReleases(String.Format(APIUrl.APIPhuongXa, idHuyen));
            List<DiaBan> lstXa = JsonConvert.DeserializeObject<List<DiaBan>>(jsonXa);
            return lstXa.Select(x => new DiaBan
            {
                ID = x.ID,
                Title = x.Title,
                TotalDoanhNghiep = x.TotalDoanhNghiep,
                SolrID = x.SolrID.Substring(1, x.SolrID.Length - 1)
            }).ToList();
        }

        public static string GetReleases(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            var content = string.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }

        public static List<DoanhNghiepModel> GetListDoanhNghiepFormMaSoThue(List<string> lstMaSoThue)
        {
            List<DoanhNghiepModel> lstDoanhNghiep = new List<DoanhNghiepModel>();
            foreach (string maSoThue in lstMaSoThue)
            {
                try
                {
                    string json = Library.GetReleases(string.Format(APIUrl.APIDetailDoanhNghiep, maSoThue));
                    DoanhNghiepModel itemDoanhNghiep = JsonConvert.DeserializeObject<DoanhNghiepModel>(json);
                    lstDoanhNghiep.Add(itemDoanhNghiep);
                }
                catch (Exception ex)
                {
                }
            }
            return lstDoanhNghiep;
        }
    }
}
