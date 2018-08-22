using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http.Headers;

namespace CsnowFramework.Database
{
    /// <summary>
    /// Base class for query execution.
    /// Each sequential query class should inherit it and use its internal facilities.
    /// </summary>
    public abstract class QueryBase
    {
        // Private static members
        public static string DefaultConnectionString { get; set; }
        public static Dictionary<string, string> AlternativeConnectionString { get; set; } = new Dictionary<string, string>();

        #region Constructor

        protected QueryBase(string connectionString = null)
        {
            ConnectionString = !string.IsNullOrEmpty(connectionString) ? connectionString : DefaultConnectionString;
        }

        public string ConnectionString { private get; set; }

        #endregion


        #region Public generic functions

        /// <summary>
        /// Inserts an object into Database
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="objectForInsertion">The object itself</param>
        /// <returns>True if insertion was successful, false otherwise</returns>
        public bool AddObject<T>(ref T objectForInsertion) where T : class, new()
        {
            string sSql = "INSERT INTO ";
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sSql += GetTableName(objectForInsertion.GetType());
            sSql += GetColumnNamesForInsertion(ref objectForInsertion, ref sqlParams);
            sSql += " SELECT @@IDENTITY";
            int? insertedUId = ExecScalarSelectQuery(sSql, sqlParams, 3);
            if (insertedUId != null)
            {
                SetPrimaryKey((int)insertedUId, ref objectForInsertion);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Inserts a unique object into Database, if such object already exists, then returns 
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="objectForInsertion">The object itself</param>
        /// <returns>True if insertion and selection was successful, false otherwise</returns>
        public bool AddUniqueObject<T>(ref T objectForInsertion) where T : class, new()
        {
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();

            StringBuilder sSql = new StringBuilder(4096);
            sSql.Append("IF NOT EXISTS ");
            sSql.Append("(");
            sSql.Append("SELECT ");
            sSql.Append(GetPrimaryKeyColumnName(typeof(T)));
            sSql.Append(" FROM ");
            sSql.Append(GetTableName(typeof(T)));
            sSql.Append(" WHERE ");
            sSql.Append(GetWhereClauseForSelect(ref objectForInsertion, ref sqlParams));
            sSql.Append(" ) ");
            sSql.Append(" BEGIN ");
            sSql.Append(" INSERT INTO ");
            sSql.Append(GetTableName(typeof(T)));
            sSql.Append(GetColumnNamesForInsertion(ref objectForInsertion, ref sqlParams));
            sSql.Append(" SELECT @@IDENTITY");
            sSql.Append(" END ");
            sSql.Append(" ELSE ");
            sSql.Append(" BEGIN ");
            sSql.Append(" SELECT ");
            sSql.Append(GetPrimaryKeyColumnName(typeof(T)));
            sSql.Append(" FROM ");
            sSql.Append(GetTableName(typeof(T)));
            sSql.Append(" WHERE ");
            sSql.Append(GetWhereClauseForSelect(ref objectForInsertion, ref sqlParams));
            sSql.Append(" END ");
            int? insertedUId = ExecScalarSelectQuery(sSql.ToString(), sqlParams, 1);
            if (insertedUId != null)
            {
                SetPrimaryKey((int)insertedUId, ref objectForInsertion);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Updates an object with values 
        /// </summary>
        /// <typeparam name="T">Object type, must be a class</typeparam>
        /// <param name="objectToUpdate">Object to get updated</param>
        /// <returns>True of the object got updated, False otherwise.</returns>
        public bool UpdateObject<T>(ref T objectToUpdate) where T : class, new()
        {
            string sSql = "UPDATE ";
            sSql += GetTableName(objectToUpdate.GetType());
            sSql += " SET ";

            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sSql += GetColumnNamesToUpdate(ref objectToUpdate, ref sqlParams);
            int noOfUpdatedItems = ExecNonQuery(sSql, sqlParams);
            return noOfUpdatedItems == 1;
        }

        #endregion


        #region Protected functions - Selecting Query handling

        protected List<DataObject> ExecSelectQuery<DataObject>(
                    string query, 
                    Dictionary<string, Object> parameters = null) where DataObject : class, new()
        {
            int backOffCount = 1;
retry:
            Dictionary<PropertyInfo, QueryColumnAttribute> mappedProperties = new Dictionary<PropertyInfo, QueryColumnAttribute>();
            foreach (PropertyInfo property in typeof(DataObject).GetProperties())
            {
                QueryColumnAttribute attr = GetColumnAttribute(property);
                if (attr != null)
                {
                    mappedProperties.Add(property, attr);
                }
            }

            if (mappedProperties.Count == 0)
                throw new Exception("Invalid structure type provided.");
            using (SqlConnection sqlConnection = GetCurrentSqlConnection())
            {
                if (sqlConnection != null)
                {
                    SqlCommand selectQuery = new SqlCommand(query, sqlConnection);
                    selectQuery.CommandTimeout = 0;
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, Object> parameter in parameters)
                        {
                            selectQuery.Parameters.AddWithValue(parameter.Key, parameter.Value);
                        }
                    }
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(selectQuery);
                    DataTable queryResultTable = new DataTable();
                    try
                    {
                        dataAdapter.Fill(queryResultTable);
                        List<DataObject> listResult = new List<DataObject>();
                        foreach (DataRow dataRow in queryResultTable.Rows)
                        {
                            DataObject dataObject = new DataObject();
                            foreach (KeyValuePair<PropertyInfo, QueryColumnAttribute> property in mappedProperties)
                            {
                                if (queryResultTable.Columns.Contains(property.Value.ColumnName))
                                {
                                    if (DBNull.Value.Equals(dataRow[property.Value.ColumnName]) &&
                                        property.Value.IsNullable)
                                    {
                                        // We can safely skip this mapping since column is nulable
                                        continue;
                                    }
                                    property.Key.SetValue(dataObject, dataRow[property.Value.ColumnName]);
                                }
                            }
                            listResult.Add(dataObject);
                        }
                        return listResult;
                    }
                    catch (SqlException sqlException)
                    {
                        if (ShouldRetry(sqlException.Number, ref backOffCount))
                            goto retry;
                    }
                }
            }
            throw new Exception("Cannot connect to DB.");
        }

        protected int? ExecScalarSelectQuery(
                    string query, 
                    Dictionary<string, object> parameters = null,
                    int forceRepeatMax = 0)
        {
            int backOffCount = 1;
            int forceRepeatCount = 0;
retry:
            using (SqlConnection sqlConnection = GetCurrentSqlConnection())
            {
                if (sqlConnection != null)
                {
                    SqlCommand selectQuery = new SqlCommand(query, sqlConnection);
                    var queryWithNulls = query;
                    selectQuery.CommandTimeout = 0;
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                        {
                            if (parameter.Value == null)
                            {
                                queryWithNulls = queryWithNulls.Replace(parameter.Key, "NULL");
                            }
                            else
                            {
                                selectQuery.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }
                        }
                    }
                    selectQuery.CommandText = queryWithNulls;
                    object retVal = null;
                    try
                    {
                        retVal = selectQuery.ExecuteScalar();
                    }
                    catch (SqlException sqlException)
                    {
                        if (ShouldRetry(sqlException.Number, ref backOffCount))
                            goto retry;
                        if (backOffCount < forceRepeatMax)
                            goto retry;
                    }
                    catch (Exception exception)
                    {
                    }
                    if (!(retVal is decimal) && !(retVal is int)) return null;
                    int? intRetVal = Convert.ToInt32(retVal);
                    return intRetVal;
                }
            }
            return null;
        }

        protected int ExecNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            int backOffCount = 1;
retry:

            using (SqlConnection sqlConnection = GetCurrentSqlConnection())
            {
                if (sqlConnection != null)
                {
                    SqlCommand selectQuery = new SqlCommand(query, sqlConnection);
                    selectQuery.CommandTimeout = 0;
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                        {
                            selectQuery.Parameters.AddWithValue(parameter.Key, parameter.Value);
                        }
                    }
                    int retVal = -1;
                    try
                    {
                        retVal = selectQuery.ExecuteNonQuery();
                        return retVal;
                    }
                    catch (SqlException sqlException)
                    {
                        if (ShouldRetry(sqlException.Number, ref backOffCount))
                            goto retry;
                        throw;
                    }
                }
            }
            return -1;
        }

