// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.


//Jquery end//
$(document).ready(function () {
    $('.count').prop('disabled', true);
    $(document).on('click', '.plus', function () {
        $('.count').val(parseInt($('.count').val()) + 1);
    });
    $(document).on('click', '.minus', function () {
        $('.count').val(parseInt($('.count').val()) - 1);
        if ($('.count').val() == 0) {
            $('.count').val(1);
        }
    });
});
$('.qtybtn').on('click', function () {


    var d = $("#hiddenId").val();

    var $button = $(this);

    var oldValue = $button.parent().find('input').val();

    var newVal = parseFloat(oldValue);
    if ($button.hasClass('inc')) {
        $.ajax({
            type: 'GET',
            url: '/Home/GetData/?id=' + d,
            dataType: 'JSON',
            success: function (data) {

                newVal = parseFloat(oldValue) + 1;
                $button.parent().find('input').val(newVal);
                if (newVal > data) {
                    alert("Not Available!");
                    newVal = newVal - 1;
                    $button.parent().find('input').val(newVal);
                    console.log(newVal);
                    return;
                }

            },
            error: function (err) {
                console.log(err)
            }
        });


    } else {
        newVal = newVal - 1;
        // Don't allow decrementing below zero
        if (oldValue <= 0) {
            newVal = 0;
        }
    }
    $button.parent().find('input').val(newVal);
});


//Increment-Decrement Quantity of cart page

$(document).ready(function () {

    document.getElementById("sbtotal").readOnly = true;
    document.getElementById("qt").readOnly = true;
    document.getElementById("sbtotal").style.color = "#black";

    totalIt();


    //1. Increment Decrement

    $('.quantitybtn').on('click', function () {
        var $row = $(this);
        var $row0 = $(this).closest("div");

        var d = $(".Id").val();
        var oldValue = $row.parent().find('.quantity').val();
        if ($row.hasClass('inc')) {

            var newVal = parseFloat(oldValue);

            var $row1 = $(this).closest("div"),
                prce = parseFloat($row1.find('.price').val()),
                qnty = parseFloat($row1.find('.quantity').val()),
                productId = $row1.find('.Id').val(),


                subTotal = (prce * qnty);


            $row1.find('.subtotal').val(isNaN(subTotal) ? 0 : subTotal);



            $.ajax({
                type: 'GET',
                url: '/Home/GetData/?id=' + d,
                dataType: 'JSON',
                success: function (data) {

                    newVal = parseFloat(oldValue) + 1;
                    $row.parent().find('.quantity').val(newVal);
                    if (newVal <= data) {



                        $row.parent().find('.quantity').val(newVal);
                        qnty = newVal;
                        subTotal = (prce * qnty);
                        $row1.find('.subtotal').val(subTotal);
                        console.log(qnty);
                        totalIt();
                    }
                    else {
                        alert("Not Available!");
                        newVal = newVal - 1;
                        $row.parent().find('.quantity').val(newVal);

                        return;
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });



            totalIt();
            $.ajax({
                type: "POST",
                url: "/Home/Cart",
                data: { number1: qnty, number2: subTotal, number3: productId },
                dataType: "text",
                success: function (msg) {
                    console.log();
                },
                error: function (req, status, error) {
                    console.log();
                }
            });


        }
        else if ($row.hasClass('dec')) {
            // Don't allow decrementing below zero
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
                $row.parent().find('.quantity').val(newVal);
                var $row1 = $(this).closest("div"),
                    prce = parseFloat($row1.find('.price').val()),
                    qnty = parseFloat($row1.find('.quantity').val()),
                    productId = $row1.find('.Id').val(),


                    subTotal = (prce * qnty);
                $row1.find('.subtotal').val(isNaN(subTotal) ? 0 : subTotal);
                $row1.find('.quantity').val(isNaN(qnty) ? 0 : qnty);

                $.ajax({
                    type: "POST",
                    url: "/Home/Cart",
                    data: { number1: qnty, number2: subTotal, number3: productId },
                    dataType: "text",
                    success: function (msg) {
                        console.log(msg);
                    },
                    error: function (req, status, error) {
                        console.log(msg);
                    }
                });




                totalIt();


            } else {
                newVal = 0;
            }
        }
        $row.parent().find('.quantity').val(newVal);
    });


    //2. calculation
    function totalIt() {
        var total = 0;
        var tax = 0;
        $(".subtotal").each(function () {
            var val = this.value;
            tax +=  val * (10 / 100);
            total += val == "" || isNaN(val) ? 0 : parseFloat(val);
        });
        $("#Total").val(total);
        $("#Ttax").val(tax);
        $("#VatTotal").val(total + tax);
    }




});


// crud operation start
function loadImg(event) {
    var output = document.getElementById('mainimgDiv');
    var primaryimg = document.createElement("img");
    primaryimg.setAttribute("id", "primaryimg");
    var output = document.getElementById('mainimgDiv');
    primaryimg.src = URL.createObjectURL(event.target.files[0]);
    localStorage.setItem('primaryimg', primaryimg);
    primaryimg.onload = function () {
        URL.revokeObjectURL(primaryimg.src) // free memory
    }
    output.appendChild(primaryimg);

    if (event.target.files.length > 1) {
        var parent = document.getElementById('galleryParent');
        for (var i = 0; i < event.target.files.length; i++) {
            var childDiv = document.createElement("div");
            var gallery = document.createElement("img");
            var gallery = document.createElement("img");
            childDiv.setAttribute("class", "col-md-2 col-3 col-lg-2 removeclass" + i);
            gallery.setAttribute("id", "galleryimg");
            gallery.setAttribute("class", "minclass" + i);

            gallery.src = URL.createObjectURL(event.target.files[i]);
            gallery.onload = function () {
                URL.revokeObjectURL(gallery.src) // free memory
            }
            childDiv.appendChild(gallery);
            parent.appendChild(childDiv);

            var StoredDiv = $('.removeclass' + i).html();
            manage_append(i, StoredDiv);
        }
        function manage_append(i, html) {

            localStorage.setItem(i, html);
            console.log("html", html);
            //  localStorage.removeItem(i);    
        }
    }
};
//For small popup start
ShowInSmallPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#small-modal .modal-body').html(res);
            $('#small-modal .modal-title').html(title);
            $('#small-modal').modal('show');

            $("#Designation").change(function () {
                var d = $("#Designation option:selected").val();
                $.ajax({
                    type: 'GET',
                    url: '/Employees/GetDesignationSalary/?id=' + d,
                    dataType: 'JSON',
                    success: function (data) {
                        console.log("salary", data)
                        var salary = document.getElementById("Salary");
                        salary.value = data;
                    },
                    error: function (err) {
                        console.log(err)
                    }
                })
            });
            $("#Designation").trigger("change");
        }
    });

}
ShowInLargePopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {

            $('#large-modal .modal-body').html(res);
            $('#large-modal .modal-title').html(title);
            $('#large-modal').modal('show');

            //$(".subcategory").v.empty();
            //$(".brand").empty();
            $(".category").click(function (e) {
                var selectedCategoryId = $(".category option:selected").val();

                $(".subcategory").empty();
                $(".brand").empty();
                $.ajax({
                    url: '/Products/GetAllSubCategory/?id=' + selectedCategoryId,
                    type: 'GET',
                    dataType: 'JSON',
                    success: function (data) {
                        $.each(data, function (i, obj) {
                            var s = '<option value="' + obj.id + '">' + obj.name + '</option>';
                            console.log("ggg", obj);
                            $(".subcategory").append(s);
                        });
                    },
                    error: function (res) {
                        console.log(res);
                    }
                })


            })
            $(".subcategory").click(function (e) {
                $(".brand").empty();
                var selectedSubCategoryId = $(".subcategory option:selected").val();
                $.ajax({
                    url: '/Products/GetAllBrand/?id=' + selectedSubCategoryId,
                    type: 'GET',
                    dataType: 'JSON',
                    success: function (data) {
                        $.each(data, function (i, obj) {
                            var s = '<option value="' + obj.id + '">' + obj.name + '</option>';
                            console.log("ggg", obj);
                            $(".brand").append(s);
                        });
                    },
                    error: function (res) {
                        console.log(res);
                    }
                })
            })

            //just for this portion i add 2 popup function
        }
    });

}

