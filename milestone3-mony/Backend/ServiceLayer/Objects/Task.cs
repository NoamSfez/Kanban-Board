using System;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Task
    {
        public readonly int Id;
        public readonly DateTime CreationTime;
        public readonly string Title;
        public readonly string Description;
        public readonly DateTime DueDate;
        public readonly string emailAssignee;
        internal Task(int id, DateTime creationTime, string title, string description, DateTime DueDate, string emailAssignee)
        {
            this.Id = id;
            this.CreationTime = creationTime;
            this.Title = title;
            this.Description = description;
            this.DueDate = DueDate;
            this.emailAssignee = emailAssignee;
        }

        internal Task(BusinessLayer.Task task)
        {
            this.Id = task.Id;
            this.CreationTime = task.CreationTime;
            this.Title = task.Title;
            this.Description = task.Description;
            this.DueDate = task.DueDate;
            this.emailAssignee = task.EmailAssignee;
        }
    }
}
