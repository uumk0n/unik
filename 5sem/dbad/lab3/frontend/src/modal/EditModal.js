import React, { useState, useEffect } from "react";
import Modal from "react-modal";
import axios from "axios";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

const EditModal = ({
  isOpen,
  onClose,
  tableName,
  rowData,
  columnNames,
  tabs,
}) => {
  const [formData, setFormData] = useState({});
  const [comboBoxOptions, setComboBoxOptions] = useState([]);

  const russianHeaders = {
    inila: "СНИЛС",
    sndedudoc: "Дополнительный номер образовательного документа",
    dateend: "Дата окончания",
    regedudoc: "Регистрационный номер образовательного документа",
    institut: "Институт",
    status: "Статус",
    idinfo: "Идентификатор информации",
    dateord: "Дата приказа",
    regorgnum: "Регистрационный номер организации",
    work: "Место работы",
    type: "Тип",
    name: "Название",
    fcs: "ФИО",
    itn: "ИНН",
    address: "Адрес",
    snpassport: "Серия и номер паспорта",
    married: "Семейное положение",
    kids: "Количество детей",
    nameorg: "Название организации",
    itnorg: "ИНН организации",
    orgaddress: "Адрес организации",
    hfinfo: "Трудовая книжка",
  };

  useEffect(() => {
    const fetchData = async () => {
      if (tableName && tabs) {
        const optionsData = {};

        // Fetch options for each column
        await Promise.all(
          columnNames.map(async (columnName) => {
            const tab = tabs.find((tab) => tab.label === tableName);
            if (tab) {
              setFormData(rowData || {});
              try {
                const optionsResponse = await axios.get(
                  `${tab.apiUrl}/GetComboBoxOptions`,
                  {
                    params: {
                      columnName: columnName,
                    },
                  }
                );

                optionsData[columnName] = optionsResponse.data;
              } catch (error) {
                console.error(
                  `Error fetching ComboBox options for column ${columnName}:`,
                  error
                );
              }
            }
          })
        );

        // Update state with the fetched options
        setComboBoxOptions(optionsData);
      }
    };
    fetchData();
  }, [tableName, tabs, columnNames, rowData]);

  const getColumnDataType = (columnName) => {
    if (
      columnName.toLowerCase() === "dateend" ||
      columnName.toLowerCase() === "dateord"
    ) {
      return "datetime";
    }
    return "text";
  };

  const truncateTime = (date) => {
    const trueDate = new Date(date);
    return date
      ? new Date(
          trueDate.getFullYear(),
          trueDate.getMonth(),
          trueDate.getDate()
        )
      : null;
  };

  const renderInput = (columnName, columnType) => {
    if (columnType === "datetime") {
      const selectedDate = formData[columnName.toLowerCase()] || null;
      const dateWithoutTime = truncateTime(selectedDate);

      return (
        <DatePicker
          selected={dateWithoutTime}
          onChange={(date) => handleDateChange(columnName, date)}
          dateFormat="yyyy-MM-dd"
        />
      );
    } else if (columnName.toLowerCase() === "married") {
      return (
        <select
          name={columnName}
          value={formData[columnName.toLowerCase()] || ""}
          onChange={(e) => handleComboBoxChange(columnName, e.target.value)}
        >
          <option value="">Select an option</option>
          <option value="true">Yes</option>
          <option value="false">No</option>
        </select>
      );
    } else if (isForeignKey(columnName, tableName)) {
      const options = comboBoxOptions[columnName] || [];
      return (
        <select
          name={columnName}
          value={formData[columnName.toLowerCase()] || ""}
          onChange={(e) => handleComboBoxChange(columnName, e.target.value)}
        >
          <option value="">Select an option</option>
          {options.map((option) => (
            <option key={option} value={option}>
              {option}
            </option>
          ))}
        </select>
      );
    } else {
      return (
        <input
          type="text"
          name={columnName}
          value={formData[columnName.toLowerCase()] || ""}
          onChange={(e) => handleInputChange(columnName, e.target.value)}
        />
      );
    }
  };

  const isForeignKey = (columnName, tableName) => {
    if (tableName !== null) {
      if (
        tableName.toLowerCase() === "edudocs" &&
        columnName.toLowerCase() === "regedudoc"
      ) {
        return true;
      } else if (
        tableName.toLowerCase() === "hfinfos" &&
        columnName.toLowerCase() === "regorgnum"
      ) {
        return true;
      } else if (
        tableName.toLowerCase() === "personaldata" &&
        columnName.toLowerCase() === "inila"
      ) {
        return true;
      }
    }
    return false;
  };

  const handleDateChange = (columnName, date) => {
    const dateValue = new Date(date);
    setFormData((prevData) => ({
      ...prevData,
      [columnName.toLowerCase()]: dateValue,
    }));
  };

  const handleComboBoxChange = (columnName, value) => {
    const parsedValue =
      value === "true" || value === "false" ? JSON.parse(value) : value;

    setFormData((prevData) => ({
      ...prevData,
      [columnName.toLowerCase()]: parsedValue,
    }));
  };

  const handleInputChange = (columnName, value, columnType) => {
    setFormData((prevData) => ({
      ...prevData,
      [columnName.toLowerCase()]: value,
    }));
  };

  const handleEditClick = async () => {
    const formattedData = Object.fromEntries(
      Object.entries(formData).map(([key, value]) => {
        if (value instanceof Date) {
          return [key, value.toISOString()];
        }
        return [key, value];
      })
    );

    if (
      !Object.values(formattedData).every(
        (value) => value !== null && value !== ""
      )
    ) {
      alert("Please fill in all fields before updating.");
      return;
    }

    const tab = tabs.find((tab) => tab.label === tableName);
    if (tab) {
      try {
        const primaryKey = getPrimaryKey(tableName);
        if (!primaryKey) {
          console.error(`Primary key not defined for table ${tableName}.`);
          return;
        }

        const primaryKeyValue = formattedData[primaryKey];

        if (primaryKeyValue === null || primaryKeyValue === "") {
          console.error(`Primary key value is null or empty.`);
          return;
        }

        const response = await axios.put(
          `${tab.apiUrl}/${primaryKeyValue}`,
          formattedData,
          {
            headers: {
              "Content-Type": "application/json",
            },
          }
        );

        if (response.status !== 204) {
          console.error("Failed to update data:", response.data);
          // Handle the error, display a message, etc.
        } else {
          onClose();
        }
      } catch (error) {
        console.error("Error updating data:", error);
        // Handle the error, display a message, etc.
      }
    }
  };

  // Function to get the primary key based on the table name
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
        return null;
    }
  };

  return (
    <Modal isOpen={isOpen} onRequestClose={onClose}>
      <h2>Редактировать запись в таблице {tableName}</h2>
      <form>
        {Array.isArray(columnNames) && columnNames.length > 0 ? (
          columnNames.map((columnName) => (
            <div key={columnName}>
              <div>
                <label>{russianHeaders[columnName.toLowerCase()]}</label>
              </div>
              <div>
                {renderInput(columnName, getColumnDataType(columnName))}
              </div>
            </div>
          ))
        ) : (
          <p>No columns to display.</p>
        )}

        <button type="button" onClick={handleEditClick}>
          Редактировать
        </button>
      </form>
    </Modal>
  );
};

export default EditModal;
