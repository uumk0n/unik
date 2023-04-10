import $ from "jquery";
import "bootstrap";
import "popper.js";

$(function () {
   $('[data-toggle="popover"]').popover();
  // console.log($('[data-toggle="popover"]'));
});

$(function () {
  $(".download-btn").on("click", function () {
    $(".toast").toast("show");
  });
});

// Initialize the current object index to zero
let currentObjectIndex = 0;

// Get all the modal elements
let modalElements = document.querySelectorAll(".modal");

// Add a keydown event listener to the document object
document.addEventListener("keydown", function (event) {
  // Check if the pressed key is the left arrow
  if (event.keyCode === 37) {
    // Decrement the current object index
    currentObjectIndex--;

    // If the index goes below zero, set it to the last index
    if (currentObjectIndex < 0) {
      currentObjectIndex = modalElements.length - 1;
    }

    // Show the modal element at the current index and hide all others
    showCurrentModal();
  }
  // Check if the pressed key is the right arrow
  else if (event.keyCode === 39) {
    // Increment the current object index
    currentObjectIndex++;

    // If the index goes above the last index, set it to zero
    if (currentObjectIndex >= modalElements.length) {
      currentObjectIndex = 0;
    }

    // Show the modal element at the current index and hide all others
    showCurrentModal();
  }
});

// Function to show the modal element at the current index and hide all others
function showCurrentModal() {
  for (let i = 0; i < modalElements.length; i++) {
    if (i === currentObjectIndex) {
      // console.log(modalElements[i]);
      $(modalElements[i]).modal("show");
    } else {
      $(modalElements[i]).modal("hide");
    }
  }
}
