using ClosedXML.Excel;
using System.Data;
//using System.Data.OleDb;

namespace NEXT_BMS.Utilities
{
    public interface IExcelImporter
    {
        string Documentupload(IFormFile formFile, int userId);
        DataTable ToBeImportedDataTable(string path, bool preview);
        DataTable GetExcelColumns(string path);
        //void ImportCustomer(DataTable ToBeImported);
    }
    public class ExcelImporter : IExcelImporter
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _environment;
        public ExcelImporter(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }
        //public DataTable ToBeImportedDataTable(string path)
        //{
        //    var constr = _configuration.GetConnectionString("excelconnection");
        //    DataTable datatable = new DataTable();

        //    constr = string.Format(constr, path);

        //    using (OleDbConnection excelconn = new OleDbConnection(constr))
        //    {
        //        using (OleDbCommand cmd = new OleDbCommand())
        //        {
        //            using (OleDbDataAdapter adapterexcel = new OleDbDataAdapter())
        //            {
        //                excelconn.Open();
        //                cmd.Connection = excelconn;
        //                DataTable excelschema;
        //                excelschema = excelconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //                var sheetname = excelschema.Rows[0]["Table_Name"].ToString();
        //                excelconn.Close();

        //                excelconn.Open();
        //                cmd.CommandText = "SELECT * From [" + sheetname + "]";
        //                adapterexcel.SelectCommand = cmd;
        //                adapterexcel.Fill(datatable);
        //                excelconn.Close();
        //            }
        //        }
        //    }
        //    return datatable;
        //} 

        public DataTable ToBeImportedDataTable(string path, bool preview)
        {
            DataTable datatable = new DataTable();
            int j = 0;
            if (path.EndsWith(".xlsx"))
            {
                //Open the Excel file using ClosedXML.
                using (XLWorkbook workBook = new XLWorkbook(path))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                datatable.Columns.Add(cell.Value.ToString());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            //Add rows to DataTable.
                            datatable.Rows.Add();
                            int i = 0;
                            foreach (IXLCell cell in row.Cells())
                            {
                                datatable.Rows[datatable.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                            if (preview && j == 100) { break; }
                            j++;
                        }

                    }
                }

            }
            else if (path.EndsWith(".csv"))
            {
                //Read the contents of CSV file.
                string csvData = File.ReadAllText(path);
                bool firstRow = true;
                //Execute a loop over the rows.
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (firstRow)
                        {
                            foreach (string cell in row.Split(','))
                            {
                                datatable.Columns.Add(cell.Replace("\"", ""));
                            }
                            firstRow = false;
                        }
                        else
                        {
                            //Add rows to DataTable.
                            datatable.Rows.Add();
                            int i = 0;
                            foreach (string cell in row.Split(','))
                            {
                                datatable.Rows[datatable.Rows.Count - 1][i] = cell.Replace("\"", "");
                                i++;
                            }
                            if (preview && j == 100) { break; }
                            j++;
                        }
                    }
                }
            }

            return datatable;
        }

        public DataTable GetExcelColumns(string path)
        {
            DataTable datatable = new DataTable();
            if (path.EndsWith(".xlsx"))
            {
                //Open the Excel file using ClosedXML.
                using (XLWorkbook workBook = new XLWorkbook(path))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    //Loop through the Worksheet rows.
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            datatable.Columns.Add(cell.Value.ToString());
                        }
                        break;
                    }
                }

            }
            else if (path.EndsWith(".csv"))
            {
                //Read the contents of CSV file.
                string csvData = File.ReadAllText(path);

                //Execute a loop over the rows.
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        foreach (string cell in row.Split(','))
                        {
                            datatable.Columns.Add(cell.Replace("\"", ""));
                        }
                        break;
                    }
                }
            }
            return datatable;
        }

        public string Documentupload(IFormFile fromFiles, int userId)
        {
            string uploadpath = Directory.GetCurrentDirectory();//_environment.WebRootPath;
            string dest_path = Path.Combine(uploadpath, "AttachedDocuments", userId.ToString());

            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }
            string sourcefile = Path.GetFileName(fromFiles.FileName);
            string path = Path.Combine(dest_path, sourcefile);

            using (FileStream filestream = new FileStream(path, FileMode.Create))
            {
                fromFiles.CopyTo(filestream);
            }
            return path;
        }
    }
}