//Large Modal
jQueryAjaxPostLargeModal = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                console.log("res", res)
                if (res.isValid) {

                    $('#view-all').html(res.html)
                    $('#large-modal .modal-body').html('');
                    $('#large-modal .modal-title').html('');
                    $('#large-modal').modal('hide');
                    toastr.success("Successfully", "Save");
                }
                else
                    $('#large-modal .modal-body').html(res.html);
            },
            error: function (err) {
                console.log(err, "errrrrrrrrrr")
                toastr.success(" ", "Faild");


            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                console.log("res", res)
                if (res.isValid) {

                    $('#view-all').html(res.html)
                    $('#small-modal .modal-body').html('');
                    $('#small-modal .modal-title').html('');
                    $('#small-modal').modal('hide');
                    toastr.success("Successfully", "Save");
                }
                else
                    $('#small-modal .modal-body').html(res.html);
            },
            error: function (err) {
                console.log("res", err)
                toastr.success(" ", "Faild");


            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

//For small popup end

// crud operation end


jQueryAjaxPostSalaryModal = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                console.log(res)
                if (res.isValid) {

                    $('#view-all').html(res.html)
                    $('#large-modal .modal-body').html('');
                    $('#large-modal .modal-title').html('');
                    $('#large-modal').modal('hide');
                    toastr.success("Successfully", "Save");
                }
                else
                    $('#large-modal .modal-body').html(res.html);
            },
            error: function (err) {
                console.log(err, "errrrrrrrrrr")
                toastr.success(" ", "Faild");


            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}


ShowInSalaryBillLargePopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {

            $('#large-modal .modal-body').html(res);
            $('#large-modal .modal-title').html(title);
            $('#large-modal').modal('show');

        }
    });

}




showInPopup1 = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');


        }
    })
}
//for post data
jQueryAjaxPost1 = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                console.log("view all", res)
                if (res.isValid) {

                    $('#view-all').html(res.html)
                    toastr.success("Crated successfully");
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');

                }
                else
                    $('#form-modal .modal-body').html(res.html);
            },
            error: function (err) {
                console.log('fgdfgfgfd', err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}
///for delete
Delete = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {

            if (res.isValid) {
                $('#view-all').html(res.html)
                toastr.success("Delete Success");
            } else {
                toastr.warning("Delete Wrong");
            }
        },
        error: function (res) {
            console.log(res);
        }
    });
}



$('.owl-carousel').owlCarousel({
    loop: true,
    margin: 10,
    nav: false,
    dot: false,
    responsive: {
        0: {
            items: 1
        },
        600: {
            items: 3
        },
        1000: {
            items: 5
        }
    }
})

var slideIndex = [1, 1];
var slideId = ["mySlides1", "mySlides2"]
showSlides(1, 0);
showSlides(1, 1);

function plusSlides(n, no) {
    showSlides(slideIndex[no] += n, no);
}

function showSlides(n, no) {
    var i;
    var x = document.getElementsByClassName(slideId[no]);
    if (n > x.length) { slideIndex[no] = 1 }
    if (n < 1) { slideIndex[no] = x.length }
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    x[slideIndex[no] - 1].style.display = "block";
}




    

AOS.init();