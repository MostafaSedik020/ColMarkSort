using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ColMarkSort.Model.Utils;
using ColMarkSort.Model.Revit;

namespace ColMarkSort.Model.Data
{
    public static class ManageData
    {
        public static void SortColumnData(Document doc , string foundationLevel)
        {
            var columnArrayGroup = new ColumnArrayGroup();
            // Here you would typically query the document for column elements,
            // extract their properties, and populate the ColumnArrayGroup.
            // This is a placeholder for the actual implementation.

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            FilteredElementCollector collector2 = new FilteredElementCollector(doc);

            var columns = collector.OfCategory(BuiltInCategory.OST_StructuralColumns)
                                   .WhereElementIsNotElementType()
                                   .ToElements();

            var levels = collector2.OfClass(typeof(Level))
                                  .Cast<Level>()
                                  .OrderBy(level => level.Elevation)
                                  .ToList();


            List<Column> allColumns = new List<Column>();

            foreach (var column in columns)
            {
                if (string.IsNullOrEmpty(column.LookupParameter("COLUMN MARK").AsString()))
                {
                    continue; // Skip columns without the "COLUMN MARK" parameter
                }
                ElementId typeId = column.GetTypeId();
                ElementType columnType = doc.GetElement(typeId) as ElementType;

                Column col = new Column
                {

                    MarkLabel = column.LookupParameter("COLUMN MARK").AsString(),
                    ID = column.Id.IntegerValue,
                    BaseLevel = column.LookupParameter("Base Level").AsValueString(),
                    TopLevel = column.LookupParameter("Top Level").AsValueString(),
                    RebarDia = column.LookupParameter("Rebar: Diameter").AsInteger() ,
                    BarsNumber = column.LookupParameter("Rebar: No.of bars").AsInteger(),
                    Volume = UnitConverter.convertUnitsToCubicMeters(column.LookupParameter("Volume").AsDouble())
                };


                string familyName = column.LookupParameter("Family").AsValueString();
                if (familyName.Contains("RECTANGULAR_T"))
                {
                    col.Width = UnitConverter.convertUnitsToMeters(columnType.LookupParameter("b").AsDouble());
                    col.Length = UnitConverter.convertUnitsToMeters(columnType.LookupParameter("h").AsDouble());
                }
                else if (familyName.Contains("CIRCULAR_T"))
                {
                    col.Width = UnitConverter.convertUnitsToMeters(columnType.LookupParameter("b").AsDouble());
                }
                //else
                //{
                //    // Handle other types or skip
                //    continue;
                //}
                allColumns.Add(col);
            }
            

            foreach (var column in allColumns)
            {
                // Check if the column's mark already exists in the group
                var existingArray = columnArrayGroup.ColumnsArrays
                    .FirstOrDefault(ca => ca.ColumnMark == column.ColumnMark);

                if (existingArray != null)
                {
                    // If it exists, add the column to the column array group

                    
                    columnArrayGroup.ColumnsArrays.Where(ca => ca.ColumnMark == column.ColumnMark).FirstOrDefault().ColumnList.Add(column);

                }
                else
                {
                    // If it doesn't exist, create a new array and add the column to it
                    ColumnArray newArray = new ColumnArray
                    {
                        MarkLabel = column.MarkLabel,
                        MarkNumber = column.MarkNumber // Initialize with 1 or any other logic you want
                    };
                    newArray.ColumnList.Add(column);
                    columnArrayGroup.ColumnsArrays.Add(newArray);


                }
 
            }
            

            columnArrayGroup.SortArrays(levels,foundationLevel); // sort the arrays and assign MarkNumbers
            
            RvtUtils.SendDataToRevit(doc, columnArrayGroup); // Send the data to Revit
            return;
        }
    }
}
