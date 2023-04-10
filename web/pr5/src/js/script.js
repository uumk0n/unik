import $ from "jquery";

import "bootstrap";
import Popper from "popper.js";

var modal = document.getElementById("modalJQ");

var span = document.getElementsByClassName("jq-close")[0];

$("#openModal").click(function () {
  modal.style.display = "block";
});

span.onclick = function () {
  modal.style.display = "none";
};

window.onclick = function (event) {
  if (event.target == modal) {
    modal.style.display = "none";
  }
};

$("#openModal").click(function () {
  $("#modalJQ").css("display", "block");
});

$(function () {
  var $slider = $(".slider");
  var $backButton = $(".slider-back");
  var $goButton = $(".sliderGO");
  var slideWidth = $slider.find("img").width();
  var NUM = $slider.find("img").length;
  var CurSl = 1;

  function updateButtons() {
    if (CurSl === 1) {
      $backButton.prop("disabled", true);
    } else {
      $backButton.prop("disabled", false);
    }
    if (CurSl === NUM) {
      $goButton.prop("disabled", true);
    } else {
      $goButton.prop("disabled", false);
    }
  }

  function slideTo(slide) {
    $slider.css("transform", "translateX(" + -(slide - 1) * slideWidth + "px)");
    CurSl = slide;
    updateButtons();
  }

  $backButton.on("click", function () {
    if (CurSl > 1) {
      slideTo(CurSl - 1);
    }
  });

  $goButton.on("click", function () {
    if (CurSl < NUM) {
      slideTo(CurSl + 1);
    }
  });

  updateButtons();
});