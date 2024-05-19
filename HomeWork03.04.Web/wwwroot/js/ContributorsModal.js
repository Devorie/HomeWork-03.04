$(() => {
    $('.btn-success').on('click', function () {
        
        const contribId = $(this).data('contribid');
        $('[name="id"]').val(contribId);

        const tr = $(this).closest('tr');
        const name = tr.find('td:eq(1)').text();
        $("#deposit-name").text(name);

        console.log(contribId)

        new bootstrap.Modal($('.deposit')[0]).show();
    });

    let modalC = new bootstrap.Modal($(".new-contrib")[0]);

    $("#new-contributor").on('click', function () {
        modalC.show();
    });


    $('.btn-danger').on('click', function () {
        const contribId = $(this).data('contribid');
        $('[name="id"]').val(contribId);

        const firstName = $(this).data('firstName');
        const lastName = $(this).data('lastName');
        const cell = $(this).data('cell');
        const alwaysInclude = $(this).data('alwaysInclude');

        console.log(cell)

        $('[name="firstName"]').val(firstName);
        $('[name="lastName"]').val(lastName);
        $('[name="cell"]').val(cell);
        $('[name="alwaysInclude"]').prop('checked', alwaysInclude === "True");


        new bootstrap.Modal($(".edit-contrib")[0]).show();
    });
});