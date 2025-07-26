
function capNhatSoLuongGioHang() {
    $.get('/Cart/LayTongSanPham', function (res) {
        if (res.success) {
            $('#gioHangSoLuong').text(res.soLuong);
        }
    });
}

function capNhatSoLuongYeuThich() {
    $.get('/Favorite/LayTongYeuThich', function (res) {
        if (res.success) {
            $('#soLuongYeuThich').text(res.soLuong);
        }
    });
}

$(document).ready(function () {
    capNhatSoLuongGioHang();
    capNhatSoLuongYeuThich();
});

function submitThemGio(maSp) {
    $.ajax({
        url: '/Cart/ThemVaoGio',
        type: 'POST',
        data: {
            maSp: maSp,
            soLuong: 1
        },
        success: function (res) {
            if (res.success) {
                capNhatSoLuongGioHang(); // ✅ Gọi lại để cập nhật
                alert('Đã thêm vào giỏ hàng!');
            } else {
                alert('Lỗi: ' + res.message);
            }
        },
        error: function () {
            alert('Có lỗi xảy ra khi thêm sản phẩm!');
        }
    });
}
function xacNhanDangNhap() {
    if (confirm("Bạn cần đăng nhập để yêu thích sản phẩm. Bạn có muốn đăng nhập ngay không?")) {
        window.location.href = '/Account/Login';
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const success = document.getElementById("toast-success");
    const error = document.getElementById("toast-error");

    if (success) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'success',
            title: success.value,
            showConfirmButton: false,
            timer: 5000,
            background: '#333',
            color: '#fff'
        });
    }

    if (error) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'error',
            title: error.value,
            showConfirmButton: false,
            timer: 5000,
            background: '#333',
            color: '#fff'
        });
    }
});
