using ColMarkSort.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColMarkSort.Utils
{
    public static class MultiUtils
    {
        public static bool AreListsSimilar(List<Column> list1, List<Column> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].IsSimilar(list2[i]))
                    return false;
            }

            return true;
        }
    }
}
