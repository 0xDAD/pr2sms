using System;
using System.Collections.Generic;
using System.Text;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace PR2_SMS
{
    class ExcelMessageList
    {
        #region ExcelVars
        private static object vk_missing = System.Reflection.Missing.Value;

        private static object vk_visible = false;
        private static object vk_false = false;
        private static object vk_true = true;
        private bool docOpened = false;

        public bool DocOpened
        {
            get { return docOpened; }

        }

        private bool vk_app_visible = false;

        private object vk_filename=null;

        #region OPEN WORKBOOK VARIABLES
        private object vk_update_links = 0;
        private object vk_read_only = vk_true;
        private object vk_format = 1;
        private object vk_password = vk_missing;
        private object vk_write_res_password = vk_missing;
        private object vk_ignore_read_only_recommend = vk_true;
        private object vk_origin = vk_missing;
        private object vk_delimiter = vk_missing;
        private object vk_editable = vk_false;
        private object vk_notify = vk_false;
        private object vk_converter = vk_missing;
        private object vk_add_to_mru = vk_false;
        private object vk_local = vk_false;
        private object vk_corrupt_load = vk_false;
        #endregion

        #region CLOSE WORKBOOK VARIABLES
        private object vk_save_changes = vk_false;
        private object vk_route_workbook = vk_false;
        #endregion
        #endregion
        private Excel.Application excelApp = null;
        private Excel.Workbook excelWorkbook = null;
        private Excel.Worksheet excelWorksheet = null;

        public ExcelMessageList(string fileName)
        {
            OpenDocument(fileName);
        }

        public bool OpenDocument(string fileName)
        {
            CloseDocument();
            excelApp = new Excel.ApplicationClass();

            if (excelApp == null) return false;
            excelApp.Visible = vk_app_visible;

            this.excelWorkbook = this.excelApp.Workbooks.Open(
                fileName, vk_update_links, vk_read_only,
                vk_format, vk_password,
                vk_write_res_password,
                vk_ignore_read_only_recommend, vk_origin,
                vk_delimiter, vk_editable, vk_notify,
                vk_converter, vk_add_to_mru,
                vk_local, vk_corrupt_load);

            excelWorksheet = (Excel.Worksheet)excelWorkbook.Worksheets[1];

            docOpened = excelWorkbook != null;
            return docOpened;
        }
        public void CloseDocument()
        {
            if (docOpened)
            {
                excelWorkbook.Close(vk_save_changes, vk_filename, vk_route_workbook);                
                KillExcel();
                docOpened = false;
            }
        }
        public void KillExcel()
        {
            if (this.excelApp != null)
            {
                Process[] pProcess;
                pProcess = System.Diagnostics.Process.GetProcessesByName("Excel");
                pProcess[0].Kill();
            }
        }
        public string[] GetRange(string range)
        {
            Excel.Range workingRangeCells =
              excelWorksheet.get_Range(range, Type.Missing);

            System.Array array = (System.Array)workingRangeCells.Cells.Value2;
            string[] arrayS = this.ConvertToStringArray(array);

            return arrayS;
        }

        public string[] GetRange(int r1,int c1,int r2,int c2)
        {
            Excel.Range workingRangeCells =
              excelWorksheet.get_Range(((Excel.Range)excelWorksheet.Cells[r1,c1]),((Excel.Range)excelWorksheet.Cells[r2,c2]));

            System.Array array = (System.Array)workingRangeCells.Cells.Value2;
            string[] arrayS = this.ConvertToStringArray(array);

            return arrayS;
        }

        internal string GetSingleValue(int row, int col)
        {
            string val = null;
            if (!docOpened) return null;
            Excel.Range range= (Excel.Range)excelWorksheet.Cells.get_Item(row, col);
            if (range.Value2 != null)
                val = range.Value2.ToString();
            else
                val = String.Empty;
            return val;
        }
        #region CONVERT TO STRING ARRAY
        private string[] ConvertToStringArray(System.Array values)
        {
            string[] newArray = new string[values.Length];

            int index = 0;
            for (int i = values.GetLowerBound(0);
                  i <= values.GetUpperBound(0); i++)
            {
                for (int j = values.GetLowerBound(1);
                          j <= values.GetUpperBound(1); j++)
                {
                    if (values.GetValue(i, j) == null)
                    {
                        newArray[index] = "";
                    }
                    else
                    {
                        newArray[index] = (string)values.GetValue(i, j).ToString();
                    }
                    index++;
                }
            }
            return newArray;
        }
        #endregion

    }
}
