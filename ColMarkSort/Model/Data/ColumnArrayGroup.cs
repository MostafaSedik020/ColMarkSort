using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using ColMarkSort.Model.Utils;

namespace ColMarkSort.Model.Data
{
    public  class ColumnArrayGroup
    {
        public  List<ColumnArray> ColumnsArrays { get; set; } 

        public ColumnArrayGroup()
        {
            // Initialize the ColumnsArrays list if needed
            ColumnsArrays = new List<ColumnArray>();
        }
        public  void SortArrays(List<Level> levels, string foundationLevel)
        {
            var levelElevationDict = levels.ToDictionary(lvl => lvl.Name, lvl => lvl.Elevation);
            //sort the array instances 
            foreach (var columnArray in ColumnsArrays)
            {
                
                // Sort the ColumnList within each ColumnArray by BaseLevel and TopLevel
                columnArray.ColumnList = columnArray.ColumnList.OrderBy(c => levelElevationDict[c.BaseLevel])
                                                               //.ThenBy(c => levels.FirstOrDefault(l => l.Name == c.TopLevel)?.Elevation)
                                                                .ToList();
            }
            //sort the whole group and give them MarkNumber
            ColumnsArrays = ColumnsArrays.OrderBy(ca => levelElevationDict[ca.ColumnList.FirstOrDefault()?.BaseLevel])
                                         .ThenBy(ca => ca.ColumnList.FirstOrDefault()?.Width)
                                         .ThenBy(ca => ca.ColumnList.FirstOrDefault()?.Length)
                                         .ThenBy(ca => ca.ColumnList.Select(c => c.BaseLevel).Distinct().Count())
                                         .ThenBy(ca => ca.ColumnList.FirstOrDefault()?.RebarDia)
                                         .ThenBy(ca => ca.ColumnList.FirstOrDefault()?.Volume)
                                         .ToList();

            // Assign MarkNumber to each ColumnArray based on its position in the sorted list
            int mainColumn = 1;
            int plantedColumn = 0;
            for (int i = 0; i < ColumnsArrays.Count; i++)
            {
                if(ColumnsArrays[i].ColumnList.FirstOrDefault().MarkLabel == "C7")
                {
                    MessageBox.Show("C7 found in ColumnArrayGroup.SortArrays");
                }
                //if (i == 4) // idk what this is for, but it seems like a debug line
                //{
                //    var oldList = ColumnsArrays[i].ColumnList;
                //    var newList = ColumnsArrays[i].ColumnList.GroupBy(c => new
                //    {
                //        c.Width,
                //        c.Length,
                //        c.BaseLevel,
                //        c.TopLevel,
                //        c.RebarDia,
                //        c.BarsNumber
                //    })
                //    .Select(g => g.First())
                //    .ToList();
                //}

                if (ColumnsArrays[i].ColumnList.FirstOrDefault().BaseLevel == foundationLevel)
                {
                    var previousList = new List<Column>(); // Initialize to an empty list for the first iteration
                    var currentList = ColumnsArrays[i].ColumnList.GroupBy(c => new
                    {
                        c.Width,
                        c.Length,
                        c.BaseLevel,
                        c.TopLevel,
                        c.RebarDia,
                        c.BarsNumber
                    })
                    .Select(g => g.First())
                    .ToList();

                    if( i != 0)
                    {
                         previousList = ColumnsArrays[i-1].ColumnList.GroupBy(c => new
                        {
                            c.Width,
                            c.Length,
                            c.BaseLevel,
                            c.TopLevel,
                            c.RebarDia,
                            c.BarsNumber
                        })
                         .Select(g => g.First())
                         .ToList();
                    }
                    

                    
                    if ( i != 0 && !MultiUtils.AreListsSimilar(currentList, previousList) )
                    {
                        mainColumn++; // Increment the main column number for the next ColumnArray

                    }

                    ColumnsArrays[i].MarkNumber = mainColumn; // Start numbering from 1
                    ColumnsArrays[i].MarkLabel = "C"; // Set the label for each ColumnArray
                    foreach (var column in ColumnsArrays[i].ColumnList)
                    {
                        column.MarkNumber = mainColumn; // Assign the same MarkNumber to each Column in the ColumnArray
                        column.MarkLabel = ColumnsArrays[i].MarkLabel; // Set the Mark for each Column
                         
                    }
                    
                }
                else
                {
                    if ( i != 0 && !MultiUtils.AreListsSimilar(ColumnsArrays[i].ColumnList, ColumnsArrays[i - 1].ColumnList) )
                    {
                        plantedColumn++; // Increment the planted column number for the next ColumnArray

                    }
                    ColumnsArrays[i].MarkNumber = plantedColumn; // Set MarkNumber to 0 for non-foundation levels
                    ColumnsArrays[i].MarkLabel = "PC"; // Clear the label for non-foundation levels
                    foreach (var column in ColumnsArrays[i].ColumnList)
                    {
                        column.MarkNumber = plantedColumn; // Assign 0 MarkNumber to each Column in non-foundation levels
                        column.MarkLabel = "PC"; // Clear the Mark for each Column
                        
                    }
                    
                }
                
            }
            return;
        }
    }
}
