using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ColMarkSort.Data;

namespace ColMarkSort.Revit
{
    public static class RvtUtils
    {
        public static void SendDataToRevit(Document doc , ColumnArrayGroup columnArrayGroup)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            var columns = collector.OfCategory(BuiltInCategory.OST_StructuralColumns)
                                 .WhereElementIsNotElementType()
                                 .ToElements()
                                 .ToList();

            using (Transaction trn = new Transaction(doc, "Assgin Column Mark"))
            {
                trn.Start();

                foreach (var columnArray in columnArrayGroup.ColumnsArrays)
                {
                    foreach (var column in columnArray.ColumnList)
                    {
                        // Find the corresponding Revit column element by ID
                        Element revitColumn = columns.FirstOrDefault(c => c.Id.IntegerValue == column.ID);
                        if (revitColumn != null)
                        {
                            // Set the "COLUMN MARK" parameter
                            Parameter markParam = revitColumn.LookupParameter("COLUMN MARK");
                            if (markParam != null && markParam.IsReadOnly == false)
                            {
                                revitColumn.LookupParameter("COLUMN MARK").Set(column.ColumnMark);
                            }
                        }
                    }
                }

                trn.Commit();
            }
        }
    }
}
