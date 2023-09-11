using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppinnoNew.ClassCollection
{
    public static class Excel
    {
        public static class Import
        {
            public static class PartnerLoader
            {
                public class Partner
                {
                    public long ID { get; set; }
                    public string name { get; set; }
                    public string family { get; set; }
                    public string innerTell { get; set; }
                    public string level { get; set; }
                    public string email { get; set; }
                    public string registrationmobile { get; set; }
                    public string optionalmobile { get; set; }
                }

                public static List<Partner> LoadPartnerInfo(string filePath)
                {
                    var partners = new List<Partner>();

                    var existingFile = new System.IO.FileInfo(filePath);
                    using (ExcelPackage xlPackage = new ExcelPackage(existingFile))
                    {

                        ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
                        var colCount = worksheet.Dimension.End.Column;
                        var rowCount = worksheet.Dimension.End.Row;

                        for (int iRow = 2; iRow <= rowCount; iRow++)
                        {
                            var tmp = new Partner();

                            tmp.name = worksheet.GetValue(iRow, 1).ToString();
                            tmp.family = worksheet.GetValue(iRow, 2).ToString();
                            tmp.level = worksheet.GetValue(iRow, 3).ToString();
                            tmp.innerTell = worksheet.GetValue(iRow, 4).ToString();
                            tmp.email = worksheet.GetValue(iRow, 5).ToString();
                            tmp.optionalmobile = worksheet.GetValue(iRow, 6).ToString();
                            tmp.registrationmobile = worksheet.GetValue(iRow, 7).ToString();

                            partners.Add(tmp);
                        }
                    }

                    return partners;
                }
                public static List<Partner> LoadPartnerInfo(System.IO.Stream stream)
                {
                    var partners = new List<Partner>();

                    using (ExcelPackage xlPackage = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
                        var colCount = worksheet.Dimension.End.Column;
                        var rowCount = worksheet.Dimension.End.Row;

                        for (int iRow = 2; iRow <= rowCount; iRow++)
                        {
                            var tmp = new Partner();

                            tmp.name = worksheet.GetValue(iRow, 1).ToString();
                            tmp.family = worksheet.GetValue(iRow, 2).ToString();
                            tmp.level = worksheet.GetValue(iRow, 3).ToString();
                            tmp.innerTell = worksheet.GetValue(iRow, 4).ToString();
                            tmp.email = worksheet.GetValue(iRow, 5).ToString();
                            tmp.optionalmobile = worksheet.GetValue(iRow, 6).ToString();
                            tmp.registrationmobile = worksheet.GetValue(iRow, 7).ToString();

                            partners.Add(tmp);
                        }


                    }

                    return partners;
                }
            }
            public static class ChartLoader
            {
                public class Value
                {
                    public string xtitle { get; set; }
                    public string yvalue { get; set; }
                    
                }

                public static List<Value> LoadChart(string filePath)
                {
                    var values = new List<Value>();

                    var existingFile = new System.IO.FileInfo(filePath);
                    using (ExcelPackage xlPackage = new ExcelPackage(existingFile))
                    {

                        ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
                        var colCount = worksheet.Dimension.End.Column;
                        var rowCount = worksheet.Dimension.End.Row;

                        for (int iCol = 1; iCol <= colCount; iCol++)
                        {
                            var tmp = new Value();

                            tmp.xtitle = worksheet.GetValue(1, iCol).ToString();
                            tmp.yvalue = worksheet.GetValue(2, iCol).ToString();
                            values.Add(tmp);
                        }
                    }

                    return values;
                }
                public static List<Value> LoadChart(System.IO.Stream stream)
                {
                    var values = new List<Value>();

                    using (ExcelPackage xlPackage = new ExcelPackage(stream))
                    {

                        ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
                        var colCount = worksheet.Dimension.End.Column;
                        var rowCount = worksheet.Dimension.End.Row;

                        for (int iCol = 1; iCol <= colCount; iCol++)
                        {
                            var tmp = new Value();

                            tmp.xtitle = worksheet.GetValue(1, iCol).ToString();
                            tmp.yvalue = worksheet.GetValue(2, iCol).ToString();
                            values.Add(tmp);
                        }
                    }

                    return values;
                }
            }
        }
    }
}