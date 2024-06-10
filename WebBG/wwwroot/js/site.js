function myFunction() {
    var element = document.body;
    var newTheme = element.dataset.bsTheme === "light" ? "dark" : "light";
    element.dataset.bsTheme = newTheme;
    localStorage.setItem("theme", newTheme);
}

function stepFunction(event) {
    var element = document.getElementsByClassName("collapse");
    for (var i = 0; i < element.length; i++) {
        if (element[i] !== event.target.ariaControls) {
            element[i].classList.remove("show");
        }
    }
}

// Kiểm tra xem đã có chế độ sáng tối được lưu trữ hay chưa
var savedTheme = localStorage.getItem("theme");
if (savedTheme) {
    var element = document.body;
    element.dataset.bsTheme = savedTheme;
}