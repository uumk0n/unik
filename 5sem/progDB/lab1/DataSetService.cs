using System;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace lab1;

class DataSetService
{
    public DataSetService()
    {
        // init 
    }
    public void CreateDataSet()
    {
        if (File.Exists("accounting_for_leased_premises.json"))
            return;

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

        DataRelation relationRenterAndRoom = new DataRelation(
        "RenterRoomRelation",
        renterTable.Columns["RenterID"],
        roomTable.Columns["RoomID"]
        );

        dataSet.Relations.Add(relationBuildingAndRoom);
        dataSet.Relations.Add(relationRenterAndRoom);

        string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);

        string jsonFilePath = "accounting_for_leased_premises.json";
        File.WriteAllText(jsonFilePath, json);
    }
}
