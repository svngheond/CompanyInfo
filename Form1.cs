using BussinessInfo.Dapper;
using CompanyInfo.Models;
using FastMember;
using Newtonsoft.Json;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompanyInfo
{
    public partial class Form1 : Form
    {
        private bool renderExcel = false;
        private string diaBanText = string.Empty;
        private int pageTotal = 0;
        private string folderPath = string.Empty, fileNameOutput = string.Empty, filePathFull = string.Empty;
        private string apiUrl = string.Empty;
        private List<DiaBan> lstTinh = new List<DiaBan>(), lstHuyen = new List<DiaBan>(), lstXa = new List<DiaBan>();
        public Form1()
        {
            InitializeComponent();
            BindDataControl();
        }

        private void BindDataControl()
        {
            //Bind data Tinh/ThanhPho
            lstTinh = Library.LayDSTinhThanh();
            BindControl(cbxTinh, lstTinh, "SubTitle");

            //Set folder path default
            folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            textBox1.Text = folderPath;
            SetFileOutputPath();

            //Set show button new folder
            folderBrowserDialog1.ShowNewFolderButton = true;

            //Set Syncfusion Licensing
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDMyNzk5QDMxMzkyZTMxMmUzMEExTW04cVJrSEFqckVZNmxNOW5hTE9mb1haRzZsNkZWZjUyRWQ1aHRMenM9");
        }

        private void BindControl(ComboBox cb, List<DiaBan> list, string displayField = "Title", string valueField = "ID")
        {
            cb.DataSource = list;
            cb.DisplayMember = displayField;
            cb.ValueMember = valueField;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                textBox1.Enabled = true;
            }
            else
            {
                textBox1.Enabled = false;
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderPath = folderDlg.SelectedPath;
                SetFileOutputPath();
            }
        }

        private void cbxTinh_SelectedValueChanged(object sender, EventArgs e)
        {
            lstHuyen = Library.LayDSQuanHuyen(((DiaBan)(cbxTinh.SelectedItem)).ID);
            BindControl(cbxHuyen, lstHuyen);
            SetFileOutputPath();
        }

        private void cbxHuyen_SelectedValueChanged(object sender, EventArgs e)
        {
            lstXa = Library.LayDSPhuongXa(int.Parse(cbxHuyen.SelectedValue.ToString()));
            BindControl(cbxXa, lstXa);
            SetFileOutputPath();
        }

        private void cbxXa_SelectedValueChanged(object sender, EventArgs e)
        {
            SetFileOutputPath();
        }

        private void cbTinh_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTinh.Checked)
            {
                cbHuyen.Checked = false;
                cbXa.Checked = false;
            }
            CheckingSelected();
            SetFileOutputPath();
        }

        private void cbHuyen_CheckedChanged(object sender, EventArgs e)
        {
            if (cbHuyen.Checked)
            {
                cbTinh.Checked = false;
                cbXa.Checked = false;
            }
            CheckingSelected();
            SetFileOutputPath();
        }

        private void cbXa_CheckedChanged(object sender, EventArgs e)
        {
            if (cbXa.Checked)
            {
                cbTinh.Checked = false;
                cbHuyen.Checked = false;
            }
            CheckingSelected();
            SetFileOutputPath();
        }

        void SetFileOutputPath()
        {
            fileNameOutput = lstTinh[cbxTinh.SelectedIndex].Title + ".xlsx";
            apiUrl = lstTinh[cbxTinh.SelectedIndex].SolrID;
            diaBanText = lstTinh[cbxTinh.SelectedIndex].Title;
            if (cbHuyen.Checked)
            {
                fileNameOutput = lstTinh[cbxTinh.SelectedIndex].Title + "_" + cbxHuyen.Text + ".xlsx";
                apiUrl = lstHuyen[cbxHuyen.SelectedIndex].SolrID;
                diaBanText = cbxHuyen.Text;
            }
            else if (cbXa.Checked)
            {
                fileNameOutput = lstTinh[cbxTinh.SelectedIndex].Title + "_" + cbxHuyen.Text + "_" + cbxXa.Text + ".xlsx";
                apiUrl = lstXa[cbxXa.SelectedIndex].SolrID;
                diaBanText = cbxXa.Text;
            }

            filePathFull = folderPath + "\\" + fileNameOutput;
            textBox1.Text = filePathFull;
        }
        private void CheckingSelected()
        {
            if (!cbTinh.Checked && !cbHuyen.Checked && !cbXa.Checked)
                cbTinh.Checked = true;
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            renderExcel = radioButton4.Checked ? false : true;
            TurnOffControl();
            Thread newThread = new Thread(SyncData);
            newThread.Start();
        }
        private void TurnOffControl()
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "Đang convert: " + diaBanText;
        }
        private void TurnOnControl()
        {
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            toolStripStatusLabel1.Text = "Đã convert xong: " + diaBanText +" (100%)";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void SyncData()
        {
            int pageIndex = 1;
            string fullUrlAPI = string.Format(APIUrl.APIListDoanhNghiep, apiUrl, 100, pageIndex);
            string jsonPage = Library.GetReleases(fullUrlAPI);
            ObjFromJson obj = JsonConvert.DeserializeObject<ObjFromJson>(jsonPage);
            pageTotal = obj.Option.TotalRow / 100 + 1;//(Rowlimit = 100) + 1

            //Xử lý import hoặc save ra json
            if (renderExcel)//Save file excel
                SaveExcel();
            else//Import PM QL Dự án
                ImportPMQLDA();

            ShowStatusDone();
        }

        private void ShowStatusDone()
        {
            bool hasError = true;
            while (hasError)
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        TurnOnControl();
                    }));
                    hasError = false;
                }
                catch (Exception)
                {
                    hasError = true;
                }
                Thread.Sleep(100);
            }
        }

        private void SaveExcel()
        {
            string jsonPage, fullUrlAPI;
            List<DoanhNghiepModel> lstDNDetail = new List<DoanhNghiepModel>();

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                //Create a new workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];
                DataTable dt = new DataTable();
                for (int i = 1; i <= pageTotal; i++)
                {
                    try
                    {
                        fullUrlAPI = string.Format(APIUrl.APIListDoanhNghiep, apiUrl, 100, i);
                        jsonPage = Library.GetReleases(fullUrlAPI);
                        var _obj = JsonConvert.DeserializeObject<ObjFromJson>(jsonPage);
                        lstDNDetail.AddRange(Library.GetListDoanhNghiepFormMaSoThue(_obj.LtsItems.Select(x => x.MaSoThue).ToList()));

                        DataTable _dt = new DataTable();
                        using (var reader = ObjectReader.Create(lstDNDetail))
                        {
                            _dt.Load(reader);
                        }
                        dt.Merge(_dt);

                        //Show trạng thái trên form
                        this.Invoke(new Action(() =>
                        {
                            toolStripProgressBar1.Value = (int)Math.Round((double)i * 100 / pageTotal);
                            toolStripStatusLabel1.Text = "Đang convert: " + diaBanText + " (" + toolStripProgressBar1.Value + "%)";
                        }));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Có lỗi xảy ra");
                    }
                }

                //Import data from the data table with column header, at first row and first column, 
                sheet.ImportDataTable(dt, true, 1, 1, true);

                //Autofit the columns
                sheet.UsedRange.AutofitColumns();

                //Save the file in the given path
                Stream excelStream = File.Create(filePathFull);
                workbook.SaveAs(excelStream);
                excelStream.Dispose();
            }
        }

        private void ImportPMQLDA()
        {
            string jsonPage, fullUrlAPI;
            List<DoanhNghiepModel> lstDNDetail = new List<DoanhNghiepModel>();
            DoanhNghiepRepos doanhNghiepRepos = new DoanhNghiepRepos();
            for (int i = 1; i <= pageTotal; i++)
            {
                try
                {
                    fullUrlAPI = string.Format(APIUrl.APIListDoanhNghiep, apiUrl, 100, i);
                    jsonPage = Library.GetReleases(fullUrlAPI);
                    var _obj = JsonConvert.DeserializeObject<ObjFromJson>(jsonPage);
                    lstDNDetail = Library.GetListDoanhNghiepFormMaSoThue(_obj.LtsItems.Select(x => x.MaSoThue).ToList());
                    for (int j = 0,n= lstDNDetail.Count; j < n; j++)
                    {
                        DoanhNghiep doanhNghiep = new DoanhNghiep()
                        {
                            Ma = lstDNDetail[j].MaSoThue,
                            DiaChi = lstDNDetail[j].DiaChiCongTy,
                            DienThoai = "",
                            Email = "",
                            NguoiDaiDien = lstDNDetail[j].GiamDoc,
                            ChucVu = !string.IsNullOrEmpty(lstDNDetail[j].GiamDoc) ? "Giám đốc" : "",
                            Ten = lstDNDetail[j].Title,
                            ThoiGian = !string.IsNullOrEmpty(lstDNDetail[j].QuyetDinhThanhLap_NgayCap) ? Convert.ToDateTime(lstDNDetail[j].QuyetDinhThanhLap_NgayCap) : (DateTime?)null
                        };
                        doanhNghiepRepos.Add(doanhNghiep);
                    }

                    //Show trạng thái trên form
                    this.Invoke(new Action(() =>
                    {
                        toolStripProgressBar1.Value = (int)Math.Round((double)i * 100 / pageTotal);
                        toolStripStatusLabel1.Text = "Đang convert: " + diaBanText + " (" + toolStripProgressBar1.Value + "%)";
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Có lỗi xảy ra");
                }
            }
        }
    }
}
