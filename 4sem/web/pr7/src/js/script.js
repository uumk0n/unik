import template from "../../tamplate.hbs";
import * as data from "../../data/data.json";

document.getElementById("root").innerHTML = template(data);