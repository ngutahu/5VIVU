function flightDetails() {
    var flightId = $("#flightId").val();
    $.ajax({
        type: "GET",
        url: "https://localhost:7019/api/Flights/" + flightId,
        dataType: "json",
        success: function (flightDetails) {
            $("#aircraftName").html("Mã máy bay: " + "<br>" + flightDetails.aircraftName);
            $("#departureAirport").html("Sân bay khởi hành: " + "<br>" + flightDetails.departureAirportName);
            $("#arrivalAirport").html("Sân bay hạ cánh: " + "<br>" + flightDetails.arrivalAirPortName);
            $("#departureTime").html("Thời điểm khởi hành: " + "<br>" + flightDetails.departureTime);
            $("#arrivalTime").html("Thời điểm hạ cánh: " + "<br>" + flightDetails.arrivalTime);
            $("#price").html("Giá vé: " + "<br>" + (flightDetails.price + flightDetails.price * 5 / 100));
            $("#totalEmptySeat").html("Số ghế còn trống: " + "<br>" + flightDetails.totalEmptySeat);
        },
        error: function (req, status, error) {
            console.log(status);
        }
    });
}

$(document).ready(function () {
    flightDetails();
});
/*function redirectToBookingFlight(flightId) {
    window.location.href = '/Booking/Book/' + flightId;
}*/
function redirectToBookingFlight(flightId) {
    window.location.href = '/Booking/Book?flightId=' + flightId;
}

