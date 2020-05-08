
window.addEventListener('load', function () {
  var acc = document.getElementsByClassName("accordion_tab");
  var i;
  for (i = 0; i < acc.length; i++) {
    acc[i].addEventListener("click", function() {
      this.classList.toggle("active");
      var panel = this.nextElementSibling;
      if (panel.style.display === "block") {
        panel.style.display = "none";
      } else {
        panel.style.display = "block";
      }
    });
  }
  
  //Media path check
  
  var checkboxes = document.getElementsByClassName('media_path_checkbox');
  for (var index in checkboxes) {
    //bind event to each checkbox
    checkboxes[index].onchange = toggleMediapath;
  }
  function toggleMediapath() {
    if (this.checked) {
      this.parentNode.nextElementSibling.querySelector('.media_path_input').disabled = 'disabled';
    } else {
      this.parentNode.nextElementSibling.querySelector('.media_path_input').disabled = '';
    }
  }
});

