function thanhToan() {
    // Xử lý thanh toán
    alert("Xử lý thanh toán...");
}

function quayLai() {
    // Quay lại trang trước đó
    window.history.back();
}
    // JavaScript
    document.getElementById("user-icon").addEventListener("click", function() {
        document.getElementById("loginModal").style.display = "block";
    });

    document.getElementsByClassName("close")[0].addEventListener("click", function() {
        document.getElementById("loginModal").style.display = "none";
    });

    window.addEventListener("click", function(event) {
        if (event.target == document.getElementById("loginModal")) {
            document.getElementById("loginModal").style.display = "none";
        }
    });

    // Xử lý đăng nhập
    document.getElementById("loginForm").addEventListener("submit", function(event) {
        event.preventDefault(); // Ngăn chặn form gửi đi
        // Thực hiện kiểm tra đăng nhập ở đây
        alert("Đăng nhập thành công!"); // Đây chỉ là ví dụ, bạn cần thay đổi theo logic của bạn
        document.getElementById("loginModal").style.display = "none"; // Đóng modal sau khi đăng nhập thành công
    });

document.addEventListener("DOMContentLoaded", function () {
    // Lấy đối tượng carousel bằng cách sử dụng id của nó
    var carousel = document.getElementById("imageCarousel");

    // Tự động chuyển đổi hình ảnh sau mỗi 3 giây
    setInterval(function () {
        // Kích hoạt sự kiện click trên nút dẫn hướng tiếp theo
        document.querySelector('.carousel-control-next').click();
    }, 3000); // 3000 miligiây = 3 giây
});

$(document).ready(function () {
        $('.navbar-nav .nav-link').on('click', function () {
            $('.navbar-nav').find('.active').removeClass('active');
            $(this).addClass('active');
        });
});

function checkPasswordMatch(input) {
    var passwordInput = document.getElementById('password');
    var confirmPasswordInput = input;
    if (passwordInput.value !== confirmPasswordInput.value) {
        confirmPasswordInput.setCustomValidity('Mật khẩu không khớp');
    } else {
        confirmPasswordInput.setCustomValidity('');
    }
}


