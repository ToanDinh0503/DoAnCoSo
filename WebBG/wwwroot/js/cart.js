
var cart = {
    init: function () {
        cart.regEvents();
    },
    regEvents: function () {
        $('#btnContinue').off('click').on('click', function () {
            window.location.href = "/san-pham";
        });
        $('#btnPayment').off('click').on('click', function () {
            window.location.href = "/order";
        });
        $('#btnUpdate').off('click').on('click', function () {
            // Hiển thị thông báo và yêu cầu xác nhận trước khi cập nhật
            if (confirm("Bạn có chắc chắn muốn cập nhật giỏ hàng không?")) {
                var listProduct = $('.txtQuantity');
                console.log(listProduct)
                var cartList = [];
                $.each(listProduct, function (i, item) {
                    cartList.push({
                        Quantity: $(item).val(),
                        Product: {
                            BoardGameId: $(item).data('id')
                        }
                    });
                });
                $.ajax({
                    url: '/Cart/Update',
                    data: { cartModel: JSON.stringify(cartList) },
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            // Hiển thị thông báo cập nhật thành công
                            alert("Cập nhật giỏ hàng thành công!");
                            window.location.href = "/cart";
                        } else {
                            // Hiển thị thông báo cập nhật thất bại
                            alert("Cập nhật giỏ hàng thất bại!");
                        }
                    }
                });
            }
        });
        $('#btnDeleteAll').off('click').on('click', function () {
            // Hiển thị thông báo và yêu cầu xác nhận trước khi xóa giỏ hàng
            if (confirm("Bạn có chắc chắn muốn xóa giỏ hàng không?")) {
                $.ajax({
                    url: '/Cart/DeleteAll',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            // Hiển thị thông báo xóa giỏ hàng thành công
                            alert("Xóa giỏ hàng thành công!");
                            window.location.href = "/cart";
                        } else {
                            // Hiển thị thông báo xóa giỏ hàng thất bại
                            alert("Xóa giỏ hàng thất bại!");
                        }
                    }
                });
            }
        });
        $('.btn-delete').off('click').on('click', function (e) {
            e.preventDefault();
            // Hiển thị thông báo và yêu cầu xác nhận trước khi xóa sản phẩm khỏi giỏ hàng
            if (confirm("Bạn có chắc chắn muốn xóa sản phẩm này khỏi giỏ hàng không?")) {
                $.ajax({
                    data: { id: $(this).data('id') },
                    url: '/Cart/Delete',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            // Hiển thị thông báo xóa sản phẩm thành công
                            alert("Xóa sản phẩm khỏi giỏ hàng thành công!");
                            window.location.href = "/cart";
                        } else {
                            // Hiển thị thông báo xóa sản phẩm thất bại
                            alert("Xóa sản phẩm khỏi giỏ hàng thất bại!");
                        }
                    }
                });
            }
        });
        $('.addToCartBtn').off('click').on('click', function (event) {

            // Lấy thông tin sản phẩm
            var productId = $(this).data('productid');
            var quantity = $(this).data('quantity');

            // Gửi yêu cầu AJAX để thêm sản phẩm vào giỏ hàng
            $.ajax({
                url: '/AddCart',
                data: {
                    ProductId: productId,
                    Quantity: quantity
                },
                dataType: 'json',
                type: 'GET',
                success: function () {
                    // Hiển thị thông báo thêm thành công
                    Alert("Đã thêm vào giỏ hàng thành công!");
                },
                error: function (error) {
                    // Xử lý lỗi nếu cần
                    console.error(error);
                }
            });
        });

        

        $(document).ready(function () {
            // Gọi action để lấy số lượng sản phẩm trong giỏ hàng
            $.get("/Cart/GetCartItemCount", function (data) {
                // Cập nhật số lượng sản phẩm trong giỏ hàng trên icon cart
                $(".cart-item-count").text(data);
            });
        });
    }

}
cart.init();

