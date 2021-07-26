$("#ajaxMultiSelect2").select2({
    placeholder: "Authors",
    theme: "bootstrap4",
    allowClear: true,
    ajax: {
        url: "/api/authors/search",
        contentType: "application/json; charset=utf-8",
        data: function (params) {
            var query =
            {
                term: params.term,
            };
            return query;
        },
        processResults: function (result) {
            return {
                results: $.map(result, function (item) {
                    return {
                        id: item.id,
                        text: item.firstName + ' ' + ((item.lastName) ? item.lastName : "")
                    };
                }),
            };
        }
    }
});
$("#ajaxSelect2Book").select2({
    placeholder: "Books",
    theme: "bootstrap4",
    allowClear: true,
    ajax: {
        url: "/api/books/search",
        contentType: "application/json; charset=utf-8",
        data: function (params) {
            var query =
            {
                term: params.term,
            };
            return query;
        },
        processResults: function (result) {
            return {
                results: $.map(result, function (item) {
                    return {
                        id: item.id,
                        text: item.title
                    };
                }),
            };
        }
    }
});
$("#ajaxSelect2").select2({
    placeholder: "Status",
    theme: "bootstrap4",
    allowClear: true,
    ajax: {
        url: "/api/records/search",
        contentType: "application/json; charset=utf-8",
        data: function (params) {
            var query =
            {
                term: params.term,
            };
            return query;
        },
        processResults: function (result) {
            return {
                results: $.map(result, function (item) {
                    return {
                        id: item.id,
                        text: item.type
                    };
                }),
            };
        }
    }
});
$("#ajaxSelect2RecordEditBook").select2({
    placeholder: "Status",
    theme: "bootstrap4",
    allowClear: true,
    ajax: {
        url: "/api/books/search",
        contentType: "application/json; charset=utf-8",
        data: function (params) {
            var query =
            {
                term: params.term,
            };
            return query;
        },
        processResults: function (result) {
            return {
                results: $.map(result, function (item) {
                    return {
                        id: item.id,
                        text: item.type
                    };
                }),
            };
        }
    }
});
$("#ajaxSelect2RecordEditStatus").select2({
    placeholder: "Status",
    theme: "bootstrap4",
    allowClear: true,
    ajax: {
        url: "/api/records/search",
        contentType: "application/json; charset=utf-8",
        data: function (params) {
            var query =
            {
                term: params.term,
            };
            return query;
        },
        processResults: function (result) {
            return {
                results: $.map(result, function (item) {
                    return {
                        id: item.id,
                        text: item.type
                    };
                }),
            };
        }
    }
});