using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ColMarkSort.Model.Data;
using ColMarkSort.View.UI;

namespace ColMarkSort.Model.Entry
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

            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            var levels = collector.OfClass(typeof(Level))
                                  .Cast<Level>()
                                  .OrderBy(level => level.Elevation)
                                  .Select(level => level.Name)
                                  .ToArray();

            MainWindow mainWindow = new MainWindow(Doc,levels);
            mainWindow.ShowDialog();
            //mainWindow.Show();
            

            return Result.Succeeded;
        }
    }
}
