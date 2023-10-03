
using System;
using System.Data;

namespace lab1;

public class Building
{
    public int BuildingID { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
}

public class Room
{
    public int RoomID { get; set; }
    public string? Name { get; set; }
    public int Area { get; set; }
    public int BuildingID { get; set; }
}

public class Renter
{
    public int RenterID { get; set; }
    public string? CompanyName { get; set; }
    public string? LegalAddress { get; set; }
    public string? CEOFullName { get; set; }
    public string? ContactPhone { get; set; }
}

public class Rent
{
    public int RentID { get; set; }
    public int RoomID { get; set; }
    public int RenterID { get; set; }
    public string? ContractNumber { get; set; }
    public DateTime ContractDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}