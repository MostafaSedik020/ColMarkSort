using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColMarkSort.Data
{
    public class ColumnArray
    {
        //this clas contains group of columns share the same mark and postion
        public List<Column> ColumnList { get; set; }
        public string Label { get; set; }
        public int MarkNumber { get; set; }
        public string ColumnMark => $"{Label}{MarkNumber}";

        public ColumnArray()
        {
            ColumnList = new List<Column>();
        }
    }
}
