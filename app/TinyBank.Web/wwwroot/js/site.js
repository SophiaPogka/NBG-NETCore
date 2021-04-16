// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.js-update-customer').on('click',
    (event) => {
        debugger;
        let firstName = $('.js-first-name').val();
        let lastName = $('.js-last-name').val();
        let customerId = $('.js-customer-id').val();

        console.log(`${firstName} ${lastName}`);

        let data = JSON.stringify({
            firstName: firstName,
            lastName: lastName
        });

        // ajax call
        let result = $.ajax({
            url: `/customer/${customerId}`,
            method: 'PUT',
            contentType: 'application/json',
            data: data
        }).done(response => {
            console.log('Update was successful');
            // success
        }).fail(failure => {
            // fail
            console.log('Update failed');
        });
    });

$('.js-customers-list tbody tr').on('click',
    (event) => {
        console.log($(event.currentTarget).attr('id'));
    });


$('.js-card-payment').on('click', (event) => {

    $('.js-alert-success').addClass('collapse in');
    $('.js-alert-failure').addClass('collapse in');


    document.getElementById("cardNumber").disabled = true;
    document.getElementById("expirationMonth").disabled = true;
    document.getElementById("expirationYear").disabled = true;
    document.getElementById("amount").disabled = true;
    document.getElementById("paymentButton").disabled = true;

    let cardNumber = $('.js-card-number').val();
    let expirationYear = $('.js-expiration-year').val();
    let expirationMonth = $('.js-expiration-month').val();
    let amount = $('.js-amount').val();


    let data = JSON.stringify(
        {
            cardNumber: cardNumber,
            expirationMonth: expirationMonth,
            expirationYear: expirationYear,
            amount: amount
        });


    // ajax call
    let result = $.ajax(
        {
            url: '/card/checkout',
            method: 'PUT',
            contentType: 'application/json',
            data: data
        }).done(response => {
            if (response.code == 200) {
                $('.js-alert-success').removeClass('collapse in');
            }
            else {
                $('.js-errorMsg').val(response.errorText);
                $('.js-alert-failure').removeClass('collapse in');
            }
        }).fail(failure => {
            alert('failure');
        }).always(() => {
            document.getElementById("cardNumber").disabled = false;
            document.getElementById("expirationMonth").disabled = false;
            document.getElementById("expirationYear").disabled = false;
            document.getElementById("amount").disabled = false;
            document.getElementById("paymentButton").disabled = false;
        });

});