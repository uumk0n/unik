import React, { useState, useEffect } from "react";
import axios from "axios";
import DataTable from "./DataTable";

const TabbedTables = ({ tabs }) => {
  const storedTab = localStorage.getItem("selectedTab");
  const storedTableName = localStorage.getItem("selectedTableName");
  const initialActiveTab = storedTab
    ? tabs.findIndex((tab) => tab.label === storedTab)
    : 0;

  const [activeTab, setActiveTab] = useState(initialActiveTab);
  const [tableData, setTableData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedTableName, setSelectedTableName] = useState(storedTableName || null);
  const [selectedTableColumnNames, setSelectedTableColumnNames] = useState([]);

  const handleTabClick = (index) => {
    setActiveTab(index);
    const selectedTab = tabs[index];
    setSelectedTableName(selectedTab.label);
    setSelectedTableColumnNames(getColumnNamesForTable(selectedTab.label));
    localStorage.setItem("selectedTab", selectedTab.label);
    localStorage.setItem("selectedTableName", selectedTab.label);
  };

  const getColumnNamesForTable = (tableName) => {
    const tab = tabs.find((tab) => tab.label === tableName);
    if (tab && tab.columns) {
      return tab.columns;
    } else {
      return [];
    }
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const currentTab = tabs[activeTab];
        const response = await axios.get(currentTab.apiUrl);
        const extractedData = response.data.map((item) => {
          const fields = item.fields;
          const primaryKey = getPrimaryKey(currentTab.label);
          // Add the primary key to the fields object
          fields[primaryKey] = item.pk;
          return fields;
        });
        setTableData(extractedData);
  
      } catch (error) {
        console.error("Error fetching data:", error);
      } finally {
        setLoading(false);
      }
    };
  
    fetchData();
  }, [activeTab, tabs]);

  const getPrimaryKey = (tableName) => {
    switch (tableName.toLowerCase()) {
      case "personaldata":
        return "fcs";
      case "hfinfos":
      case "edudocs":
        return "inila";
      case "works":
        return "regorgnum";
      case "institutes":
        return "regedudoc";
      default:
        return null; // Handle other cases or return the correct primary key
    }
  };

  return (
    <div>
      <div style={{ marginBottom: "10px" }}>
        {tabs.map((tab, index) => (
          <button
            key={index}
            style={{
              marginRight: "10px",
              padding: "5px 10px",
              cursor: "pointer",
              backgroundColor: index === activeTab ? "#ddd" : "transparent",
              border: "1px solid #aaa",
              borderRadius: "4px",
            }}
            onClick={() => handleTabClick(index)}
          >
            {tab.label}
          </button>
        ))}
      </div>

      <div>
        <p>{tabs[activeTab].label}</p>
        {tableData.length > 0 && (
          <DataTable
            data={tableData}
            tabs={tabs}
            selectedTableName={selectedTableName}
            selectedTableColumnNames={selectedTableColumnNames}
          />
        )}
      </div>
    </div>
  );
};

export default TabbedTables;
