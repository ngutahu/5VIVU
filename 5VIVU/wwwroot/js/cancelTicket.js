function showCancellationMessage() {
    var departureDateString = $("#departureTime").val();
    var departureDate = new Date(departureDateString);
    var currentDate = new Date();


    var timeDiff = departureDate.getTime() - currentDate.getTime();


    var daysDiff = Math.ceil(timeDiff / (1000 * 3600 * 24));


    if (daysDiff <= 10) {
        alert("Bạn chỉ có thể hủy vé trước ngày bay ít nhất 10 ngày.");
        return false;
    }
    alert("Vé đã được hủy thành công.");

    return true;
}