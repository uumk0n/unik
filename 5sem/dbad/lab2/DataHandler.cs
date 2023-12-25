using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using lab2;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DataHandler
{
    private string connectionString;
    private List<RelationInfo> tableRelations; // Added field to store relationships

    public DataHandler(string databasePath)
    {
        connectionString = $"Data Source={databasePath}";
        tableRelations = GetSampleTableRelations(); // Initialize with sample relationships
    }


    public bool IsDatabaseEmpty()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                // Выбираем количество записей в каждой таблице и суммируем их
                command.CommandText = "SELECT SUM(cnt) FROM (SELECT COUNT(*) AS cnt FROM Building UNION SELECT COUNT(*) AS cnt FROM Room UNION SELECT COUNT(*) AS cnt FROM Renter UNION SELECT COUNT(*) AS cnt FROM Rent)";

                var result = command.ExecuteScalar();

                if (result is long count && count == 0)
                {
                    return true; // База данных пуста
                }
                else
                {
                    return false; // База данных содержит записи
                }
            }
        }
    }

    public void CreateDatabase()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                // Create Building table
                command.CommandText = "CREATE TABLE IF NOT EXISTS Building (BuildingID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Address TEXT)";
                command.ExecuteNonQuery();

                // Create Room table
                command.CommandText = "CREATE TABLE IF NOT EXISTS Room (RoomID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Area INTEGER, BuildingID INTEGER)";
                command.ExecuteNonQuery();

                // Create Renter table
                command.CommandText = "CREATE TABLE IF NOT EXISTS Renter (RenterID INTEGER PRIMARY KEY AUTOINCREMENT, CompanyName TEXT, LegalAddress TEXT, CEOFullName TEXT, ContactPhone TEXT)";
                command.ExecuteNonQuery();

                // Create Rent table
                command.CommandText = "CREATE TABLE IF NOT EXISTS Rent (RentID INTEGER PRIMARY KEY AUTOINCREMENT, RoomID INTEGER, RenterID INTEGER, ContractNumber TEXT, ContractDate DATETIME, StartDate DATETIME, EndDate DATETIME)";
                command.ExecuteNonQuery();
            }
        }
    }

    public List<T> GetRecordsFromTable<T>(string tableName, Func<SqliteDataReader, T> mapFunction)
    {
        List<T> records = new List<T>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                // Выполнить SQL-запрос для выбора данных из указанной таблицы
                command.CommandText = $"SELECT * FROM {tableName}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Используйте переданную функцию mapFunction для маппинга данных
                        T record = mapFunction(reader);

                        // Заполним RoomName, RenterName, BuildingName, если применимо
                        FillAdditionalNames(record);

                        records.Add(record);
                    }
                }
            }
        }

        return records;
    }

    private void FillAdditionalNames<T>(T record)
    {
        // Определяем тип записи и заполняем дополнительные имена в соответствии с логикой
        if (record is Room room)
        {
            room.BuildingName = GetBuildingNameById(room.BuildingID);
        }
        else if (record is Rent rent)
        {
            rent.RoomName = GetRoomNameById(rent.RoomID);
            rent.RenterName = GetRenterNameById(rent.RenterID);
        }
    }

    private string GetBuildingNameById(int buildingId)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand($"SELECT Name FROM Building WHERE BuildingID = @BuildingId", connection))
            {
                command.Parameters.AddWithValue("@BuildingId", buildingId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
            }
        }

        return string.Empty;
    }

    private string GetRoomNameById(int roomId)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand($"SELECT Name FROM Room WHERE RoomID = @RoomId", connection))
            {
                command.Parameters.AddWithValue("@RoomId", roomId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
            }
        }

        return string.Empty;
    }

    private string GetRenterNameById(int renterId)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand($"SELECT CompanyName FROM Renter WHERE RenterID = @RenterId", connection))
            {
                command.Parameters.AddWithValue("@RenterId", renterId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
            }
        }

        return string.Empty;
    }


    public List<string> GetTableNames()
    {
        List<string> tableNames = new List<string>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table';", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tableNames.Add(reader.GetString(0));
                    }
                }
            }
        }

        return tableNames;
    }

    public DataSet GetDataSetFromJson(string jsonFilePath)
    {
        DataSet dataSet = new DataSet("accounting_for_leased_premises");

        DataTable buildingTable = new DataTable("Building");
        buildingTable.Columns.Add("BuildingID", typeof(int)).AutoIncrement = true;
        buildingTable.Columns.Add("Name", typeof(string));
        buildingTable.Columns.Add("Address", typeof(string));
        buildingTable.PrimaryKey = new DataColumn[] { buildingTable.Columns["BuildingID"] };

        DataTable roomTable = new DataTable("Room");
        roomTable.Columns.Add("RoomID", typeof(int)).AutoIncrement = true;
        roomTable.Columns.Add("Name", typeof(string));
        roomTable.Columns.Add("Area", typeof(int));
        roomTable.Columns.Add("BuildingID", typeof(int));
        roomTable.PrimaryKey = new DataColumn[] { roomTable.Columns["RoomID"] };

        DataTable renterTable = new DataTable("Renter");
        renterTable.Columns.Add("RenterID", typeof(int)).AutoIncrement = true;
        renterTable.Columns.Add("CompanyName", typeof(string));
        renterTable.Columns.Add("LegalAddress", typeof(string));
        renterTable.Columns.Add("CEOFullName", typeof(string));
        renterTable.Columns.Add("ContactPhone", typeof(string));
        renterTable.PrimaryKey = new DataColumn[] { renterTable.Columns["RenterID"] };

        DataTable rentTable = new DataTable("Rent");
        rentTable.Columns.Add("RentID", typeof(int)).AutoIncrement = true;
        rentTable.Columns.Add("RoomID", typeof(int));
        rentTable.Columns.Add("RenterID", typeof(int));
        rentTable.Columns.Add("ContractNumber", typeof(string));
        renterTable.Columns.Add("ContractDate", typeof(DateTime));
        rentTable.Columns.Add("StartDate", typeof(DateTime));
        rentTable.Columns.Add("EndDate", typeof(DateTime));
        rentTable.PrimaryKey = new DataColumn[] { rentTable.Columns["RentID"] };

        dataSet.Tables.Add(buildingTable);
        dataSet.Tables.Add(roomTable);
        dataSet.Tables.Add(renterTable);
        dataSet.Tables.Add(rentTable);



        DataRelation relationBuildingAndRoom = new DataRelation(
        "BuildingRoomRelation",
        buildingTable.Columns["BuildingID"],
        roomTable.Columns["BuildingID"]
        );

        DataRelation relationRenterAndRent = new DataRelation(
            "RenterRentRelation",
            renterTable.Columns["RenterID"],
            rentTable.Columns["RenterID"]
        );
        DataRelation relationRentAndRoom = new DataRelation(
             "RentRoomRelation",
             roomTable.Columns["RoomID"],
             rentTable.Columns["RoomID"]
             );

        dataSet.Relations.Add(relationBuildingAndRoom);
        dataSet.Relations.Add(relationRentAndRoom);
        dataSet.Relations.Add(relationRenterAndRent);

        string json = File.ReadAllText(jsonFilePath);
        DataSetContainer dataSetContainer = JsonConvert.DeserializeObject<DataSetContainer>(json);

        DataSet dataSetFromFile = new DataSet("accounting_for_leased_premises");

        FillTableInfoFromDataSet(dataSetFromFile, dataSetContainer);
        FillDataSetFromDataSetContainer(dataSetFromFile, dataSetContainer);
        dataSet = dataSetFromFile;

        return dataSet;
    }
    public int GetParentId(string parentName, string parentValue)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                switch (parentName)
                {
                    case "BuildingName":
                        command.CommandText = "SELECT BuildingID FROM Building WHERE Name = @Name";
                        break;
                    case "RoomName":
                        command.CommandText = "SELECT RoomID FROM Room WHERE Name = @Name";
                        break;
                    case "RenterName":
                        command.CommandText = "SELECT RenterID FROM Renter WHERE Name = @Name";
                        break;
                    default:
                        return -1; // Неизвестный parentName
                }

                command.Parameters.AddWithValue("@Name", parentValue);

                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return -1; // Ничего не найдено
                }
            }
        }
    }
    public bool TableHasRows(string tableName)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM {tableName}";
                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result) > 0;
                }
                else
                {
                    return false; // Ничего не найдено
                }
            }
        }
    }

    public long GetMaxId(string tableName, string idColumnName)
    {
        using (var connection = new SqliteConnection(connectionString))
        {

            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT MAX({idColumnName}) FROM {tableName}";
                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt64(result);
                }
                else
                {
                    return -1; // Ничего не найдено
                }
            }
        }
    }


    private void FillTableInfoFromDataSet(DataSet dataSetFromFile, DataSetContainer dataSetContainer)
    {
        foreach (var tableInfo in dataSetContainer.TableInfo)
        {
            // Create a DataTable for each table
            DataTable dataTable = new DataTable(tableInfo.TableName);

            // Add primary key columns
            foreach (var primaryKey in tableInfo.PrimaryKeys)
            {
                if (dataSetContainer.DataSet[tableInfo.TableName].Count != 0)
                {
                    Type keyType = dataSetContainer.DataSet[tableInfo.TableName][0][primaryKey].GetType();
                    dataTable.Columns.Add(primaryKey, keyType);
                }
            }

            Dictionary<string, object>.KeyCollection allColumns = new Dictionary<string, object>().Keys;
            if (dataSetContainer.DataSet[tableInfo.TableName].Count != 0)
            {
                allColumns = dataSetContainer.DataSet[tableInfo.TableName][0].Keys;
            }

            // Add columns from the DataSet that are not already in the DataTable
            foreach (var column in allColumns)
            {
                if (!dataTable.Columns.Contains(column))
                {
                    Type columnType = dataSetContainer.DataSet[tableInfo.TableName][0][column].GetType();
                    dataTable.Columns.Add(column, columnType);
                }
            }

            // Iterate through the first row of data to add columns
            foreach (var dataRow in dataSetContainer.DataSet[tableInfo.TableName])
            {
                foreach (var key in dataRow.Keys)
                {
                    Type columnType = dataRow[key].GetType();
                    if (!dataTable.Columns.Contains(key))
                    {
                        dataTable.Columns.Add(key, columnType);
                    }
                }
            }

            // Add data rows
            foreach (var dataRow in dataSetContainer.DataSet[tableInfo.TableName])
            {
                DataRow row = dataTable.NewRow();
                foreach (var key in dataRow.Keys)
                {
                    row[key] = dataRow[key];
                }
                dataTable.Rows.Add(row);
            }

            // Add the DataTable to the DataSet
            dataSetFromFile.Tables.Add(dataTable);
        }
    }

    private void FillDataSetFromDataSetContainer(DataSet dataSet, DataSetContainer dataSetContainer)
    {
        foreach (var tableName in dataSetContainer.DataSet.Keys)
        {

            if (dataSet.Tables.Contains(tableName))
            {
                continue;
            }

            DataTable dataTable = new DataTable(tableName);

            // Add primary key columns
            foreach (var primaryKey in dataSetContainer.TableInfo
                .First(info => info.TableName == tableName).PrimaryKeys)
            {
                Type keyType = dataSetContainer.DataSet[tableName][0][primaryKey].GetType();
                dataTable.Columns.Add(primaryKey, keyType);
            }

            // Iterate through the first row of data to add columns
            foreach (var dataRow in dataSetContainer.DataSet[tableName])
            {
                foreach (var key in dataRow.Keys)
                {
                    Type columnType = dataRow[key].GetType();
                    if (!dataTable.Columns.Contains(key))
                    {
                        dataTable.Columns.Add(key, columnType);
                    }
                }
            }

            // Add data rows
            foreach (var dataRow in dataSetContainer.DataSet[tableName])
            {
                DataRow row = dataTable.NewRow();
                foreach (var key in dataRow.Keys)
                {
                    row[key] = dataRow[key];
                }
                dataTable.Rows.Add(row);
            }

            // Add the DataTable to the DataSet
            dataSet.Tables.Add(dataTable);
        }
    }

    public void InsertDataSetIntoSQLite(DataSet dataSet)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            foreach (DataTable table in dataSet.Tables)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (DataRow row in table.Rows)
                    {
                        // Создайте команду SQL для вставки данных
                        var insertCommand = new SqliteCommand();
                        insertCommand.Connection = connection;
                        insertCommand.Transaction = transaction;

                        insertCommand.CommandText = $"INSERT INTO {table.TableName} ({string.Join(",", table.Columns.Cast<DataColumn>().Select(col => col.ColumnName))}) " +
                                                   $"VALUES ({string.Join(",", table.Columns.Cast<DataColumn>().Select(col => $"@{col.ColumnName}"))})";

                        foreach (DataColumn column in table.Columns)
                        {
                            insertCommand.Parameters.Add(new SqliteParameter($"@{column.ColumnName}", GetSqliteType(column.DataType)));
                        }

                        // Задайте значения параметров
                        foreach (DataColumn column in table.Columns)
                        {
                            insertCommand.Parameters[$"@{column.ColumnName}"].Value = row[column];
                        }

                        // Выполните команду SQL для вставки данных
                        insertCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }

    public DataTable CreateCombinedTable(DateTime startDate, DateTime endDate, List<string> tableNames)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            DataTable combinedDataTable = new DataTable();

            foreach (var tableName in tableNames)
            {
                if (tableName == "Rent")
                {
                    combinedDataTable.Columns.Add("ContractNumber", typeof(string));
                    combinedDataTable.Columns.Add("ContractDate", typeof(DateTime));
                    combinedDataTable.Columns.Add("StartDate", typeof(DateTime));
                    combinedDataTable.Columns.Add("EndDate", typeof(DateTime));
                }
                else if (tableName == "Room")
                {
                    combinedDataTable.Columns.Add("RoomName", typeof(string));
                    combinedDataTable.Columns.Add("Area", typeof(long));
                }
                else if (tableName == "Renter")
                {
                    combinedDataTable.Columns.Add("CompanyName", typeof(string));
                    combinedDataTable.Columns.Add("LegalAddress", typeof(string));
                    combinedDataTable.Columns.Add("CEOFullName", typeof(string));
                    combinedDataTable.Columns.Add("ContactPhone", typeof(string));
                }
                else if (tableName == "Building")
                {
                    combinedDataTable.Columns.Add("BuildingName", typeof(string));
                    combinedDataTable.Columns.Add("BuildingAddress", typeof(string));
                }
            }

            var query = @"SELECT Rent.ContractNumber, Rent.ContractDate, Rent.StartDate, Rent.EndDate, 
                            Room.Name AS RoomName, Room.Area, 
                            Renter.CompanyName, Renter.LegalAddress, Renter.CEOFullName, Renter.ContactPhone, 
                            Building.Name AS BuildingName, Building.Address AS BuildingAddress
                    FROM Rent
                    JOIN Room ON Rent.RoomID = Room.RoomID
                    JOIN Renter ON Rent.RenterID = Renter.RenterID
                    JOIN Building ON Room.BuildingID = Building.BuildingID
                    WHERE Rent.ContractDate BETWEEN @StartDate AND @EndDate";

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataRow newRow = combinedDataTable.NewRow();

                        if (tableNames.Contains("Rent"))
                        {
                            newRow["ContractNumber"] = reader["ContractNumber"];
                            newRow["ContractDate"] = reader.GetDateTime("ContractDate");
                            newRow["StartDate"] = reader.GetDateTime("StartDate");
                            newRow["EndDate"] = reader.GetDateTime("EndDate");
                        }

                        if (tableNames.Contains("Room"))
                        {
                            newRow["RoomName"] = reader["RoomName"];
                            newRow["Area"] = reader.GetInt64("Area");
                        }

                        if (tableNames.Contains("Renter"))
                        {
                            newRow["CompanyName"] = reader["CompanyName"];
                            newRow["LegalAddress"] = reader["LegalAddress"];
                            newRow["CEOFullName"] = reader["CEOFullName"];
                            newRow["ContactPhone"] = reader["ContactPhone"];
                        }

                        if (tableNames.Contains("Building"))
                        {
                            newRow["BuildingName"] = reader["BuildingName"];
                            newRow["BuildingAddress"] = reader["BuildingAddress"];
                        }

                        combinedDataTable.Rows.Add(newRow);
                    }
                }
            }
            return combinedDataTable;
        }
    }

    public void SaveToDb(dynamic formData, Type typeData, string idPropertyName, bool isEditForm = false, bool isAddForm = false)
{
    try
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var properties = typeData.GetProperties();
                // Создайте SQL-запрос для вставки или обновления данных
                var insertOrUpdateQuery = isAddForm
                    ? $"INSERT INTO {typeData.Name} ({string.Join(",", properties.Where(prop => prop.Name != "BuildingName" && prop.Name != "RoomName" && prop.Name != "RenterName").Select(prop => prop.Name))}) " +
                      $"VALUES ({string.Join(",", properties.Where(prop => prop.Name != "BuildingName" && prop.Name != "RoomName" && prop.Name != "RenterName").Select(prop => $"@{prop.Name}"))})"
                    : $"UPDATE {typeData.Name} SET {string.Join(",", properties.Where(prop => prop.Name != "BuildingName" && prop.Name != "RoomName" && prop.Name != "RenterName").Select(prop => $"{prop.Name} = @{prop.Name}"))} WHERE {idPropertyName} = @{idPropertyName}";

                using (var command = new SqliteCommand(insertOrUpdateQuery, connection, transaction))
                {
                    // Установите параметры команды
                    foreach (var property in properties)
                    {
                        var paramName = $"@{property.Name}";

                        // Пропускаем определенные свойства
                        if (property.Name == "BuildingName" || property.Name == "RoomName" || property.Name == "RenterName")
                            continue;

                        // Добавляем параметры в зависимости от типа свойства
                        if (property.PropertyType == typeof(string) || property.PropertyType == typeof(int))
                        {
                            ((IDictionary<string, object>)formData).TryGetValue(property.Name, out var value);
                            command.Parameters.AddWithValue(paramName, value);
                        }
                        else if (property.PropertyType == typeof(DateTime))
                        {
                            ((IDictionary<string, object>)formData).TryGetValue(property.Name, out var value);
                            var dateValue = (DateTime?)value;
                            if (dateValue.HasValue)
                            {
                                command.Parameters.AddWithValue(paramName, dateValue.Value.ToString("yyyy-MM-dd"));
                            }
                            else
                            {
                                command.Parameters.AddWithValue(paramName, DBNull.Value);
                            }
                        }
                    }

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}


    public List<string> GetColumnValuesFromTable(string tableName, string columnName)
    {
        List<string> values = new List<string>();

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqliteCommand($"SELECT DISTINCT {columnName} FROM {tableName}", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            values.Add(reader[columnName].ToString());
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return values;
    }

    // Метод для получения типа SQLite на основе типа данных .NET
    private SqliteType GetSqliteType(Type dataType)
    {
        if (dataType == typeof(int))
            return SqliteType.Integer;
        else if (dataType == typeof(string))
            return SqliteType.Text;
        else if (dataType == typeof(DateTime))
            return SqliteType.Text; // SQLite не имеет отдельного типа для даты, мы можем хранить дату как текст
                                    // Другие типы данных, такие как десятичные, булевы и другие, могут потребовать дополнительной обработки.

        return SqliteType.Text; // По умолчанию используем текстовый тип.
    }

    public void DeleteCascadeRecord(string tableName, int recordId)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Delete the record from the specified table
                    using (var deleteCommand = new SqliteCommand($"DELETE FROM {tableName} WHERE {GetPrimaryKeyColumn(tableName)} = @RecordId", connection, transaction))
                    {
                        deleteCommand.Parameters.AddWithValue("@RecordId", recordId);
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Check for child tables and delete related records
                    foreach (var relation in GetChildRelations(tableName))
                    {
                        using (var deleteChildCommand = new SqliteCommand($"DELETE FROM {relation.ChildTable} WHERE {relation.ChildColumn} = @RecordId", connection, transaction))
                        {
                            deleteChildCommand.Parameters.AddWithValue("@RecordId", recordId);
                            deleteChildCommand.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error deleting record: {ex.Message}");
                }
            }
        }
    }

    public void DeleteRecord(string tableName, long primaryKeyValue)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Check if the record exists
                    var recordExistsQuery = $"SELECT COUNT(*) FROM {tableName} WHERE {GetPrimaryKeyColumn(tableName)} = @PrimaryKeyValue";
                    using (var checkCommand = new SqliteCommand(recordExistsQuery, connection, transaction))
                    {
                        checkCommand.Parameters.AddWithValue("@PrimaryKeyValue", primaryKeyValue);
                        var recordCount = (long)checkCommand.ExecuteScalar();

                        if (recordCount == 0)
                        {
                            // Record doesn't exist, no need to delete
                            return;
                        }
                    }

                    // Delete the record
                    var deleteCommand = new SqliteCommand($"DELETE FROM {tableName} WHERE {GetPrimaryKeyColumn(tableName)} = @PrimaryKeyValue", connection, transaction);
                    deleteCommand.Parameters.AddWithValue("@PrimaryKeyValue", primaryKeyValue);
                    deleteCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting record from {tableName}: {ex.Message}");
                    transaction.Rollback();
                }
            }
        }
    }

    public string GetPrimaryKeyColumn(string tableName)
    {
        return GetTableColumns(tableName).FirstOrDefault();
    }

    private IEnumerable<string> GetTableColumns(string tableName)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand($"PRAGMA table_info({tableName})", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return reader.GetString(reader.GetOrdinal("name"));
                }
            }
        }
    }

    private IEnumerable<RelationInfo> GetChildRelations(string parentTableName)
    {
        return tableRelations
            .Where(relation => relation.ParentTable == parentTableName);
    }

    private List<RelationInfo> GetSampleTableRelations()
    {
        // Define sample relationships (replace with actual relationships)
        return new List<RelationInfo>
        {
            new RelationInfo
            {
                ParentTable = "Building",
                ChildTable = "Room",
                ParentColumn = "BuildingID",
                ChildColumn = "BuildingID"
            },
            new RelationInfo
            {
                ParentTable = "Room",
                ChildTable = "Rent",
                ParentColumn = "RoomID",
                ChildColumn = "RoomID"
            },
            // Add more relationships as needed
        };
    }
}

public class TableInfo
{
    public string TableName { get; set; }
    public List<string> PrimaryKeys { get; set; }
    public List<RelationInfo> Relations { get; set; }
}

public class RelationInfo
{
    public string ParentTable { get; set; }
    public string ChildTable { get; set; }
    public string ParentColumn { get; set; }
    public string ChildColumn { get; set; }
}

public class DataSetContainer
{
    public Dictionary<string, List<Dictionary<string, object>>> DataSet { get; set; }
    public List<TableInfo> TableInfo { get; set; }
}
