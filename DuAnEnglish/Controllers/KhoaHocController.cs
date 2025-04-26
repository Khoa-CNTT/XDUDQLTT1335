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
            System.Diagnostics.Debug.WriteLine(">>>> Gọi DangKyKhoaHoc");
            System.Diagnostics.Debug.WriteLine(">>>> IDKhoaHoc: " + id);
            System.Diagnostics.Debug.WriteLine(">>>> IDLopHoc: " + lopId);
            
            // Kiểm tra nếu chưa đăng nhập
            if (Session["User"] == null)
            {
                TempData["ThongBaoDangNhap"] = "Bạn cần đăng nhập để đăng ký khóa học";
                return RedirectToAction("DangNhap", "DangNhap");
            }
            // Lấy TenDangNhap từ Session
            string tenDangNhap = Session["User"].ToString();

            // Lấy thông tin lớp học và khóa học dựa vào ID
            var lopHoc = db.LopHocs.FirstOrDefault(l => l.IDLopHoc == lopId);
            var khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.IDKhoaHoc == id);

            if (lopHoc == null || khoaHoc == null)
            {
                return HttpNotFound();
            }
            //System.Diagnostics.Debug.WriteLine(">>>> Slot HasValue: " + lopHoc.Slot.HasValue);  // Kiểm tra xem Slot có giá trị hay không
            //System.Diagnostics.Debug.WriteLine(">>>> Slot Value: " + lopHoc.Slot);  // Kiểm tra giá trị của Slot

            
            if (lopHoc.Slot.HasValue && lopHoc.Slot.Value == 0)
            {
                //System.Diagnostics.Debug.WriteLine(">>>> SLOT == 0, redirecting...");
                //System.Diagnostics.Debug.WriteLine(">>>> Redirect with IDKhoaHoc: " + id);
                TempData["ThongBao"] = "Bạn cần đăng nhập để đăng ký khóa học";
              
                return RedirectToAction("DanhSachLopHoc", "LopHoc", new { id = id });
               
            }
           
            // 2. Kiểm tra học viên đã đăng ký lớp này chưa (chưa cần thanh toán)
            var daDangKy = db.ThanhToans.Any(tt =>
                tt.TenDangNhap == tenDangNhap &&
                tt.IDKhoaHoc == id &&
                tt.IDLopHoc == lopId);

            if (daDangKy)
            {
                TempData["ThongBao"] = "Bạn đã đăng ký lớp này. Vui lòng thanh toán để hoàn tất!";
                return RedirectToAction("DanhSachKhoaHoc", "KhoaHoc"); // Hoặc redirect tới trang chi tiết lớp
            }
            // Lấy thông tin học viên thông qua TenDangNhap
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.IDTenDangNhap == tenDangNhap);
            if (hocVien == null)
            {
                return HttpNotFound(); // Hoặc xử lý khác nếu không có học viên
            }

            // Gán tên học viên vào ViewBag để hiển thị ở View
            ViewBag.TenHocVien = hocVien.TenHV;
            // Tạo model ThanhToan để truyền vào view
            var model = new ThanhToan
            {
                IDKhoaHoc = khoaHoc.IDKhoaHoc,
                IDLopHoc = lopHoc.IDLopHoc,
                KhoaHoc = khoaHoc,
                LopHoc = lopHoc,
                TenDangNhap = tenDangNhap
            };
            return View(model);

        }
    
        // POST: /KhoaHoc/DangKyKhoaHoc
        [HttpPost]
        public ActionResult DangKyKhoaHoc()
        {  
            return View();
        }

    }
}