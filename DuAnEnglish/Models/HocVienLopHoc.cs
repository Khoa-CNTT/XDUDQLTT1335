//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DuAnEnglish.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HocVienLopHoc
    {
        public int IDHocVien { get; set; }
        public int IDLopHoc { get; set; }
        public string TrangThai { get; set; }
        public Nullable<System.DateTime> DiemDanh { get; set; }
    
        public virtual DiemSo DiemSo { get; set; }
        public virtual HocVien HocVien { get; set; }
        public virtual LopHoc LopHoc { get; set; }
    }
}
