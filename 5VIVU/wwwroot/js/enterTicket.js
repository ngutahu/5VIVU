document.addEventListener("DOMContentLoaded", function () {
    // Lưu trữ dữ liệu vào sessionStorage khi click vào nút "SelectSeat"
    document.getElementById("selectSeatButton").addEventListener("click", function (event) {
        sessionStorage.setItem("fullName", document.getElementById("fullName").value);
        sessionStorage.setItem("birthday", document.getElementById("birthday").value);
        sessionStorage.setItem("cmnd", document.getElementById("cmnd").value);
        sessionStorage.setItem("email", document.getElementById("email").value);
        sessionStorage.setItem("phone", document.getElementById("phone").value);
        window.location.href = "/SelectSeat/PickSeat";
    });

    // Lấy dữ liệu từ sessionStorage để hiển thị
    document.getElementById("fullName").value = sessionStorage.getItem("fullName") || "";
    document.getElementById("birthday").value = sessionStorage.getItem("birthday") || "";
    document.getElementById("cmnd").value = sessionStorage.getItem("cmnd") || "";
    document.getElementById("email").value = sessionStorage.getItem("email") || "";
    document.getElementById("phone").value = sessionStorage.getItem("phone") || "";
});


function redirectToPickSeat(flightId) {
    window.location.href = '/SelectSeat/PickSeat?flightId=' + flightId;
}
function redirecToTicket(flightId) {
    window.location.href = '/Booking/BookingSuccess?flightId=' + flightId;
}

