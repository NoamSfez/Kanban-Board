using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Column
    {
        public readonly int ColumnOrdinal;
        public readonly int TaskLimit;
        public readonly string Name;
        public readonly List<int> Tasks;

        public Column(int columnOrdinal, int taskLimit, string name, List<int> tasks)
        {
            ColumnOrdinal = columnOrdinal;
            TaskLimit = taskLimit;
            Name = name;
            Tasks = tasks;
        }

        internal Column(BusinessLayer.Column column)
        {
            ColumnOrdinal = column.ColumnOrdinal;
            TaskLimit = column.TaskLimit;
            Name = column.Name;
            Tasks = column.TaskIds;
        }
    }
}
