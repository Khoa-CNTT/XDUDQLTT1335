using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;

namespace DuAnEnglish.Controllers
{
    public class QuanLyKhoaHocController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: QuanLyKhoaHoc
        public ActionResult QuanLyKhoaHoc(string danhmuc)
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

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            // Trả lại danh sách khóa học (đã lọc hoặc tất cả) cho view để hiển thị.
            return View(ds);
        }
    }
}