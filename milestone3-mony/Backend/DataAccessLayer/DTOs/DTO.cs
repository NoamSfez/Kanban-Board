using System;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal abstract class DTO
    {
        protected bool persisted = false;
        public string[] PrimaryKeyColumnNames { get; set; }
        protected DalController _controller;
        public string[] PrimaryKey { get; set; } = { "-1" };

        ///<summary>Constructor for the DTO.</summary>
        ///<param name="controller">the dal controller.</param
        protected DTO(DalController controller)
        {
            _controller = controller;
        }

        ///<summary>This method inserts this DTO into the data. 
        public bool Insert()
        {
            persisted = true;
            return _controller.Insert(this);
        }

        ///<summary>This method deletes this DTO from the data. 
        public bool Delete()
        {
            persisted = false;
            return _controller.Delete(this);
        }

        internal abstract string GetIdentity();
    }
}
