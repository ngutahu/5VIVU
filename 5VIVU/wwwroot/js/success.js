function bookingSuccess() {
    var flightId = $("#flightId").val();
    var email = $("#email").val();
    var fullName = $("#name").val();
    var identificationNumber = $("#identificationNumber").val();
    var amount = $("#amount").val();

    // Kiểm tra các trường dữ liệu cần thiết
    if (!flightId || !email || !fullName || !identificationNumber || !amount) {
        console.log("Vui lòng điền đầy đủ thông tin.");
        return;
    }

    $.ajax({
        url: "https://localhost:7019/api/Bookings/createBooking",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            "flightId": flightId,
            "amount": amount,
            "fullName": fullName,
            "identificationNumber": identificationNumber,
            "email": email
        }),
        success: function (response) {
            console.log("Đặt vé thành công!");
            console.log(response);
            flightDetails(flightId, function (flightDetails) {
                createTicket(flightDetails, response); 
            });
        },
        error: function (xhr, status, error) {
            console.log("Đặt vé thất bại!");
            console.log(xhr.responseText);
            alert("Đặt vé thất bại. Vui lòng thử lại sau.");
        }
    });
}

bookingSuccess();

function flightDetails(flightId, callback) {
    $.ajax({
        type: "GET",
        url: "https://localhost:7019/api/Flights/" + flightId,
        dataType: "json",
        success: function (flightDetails) {
            callback(flightDetails);
        },
        error: function (req, status, error) {
            console.log(status);
        }
    });
}

function createTicket(flightDetails, response) {
    var aircraftName = flightDetails.aircraftName;
    var departureAirportName = flightDetails.departureAirportName;
    var arrivalAirportName = flightDetails.arrivalAirportName;
    var departureTime = flightDetails.departureTime;
    var arrivalTime = flightDetails.arrivalTime;
    var price = flightDetails.price;
    var totalEmptySeat = flightDetails.totalEmptySeat;

    var userId = response.userId;
    var cmnd = response.cmnd;
    var birthday = response.birthday;
    var phone = response.phone;
    var transTime = response.transTime;
    var airCraftId = flightDetails.aircraftId;
    var arrivalAirport = flightDetails.arrivalAirportId;
    var departureAirport = flightDetails.departureAirportId;
    var seatNumber = response.seatNumber;

    var ticketData = {
        "FlightId": flightId,
        "UserId": userId,
        "Cmnd": cmnd,
        "FullName": fullName,
        "Birthday": birthday,
        "Phone": phone,
        "Email": email,
        "SeatNumber": seatNumber,
        "TransactionTime": transTime,
        "TransactionAmount": amount + amount * 5 / 100,
        "AircraftId": airCraftId,
        "DepartureAirportId": departureAirport,
        "ArrivalAirportId": arrivalAiport,
        "DepartureTime": departureTime,
        "ArrivalTime": arrivalTime,
        "Price": amount
    };

    console.log(ticketData);

    $.ajax({
        url: "https://localhost:7129/Booking/createTicket",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(ticketData),
        success: function (response) {
            console.log("Tạo vé ở đại lý thành công");
            console.log(response);
        },
        error: function (xhr, status, error) {
            console.log("Tạo vé ở đại lý thất bại!");
            console.log(xhr.responseText);
            alert("Đặt vé o dai li thất bại. Vui lòng thử lại sau.");
        }
    });
}

/*function createTicket(flightDetails) {
    var aircraftName = flightDetails.aircraftName;
    var departureAirportName = flightDetails.departureAirportName;
    var arrivalAirPortName = flightDetails.arrivalAirPortName;
    var departureTime = flightDetails.departureTime;
    var arrivalTime = flightDetails.arrivalTime;
    var price = flightDetails.price;
    var totalEmptySeat = flightDetails.totalEmptySeat;
    var flightId = $("#flightId").val();
    var email = $("#email").val();
    var fullName = $("#name").val();
    var identificationNumber = $("#identificationNumber").val();
    var amount = $("#amount").val();
    var userId = $("#userId").val();
    var cmnd = $("#cmnd").val();
    var birthday = $("#birthday").val();
    var phone = $("#phone").val();
    var transTime = $("#transTime").val();
    var airCraftId = flightDetails.aircraftId;
    var arrivalAiport = flightDetailsarrivalAirPortId;
    var departureAirport = flightDetails.departureAirportId;
    var seatNumber = response.seatNumber;

    var data = {
        aircraftName, 
        departureAirportName,
        arrivalAirPortName,
        departureTime,
        arrivalTime,
        price ,
        totalEmptySeat,
        flightId,
        email ,
        fullName ,
        identificationNumber,
        amount ,
        userId,
        cmnd ,
        birthday,
        phone,
        transTime 
    };
    console.log(data);
    var ticket = {
        "FlightId": flightId,
        "UserId": userId,
        "Cmnd": cmnd,
        "FullName": fullName,
        "Birthday": birthday,
        "Phone": phone,
        "Email": email,
        "SeatNumber": seatNumber,
        "TransactionTime": transTime,
        "TransactionAmount": amount + amount * 5 / 100,
        "AircraftId": airCraftId,
        "DepartureAirportId": departureAirport,
        "ArrivalAirportId": arrivalAiport,
        "DepartureTime": departureTime,
        "ArrivalTime": arrivalTime,
        "Price": amount
    };  

    $.ajax({
        url: "@Url.Action("AddTicket", "Booking")",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(ticket),
        success: function (response) {
            console.log("Tạo vé ở đại lý thành công");
            console.log(response);
        },
        error: function (xhr, status, error) {
            console.log("Tạo vé ở đại lý thất bại!");
            console.log(xhr.responseText);
            alert("Đặt vé o dai li thất bại. Vui lòng thử lại sau.");
        }
    });
}



*/