using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Data.SqlClient;

namespace NEXT_BMS.Utilities
{
    public interface IExcelExport
    {
        DataSet ExportDataTable(string sqlQuery);
        DataTable Export(string sqlQuery);
    }

    public class ExcelExport : IExcelExport
    {
        private IConfiguration _configuration;

        public ExcelExport(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DataTable Export(string sqlQuery)
        {
            DataTable Custdatatable = ExportDataTable(sqlQuery).Tables[0];
            return Custdatatable;

        }
        public DataSet ExportDataTable(string sqlQuery)
        {
            DataSet ds = new DataSet();
            var sqlconn = _configuration.GetConnectionString("NEXT_BMSConnection");
            using (SqlConnection scon = new SqlConnection(sqlconn))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery))
                {
                    cmd.Connection = scon;
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }
    }
}
