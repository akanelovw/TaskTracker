$(document).ready(function () {
     $('#manager').select2({
        placeholder: "Select a manager",
        ajax: {
            url: '/Account/SearchUsers',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    term: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.results
                };
            }
        },
        width: '100%'
    });

    $('#employees').select2({
        placeholder: "Select employees",
        ajax: {
            url: '/Account/SearchUsers',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    term: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.results
                };
            }
        },
        multiple: true,
        width: '100%'
        });
    });