        // Checks if we should retry or not.
        private bool ShouldRetry(int errorNumber, ref int backoffCount)
        {
            // Fail after 100 attempts.
            if (backoffCount > 100)
                return false;
            // Sleep based on error No
            switch (errorNumber)
            {
                case 1205:
                case 9001:
                    Thread.Sleep(100 * backoffCount);
                    backoffCount++;
                    return true;
                case 2601:
                    // Same key insertion
                    backoffCount++;
                    return true;
                case 2627:
                    // UQ violation
                    backoffCount++;
                    return true;
                case 8642:
                    Thread.Sleep(100 * backoffCount);
                    backoffCount++;
                    return true;
                default:
                    return false;
            }
        }

        #endregion


        #region Private helper functions to handle Query attributes

        /// <summary>
        /// Gets a table name for a type
        /// </summary>
        /// <param name="type">Type a table is associated with</param>
        /// <returns>Table name string</returns>
        private static string GetTableName(Type type)
        {
            object[] tableAttrs = type.GetTypeInfo().GetCustomAttributes(typeof(QueryTableAttribute), false);
            if (tableAttrs.Length == 0)
                throw new Exception("QueryTable Attribute is missing. Cannot generate INSERT statement");
            QueryTableAttribute tableAttr = tableAttrs[0] as QueryTableAttribute;
            if (tableAttr == null)
                throw new Exception("Cannot obtain QueryTable Attribute. Cannot generate INSERT statement");
            return tableAttr.TableName;
        }

