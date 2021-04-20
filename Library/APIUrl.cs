using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyInfo
{
    public class APIUrl
    {
        public static string APITinhThanh = "https://thongtindoanhnghiep.co/api/city";
        public static string APIQuanHuyen = "https://thongtindoanhnghiep.co/api/city/{0}/district";
        public static string APIPhuongXa = "https://thongtindoanhnghiep.co/api/district/{0}/ward";
        public static string APIListDoanhNghiep = "https://thongtindoanhnghiep.co/api/company?l={0}&r={1}&p={2}";
        public static string APIDetailDoanhNghiep = "https://thongtindoanhnghiep.co/api/company/{0}";
    }
}
