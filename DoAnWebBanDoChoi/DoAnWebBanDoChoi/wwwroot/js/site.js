
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
                // ✅ THÀNH CÔNG: Dùng SweetAlert2 với cấu hình Tự Biến Mất
                capNhatSoLuongGioHang(); // Gọi lại để cập nhật số lượng

                const Toast = Swal.mixin({
                    toast: true, // Hiển thị dưới dạng toast (thông báo nhỏ)
                    position: 'top-end', // Vị trí ở góc trên bên phải
                    showConfirmButton: false, // Ẩn nút OK
                    timer: 3000, // Tự động đóng sau 3 giây (3000ms)
                    timerProgressBar: true, // Hiển thị thanh tiến trình
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                });

                Toast.fire({
                    icon: 'success', // Biểu tượng thành công (tick xanh)
                    title: 'Đã thêm sản phẩm vào giỏ hàng!' // Nội dung thông báo
                });

            } else {
                // ❌ LỖI LOGIC: Dùng SweetAlert2 dạng alert thông thường
                Swal.fire({
                    icon: 'error',
                    title: 'Thêm thất bại',
                    text: 'Lỗi: ' + res.message,
                });
            }
        },
        error: function () {
            // 🛑 LỖI HỆ THỐNG/AJAX: Dùng SweetAlert2 dạng alert thông thường
            Swal.fire({
                icon: 'error',
                title: 'Lỗi hệ thống',
                text: 'Có lỗi xảy ra khi gửi yêu cầu thêm sản phẩm.',
            });
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
