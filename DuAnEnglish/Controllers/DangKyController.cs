using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class DangKyController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: DangKy
        public ActionResult DangKy()
        {
            return View();
        }
        // POST: Xử lý đăng ký
        [HttpPost]
        public ActionResult DangKy(string TenDangNhap, string MatKhau, string NhapLaiMatKhau, string Email, string SDT, string HoTen)
        {
            // So sánh mật khẩu
            if (MatKhau != NhapLaiMatKhau)
            {
                ViewBag.ThongBao = "Mật khẩu và nhập lại mật khẩu không khớp!";
                return View();
            }

            // Kiểm tra nếu tên đăng nhập đã tồn tại
            var user = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == TenDangNhap);
            if (user != null)
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            // Kiểm tra định dạng email
            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com)$", RegexOptions.IgnoreCase);
            bool emailHopLe = emailRegex.IsMatch(Email);

            var sdtRegex = new Regex(@"^\d{10}$");
            bool sdtHopLe = sdtRegex.IsMatch(SDT);

            if (!emailHopLe && !sdtHopLe)
            {
                ViewBag.ThongBao = "Email không hợp lệ! Số điện thoại phải có 10 chữ số!";
                return View();
            }
            if (!emailHopLe)
            {
                ViewBag.ThongBao = "Email không hợp lệ!";
                return View();
            }
            if (!sdtHopLe)
            {
                ViewBag.ThongBao = "Số điện thoại phải có 10 chữ số!";
                return View();
            }

            var taiKhoanMoi = new TaiKhoan
            {
                TenDangNhap = TenDangNhap,
                MatKhau = MatKhau,
                Email = Email,
                SDT = SDT,
                LoaiTK = "hocvien",
                TrangThai = "hoạt động"
            };

            db.TaiKhoans.Add(taiKhoanMoi);
            db.SaveChanges();

            var hocVienMoi = new HocVien
            {
                IDTenDangNhap = TenDangNhap,
                TenHV = HoTen,
                NgaySinh = null,
                GioiTinh = null,
                DiaChi = null
            };

            db.HocViens.Add(hocVienMoi);
            db.SaveChanges();

            return RedirectToAction("DangNhap", "DangNhap");
        }
    }
}