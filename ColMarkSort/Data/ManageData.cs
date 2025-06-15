using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ColMarkSort.Utils;

namespace ColMarkSort.Data
{
    public class ManageData
    {
        public static ColumnArrayGroup GetColumnData(Document doc)
        {
            ColumnArrayGroup columnArrayGroup = new ColumnArrayGroup();
            // Here you would typically query the document for column elements,
            // extract their properties, and populate the ColumnArrayGroup.
            // This is a placeholder for the actual implementation.
            
            FilteredElementCollector collector = new FilteredElementCollector(doc);


            var columns = collector.OfCategory(BuiltInCategory.OST_StructuralColumns)
                                   .WhereElementIsNotElementType()
                                   .ToElements();
   

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
                    
                    Mark = column.LookupParameter("COLUMN MARK").AsString(),
                    ID = column.Id.IntegerValue,
                    BaseLevel = column.LookupParameter("Base Level").AsValueString(),
                    TopLevel = column.LookupParameter("Top Level").AsValueString(),
                    RebarDia = column.LookupParameter("Rebar: Diameter").AsInteger(),
                    BarsNumber = column.LookupParameter("Rebar: No.of bars").AsInteger(),
                    Volume =UnitConverter.convertUnitsToCubicMeters(column.LookupParameter("Volume").AsDouble())
                };
                
                
                string familyName = column.LookupParameter("Family").AsValueString();
                if (familyName.Contains("RECTANGULAR_T"))
                {
                    col.Width =UnitConverter.convertUnitsToMeters(columnType.LookupParameter("b").AsDouble());
                    col.Length =UnitConverter.convertUnitsToMeters(columnType.LookupParameter("h").AsDouble());
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



            return columnArrayGroup;
        }
    }
}
