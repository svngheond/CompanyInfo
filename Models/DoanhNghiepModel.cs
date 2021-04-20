using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyInfo.Models
{
    public class DoanhNghiepModel
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string TitleEn { get; set; }

        public string MaSoThue { get; set; }

        public DateTime? NgayCap { get; set; }

        public DateTime? NgayDongMST { get; set; }

        public string DiaChiCongTy { get; set; }

        public DateTime? NamTaiChinh { get; set; }

        public string MaSoHienThoi { get; set; }

        public DateTime? NgayNhanToKhai { get; set; }

        public DateTime? NgayBatDauHopDong { get; set; }

        public string VonDieuLe { get; set; }

        public int? TongSoLaoDong { get; set; }

        public string PhuongPhapTinhThueGTGT { get; set; }

        public string ChuSoHuu { get; set; }

        public string ChuSoHuu_DiaChi { get; set; }

        public string GiamDoc { get; set; }

        public string GiamDoc_DiaChi { get; set; }

        public string KeToanTruong { get; set; }

        public string KeToanTruong_DiaChi { get; set; }

        public int? TinhThanhID { get; set; }
        public string TinhThanhTitle { get; set; }
        public int? QuanHuyenID { get; set; }
        public string QuanHuyenTitle { get; set; }
        public int? PhuongXaID { get; set; }
        public string PhuongXaTitle { get; set; }
        public int? NoiDangKyQuanLyID { get; set; }
        public string NoiDangKyQuanLy_CoQuanTitle { get; set; }
        public string NoiDangKyQuanLy_DienThoai { get; set; }
        public string NoiDangKyQuanLy_Fax { get; set; }
        public int? NoiNopThueID { get; set; }
        public string NoiNopThue_DienThoai { get; set; }
        public string NoiNopThue_Fax { get; set; }
        public string NoiNopThue_CoQuanTitle { get; set; }
        public string NoiNopThue_CoQuanTitleAscii { get; set; }
        public string QuyetDinhThanhLap { get; set; }
        public string QuyetDinhThanhLap_NgayCap { get; set; }
        public int? QuyetDinhThanhLap_CoQuanCapID { get; set; }
        public string QuyetDinhThanhLap_CoQuanCapTitle { get; set; }
        public int? GiayPhepKinhDoanh_CoQuanCapID { get; set; }
        public string GiayPhepKinhDoanh_CoQuanCapTitle { get; set; }
        public string GiayPhepKinhDoanh { get; set; }
        public DateTime? GiayPhepKinhDoanh_NgayCap { get; set; }
        public int? LoaiHinhID { get; set; }
        public string LoaiHinhTitle { get; set; }
        public string LoaiHinhTitleAscii { get; set; }
        public int? NganhNgheID { get; set; }
        public string NganhNgheTitle { get; set; }
    }

    public class DoanhNghiepFirstModel
    {
        public string MaSoThue { get; set; }
    }
}
