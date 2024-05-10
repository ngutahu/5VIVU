function bookingSuccess() {
    var flightId = $("#flightId").val();
    var email = $("#email").val();
    var fullName = $("#name").val();
    var identificationNumber = $("#identificationNumber").val();
    var amount = $("#amount").val();
    var seatNumber = $("#selectedSeat").val();
    var data = {
        flightId: flightId,
        amount: amount,
        fullName: fullName,
        identificationNumber: identificationNumber,
        email: email,
        seatNumber: seatNumber
    };

    console.log(data);

    // Kiểm tra các trường dữ liệu cần thiết
    if (!flightId || !email || !fullName || !identificationNumber || !amount || !seatNumber) {
        console.log("Vui lòng điền đầy đủ thông tin.");
        return;
    }   

    $.ajax({
        url: "https://localhost:7019/api/Bookings/createBooking",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            "flightId": flightId,
            "amount": (amount * 100) / 105,
            "fullName": fullName,
            "identificationNumber": identificationNumber,
            "email": email,
            "seatNumber": seatNumber
        }),
        success: function (response) {
            console.log("Đặt vé thành công!");
            console.log(response);
        },
        error: function (xhr, status, error) {
            console.log("Đặt vé thất bại!");
            console.log(xhr.responseText);
            // Xử lý lỗi một cách cụ thể
            alert("Đặt vé thất bại. Vui lòng thử lại sau.");
        }
    });
}

bookingSuccess();