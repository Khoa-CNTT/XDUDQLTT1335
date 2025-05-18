using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuAnEnglish.Models;


namespace DuAnEnglish.Controllers
{
    public class QuanLyTaiKhoanController : Controller
    {
        private trungtamtienganhEntities db = new trungtamtienganhEntities();
        // GET: QuanLyTaiKhoan
        public ActionResult QuanLyTaiKhoan(string loaitaikhoan)
        {
            //Lấy toàn bộ danh sách khóa học từ cơ sở dữ liệu.
            var ds = db.TaiKhoans.ToList();

            //Kiểm tra xem biến 'danhmuc' có giá trị không và không phải là "all".
            //Nếu có giá trị và không phải là "all", tiến hành lọc danh sách khóa học theo 'DanhMuc'.
            if (!string.IsNullOrEmpty(loaitaikhoan) && loaitaikhoan != "all")
            {
                //Lọc danh sách khóa học theo danh mục ('DanhMuc') mà người dùng chọn.
                ds = ds.Where(k => k.LoaiTK == loaitaikhoan).ToList();
            }

            if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }
            // Trả lại danh sách khóa học (đã lọc hoặc tất cả) cho view để hiển thị.
            return View(ds);
        }
        // GET: Create
        public ActionResult Create()
        {
            ViewBag.LoaiTK = new SelectList(db.LoaiTaiKhoans, "LoaiTK", "LoaiTK");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string TenDangNhap, string LoaiTK)
        {
            // Kiểm tra tên đăng nhập có dấu cách không
            if (TenDangNhap.Contains(" "))
            {
                ViewBag.ThongBao = "Tên đăng nhập không được chứa dấu cách.";
            }
            else
            {
                // Kiểm tra nếu tài khoản đã tồn tại
                var existingTaiKhoan = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == TenDangNhap && t.LoaiTK == LoaiTK);
                if (existingTaiKhoan != null)
                {
                    ViewBag.ThongBao = "Tài khoản đã tồn tại.";
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        // Tạo tài khoản mới
                        TaiKhoan tk = new TaiKhoan
                        {
                            TenDangNhap = TenDangNhap,
                            LoaiTK = LoaiTK,
                            MatKhau = "123",            // Mật khẩu mặc định
                            Email = null,               // Email để null
                            SDT = null,                 // SĐT để null
                            TrangThai = "Hoạt động"     // Trạng thái mặc định
                        };

                        db.TaiKhoans.Add(tk);
                        db.SaveChanges();

                        // Thêm vào bảng HocVien hoặc GiangVien tương ứng
                        if (LoaiTK.ToLower() == "hocvien")
                        {
                            HocVien hv = new HocVien
                            {
                                IDTenDangNhap = TenDangNhap,
                                TenHV = null,
                                NgaySinh = null,
                                GioiTinh = null,
                                DiaChi = null
                            };
                            db.HocViens.Add(hv);
                        }
                        else if (LoaiTK.ToLower() == "giangvien")
                        {
                            GiangVien gv = new GiangVien
                            {
                                IDTenDangNhap = TenDangNhap,
                                TenGV = null,
                                NgaySinh = null,
                                GioiTinh = null,
                                DiaChi = null
                            };
                            db.GiangViens.Add(gv);
                        }

                        db.SaveChanges();

                        TempData["ThongBao"] = "Thêm tài khoản thành công";
                        return RedirectToAction("QuanLyTaiKhoan");
                    }
                }
            }

            // Nếu có lỗi, load lại danh sách loại tài khoản
            ViewBag.LoaiTKList = db.LoaiTaiKhoans.Select(l => new SelectListItem
            {
                Value = l.LoaiTK,
                Text = l.LoaiTK
            }).ToList();

            return View();
        }




        // GET: Edit
        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            TaiKhoan tk = db.TaiKhoans.Find(id);
            if (tk == null)
                return HttpNotFound();

            ViewBag.LoaiTK = new SelectList(db.LoaiTaiKhoans, "LoaiTK", "LoaiTK", tk.LoaiTK);
            return View(tk);
        }

        public ActionResult Details(string id)
        {
            var taiKhoan = db.TaiKhoans.Find(id);
            if (taiKhoan == null)
            {
                return HttpNotFound();
            }
            return View(taiKhoan);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaiKhoan tk)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tk).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QuanLyTaiKhoan");
            }

            ViewBag.LoaiTK = new SelectList(db.LoaiTaiKhoans, "LoaiTK", "LoaiTK", tk.LoaiTK);
            return View(tk);
        }

        // GET: Delete
        public ActionResult Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            TaiKhoan tk = db.TaiKhoans.Find(id);
            if (tk == null)
                return HttpNotFound();

            db.TaiKhoans.Remove(tk);
            db.SaveChanges();
            return RedirectToAction("QuanLyTaiKhoan");
        }
        // POST: QuanLyTaiKhoan/Details
        
        [HttpPost]
        public ActionResult Details(TaiKhoan taiKhoan)
        {
            if (string.IsNullOrWhiteSpace(taiKhoan.Email))
            {
                ViewBag.ThongBao = "Vui lòng nhập địa chỉ email.";
                return View("Details", taiKhoan);
            }

            // Kiểm tra số điện thoại có hợp lệ hay không
            if (!string.IsNullOrWhiteSpace(taiKhoan.SDT) && !taiKhoan.SDT.All(char.IsDigit))
            {
                ViewBag.ThongBao = "Số điện thoại không hợp lệ.";
                return View("Details", taiKhoan);
            }

            // Kiểm tra loại tài khoản
            if (string.IsNullOrWhiteSpace(taiKhoan.LoaiTK) || !new List<string> { "admin", "giangvien", "hocvien" }.Contains(taiKhoan.LoaiTK))
            {
                ViewBag.ThongBao = "Loại tài khoản không hợp lệ.";
                return View("Details", taiKhoan);
            }

            if (ModelState.IsValid)
            {
                var existing = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == taiKhoan.TenDangNhap);
                if (existing != null)
                {
                    existing.Email = taiKhoan.Email;
                    existing.SDT = taiKhoan.SDT;
                    existing.TrangThai = taiKhoan.TrangThai;
                    existing.LoaiTK = taiKhoan.LoaiTK;

                    db.SaveChanges();
                    TempData["ThongBao"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction("QuanLyTaiKhoan");
                }
                else
                {
                    ViewBag.ThongBao = "Không tìm thấy tài khoản.";
                    return View("Details", taiKhoan);
                }
            }

            ViewBag.ThongBao = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
            return View("Details", taiKhoan);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
