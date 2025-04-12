using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class KhoaHocController : Controller
    {
        // Khởi tạo DbContext
        private trungtamtienganhEntities db = new trungtamtienganhEntities();

        // GET: KhoaHoc
        public ActionResult KhoaHoc(string danhmuc)
        {
            //Lấy toàn bộ danh sách khóa học từ cơ sở dữ liệu.
            var ds = db.KhoaHocs.ToList();

            //Kiểm tra xem biến 'danhmuc' có giá trị không và không phải là "all".
            //Nếu có giá trị và không phải là "all", tiến hành lọc danh sách khóa học theo 'DanhMuc'.
            if (!string.IsNullOrEmpty(danhmuc) && danhmuc != "all")
            {
                //Lọc danh sách khóa học theo danh mục ('DanhMuc') mà người dùng chọn.
                ds = ds.Where(k => k.DanhMuc == danhmuc).ToList();
            }

            // Trả lại danh sách khóa học (đã lọc hoặc tất cả) cho view để hiển thị.
            return View(ds);
        }

        // GET: Chi tiết khóa học
        public ActionResult ChiTietKhoaHoc(string id)
        {
            // Lấy thông tin khóa học theo ID
            var khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == id);

            // Nếu không tìm thấy khóa học, trả về lỗi 404
            if (khoaHoc == null)
            {
                return HttpNotFound();
            }

            // Trả thông tin khóa học cho view
            return View(khoaHoc);
        }

        // GET: /KhoaHoc/DangKyKhoaHoc
        public ActionResult DangKyKhoaHoc(string id, string lopId)
        {
            // Kiểm tra nếu chưa đăng nhập
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để đăng ký khóa học.";
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var lopHoc = db.LopHocs.FirstOrDefault(l => l.IDLopHoc == lopId);
            var khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == id);

            if (lopHoc == null || khoaHoc == null)
            {
                return HttpNotFound();
            }

            // Lấy TenDangNhap từ Session
            var tenDangNhap = Session["User"]?.ToString();
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                return RedirectToAction("DangNhap", "TaiKhoan"); // Nếu chưa đăng nhập thì đá về login
            }

            // Lấy tên học viên từ bảng HocVien
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.IDTenDangNhap == tenDangNhap);
            string tenHocVien = hocVien?.TenHV ?? "Không rõ";

            // Mặc định số khóa học là 1
            int soKhoaHoc = 1;
            decimal soTien = khoaHoc.HocPhi ?? 0; // Lấy giá tiền từ bảng KhoaHoc

            var viewModel = new ThanhToan
            {
                IDLopHoc = lopHoc.IDLopHoc,
                IDKhoaHoc = khoaHoc.IDKhoaHoc,
                TenDangNhap = tenDangNhap,
                SoKhoaHoc = soKhoaHoc,
                SoTien = soTien,
                PhuongThucTT = "Chưa chọn",
                TrangThai = "Chưa thanh toán",
                LopHoc = lopHoc,
                KhoaHoc = khoaHoc,
                TaiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.TenDangNhap == tenDangNhap)
            };

            // Gửi tên học viên ra View
            ViewBag.TenHocVien = tenHocVien;
            return View(viewModel);
        }

        //// POST: /KhoaHoc/DangKyKhoaHoc
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult DangKyKhoaHoc(ThanhToan model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Lấy TenDangNhap từ Session
        //        var tenDangNhap = Session["User"]?.ToString();
        //        if (string.IsNullOrEmpty(tenDangNhap))
        //        {
        //            ViewBag.ThongBaoDangKy = "Bạn cần đăng nhập để đăng ký khóa học.";
        //            return RedirectToAction("DangNhap", "DangNhap"); // Nếu không có TenDangNhap thì đá về login
        //        }

        //        // Kiểm tra các thông tin trong model để xác định tính hợp lệ
        //        if (model == null || string.IsNullOrEmpty(model.IDLopHoc) || string.IsNullOrEmpty(model.IDKhoaHoc))
        //        {
        //            ViewBag.ThongBaoDangKy = "Thông tin đăng ký chưa đầy đủ.";
        //            return View(model);
        //        }

               


        //        // Cập nhật thông tin thanh toán vào model
        //        model.SoTien = soTien;
        //        model.NgayThanhToan = null; // Chưa thanh toán

        //        // Lưu thông tin vào bảng ThanhToan
        //        db.ThanhToans.Add(new ThanhToan
        //        {
        //            IDLopHoc = model.IDLopHoc,
        //            TenDangNhap = tenDangNhap, // Lấy TenDangNhap từ session
        //            IDKhoaHoc = model.IDKhoaHoc,
        //            SoKhoaHoc = model.SoKhoaHoc,
        //            SoTien = model.SoTien,
        //            PhuongThucTT = model.PhuongThucTT, // Mặc định "Chưa chọn"
        //            TrangThai = "Chưa thanh toán", // Mặc định "Chưa thanh toán"
        //            NgayThanhToan = model.NgayThanhToan
        //        });

        //        // Lưu vào cơ sở dữ liệu
        //        db.SaveChanges();

        //        // Lưu thông báo thành công vào ViewBag
        //        ViewBag.ThongBaoDangKy = "Đăng ký khóa học thành công!";

        //        // Trả lại View với thông báo thành công
        //        return View(model);
        //    }

        //    // Nếu có lỗi, trả lại view với thông báo lỗi
        //    ViewBag.ThongBaoDangKy = "Đăng ký không thành công. Vui lòng thử lại!";
        //    return View(model);
        //}

    }
}