using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;
namespace DuAnEnglish.Controllers
{
    public class QuanLyPhongHocController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: QuanLyPhongHoc
        public ActionResult QuanLyPhongHoc()
        {
            string tenDangNhap = Session["User"] as string;
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }
            var danhSachPhongHoc = db.PhongHocs.ToList();
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            return View(danhSachPhongHoc);
        }
        //GET
        public ActionResult Them()
        {
            return View();
        }
        // POST: Them Phong Hoc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them(PhongHoc phongHoc)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra mã phòng học đã tồn tại chưa
                var existingPhongHoc = db.PhongHocs.FirstOrDefault(p => p.IDPhongHoc == phongHoc.IDPhongHoc);
                if (existingPhongHoc != null)
                {
                    ViewBag.ThongBao = "Mã phòng học đã tồn tại.";
                    return View();
                }

                // Kiểm tra sức chứa hợp lệ
                if (phongHoc.SucChua < 0 || phongHoc.SucChua > 20)
                {
                    ViewBag.ThongBao = "Sức chứa phải lớn hơn hoặc bằng 0 và nhỏ hơn hoặc bằng 20.";
                    return View();
                }

                // Thêm phòng học vào database
                db.PhongHocs.Add(phongHoc);
                db.SaveChanges();

                TempData["ThongBao"] = "Thêm phòng học thành công!";
                return RedirectToAction("QuanLyPhongHoc");
            }

            return View();
        }

        public ActionResult xoa(int id)
        {
            string tenDangNhap = Session["User"] as string;

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var phonghoc = db.PhongHocs.FirstOrDefault(ph => ph.IDPhongHoc == id);

            if (phonghoc == null)
            {
                TempData["ThongBao"] = "Thông báo không tồn tại";
                return RedirectToAction("QuanLyPhongHoc");
            }

            db.PhongHocs.Remove(phonghoc);
            db.SaveChanges();

            TempData["ThongBao"] = "Xóa phòng học thành công";
            return RedirectToAction("QuanLyPhongHoc");
        }
    }
}