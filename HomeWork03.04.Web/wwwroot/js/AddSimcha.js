$(() => {
    let modal = new bootstrap.Modal($(".new-simcha")[0]);

    $("#new-simcha").on('click', function () {
        modal.show();
    });
});