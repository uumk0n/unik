using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lab1;

class DataSetService
{
    public DataSet dataSet;
    public DataRelation relationBuildingAndRoom;
    public DataRelation relationRentAndRoom;
    public DataRelation relationRenterAndRent;
    public DataSetService()
    {
        CreateDataSet();
    }
    private void CreateDataSet()
    {
        dataSet = new DataSet("accounting_for_leased_premises");

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

        SetRelationShips(buildingTable, roomTable, renterTable, rentTable);
    }

    public void SetRelationShips(DataTable buildingTable, DataTable roomTable, DataTable renterTable, DataTable rentTable)
    {
        relationBuildingAndRoom = new DataRelation(
        "BuildingRoomRelation",
        buildingTable.Columns["BuildingID"],
        roomTable.Columns["BuildingID"]
        );

        relationRenterAndRent = new DataRelation(
            "RenterRentRelation",
            renterTable.Columns["RenterID"],
            rentTable.Columns["RenterID"]
        );
        relationRentAndRoom = new DataRelation(
            "RentRoomRelation",
            roomTable.Columns["RoomID"],
            rentTable.Columns["RoomID"]
            );

        dataSet.Relations.Add(relationBuildingAndRoom);
        dataSet.Relations.Add(relationRentAndRoom);
        dataSet.Relations.Add(relationRenterAndRent);
    }



    public void SaveDataSet()
    {
        List<TableInfo> tableInfoList = new List<TableInfo>();

        // Loop through the tables in the dataset
        foreach (DataTable table in dataSet.Tables)
        {
            TableInfo tableInfo = new TableInfo
            {
                TableName = table.TableName,
                PrimaryKeys = table.PrimaryKey.Select(pk => pk.ColumnName).ToList(),
                Relations = new List<RelationInfo>()
            };

            // Check if the table has relationships
            if (dataSet.Relations.Count > 0)
            {
                foreach (DataRelation relation in dataSet.Relations)
                {
                    if (relation.ChildTable.TableName == table.TableName)
                    {
                        RelationInfo relationInfo = new RelationInfo
                        {
                            ParentTable = relation.ParentTable.TableName,
                            ChildTable = relation.ChildTable.TableName,
                            ParentColumn = relation.ParentColumns[0].ColumnName,
                            ChildColumn = relation.ChildColumns[0].ColumnName
                        };

                        tableInfo.Relations.Add(relationInfo);
                    }
                }
            }

            tableInfoList.Add(tableInfo);
        }

        // Serialize the dataset and table information
        var dataToSerialize = new
        {
            DataSet = dataSet,
            TableInfo = tableInfoList
        };

        string json = JsonConvert.SerializeObject(dataToSerialize, Formatting.Indented);
        string jsonFilePath = "accounting_for_leased_premises.json";
        File.WriteAllText(jsonFilePath, json);
    }

    public List<object> DataTableToList(DataTable dataTable, Type dataType)
    {
        List<object> list = new List<object>();

        foreach (DataRow row in dataTable.Rows)
        {
            object obj = Activator.CreateInstance(dataType);

            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.ColumnName == "BuildingID" && dataTable.TableName == "Room")
                {
                    int foregingKey = Convert.ToInt32(row[column]);
                    string propertyName = "BuildingName";
                    object value = null;
                    foreach (DataRow buildingRow in dataSet.Tables["Building"].Rows)
                    {
                        if (buildingRow[0].ToString() == foregingKey.ToString())
                        {
                            value = buildingRow["Name"];
                            break;
                        }
                    }
                    var property = dataType.GetProperty(propertyName);
                    if (value == DBNull.Value)
                    {
                        property.SetValue(obj, null);
                        continue;
                    }

                    if (property != null)
                    {
                        if (property.PropertyType == typeof(int))
                            property.SetValue(obj, Convert.ToInt32(value));
                        else if (property.PropertyType == typeof(DateTime))
                            property.SetValue(obj, Convert.ToDateTime(value));
                        else
                            property.SetValue(obj, value);
                    }
                }
                else if ((column.ColumnName == "RoomID" || column.ColumnName == "RenterID") && dataTable.TableName == "Rent")
                {
                    int foregingKey = Convert.ToInt32(row[column]);
                    string propertyName = null;
                    if (column.ColumnName == "RoomID")
                        propertyName = "RoomName";
                    else
                        propertyName = "RenterName";

                    object value = null;
                    if (propertyName == "RoomName")
                    {
                        foreach (DataRow roomRow in dataSet.Tables["Room"].Rows)
                        {
                            if (roomRow[0].ToString() == foregingKey.ToString())
                            {
                                value = roomRow["Name"];
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow renterRow in dataSet.Tables["Renter"].Rows)
                        {
                            if (renterRow[0].ToString() == foregingKey.ToString())
                            {
                                value = renterRow["CompanyName"];
                                break;
                            }
                        }
                    }
                    var property = dataType.GetProperty(propertyName);
                    if (value == DBNull.Value)
                    {
                        property.SetValue(obj, null);
                        continue;
                    }
                    if (property != null)
                    {
                        if (property.PropertyType == typeof(int))
                            property.SetValue(obj, Convert.ToInt32(value));
                        else if (property.PropertyType == typeof(DateTime))
                            property.SetValue(obj, Convert.ToDateTime(value));
                        else
                            property.SetValue(obj, value);
                    }
                }
                else
                {
                    string propertyName = column.ColumnName;
                    object value = row[column];

                    var property = dataType.GetProperty(propertyName);
                    if (value == DBNull.Value)
                    {
                        property.SetValue(obj, null);
                        continue;
                    }

                    if (property != null)
                    {
                        if (property.PropertyType == typeof(int))
                            property.SetValue(obj, Convert.ToInt32(value));
                        else if (property.PropertyType == typeof(DateTime))
                            property.SetValue(obj, Convert.ToDateTime(value));
                        else
                            property.SetValue(obj, value);
                    }
                }
            }

            list.Add(obj);
        }

        return list;
    }

    public DataTable ConvertListToDataTable<T>(List<T> list)
    {
        DataTable table = new DataTable(typeof(T).Name);

        PropertyInfo[] properties = typeof(T).GetProperties();

        // Создаем колонки в DataTable на основе свойств типа T
        foreach (PropertyInfo property in properties)
        {
            table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }

        // Заполняем DataTable данными из List<T>
        foreach (T item in list)
        {
            DataRow row = table.NewRow();
            foreach (PropertyInfo property in properties)
            {
                row[property.Name] = property.GetValue(item) ?? DBNull.Value;
            }
            table.Rows.Add(row);
        }

        return table;
    }

    public void FillDataSetFromDataSetContainer(DataSet dataSet, DataSetContainer dataSetContainer)
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

    public void FillTableInfoFromDataSet(DataSet dataSetFromFile, DataSetContainer dataSetContainer)
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
