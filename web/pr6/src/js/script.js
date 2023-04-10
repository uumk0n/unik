fetch("../data.json")
.then((response) => response.json())
  .then((data) => {
    console.log(data);
    const source = document.getElementById("template").innerHTML;
    console.log(source);
    const template = Handlebars.compile(source);
    console.log(template);

    // Вставляем данные в шаблон и помещаем результат в блок root
    document.getElementById("root").innerHTML = template(data);
  })
  .catch((error) => console.error(error));