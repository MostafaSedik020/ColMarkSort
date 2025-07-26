using Autodesk.Revit.DB;
using ColMarkSort.Model.Data;
using ColMarkSort.Model.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ColMarkSort.Model.Revit
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
                        //Element revitColumn = columns.FirstOrDefault(c => c.LookupParameter("ETABS Unique Name").ToString() == column.UniqueName);
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
        public static void SendEtabsDataToRevit(Document doc, ColumnArrayGroup columnArrayGroup)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            var columns = collector.OfCategory(BuiltInCategory.OST_StructuralColumns)
                                 .WhereElementIsNotElementType()
                                 .ToElements()
                                 .ToList();

            using (Transaction trn = new Transaction(doc, "Assgin ETABS Data"))
            {
                trn.Start();

                foreach (var columnArray in columnArrayGroup.ColumnsArrays)
                {
                    foreach (var column in columnArray.ColumnList)
                    {
                        // Find the corresponding Revit column element by ID
                        Element revitColumn = columns.FirstOrDefault(c => c.LookupParameter("ETABS Unique Name").AsString() == column.UniqueName);

                        if (revitColumn != null)
                        {
                            FilteredElementCollector columnTypeFilter = new FilteredElementCollector(doc);

                            // Get the column types names 
                            List<string> columnTypesNames = columnTypeFilter.OfCategory(BuiltInCategory.OST_StructuralColumns)
                                              .WhereElementIsElementType().Cast<ElementType>()
                                              .Select(x => x.Name)
                                              .ToList();

                            

                            changeColumnType(column,doc,columnTypesNames,columnTypeFilter,revitColumn);


                            // Set the "COLUMN MARK" parameter
                            Parameter markParam = revitColumn.LookupParameter("COLUMN MARK");
                            if (markParam != null && markParam.IsReadOnly == false)
                            {
                                revitColumn.LookupParameter("COLUMN MARK").Set(columnArray.ColumnMark); // we use column array mark here cuz ETABS code store mark in array and not in column
                                revitColumn.LookupParameter("Rebar: Diameter").Set(column.RebarDia);
                                revitColumn.LookupParameter("Rebar: No.of bars").Set(column.BarsNumber);

                            }


                        }
                    }
                }

                trn.Commit();
            }
        }

        public static void changeColumnType(Column column,Document doc, List<string> columnTypesNames,
                                            FilteredElementCollector columnTypeFilter , Element revitColumn)
        {
            if (column.IsRectangle)
            {
                //get one family type to duplicate
                FamilySymbol columnSymbols = new FilteredElementCollector(doc)
                                  .OfClass(typeof(FamilySymbol))
                                  .Cast<FamilySymbol>()
                                  .Where(x => x.Family.Name.Contains("RECTANGULAR_"))
                                  .FirstOrDefault();
                var columnSymbolsNames = new FilteredElementCollector(doc)
                                  .OfClass(typeof(FamilySymbol))
                                  .Cast<FamilySymbol>()
                                  .Select(x => x.Family.Name)
                                  .ToList();
                ElementType chosenType = null;
                string duplicateName = $"C{column.Width}X{column.Length}";
                if (!columnTypesNames.Contains(duplicateName))
                {
                    //if the type does not exist, we duplicate it
                    chosenType = columnSymbols.Duplicate(duplicateName);
                    chosenType.LookupParameter("h").Set(UnitConverter.convertMiliMetersToFeet(column.Length));
                    chosenType.LookupParameter("b").Set(UnitConverter.convertMiliMetersToFeet(column.Width));
                }
                else
                {
                    //if the type already exists, we just get it
                    chosenType = columnTypeFilter.OfCategory(BuiltInCategory.OST_StructuralColumns)
                                  .WhereElementIsElementType()
                                  .Cast<ElementType>()
                                  .FirstOrDefault(x => x.Name.Equals(duplicateName));
                }
                revitColumn.ChangeTypeId(chosenType.Id);//change the column type to the new one
            }
            else if (column.IsCircular)
            {
                //get one family type to duplicate
                FamilySymbol columnSymbols = new FilteredElementCollector(doc)
                                  .OfClass(typeof(FamilySymbol))
                                  .Cast<FamilySymbol>()
                                  .Where(x => x.Family.Name.Contains("CIRCULAR_"))
                                  .FirstOrDefault();
                ElementType chosenType = null;
                string duplicateName = $"C{column.Length}";
                if (!columnTypesNames.Contains(duplicateName))
                {
                    chosenType = columnSymbols.Duplicate(duplicateName);
                    chosenType.LookupParameter("b").Set(UnitConverter.convertMiliMetersToFeet(column.Length));

                }
                else
                {
                    chosenType = columnTypeFilter.OfCategory(BuiltInCategory.OST_StructuralColumns)
                                  .WhereElementIsElementType()
                                  .Cast<ElementType>()
                                  .FirstOrDefault(x => x.Name.Equals(duplicateName));
                }
                revitColumn.ChangeTypeId(chosenType.Id);//change the column type to the new one
            }
        }
    }
}