        private string GetPrimaryKeyColumnName(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                QueryColumnAttribute attr = GetColumnAttribute(property);
                if (attr != null && attr.IsPrimaryKey)
                    return attr.ColumnName;
            }
            throw new Exception("Cannot find primary key");
        }

        private string GetColumnNamesForInsertion<T>(ref T objectToInsert, ref Dictionary<string, object> sqlParams, bool forInsertQuery = true)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            string sColumnAndParams = "(";
            string sValueParameters = "(";
            foreach (PropertyInfo property in properties)
            {
                QueryColumnAttribute attr = GetColumnAttribute(property);
                if (attr != null)
                {
                    object value = property.GetValue(objectToInsert);
                    // Skip PKs for INSERT queries
                    if (forInsertQuery && attr.IsPrimaryKey)
                        continue;
                    if ((value == null) && !attr.IsNullable)
                        throw new Exception($"Value for {attr.ColumnName} column is not provided and the column does not allow NULL values.");
                    if (value != null)
                    {
                        sColumnAndParams += attr.ColumnName + ",";
                        sValueParameters += "@" + attr.ColumnName + ",";
                        if (!sqlParams.ContainsKey("@" + attr.ColumnName))
                        {
                            sqlParams.Add("@" + attr.ColumnName, value);
                        }
                    }
                }
            }
            sColumnAndParams = sColumnAndParams.TrimEnd(',') + ")";
            sColumnAndParams += " VALUES " + sValueParameters.TrimEnd(',') + ")";
            return sColumnAndParams;
        }

        private string GetWhereClauseForSelect<T>(ref T objectToInsert, ref Dictionary<string, object> sqlParams)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            string sWhereClause = "";
            foreach (PropertyInfo property in properties)
            {
                QueryColumnAttribute attr = GetColumnAttribute(property);
                if (attr != null)
                {
                    object value = property.GetValue(objectToInsert);
                    // Skip PKs for INSERT queries
                    if (attr.IsPrimaryKey)
                        continue;
                    if (value != null)
                    {
                        if (sWhereClause.Length > 0)
                        {
                            sWhereClause += " AND ";
                        }
                        sWhereClause += attr.ColumnName + " = @" + attr.ColumnName;
                        if (!sqlParams.ContainsKey("@" + attr.ColumnName))
                        {
                            sqlParams.Add("@" + attr.ColumnName, value);
                        }
                    }
                }
            }
            return sWhereClause;
        }
        private string GetColumnNamesToUpdate<T>(ref T objectToUpdate, ref Dictionary<string, object> sqlParams)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            string sColumnAndParams = "";
            QueryColumnAttribute pkAttribute = null;
            int pkValue = 0;
            foreach (PropertyInfo property in properties)
            {
                QueryColumnAttribute attr = GetColumnAttribute(property);
                if (attr != null)
                {
                    object value = property.GetValue(objectToUpdate);
                    // Skip PKs for To UPDATE columns, also ignore anything null
                    // TODO(ildarm): Enable setting value to NULL
                    if (attr.IsPrimaryKey && value != null)
                    {
                        // Save the PK
                        pkAttribute = attr;
                        pkValue = Convert.ToInt32(value);
                    }
                    else if (attr.IsNullable && value == null)
                    {
                        if (sColumnAndParams.Length > 0)
                            sColumnAndParams += ", ";
                        sColumnAndParams += attr.ColumnName + " = NULL";
                    }
                    else if (value != null && !attr.IsPrimaryKey)
                    {
                        if (sColumnAndParams.Length > 0)
                            sColumnAndParams += ", ";
                        sColumnAndParams += attr.ColumnName + " = @" + attr.ColumnName;
                        sqlParams.Add("@" + attr.ColumnName, value);
                    }
                }
            }
            // Add the where clause based on PK
            if (pkAttribute == null)
                throw new Exception("Cannot run UPDATE query without PK provided.");
            sColumnAndParams += " WHERE " + pkAttribute.ColumnName + " = @" + pkAttribute.ColumnName;
            sqlParams.Add("@" + pkAttribute.ColumnName, pkValue);

            return sColumnAndParams;
        }

        private void SetPrimaryKey<T>(int UId, ref T insertedObject)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo pkProp = null;
            foreach (PropertyInfo property in properties)
            {
                QueryColumnAttribute attr = GetColumnAttribute(property);
                if (attr.IsPrimaryKey)
                {
                    pkProp = property;
                    break;
                }
            }
            if (pkProp == null)
                throw new Exception(string.Format("Cannot find PK in {0}.", typeof(T).ToString()));
            pkProp.SetValue(insertedObject, UId);
        }

        private QueryColumnAttribute GetColumnAttribute(PropertyInfo propInfo)
        {
            object[] attrbiutes = propInfo.GetCustomAttributes(typeof(QueryColumnAttribute), false);
            if (attrbiutes.Length == 1)
            {
                return attrbiutes[0] as QueryColumnAttribute;
            }
            return null;
        }

        #endregion


        #region Sql Connection handling

        // Open SQL connection if we need or bail
        private SqlConnection GetCurrentSqlConnection()
        {
            var sqlConnection = new SqlConnection(ConnectionString);

            try
            {
                sqlConnection.Open();
            }
            catch
            {
                sqlConnection.Dispose();
                sqlConnection = null;
            }

            return sqlConnection;
        }

        #endregion


        #region Delayed insertion

        protected void InitCurrentMaxUId(string tableName, string columnName, ref int value, ref bool initialized, ref object lockObj)
        {
            lock (lockObj)
            {
                if (!initialized)
                {
                    initialized = true;
                    var query = $" SELECT COUNT({columnName}) FROM {tableName}";
                    value = ExecScalarSelectQuery(query).Value;
                    if (value != 0)
                    {
                        query = $" SELECT MAX({columnName}) FROM {tableName}";
                        value = ExecScalarSelectQuery(query).Value;
                    }

                }
            }
        }

        #endregion
    }
}

