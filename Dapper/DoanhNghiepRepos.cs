using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using CompanyInfo.Models;
using System.Reflection;

namespace BussinessInfo.Dapper
{
    public class DoanhNghiepRepos
    {
        public IDbConnection _db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        private string tableName = "DoanhNghieps", propName, propValue, propUpdate;

        public DoanhNghiepRepos()
        {
            PropertyInfo[] props = (new DoanhNghiep()).GetType().GetProperties().Where(x => x.Name != "Id").ToArray();
            propName = string.Join(", ", props.Where(x => x.CustomAttributes.Where(y => y.AttributeType.FullName.Contains("NotMapped")).Count() == 0).Select(x => x.Name));
            propValue = string.Join(", ", props.Where(x => x.CustomAttributes.Where(y => y.AttributeType.FullName.Contains("NotMapped")).Count() == 0).Select(x => "@" + x.Name));
            propUpdate = string.Join(", ", props.Where(x => x.CustomAttributes.Where(y => y.AttributeType.FullName.Contains("NotMapped")).Count() == 0).Select(x => x.Name + " = @" + x.Name));
        }
        public List<DoanhNghiep> GetAll()
        {
            List<DoanhNghiep> empList = this._db.Query<DoanhNghiep>("SELECT * FROM " + tableName).ToList();
            return empList;
        }
        public List<DoanhNghiep> GetAll(List<string> fieldSelect)
        {
            List<DoanhNghiep> empList = this._db.Query<DoanhNghiep>("SELECT " + string.Join(",", fieldSelect.ToArray()) + " FROM " + tableName).ToList();
            return empList;
        }
        public DoanhNghiep Find(string mst,List<DoanhNghiep> lstDoanhNghiep)
        {
            try
            {
                DoanhNghiep[] lst = new DoanhNghiep[lstDoanhNghiep.Count];
                lstDoanhNghiep.CopyTo(lst);
                var node = lst.Where(x => !string.IsNullOrEmpty(x.Ma) && x.Ma == mst);
                return node.FirstOrDefault();
            }
            catch
            {
                return new DoanhNghiep();
            }
        }

        public DoanhNghiep Find(int? id)
        {
            if (id.HasValue)
            {
                string query = "SELECT * FROM " + tableName + " WHERE Id = " + id + "";
                return this._db.Query<DoanhNghiep>(query).SingleOrDefault();
            }
            else
            {
                return new DoanhNghiep();
            }
        }

        public DoanhNghiep Find(string mst)
        {
            if (this._db.State == ConnectionState.Closed)
                _db.Open();
            if (!string.IsNullOrEmpty(mst))
            {
                string query = "SELECT Id FROM " + tableName + " WHERE Ma = '" + mst + "'";
                return this._db.Query<DoanhNghiep>(query).SingleOrDefault();
            }
            else
            {
                return new DoanhNghiep();
            }
        }
        public DoanhNghiep Add(DoanhNghiep doanhNghiep)
        {
            if (this._db.State == ConnectionState.Closed)
                _db.Open();
            var sqlQuery = "INSERT INTO " + tableName + " (" + propName + ") VALUES(" + propValue + "); " + "SELECT CAST(SCOPE_IDENTITY() as int)";
            var doanhNghiepId = this._db.Query<int>(sqlQuery, doanhNghiep).Single();
            doanhNghiep.Id = doanhNghiepId;
            return doanhNghiep;
        }

        public DoanhNghiep Update(DoanhNghiep DoanhNghiep)
        {
            if (this._db.State == ConnectionState.Closed)
                _db.Open();
            var sqlQuery = "UPDATE " + tableName + " SET " + propUpdate + " WHERE Ma = @Ma";
            this._db.Execute(sqlQuery, DoanhNghiep);
            return DoanhNghiep;
        }

        public void Delete(int id)
        {
            var sqlQuery = ("Delete From " + tableName + " Where Id = " + id + "");
            this._db.Execute(sqlQuery);
        }

        public void DeleteItems(string ids)
        {
            var sqlQuery = "";
            foreach (var id in ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrEmpty(sqlQuery))
                    sqlQuery = "Id = " + id;
                else
                    sqlQuery += " or Id = " + id;
            }
            sqlQuery = "Delete From " + tableName + " Where " + sqlQuery;
            this._db.Execute(sqlQuery);
        }

        public bool IsExisted(string ten, int id)
        {
            return GetAll().Any(x => x.Ten == ten && x.Id != id);
        }


    }
}