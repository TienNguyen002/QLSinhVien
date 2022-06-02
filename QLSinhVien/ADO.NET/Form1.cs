using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace ADO.NET
{
    public partial class frmQLSinhVien : Form
    {
        private string _connectionstring;
        private const string TimKiem = "Nhập tên sinh viên cần tìm";
        private List<Lop> _dsLop;
        private List<SinhVien> _dsSinhVien;
        public frmQLSinhVien()
        {
            InitializeComponent();
        }

        private void frmQLSinhVien_Load(object sender, EventArgs e)
        {
            _dsLop = new List<Lop>();
            _dsSinhVien = new List<SinhVien>();
            _connectionstring = ConfigurationManager.ConnectionStrings["QLSinhVien"].ConnectionString;
            CaiDatTimKiem();
            LayMaLop();
            LaySV();
            LoadListView(_dsSinhVien);
        }
        private void CaiDatTimKiem()
        {
            txtTimKiem.Text = TimKiem;
            txtTimKiem.GotFocus += TxtTimKiemGotFocus;
            txtTimKiem.LostFocus += TxtTimKiemLostFocus;
        }
        private void TxtTimKiemLostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTimKiem.Text))
            {
                txtTimKiem.Text = TimKiem;
                btnTaiLai.PerformClick();
            }
        }
        private void TxtTimKiemGotFocus(object sender, EventArgs e)
        {
            txtTimKiem.Text = "";
        }
        private SinhVien LaySinhVien()
        {
            var sv = new SinhVien();
            if (!string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                sv.HoTen = txtHoTen.Text;
                sv.ID = string.IsNullOrWhiteSpace(txtMSSV.Text) ? -1 : int.Parse(txtMSSV.Text);
                sv.LopID = Convert.ToInt32(cbbLop.SelectedValue);
            }
            return sv;
        }
        private void ThemSVvaoLV(SinhVien sv)
        {
            string[] row = { sv.ID.ToString(), sv.HoTen, sv.LopID.ToString() };
            var item = new ListViewItem(row);
            lvSinhVien.Items.Add(item);
        }
        private void LoadListView(List<SinhVien> ds)
        {
            lvSinhVien.Items.Clear();
            foreach (var sv in ds)
            {
                ThemSVvaoLV(sv);
            }
        }
        private void LayMaLop()
        {
            string connectionstring = @"server=.; Database=QLSinhVien; Integrated Security=true";
            SqlConnection cnt = new SqlConnection(connectionstring);
            SqlCommand cmd = cnt.CreateCommand();
            cmd.CommandText = "Select * FROM Lop";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            cnt.Open();
            adapter.Fill(dt);
            cnt.Close();
            cmd.Dispose();
            cnt.Dispose();
            foreach (DataRow row in dt.Rows)
            {
                _dsLop.Add(new Lop(row));
            }
            cbbLop.DataSource = dt;
            cbbLop.DisplayMember = "TenLop";
            cbbLop.ValueMember = "ID";
        }
        private void LaySV()
        {
            string connectionstring = @"server=.; Database=QLSinhVien; Integrated Security=true";
            SqlConnection conn = new SqlConnection(connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "Select * FROM SinhVien";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            conn.Open();
            adapter.Fill(dt);
            conn.Close();
            cmd.Dispose();
            conn.Dispose();
            foreach (DataRow row in dt.Rows)
            {
                _dsSinhVien.Add(new SinhVien(row));
            }
        }

        private void btnMacDinh_Click(object sender, EventArgs e)
        {
            txtMSSV.Text = "";
            txtHoTen.Text = "";
        }

        private void btnTaiLai_Click(object sender, EventArgs e)
        {
            LaySV();
            LoadListView(_dsSinhVien);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            var sv = LaySinhVien();
            if (sv == null)
            {
                MessageBox.Show(text: "Chưa nhập thông tin", caption: "Thông báo");
                return;
            }
            var cnt = new SqlConnection(_connectionstring);
            var cmd = cnt.CreateCommand();
            if (sv.ID < 0)
                cmd.CommandText = "EXEC InsertStudent @HoTen, @MaLop";
            else
                cmd.CommandText = "UPDATE SinhVien SET HoTen = @HoTen, MaLop = @MaLop WHERE Id = @Id";
            cmd.Parameters.AddWithValue(parameterName: "@HoTen", sv.HoTen);
            cmd.Parameters.AddWithValue(parameterName: "@MaLop", sv.LopID);
            cmd.Parameters.AddWithValue(parameterName: "@Id", sv.ID);
            cnt.Open();
            var num = cmd.ExecuteNonQuery();
            if (num > 0)
            {
                btnTaiLai.PerformClick();
            }
            cnt.Close();
            cnt.Close();
            cmd.Dispose();
            cnt.Dispose();
        }

        private void lvSInhVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSinhVien.SelectedItems.Count > 0)
            {
                var id = int.Parse(lvSinhVien.SelectedItems[0].SubItems[0].Text);
                var SinhVien = _dsSinhVien.FirstOrDefault(sv => sv.ID == id);
                ThietLapSinhVien(SinhVien);
            }
        }
        private void ThietLapSinhVien(SinhVien sv)
        {
            txtHoTen.Text = sv.HoTen;
            txtMSSV.Text = sv.ID.ToString();
            cbbLop.SelectedValue = sv.LopID;
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            var list = _dsSinhVien.Where(sv => sv.HoTen.IndexOf(txtTimKiem.Text, StringComparison.InvariantCultureIgnoreCase) == -1).ToList();
            LoadListView(list);
        }
    }
}
