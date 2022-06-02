using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ADO.NET
{
	class Lop
	{
		public int ID { get; set; }
		public string TenLop { get; set; }
		public Lop()
		{

		}
		public Lop(int id, string tenlop)
		{
			ID = id;
			TenLop = tenlop;
		}
		public Lop(DataRow row)
		{
			ID = (int)row["ID"];
			TenLop = row["TenLop"].ToString();
		}
	}
}
