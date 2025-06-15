using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ColMarkSort.Data;

namespace ColMarkSort.Entry
{
    [Transaction(TransactionMode.Manual)]
    public class ExtCmd : IExternalCommand
    {
        public static UIDocument UIDoc { get; set; }
        public static Document Doc { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDoc = commandData.Application.ActiveUIDocument;
            Doc = UIDoc.Document;

            //ExtEventHan = new ExtEventHan();
            //ExtEvent = ExternalEvent.Create(ExtEventHan);

            //MainWindow mainWindow = new MainWindow(Doc);
            //mainWindow.ShowDialog();
            //mainWindow.Show();
            ManageData.GetColumnData(Doc);

            return Result.Succeeded;
        }
    }
}
