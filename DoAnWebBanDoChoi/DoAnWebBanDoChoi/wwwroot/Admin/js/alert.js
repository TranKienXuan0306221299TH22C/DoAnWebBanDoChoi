//document.addEventListener("DOMContentLoaded", function () {
//    const successMsg = document.getElementById("temp-success-msg");
//    const errorMsg = document.getElementById("temp-error-msg");

//    if (successMsg) {
//        Swal.fire({
//            icon: 'success',
//            title: 'Thành công!',
//            text: successMsg.value,
//            confirmButtonText: 'OK'
//        });
//    }

//    if (errorMsg) {
//        Swal.fire({
//            icon: 'error',
//            title: 'Thất bại!',
//            text: errorMsg.value,
//            confirmButtonText: 'OK'
//        });
//    }
//});
function showTempAlert(type = null, message = null) {
    if (type && message) {
        Swal.fire({
            icon: type,
            title: type === 'success' ? 'Thành công!' : 'Thất bại!',
            text: message,
            confirmButtonText: 'OK'
        });
        return;
    }

    const successMsg = document.getElementById("temp-success-msg");
    const errorMsg = document.getElementById("temp-error-msg");

    if (successMsg && successMsg.value) {
        Swal.fire({
            icon: 'success',
            title: 'Thành công!',
            text: successMsg.value,
            confirmButtonText: 'OK'
        });
        successMsg.value = ""; // reset lại
    }

    if (errorMsg && errorMsg.value) {
        Swal.fire({
            icon: 'error',
            title: 'Thất bại!',
            text: errorMsg.value,
            confirmButtonText: 'OK'
        });
        errorMsg.value = ""; // reset lại
    }
}


// Tự gọi 1 lần khi trang load
document.addEventListener("DOMContentLoaded", function () {

    showTempAlert();
});

