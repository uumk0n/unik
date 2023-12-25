// ReportModal.js
import React, { useState } from "react";
import Modal from "react-modal";

const ReportModal = ({ isOpen, onClose, onGenerateReport, tableNames }) => {
  const customStyles = {
    content: {
      width: "400px",
      height: "400px",
      margin: "auto",
    },
  };

  const [selectedTables, setSelectedTables] = useState([]);

  const handleCheckboxChange = (tableName) => {
    if (selectedTables.includes(tableName)) {
      setSelectedTables((prevTables) =>
        prevTables.filter((table) => table !== tableName)
      );
    } else {
      setSelectedTables((prevTables) => [...prevTables, tableName]);
    }
  };

  const handleGenerateReport = () => {
    if (selectedTables.length < 3) {
      // Display an error message or handle it in your preferred way
      alert("Please select at least 3 tables.");
      return;
    }

    // Pass the selected tables to the parent component
    onGenerateReport(selectedTables);
    // Close the modal
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onRequestClose={onClose} style={customStyles}>
      <p>Select tables to include in the report:</p>
      {tableNames.map((tableName) => (
        <div key={tableName}>
          <input
            type="checkbox"
            id={tableName}
            checked={selectedTables.includes(tableName)}
            onChange={() => handleCheckboxChange(tableName)}
          />
          <label htmlFor={tableName}>{tableName}</label>
        </div>
      ))}
      <button onClick={handleGenerateReport}>Generate Report</button>
    </Modal>
  );
};

export default ReportModal;
