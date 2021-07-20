using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using log4net;
using log4net.Config;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal abstract class DalController
    {
        protected readonly string _connectionString;
        protected readonly string _tableName;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        ///<summary>Constructor for the DalController.</summary>
        ///<param name="tableName">the name of the table.</param>
        public DalController(string tableName)
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"kanban.db"));
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = tableName;
            configLog();
        }

        private void configLog()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        ///<summary>This method inserts this DTO into the data. 
        /// </summary>
        ///<param name="DTOObj">the DTO object.</param>
        /// <returns> bool </returns>
        public bool Insert(DTO DTOObj)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    MakeInsertCommand(DTOObj, command);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                    log.Info($"The {DTOObj.GetIdentity()} was inserted into the database");
                }
                catch
                {
                    log.Error($"The {DTOObj.GetIdentity()} can't be inserted into the database");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }
                return res > 0;
            }
        }

        protected abstract void MakeInsertCommand(DTO DTOObj, SQLiteCommand command);

        ///<summary>This method updates this DTO in the data. 
        /// </summary>
        ///<param name="DTOObj">the DTO object.</param>
        ///<param name="attributeName">the attribute name.</param>
        ///<param name="attributeValue">the attribute value.</param>
        /// <returns> bool </returns>
        public bool Update(DTO DTOObj, string attributeName, object attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where {SelectString(DTOObj)}"
                };
                try
                {

                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    AddParameters(DTOObj, command);  // Adds primary key parameters
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    log.Info($"The {attributeName}'s {DTOObj.GetIdentity()} was updated in {attributeValue.ToString()} on the database");

                }
                catch
                {
                    log.Error($"The {attributeName}'s {DTOObj.GetIdentity()} can't be updated in {attributeValue.ToString()} on the database");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

        ///<summary>This method selects DTO from the data.
        /// <returns> List<DTO> </returns>
        protected List<DTO> Select()
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName};";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));

                    }
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    command.Dispose();
                    connection.Close();
                }

            }
            return results;
        }

        ///<summary>This method selects all DTOObj.
        /// </summary>
        ///<param name="DTOObj">The DTO From which the information is taken.</param>
        /// <returns> List<DTO> </returns>
        protected List<DTO> Select(DTO DTOObj)
        {
            List<DTO> results = new List<DTO>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"select * from {_tableName} where {SelectString(DTOObj)}"
                };
                SQLiteDataReader dataReader = null;
                try
                {
                    AddParameters(DTOObj, command);  // Adds primary key parameters
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));

                    }
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return results;
        }

        protected abstract DTO ConvertReaderToObject(SQLiteDataReader reader);

        /// <summary>
        /// Gets a string that allows parameterization to identify this specific DTO in it's table.
        /// Examples:   ID=@ID
        ///             creatorEmail=@creatorEmail AND boardName=@boardName
        /// </summary>
        /// <returns> a string that allows parameterization </returns>
        private string SelectString(DTO DTOObj)
        {
            string selectString = DTOObj.PrimaryKeyColumnNames[0] + "=@" + DTOObj.PrimaryKeyColumnNames[0];
            for (int i = 1; i < DTOObj.PrimaryKeyColumnNames.Length; i++)
            {
                selectString = selectString + " AND " + DTOObj.PrimaryKeyColumnNames[i] + "=@" + DTOObj.PrimaryKeyColumnNames[i];
            }
            return selectString;
        }

        ///<summary>This method add parameters in this DTO in the data.
        /// </summary>
        ///<param name="DTOObj">the DTO that are added parameters to.</param>
        ///<param name="command">the data.</param>
        private void AddParameters(DTO DTOObj, SQLiteCommand command)
        {
            for (int i = 0; i < DTOObj.PrimaryKeyColumnNames.Length; i++)
            {
                command.Parameters.Add(new SQLiteParameter(DTOObj.PrimaryKeyColumnNames[i], DTOObj.PrimaryKey[i]));
            }
        }

        ///<summary>This method deletes this DTO in data.
        /// </summary>
        ///<param name="DTOObj">the DTO that are deleted from the data.</param>
        /// <returns> bool </returns>
        public bool Delete(DTO DTOObj)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where {SelectString(DTOObj)}"
                };
                try
                {
                    AddParameters(DTOObj,command);  // Adds primary key parameters
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    log.Error($"The {DTOObj.GetIdentity()} was deleted on the database");

                }
                catch
                {
                    log.Error($"The {DTOObj.GetIdentity()} can't be deleted on the database");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

    }
}
