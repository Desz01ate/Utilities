using NUnit.Framework;
using System;
using System.Text;
using Utilities.Testing.Models;
using Utilities.Shared;
using System.Linq;
using System.Data;
using Utilities.Interfaces;

namespace Utilities.Testing
{
    class UnrelatedTest
    {

        [Test]
        public void Playground()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //var data = Utilities.File.ReadExcelAs<iris>(@"C:\Users\TYCHE\Desktop\iris.xlsx", table: "Sheet1");
            var dataTable = File.ReadExcelAsDataTable(@"C:\Users\TYCHE\Desktop\sheet.xlsx", configuration: new ExcelDataReader.ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (tr) => new ExcelDataReader.ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true,
                }
            });
            dataTable.Columns["MATERIAL TYPE"].ColumnName = nameof(ClothesModel.MATERIAL_TYPE);
            dataTable.Columns["MATERIAL DESCRIPTION"].ColumnName = nameof(ClothesModel.MATERIAL_DESCRIPTION);
            var dtList = dataTable.ToEnumerable<ClothesModel>();
            foreach (DataColumn column in dataTable.Columns)
            {
                Console.WriteLine(column.ColumnName);
            }
            //var dataDynamic = File.ReadExcel(@"C:\Users\TYCHE\Desktop\iris.xlsx", hasHeader: false);
            //Console.WriteLine(data);
        }

        private ClothesModel CustomBuilder(DataTableReader arg)
        {
            var clothes = new ClothesModel();
            clothes.MATERIAL = double.Parse(arg[0].ToString());
            clothes.MATERIAL_TYPE = arg[1].ToString();
            clothes.MATERIAL_DESCRIPTION = arg[2].ToString();
            return clothes;
        }

        public class ClothesModel : IExcelReader, IComparable<ClothesModel>
        {
            public double MATERIAL { get; set; }
            public string MATERIAL_TYPE { get; set; }
            public string MATERIAL_DESCRIPTION { get; set; }
            private int GetSizePriority()
            {
                //Order by length of size to prevent 'early match'
                if (MATERIAL_DESCRIPTION.EndsWith("Generic"))
                {
                    return 0;
                }
                else if (MATERIAL_DESCRIPTION.EndsWith("XXXL"))
                {
                    return 8;
                }
                else if (MATERIAL_DESCRIPTION.EndsWith("XXL"))
                {
                    return 7;
                }
                else if (MATERIAL_DESCRIPTION.EndsWith("XL"))
                {
                    return 6;
                }
                else if (MATERIAL_DESCRIPTION.EndsWith("XS"))
                {
                    return 1;
                }
                else if (MATERIAL_DESCRIPTION.EndsWith("X"))
                {
                    return 5;
                }
                else if (MATERIAL_DESCRIPTION.EndsWith("L"))
                {
                    return 4;
                }
                else if (MATERIAL_DESCRIPTION.EndsWith("M"))
                {
                    return 3;
                }
                else if (MATERIAL_DESCRIPTION.EndsWith("S"))
                {
                    return 2;
                }
                else
                {
                    return 999;
                }
            }
            public int CompareTo(ClothesModel other)
            {
                if (other == null) return 999;
                return this.GetSizePriority().CompareTo(other.GetSizePriority());
            }

            public int GetExternalColumnIndex(string property)
            {
                switch (property)
                {
                    case nameof(MATERIAL):
                        return 0;
                    case nameof(MATERIAL_TYPE):
                        return 1;
                    case nameof(MATERIAL_DESCRIPTION):
                        return 2;
                    default:
                        return -1;
                }
            }
            public override string ToString()
            {
                var props = this.GetType().GetProperties();
                return string.Join(" ", props.Select(x => x.GetValue(this).ToString().PadRight(10)));
            }
        }

    }

}
