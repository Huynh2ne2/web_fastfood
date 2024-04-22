using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DOANLTW.Models;
using PagedList;
using PagedList.Mvc;
namespace DOANLTW.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        WEBTHUCANNHANHDataContext data = new WEBTHUCANNHANHDataContext();
        private List<MonAn> LayMonAn(int count)
        {
            return data.MonAns.OrderBy(a => a.MaMon).Take(count).ToList();
        }
        public ActionResult Index(int ? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var monan = LayMonAn(20);
            return View(monan.ToPagedList(pageNum,pageSize));
        }
        public ActionResult ThucDon()
        {
            var thucdon = from td in data.Loais select td;
            return PartialView(thucdon);        

        }
        public ActionResult MATheothucdon(int id)
        {
            var monan = from ma in data.MonAns where ma.MaLoai == id select ma;
            return View(monan);
        }
        public ActionResult Details(int id)
        {
            var monan = from ma in data.MonAns where ma.MaMon == id select ma;
            return View(monan.Single());
        }
    }
}