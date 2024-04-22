using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DOANLTW.Models;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using ClosedXML.Excel;
using OfficeOpenXml;
using DocumentFormat.OpenXml.EMMA;

namespace DOANLTW.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        WEBTHUCANNHANHDataContext data = new WEBTHUCANNHANHDataContext();
        private object ctdh;
        private int maDonHang;

        public IEnumerable<object> ChiTietDatHangs { get; private set; }
        public object ExcelWorksheet { get; private set; }

        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang==null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult ThemGioHang(int iMaMon, string strUrl)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang monan = lstGioHang.Find(n => n.iMaMon == iMaMon);
            if (monan==null)
            {
                monan = new GioHang(iMaMon);
                lstGioHang.Add(monan);
                return Redirect(strUrl);
            }
            else
            {
                monan.iSoLuong++;
                return Redirect(strUrl);
            }
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGiohang(int iMaMon)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang monan = lstGioHang.SingleOrDefault(n => n.iMaMon == iMaMon);
            if (monan != null)
            {
                lstGioHang.RemoveAll(n => n.iMaMon == iMaMon);
                return RedirectToAction("GioHang");
            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapNhatGioHang(int iMaMon, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang monan = lstGioHang.SingleOrDefault(n => n.iMaMon == iMaMon);
            if (monan != null)
            {
                monan.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // lấy giỏ hàng từ session
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult DatHang(FormCollection collection)
        {
            DonDatHang ddh = new DonDatHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            List<GioHang> gh = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(ngaygiao);
            ddh.TinhTrangGiaohang = false;
            ddh.DaThanhToan = false;
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            // thêm chi tiết đơn hàng
            foreach (var item in gh)
            {
                ChiTietDatHang ctdh = new ChiTietDatHang();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaMon = item.iMaMon;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dGiaBan;
                data.ChiTietDatHangs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");

        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }

        public FileContentResult ExportToExcel()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("GioHang");

            worksheet.Cell(1, 1).Value = "Mã món ăn";
            worksheet.Cell(1, 2).Value = "Tên món ăn";
            worksheet.Cell(1, 3).Value = "Ảnh đại diện";
            worksheet.Cell(1, 4).Value = "Số lượng";
            worksheet.Cell(1, 5).Value = "Đơn giá";
            worksheet.Cell(1, 6).Value = "Thành tiền";
            worksheet.Cell(1, 7).Value = "Họ tên";
            worksheet.Cell(1, 8).Value = "Địa chỉ";
            worksheet.Cell(1, 9).Value = "Điện thoại";
            worksheet.Cell(1, 10).Value = "Ngày đặt";
            worksheet.Cell(1, 11).Value = "Ngày giao";
            List<GioHang> lstGioHang = LayGioHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            DonDatHang ddh = new DonDatHang();
            int row = 2;
            foreach (var item in lstGioHang)
            {
                worksheet.Cell(row, 1).Value = item.iMaMon;
                worksheet.Cell(row, 2).Value = item.sTenMon;
                worksheet.Cell(row, 3).Value = item.sAnhDD;
                worksheet.Cell(row, 4).Value = item.iSoLuong;
                worksheet.Cell(row, 5).Value = item.dGiaBan;
                worksheet.Cell(row, 6).Value = item.dThanhTien;
                worksheet.Cell(row, 7).Value = kh.HoTen;
                worksheet.Cell(row, 8).Value = kh.DiaChiKH;
                worksheet.Cell(row, 9).Value = kh.DienThoaiKH;
                ddh.NgayDat = DateTime.Now;
                worksheet.Cell(row, 10).Value = ddh.NgayDat;
                var ngaygiao = String.Format("{0:MM/dd/yyyy}", ddh.NgayDat);
                ddh.NgayGiao = DateTime.Parse(ngaygiao);
                worksheet.Cell(row, 11).Value = ddh.NgayGiao;



                row++;
            }
           var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GioHang.xlsx");
        }
      


    }


}