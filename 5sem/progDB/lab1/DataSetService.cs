using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace lab1;

class DataSetService
{
    public DataSet dataSet;
    public DataRelation relationBuildingAndRoom;
    public DataRelation relationRenterAndRoom;
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

        SetRelationShips(buildingTable, roomTable, renterTable);
        dataSet.Relations.Add(relationBuildingAndRoom);
        dataSet.Relations.Add(relationRenterAndRoom);
    }

    public void SetRelationShips(DataTable buildingTable, DataTable roomTable, DataTable renterTable)
    {
        relationBuildingAndRoom = new DataRelation(
        "BuildingRoomRelation",
        buildingTable.Columns["BuildingID"],
        roomTable.Columns["BuildingID"]
        );

        relationRenterAndRoom = new DataRelation(
        "RenterRoomRelation",
        renterTable.Columns["RenterID"],
        roomTable.Columns["RoomID"]
        );
    }

    public void SaveDataSet()
    {
        if (File.Exists("accounting_for_leased_premises.json"))
            return;

        string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);

        string jsonFilePath = "accounting_for_leased_premises.json";
        File.WriteAllText(jsonFilePath, json);
    }

    public List<T> DataTableToList<T>(DataTable dataTable) where T : new()
    {
        List<T> list = new List<T>();

        foreach (DataRow row in dataTable.Rows)
        {
            T obj = new T();

            foreach (DataColumn column in dataTable.Columns)
            {
                string propertyName = column.ColumnName;
                object value = row[column];


                var property = typeof(T).GetProperty(propertyName);
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
}
