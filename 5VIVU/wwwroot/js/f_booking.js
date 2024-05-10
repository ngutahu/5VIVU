function flightDetails() {
    var flightId = $("#flightId").val();
    $.ajax({
        type: "GET",
        url: "https://localhost:7019/api/Flights/" + flightId,
        dataType: "json",
        success: function (flightDetails) {
            var aircraftName = flightDetails.aircraftName;
            var departureAirport = flightDetails.departureAirportName;
            var arrivalAirport = flightDetails.arrivalAirPortName;
            var departureTime = flightDetails.departureTime;
            var arrivalTime = flightDetails.arrivalTime;
            var price = flightDetails.price + (flightDetails.price * 5 / 100);
            var totalEmptySeat = flightDetails.totalEmptySeat;

            console.log("Aircraft Name:", aircraftName);
            console.log("Departure Airport:", departureAirport);
            console.log("Arrival Airport:", arrivalAirport);
            console.log("Departure Time:", departureTime);
            console.log("Arrival Time:", arrivalTime);
            console.log("Price:", price);
            console.log("Total Empty Seat:", totalEmptySeat);
            $("#price").val(price);
        },
        error: function (req, status, error) {
            console.log(status);
        }
    });
}

$(document).ready(function () {
    flightDetails();
});


$(document).ready(function () {
    var flightId = @ViewData["flightId"];
    $.ajax({
        url: "https://localhost:7019/api/Tickets/all?flightId=" + flightId,
        type: "GET",
        success: function (allSeats) {
            $.ajax({
                url: "https://localhost:7019/api/Tickets/booked?flightId=" + flightId,
                type: "GET",
                success: function (bookedSeats) {
                    $.ajax({
                        url: "https://localhost:7019/api/Tickets/unbooked?flightId=" + flightId,
                        type: "GET",
                        success: function (unbookedSeats) {
                            displaySeats(allSeats, bookedSeats, unbookedSeats);
                        },
                        error: function (xhr, status, error) {
                            console.log("Error fetching unbooked seats:", error);
                        }
                    });
                },
                error: function (xhr, status, error) {
                    console.log("Error fetching booked seats:", error);
                }
            });
        },
        error: function (xhr, status, error) {
            console.log("Error fetching all seats:", error);
        }
    });

    function displaySeats(allSeats, bookedSeats, unbookedSeats) {
        var seatContainer = $("#seatContainer");
        allSeats.forEach(function (seat) {
            var seatElement = $("<div class='seat'></div>");
            seatElement.text(seat.seatNumber);
            if (bookedSeats.some(function (bookedSeat) {
                return bookedSeat.seatNumber === seat.seatNumber;
            })) {
                seatElement.addClass("booked");
            } else {
                seatElement.click(function () {
                    var selectedSeatInput = $("#selectedSeat");
                    if (!selectedSeatInput.hasClass("booked")) {
                        selectedSeatInput.val(seat.seatNumber);
                    }
                });
            }
            seatContainer.append(seatElement);
        });
    }
});