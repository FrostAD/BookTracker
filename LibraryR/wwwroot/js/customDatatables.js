$(document).ready(function () {
    $("#recordDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/records",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            targets: 1, render: function (data) {
                return moment(data).format('MMMM Do YYYY');
            }
        }],
        "columns": [
            {
                "data": "bookTitle", "name": "BookTitle", "title": "Title",
                fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                    if (oData.bookTitle) {
                        $(nTd).html("<a href='/record/details?BookId=" + oData.record.bookId +"&UserId=" + oData.record.userId + "'>" + oData.bookTitle + "</a>");
                    }
                }
            },
            { "data": "record.recordDate", "name": "Record.RecordDate", "title": "Date", "autoWidth": true },
            { "data": "statusString", "name": "StatusString", "title": "Status", "autoWidth": true },
        ]
    });
    $("#authorDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/authors",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "firstName", "name": "FirstName",
                fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                    if (oData.firstName) {
                        var name = oData.firstName + " ";
                        if (oData.lastName != null)
                            name += oData.lastName;
                        $(nTd).html("<a href='/author/details/" + oData.id + "'>" + name + "</a>");
                    }
                }
            },

            { "data": "bornYear", "name": "BornYear", "autoWidth": true },
        ]
    });
    $("#bookDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/books",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "title", "name": "Title",
                fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                    if (oData.title) {
                        $(nTd).html("<a href='/book/details/" + oData.id + "'>" + oData.title + "</a>");
                    }
                }
            },

            { "data": "year", "name": "Year", "autoWidth": true },
        ]
    });
});  