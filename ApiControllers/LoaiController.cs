using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DOANLTW.Models;
using System.Xml.Linq;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Xml;
using System.Xml.XPath;

namespace DOANLTW.ApiControllers
{
    public class LoaiController : ApiController
    {

        WEBTHUCANNHANHDataContext db = new WEBTHUCANNHANHDataContext();
        [System.Web.Http.Route("api/Loai", Name = "Loai")]
        public List<Loai> Get()
        {

            List<Loai> loais = db.Loais.ToList();
            return loais;
        }
        public Loai GetLoaiByID(int id)
        {
            Loai loai = db.Loais.Where(row => row.MaLoai == id).FirstOrDefault();
            return loai;
        }
       public void PostLoai(Loai newloai)
       {
            
            db.Loais.InsertOnSubmit(newloai);
            db.SubmitChanges();
       }
        public void PutLoai(Loai loai)
        {
            Loai oldLoai = db.Loais.Where(row => row.MaLoai == loai.MaLoai).FirstOrDefault();
            oldLoai.TenLoai = loai.TenLoai;
            db.SubmitChanges();
        }

        public void DeleteLoai(int id)
        {
            Loai oldLoai = db.Loais.Where(row => row.MaLoai == id).FirstOrDefault();
            db.Loais.DeleteOnSubmit(oldLoai);
            db.SubmitChanges();
        }
        
    }
}
