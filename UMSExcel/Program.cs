using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace UMSExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["UMS"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM ExcelGenerate", sqlConnection);
            SqlCommand sqlCommand2 = new SqlCommand("SELECT * FROM UserInfo",sqlConnection);
            SqlCommand sqlCommandDel = new SqlCommand("DELETE FROM ExcelGenerate",sqlConnection);
            /*sqlCommand.CommandType = CommandType.StoredProcedure;*/
            /*sqlCommand.Parameters.AddWithValue("@UserId", userId);*/
            sqlConnection.Open();
            var reader = sqlCommand.ExecuteReader();

            if (reader.Read())
            {
                string fileName = reader.GetString(0);
                string sendEmailTo = reader.GetString(1);
               
                sqlConnection.Close();

                sqlConnection.Open();
                var getAll = sqlCommand2.ExecuteReader();

                string OpenAtDownload = "";


                /* System.Diagnostics.Debug.WriteLine("Is it empty: " + fileName.IsNullOrWhiteSpace());
                 if (fileName == "" || fileName.IsNullOrWhiteSpace() == true) { fileName = "UserList"; }*/
                if (fileName == "" || fileName == null) { fileName = "UserList"; }
                
                Microsoft.Office.Interop.Excel.Workbook ExcelWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet ExcelWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Application ExcelExport = new Microsoft.Office.Interop.Excel.Application();

                ExcelWorkBook = ExcelExport.Workbooks.Add(misValue);
                ExcelWorkSheet = ExcelWorkBook.Worksheets.get_Item(1);

                ExcelWorkSheet.Cells.NumberFormat = "@";
                var rowColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);

                ExcelWorkSheet.get_Range("A1", "K1").Interior.Color = rowColor;
                ExcelWorkSheet.get_Range("A1", "K1").Font.Bold = true;


                ExcelWorkSheet.Cells[1, 1] = "UserName";

                ExcelWorkSheet.Cells[1, 2] = "FirstName";

                ExcelWorkSheet.Cells[1, 3] = "LastName";

                ExcelWorkSheet.Cells[1, 4] = "Email";

                ExcelWorkSheet.Cells[1, 5] = "Address";

                ExcelWorkSheet.Cells[1, 6] = "City";

                ExcelWorkSheet.Cells[1, 7] = "Country";

                ExcelWorkSheet.Cells[1, 8] = "ZipCode";

                ExcelWorkSheet.Cells[1, 9] = "PhoneNumber";

                ExcelWorkSheet.Cells[1, 10] = "Role";

                ExcelWorkSheet.Cells[1, 11] = "Status";

                /*  var userInfoes = db.UserInfoes.Include(u => u.Role).Include(u => u.Status).Include(u => u.UserCredential);
                  userInfoes.ToList();*/
                int row = 2;
                while (getAll.Read())
                {

                    ExcelWorkSheet.Cells[row, 1] = getAll.GetString(1);
                    ExcelWorkSheet.Cells[row, 2] = getAll.GetString(2);
                    ExcelWorkSheet.Cells[row, 3] = getAll.GetString(3);
                    ExcelWorkSheet.Cells[row, 4] = getAll.GetString(4);
                    ExcelWorkSheet.Cells[row, 5] = getAll.GetString(5);
                    ExcelWorkSheet.Cells[row, 6] = getAll.GetString(6);
                    ExcelWorkSheet.Cells[row, 7] = getAll.GetString(7);
                    ExcelWorkSheet.Cells[row, 8] = getAll.GetString(8);
                    ExcelWorkSheet.Cells[row, 9] = getAll.GetString(9);
                    ExcelWorkSheet.Cells[row, 10] = getAll.GetInt32(10);
                    ExcelWorkSheet.Cells[row, 11] = getAll.GetInt32(11);
                    row++;
                }
                ExcelWorkSheet.Columns.AutoFit();
                /*string isEmailSend = "";*/
                sqlConnection.Close();
                sqlConnection.Open();
                try
                {
                    ExcelWorkBook.SaveAs(fileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);

                    if (sendEmailTo != null)
                    {
                        try
                        {
                            ExcelWorkBook.SendMail(sendEmailTo, "User List From the User Management System");
                            /*isEmailSend = "Email was sent.";*/
                        }
                        catch
                        {
                            /*isEmailSend = "Email was not sent.";*/
                        }

                    }

                    if (OpenAtDownload == "true")
                    {
                        ExcelWorkBook.WebPagePreview();
                        /*System.Diagnostics.Process.Start($"E:/Documents/{fileName}.xls"); */
                    }
                    /*ExcelWorkBook.WebPagePreview();*/
                    ExcelWorkBook.Close(true, misValue, misValue);

                    ExcelExport.Quit();

                    Marshal.ReleaseComObject(ExcelWorkSheet);
                    Marshal.ReleaseComObject(ExcelWorkBook);
                    Marshal.ReleaseComObject(ExcelExport);
                    sqlCommandDel.ExecuteReader();
                    sqlConnection.Close();
                    Environment.Exit(0);
                    /*return RedirectToAction("Index", new { a = "Excel Document was created sucessfully and it should be available in your Documents folder.", color = "green", emailStatus = isEmailSend });*/
                }
                catch
                {
                    ExcelWorkBook.Close(true, misValue, misValue);

                    ExcelExport.Quit();


                    Marshal.ReleaseComObject(ExcelWorkSheet);
                    Marshal.ReleaseComObject(ExcelWorkBook);
                    Marshal.ReleaseComObject(ExcelExport);

                    /*System.Diagnostics.Debug.WriteLine("Excel Document was not created.");*/
                    sqlCommandDel.ExecuteReader();
                    sqlConnection.Close();
                    /*return RedirectToAction("Index", new { a = "Excel Document was not created.", color = "red", emailStatus = isEmailSend });*/
                    Environment.Exit(0);
                }
            }

            }

    }
}

