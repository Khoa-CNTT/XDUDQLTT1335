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
    }
}