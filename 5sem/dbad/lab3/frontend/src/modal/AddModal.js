import React, { useState, useEffect } from "react";
import Modal from "react-modal";
import axios from "axios";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

const AddModal = ({ isOpen, onClose, tableName, columnNames, tabs }) => {
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
  }, [tableName, tabs, columnNames]);

  const getColumnDataType = (columnName) => {
    if (
      columnName.toLowerCase() === "dateend" ||
      columnName.toLowerCase() === "dateord"
    ) {
      return "datetime";
    }
    return "text";
  };

  const renderInput = (columnName, columnType) => {
    if (columnType === "datetime") {
      return (
        <DatePicker
          selected={formData[columnName] || null}
          onChange={(date) => handleDateChange(columnName, date)}
          showTimeSelect
          dateFormat="Pp"
        />
      );
    } else if (columnName.toLowerCase() === "married") {
      return (
        <select
          name={columnName}
          value={formData[columnName] || ""}
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
          value={formData[columnName] || ""}
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
          value={formData[columnName] || ""}
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
    setFormData((prevData) => ({ ...prevData, [columnName]: date }));
  };

  const handleComboBoxChange = (columnName, value) => {
    const parsedValue =
      value === "true" || value === "false" ? JSON.parse(value) : value;

    setFormData((prevData) => ({ ...prevData, [columnName]: parsedValue }));
  };

  const handleInputChange = (columnName, value, columnType) => {
    if (columnType === "datetime") {
      // Assuming date input (you might need to adjust this based on your requirements)
      setFormData((prevData) => ({
        ...prevData,
        [columnName]: new Date(value),
      }));
    } else {
      setFormData((prevData) => ({ ...prevData, [columnName]: value }));
    }
  };
  const handleAddClick = async () => {
    // Convert Date properties to string format
    const formattedData = Object.fromEntries(
      Object.entries(formData).map(([key, value]) => {
        if (value instanceof Date) {
          return [key, value.toISOString()];
        }
        return [key, value];
      })
    );

    if (
      Object.values(formattedData).some(
        (value) => value === "" || value === null
      )
    ) {
      // Display an error message or handle it in your preferred way
      alert("Please fill in all fields before adding.");
      return;
    }

    const tab = tabs.find((tab) => tab.label === tableName);
    if (tab) {
      try {
        // Make the API request to add data
        const response = await fetch(tab.apiUrl, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(formattedData),
        });

        if (!response.ok) {
          throw new Error("Failed to add data");
        }

        onClose();
      } catch (error) {
        console.error("Error adding data:", error);
        // Handle the error, display a message, etc.
      }
    }
  };

  return (
    <Modal isOpen={isOpen} onRequestClose={onClose}>
      <h2>Добавить запись в таблицу {tableName}</h2>
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

        <button type="button" onClick={handleAddClick}>
          Добавить
        </button>
      </form>
    </Modal>
  );
};

export default AddModal;
