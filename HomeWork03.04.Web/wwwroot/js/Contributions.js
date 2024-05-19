$(() => {

    $("form").on('submit', function () {
        let index = 0;
        $(".person-row").each(function () {
            const row = $(this);
            if (('#checkbox')) {
                row.find('.contributorId').attr('name', `Contributions[${index}].contributorId`);
                row.find('.amount').attr('name', `Contributions[${index}].amount`);
                index++;
            }
            });
    });
});