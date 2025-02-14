using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GetImagesFromDatabase.Code
{
    public class Common
    {
        public static string webConnectionString = @"Data Source=sangtaoketnoi.vn;Initial Catalog=cenContainerRepair-ICDTANCANG;Persist Security Info=True;User ID=cenContainerRepair-ICDTANCANG;Password=cenContainerRepair-ICDTANCANG;Connect Timeout=60";
        public static DataTable dtImage(string IDChungTuChiTiet)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(Common.webConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand("List_ctBaoGiaChiTietHinhAnh_Container", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter[] sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("@IDChungTuChiTiet", IDChungTuChiTiet);
                    sqlCommand.Parameters.AddRange(sqlParameters);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(dt);
                }
            }
            return dt;
        }
    }
    
}