import React, { useState } from "react";
import Modal from "react-modal";

const DeleteModal = ({ isOpen, onClose, onDelete }) => {
  const customStyles = {
    content: {
      width: "400px",
      height: "200px",
      margin: "auto",
      display: "flex",
      flexDirection: "column",
      alignItems: "center",
    },
  };

  return (
    <Modal isOpen={isOpen} onRequestClose={onClose} style={customStyles}>
      <p>Вы действительно хотите удалить?</p>
      <div>
        <button onClick={onDelete}>Да</button>
        <button onClick={onClose}>Отмена</button>
      </div>
    </Modal>
  );
};

export default DeleteModal;
