const modalWindow = document.getElementById("modalWindow");
const closeBtn = document.querySelector(".close");

closeBtn.addEventListener("click", () => {
  modalWindow.style.display = "none";
});

window.addEventListener("click", (event) => {
  if (event.target === modalWindow) {
    modalWindow.style.display = "none";
  }
});

function showmodalWindow() {
  modalWindow.style.display = "block";
}