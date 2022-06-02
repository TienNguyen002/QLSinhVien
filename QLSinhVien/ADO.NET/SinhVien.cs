using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ADO.NET
{
    class SinhVien
    {
        public int LopID { get; set; }
        public int ID { get; set; }
        public string HoTen { get; set; }
        public SinhVien()
        {

        }
        public SinhVien(int lopid, int id, string hoten)
        {
            LopID = lopid;
            ID = id;
            HoTen = hoten;
        }
        public SinhVien(DataRow row)
        {
            ID = (int)row["ID"];
            HoTen = row["HoTen"].ToString();
            LopID = (int)row["MaLop"];
        }
    }
}
