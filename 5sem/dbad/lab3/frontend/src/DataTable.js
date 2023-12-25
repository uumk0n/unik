import React, { useState } from "react";
import { DataGrid } from "@mui/x-data-grid";
import AddModal from "./modal/AddModal";
import EditModal from "./modal/EditModal";
import DeleteModal from "./modal/DeleteModal";
import ReportModal from "./modal/ReportModal";
import * as XLSX from "xlsx";

const DataTable = ({
  data,
  tabs,
  selectedTableName,
  selectedTableColumnNames,
}) => {
  const [selectedRows, setSelectedRows] = useState([]);
  const [isAddModalOpen, setAddModalOpen] = useState(false);
  const [isEditModalOpen, setEditModalOpen] = useState(false);
  const [isDeleteModalOpen, setDeleteModalOpen] = useState(false);
  const [isReportModalOpen, setReportModalOpen] = useState(false);
  const [selectedRowData, setSelectedRowData] = useState(null);

  const mappedData = data.map((item) => {
    const id = item.inila || item.regOrgNum || item.regEduDoc || item.fcs; // Choose the appropriate key
    return {
      id,
      ...item,
    };
  });

  // Русские названия колонок
  const russianHeaders = {
    inila: "СНИЛС",
    sndEduDoc: "Дополнительный номер образовательного документа",
    dateEnd: "Дата окончания",
    regEduDoc: "Регистрационный номер образовательного документа",
    institut: "Институт",
    status: "Статус",
    idInfo: "Идентификатор информации",
    dateOrd: "Дата приказа",
    regOrgNum: "Регистрационный номер организации",
    work: "Место работы",
    type: "Тип",
    name: "Название",
    fcs: "ФИО",
    itn: "ИНН",
    address: "Адрес",
    snPassport: "Серия и номер паспорта",
    married: "Семейное положение",
    kids: "Количество детей",
    nameOrg: "Название организации",
    itnOrg: "ИНН организации",
    orgAddress: "Адрес организации",
    hfInfo: "Трудовая книжка",
  };

  const handleRowSelection = (selection) => {
    setSelectedRows(selection);
    const selectedRow = data.find(
      (item) =>
        (item.inila || item.regOrgNum || item.regEduDoc || item.fcs) ===
        selection[0]
    );
    const selectedRowLowercaseKeys = Object.fromEntries(
      Object.entries(selectedRow).map(([key, value]) => [
        key.toLowerCase(),
        value,
      ])
    );
    setSelectedRowData(selectedRowLowercaseKeys);
  };

  if (!data || data.length === 0) {
    return <p>No data available.</p>;
  }

  const columns = Object.keys(data[0]).map((header) => ({
    field: header,
    headerName: russianHeaders[header],
    type: header === "dateEnd" || header === "dateOrd" ? "date" : "string",
    width: 150, // Adjust the width as needed
    valueGetter: (params) => {
      if (params.field === "dateEnd" || params.field === "dateOrd") {
        const dateString = params.row[params.field];
        return new Date(dateString);
      }
      return params.row[params.field];
    },
  }));

  const handleAddClick = () => {
    setAddModalOpen(true);
  };

  const handleEditClick = () => {
    if (selectedRows.length === 1) {
      setEditModalOpen(true);
    } else {
      console.error("Please select exactly one row to edit.");
    }
  };

  const handleDeleteClick = () => {
    if (selectedRows.length === 0) {
      console.error("Please select at least one row to delete.");
      return;
    }
    setDeleteModalOpen(true);
  };

  const handleGenerateReportClick = () => {
    setReportModalOpen(true);
  };

  const updateData = () => {
    // Перезагрузить страницу
    window.location.reload();
  };

  const deleteRecord = async () => {
    const tab = tabs.find((tab) => tab.label === selectedTableName);
    if (tab) {
      try {
        const primaryKey = getPrimaryKey(selectedTableName);

        if (!primaryKey) {
          console.error(
            `Primary key not defined for table ${selectedTableName}.`
          );
          return;
        }

        const response = await fetch(
          `${tab.apiUrl}/${selectedRowData[primaryKey]}`,
          {
            method: "DELETE",
            headers: {
              "Content-Type": "application/json",
            },
          }
        );

        if (!response.ok) {
          throw new Error("Failed to delete");
        }

        // Handle success, e.g., close modal or refresh data
        setDeleteModalOpen(false);
        updateData();
      } catch (error) {
        console.error("Error deleting:", error);
        // Handle the error, display a message, etc.
      }
    }
  };

  const handleGenerateReport = async (selectedTables) => {
    try {
      const response = await fetch(
        "http://localhost:5260/api/PersonalDatas/GenerateReport",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(selectedTables),
        }
      );

      if (!response.ok) {
        throw new Error("Failed to generate report");
      }

      const data = await response.json();

      generatePDF(data, "output.pdf");
      generateExcel(data,"output.xlsx");
    } catch (error) {
      console.error("Error generating report:", error);
      // Handle the error, display an error message, etc.
    }
  };

  async function generateExcel(data, filePath) {
    // Check if the data array is not undefined and has a length
    if (!data || !data.length) {
      console.error("Data is undefined or empty.");
      return;
    }

    // Combine records based on common keys
    const combinedData = data.map((item) => {
      const combinedItem = {};

      // Flatten each table's data into the combined item
      Object.keys(item).forEach((tableName) => {
        const tableData = item[tableName];
        Object.keys(tableData).forEach((key) => {
          const combinedKey = `${tableName}_${key}`;
          combinedItem[combinedKey] = tableData[key];
        });
      });

      return combinedItem;
    });

    const worksheet = XLSX.utils.json_to_sheet(combinedData, {
      header: Object.keys(combinedData[0]),
    });

    // Set column widths based on the length of the data in each column
    const columnWidths = Object.keys(combinedData[0]).map((key) => ({
      wch: Math.max(
        ...combinedData.map((item) => (item[key] || "").toString().length)
      ),
    }));

    worksheet["!cols"] = columnWidths;

    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet 1");

    // Use writeFileSync instead of write
    XLSX.writeFile(workbook, filePath, { compression: false });

    alert("Excel generation completed.");
  }
  const { jsPDF } = require("jspdf");
  require("jspdf-autotable");

  async function generatePDF(data, filePath) {
    // Check if the data array is not undefined and has a length
    if (!data || !data.length) {
      console.error("Data is undefined or empty.");
      return;
    }

    // Function to flatten nested objects
    const flattenObject = (obj, prefix = "") => {
      return Object.keys(obj).reduce((acc, key) => {
        const propName = prefix ? `${prefix}_${key}` : key;
        if (typeof obj[key] === "object" && obj[key] !== null) {
          return { ...acc, ...flattenObject(obj[key], propName) };
        } else {
          return { ...acc, [propName]: obj[key] };
        }
      }, {});
    };

    // Flatten the data
    const flattenedData = data.map((item) => flattenObject(item));

    // Extract column names
    const columnNames = Object.keys(flattenedData[0]);

    // Create a PDF document with landscape orientation
    const pdfDoc = new jsPDF({
      orientation: "landscape",
    });

    // Set font size
    pdfDoc.setFontSize(12);

    // Add a table for the data using autoTable
    let startY = 10;

    // Add a table for the data using autoTable
    pdfDoc.autoTable({
      head: [columnNames],
      body: flattenedData.map((row) => columnNames.map((col) => row[col])),
      startY: startY,
      theme: "plain", // No special styling
      didDrawPage: (data) => {
        // Check if the remaining height is not enough for the next table
        if (data.cursor.y > pdfDoc.internal.pageSize.height - 20) {
          pdfDoc.addPage(); // Add a new page
          startY = 10; // Reset the startY for the new page
          // Add the table header
          pdfDoc.autoTable({
            head: [columnNames],
            startY: startY,
            theme: "plain",
          });
        }
      },
    });

    // Save the PDF
    pdfDoc.save(filePath);

    alert("PDF generation completed.");
  }
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
      <DataGrid
        rows={mappedData}
        columns={columns}
        checkboxSelection
        onRowSelectionModelChange={handleRowSelection}
      />

      <div>
        <button onClick={handleAddClick}>Добавить</button>
        <button onClick={handleEditClick}>Редактировать</button>
        <button onClick={handleDeleteClick}>Удалить</button>
        <button onClick={handleGenerateReportClick}>Сформировать отчет</button>
      </div>

      {/* Модальные окна */}
      <AddModal
        isOpen={isAddModalOpen}
        onClose={() => {
          setAddModalOpen(false);
          updateData();
        }}
        tableName={selectedTableName}
        columnNames={selectedTableColumnNames}
        tabs={tabs}
      />

      <EditModal
        isOpen={isEditModalOpen}
        onClose={() => {
          setEditModalOpen(false);
          updateData();
        }}
        tableName={selectedTableName}
        rowData={selectedRowData}
        columnNames={selectedTableColumnNames}
        tabs={tabs}
      />
      <DeleteModal
        isOpen={isDeleteModalOpen}
        onClose={() => {
          updateData();
        }}
        onDelete={() => deleteRecord()}
      />
      <ReportModal
        isOpen={isReportModalOpen}
        onClose={() => setReportModalOpen(false)}
        onGenerateReport={handleGenerateReport}
        tableNames={tabs.map((tab) => tab.label)}
      />
    </div>
  );
};

export default DataTable;
