
let fclickCounter = 0;
let lastClickedFigure = null;
const clickCounter = document.getElementById("counter");
const shapesRow = document.getElementById("Inrow");

clickCounter.textContent = fclickCounter;

function FigureClick(figure) {
  if (figure.classList.contains("figure-click")) {
    figure.classList.remove("figure-click");
    fclickCounter--;
  } else {
    figure.classList.add("figure-click");
    fclickCounter++;
  }

  clickCounter.textContent = fclickCounter;

  if (figure === lastClickedFigure) {
    figure.classList.remove("pulse");
  }

  figure.classList.add("pulse");
  if (lastClickedFigure) {
    lastClickedFigure.classList.remove("pulse");
  }
  lastClickedFigure = figure;
}

function create(objectType) {
  const element = document.createElement("div");
  element.addEventListener("click", () => addFigureClick(element));

  element.classList.add(objectType);
  InRow.appendChild(element);
}

function clear_All() {
  while (InRow.lastChild) {
    InRow.removeChild(InRow.lastChild);
  }
}
