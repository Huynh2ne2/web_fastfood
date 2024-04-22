using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using DOANLTW.Models;

namespace DOANLTW.Controllers
{
    public class LoaiController : Controller
    {
        WEBTHUCANNHANHDataContext db = new WEBTHUCANNHANHDataContext();
        public ActionResult Loai()
        {
           var listLoai =  db.Loais.ToList();
            return View(listLoai);
        }
    }
}
