using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class ThanhToanController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: ThanhToan
        public ActionResult DanhSachHoaDon()
        {
            // Lấy TenDangNhap từ session
            string tenDangNhap = Session["User"]?.ToString();
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để đăng ký khóa học";
                return RedirectToAction("DangNhap", "DangNhap");
            }
            //if (string.IsNullOrEmpty(tenDangNhap))
            //{
            //    return RedirectToAction("Login", "Account"); // Redirect nếu không có session
            //}
            // Lấy danh sách các hóa đơn của người dùng dựa trên TenDangNhap
            var hoaDons = db.ThanhToans
                            .Where(t => t.TenDangNhap == tenDangNhap)
                            .ToList();  // Trả về danh sách ThanhToan
            // CHUYỂN TempData sang ViewBag để view hiển thị được
            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            // Trả kết quả về view
            return View(hoaDons);
        }
        // Xóa hóa đơn
        public ActionResult XoaHoaDon(int id)
        {
            var hoaDon = db.ThanhToans.FirstOrDefault(h => h.IDThanhToan == id && h.TrangThai == "Chưa thanh toán");

            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Không thể xóa vì đã được xử lý.";
                return RedirectToAction("DanhSachHoaDon");
            }

            // Xóa hóa đơn
            db.ThanhToans.Remove(hoaDon);
            db.SaveChanges();

            TempData["ThongBao"] = "Xóa thành công.";
            return RedirectToAction("DanhSachHoaDon");
        }
        // Xem hóa đơn
        public ActionResult XemHoaDon()
        {
            TempData["ThongBao"] = "Chưa hoàn thiện chức năng";
            return RedirectToAction("DanhSachHoaDon");
        }
        // Hiển thị thông tin hóa đơn cần thanh toán
        public ActionResult HoaDon(int id)
        {
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để thực hiện thanh toán.";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var hoaDon = db.ThanhToans.FirstOrDefault(h => h.IDThanhToan == id && h.TrangThai == "Chưa thanh toán");

            if (hoaDon == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn cần thanh toán.";
                return RedirectToAction("DanhSachHoaDon");
            }

            var hocVien = db.HocViens.FirstOrDefault(hv => hv.IDTenDangNhap == hoaDon.TenDangNhap);
            ViewBag.TenHocVien = hocVien != null ? hocVien.TenHV : "";

            var khoaHoc = db.KhoaHocs.FirstOrDefault(kh => kh.IDKhoaHoc == hoaDon.IDKhoaHoc);
            ViewBag.TenKhoaHoc = khoaHoc != null ? khoaHoc.TenKhoaHoc : "";

            var lopHoc = db.LopHocs.FirstOrDefault(lh => lh.IDLopHoc == hoaDon.IDLopHoc);
            ViewBag.TenLop = lopHoc != null ? lopHoc.TenLop : "";

            return View(hoaDon);
        }
        // POST: ThanhToan - Xác nhận thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoaDon(int idThanhToan, string phuongThucTT, string ngayThanhToan)
        {
            var thanhToan = db.ThanhToans.FirstOrDefault(t => t.IDThanhToan == idThanhToan);

            if (thanhToan == null)
            {
                ViewBag.ThongBao = "Không tìm thấy hóa đơn.";
                return RedirectToAction("DanhSachHoaDon");
            }

            DateTime parsedNgayThanhToan;
            if (!DateTime.TryParse(ngayThanhToan, out parsedNgayThanhToan))
            {
                parsedNgayThanhToan = DateTime.Now; // fallback nếu parse lỗi
            }
            // Lấy thông tin lớp học để cập nhật slot
            var lopHoc = db.LopHocs.FirstOrDefault(l => l.IDLopHoc == thanhToan.IDLopHoc);

            if (lopHoc == null)
            {
                ViewBag.ThongBao = "Không tìm thấy lớp học cho hóa đơn này.";
                return RedirectToAction("DanhSachHoaDon");
            }
            if (phuongThucTT == "Thanh toán trực tiếp")
            {
                // Cập nhật thông tin hóa đơn
                thanhToan.PhuongThucTT = "Thanh toán trực tiếp";
                thanhToan.NgayThanhToan = parsedNgayThanhToan;
                thanhToan.TrangThai = "Chờ xử lý";
                // Giảm số slot của lớp học đi 1 sau khi thanh toán
                if (lopHoc.Slot > 0)
                {
                    lopHoc.Slot -= 1;
                    db.SaveChanges(); // Lưu lại thay đổi số slot
                }
                else
                {
                    TempData["ThongBao"] = "Lớp học đã hết chỗ, không thể xác nhận thanh toán.";
                    return RedirectToAction("DanhSachHoaDon", "ThanhToan");
                    
                }
                db.SaveChanges();

                TempData["ThongBao"] = "Đã gửi yêu cầu thanh toán trực tiếp, vui lòng chờ xử lý.";
                return RedirectToAction("DanhSachHoaDon", "ThanhToan");
                //ViewBag.ThongBao = "Đã gửi yêu cầu thanh toán trực tiếp, vui lòng chờ xử lý.";
                //return RedirectToAction("DanhSachHoaDon");
            }
            else if (phuongThucTT == "Thanh toán qua VNPAY")
            {
                TempData["ThongBao"] = "Tính năng tạm thời chưa tích hợp";
                return RedirectToAction("DanhSachHoaDon", "ThanhToan");
                // Chuẩn bị gọi API VNPAY
                // Ví dụ tạo URL thanh toán (cần bổ sung logic tạo URL thực tế)
                //string vnp_ReturnUrl = Url.Action("VnpReturn", "ThanhToan", null, Request.Url.Scheme);
                //string vnpayPaymentUrl = CreateVnpayPaymentUrl(thanhToan, vnp_ReturnUrl);

                //return Redirect(vnpayPaymentUrl);
            }

            ViewBag.ThongBao = "Phương thức thanh toán không hợp lệ.";
            return RedirectToAction("DanhSachHoaDon");
        }


    }
}