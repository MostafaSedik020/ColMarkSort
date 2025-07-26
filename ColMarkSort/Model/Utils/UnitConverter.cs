using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColMarkSort.Model.Utils
{
    public static class UnitConverter
    {
        /// <summary>
        /// Change unit's number in to cubic meter.
        /// </summary>
        /// <returns>converted numbers.</returns>
        public static double convertUnitsToCubicMeters(double value)
        {
            double convertedNum = UnitUtils.ConvertFromInternalUnits((double)value, UnitTypeId.CubicMeters);//REVIT2021++
            //double convertedNum = UnitUtils.ConvertFromInternalUnits((double)value, DisplayUnitType.DUT_CUBIC_METERS);
            return convertedNum;
        }
        public static double convertUnitsToSquareMeters(double value)
        {
            double convertedNum = UnitUtils.ConvertFromInternalUnits((double)value, UnitTypeId.SquareMeters);//REVIT2021++
            //double convertedNum = UnitUtils.ConvertFromInternalUnits((double)value, DisplayUnitType.DUT_SQUARE_METERS);
            return convertedNum;
        }
        public static double convertUnitsToMeters(double value)
        {
            double convertedNum = UnitUtils.ConvertFromInternalUnits((double)value, UnitTypeId.Meters);//REVIT2021++
            //double convertedNum = UnitUtils.ConvertFromInternalUnits((double)value, DisplayUnitType.DUT_METERS);
            return convertedNum;
        }
        public static double convertMiliMetersToFeet(double value)
        {
            double convertedNum = value / 304.8; // Convert millimeters to feet (1 foot = 304.8 mm)
            return convertedNum;
        }
    }
}
