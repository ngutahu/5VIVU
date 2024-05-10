function getAllAirports() {
    $.ajax({
        type: "GET",
        url: "https://localhost:7019/api/Airports",
        dataType: "json",
        success: function (airports) {
            airports.forEach(function (airport) {
                var optionText = airport.airportName + " - " + airport.location;
                var optionValue = airport.airportId;
                $('#departureAirport').append($('<option>').val(optionValue).text(optionText));
                $('#arrivalAirport').append($('<option>').val(optionValue).text(optionText));
            });
        },
        error: function (req, status, error) {
            console.log(status);
        }
    });
}
getAllAirports();

function searchFlights() {
    var departureAirportId = $('#departureAirport').val();
    var arrivalAirportId = $('#arrivalAirport').val();
    var departureTime = $('#departureTime').val();

    $.ajax({
        type: "GET",
        url: "https://localhost:7019/api/Flights/searchFlights",
        data: {
            departureAirport: departureAirportId,
            arrivalAirport: arrivalAirportId,
            departureTime: departureTime
        },
        dataType: "json",
        success: function (flights) {
            $('#flightTableBody').empty();
            if (flights.length === 0) {
                $('#flightTableBody').html('<tr><td colspan="5">Không có chuyến bay được tìm thấy.</td></tr>');
            } else {
                flights.forEach(function (flight) {
                    var row = '<tr>' +
                        '<td>' + flight.aircraftName + '</td>' +
                        '<td>' + flight.departureAirportName + '</td>' +
                        '<td>' + flight.arrivalAirPortName + '</td>' +
                        '<td>' + flight.departureTime + '</td>' +
                        '<td><button class="btn btn-info" onclick="showFlightDetails(' + flight.flightId + ')">Đặt vé</button></td>' +
                        '</tr>';
                    $('#flightTableBody').append(row);
                });
                console.log(flights);
            }
        },
        error: function (req, status, error) {
            $('#flightTableBody').html('<tr><td colspan="5">Không có chuyến bay được tìm thấy hoặc hệ thống đang có lỗi.</td></tr>');
        }
    });
}

function showFlightDetails(flightId) {
    $.ajax({
        type: "GET",
        url: "https://localhost:7019/api/Flights/" + flightId,
        dataType: "json",
        success: function (flightDetails) {            
            console.log(flightDetails);
        },
        error: function (req, status, error) {
            console.log(status);
        }
    });
}