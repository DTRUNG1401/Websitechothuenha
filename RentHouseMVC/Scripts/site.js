/* =====================================================================
   RentHouse - JavaScript phía khách
   ===================================================================== */
(function ($) {
    'use strict';

    // Hiển thị lỗi của jQuery Validation theo kiểu Bootstrap 5 (viền đỏ ô nhập)
    if ($.validator) {
        $.validator.setDefaults({
            highlight: function (element) {
                $(element).addClass('is-invalid');
            },
            unhighlight: function (element) {
                $(element).removeClass('is-invalid input-validation-error');
            }
        });
    }

    // Ô chọn có data-auto-submit: tự gửi form khi đổi lựa chọn (VD: sắp xếp kết quả)
    $(document).on('change', 'select[data-auto-submit]', function () {
        if (this.form) {
            this.form.submit();
        }
    });

    // Nút "lên đầu trang"
    var $backToTop = $('#backToTop');
    $(window).on('scroll', function () {
        $backToTop.toggleClass('show', $(this).scrollTop() > 400);
    });
    $backToTop.on('click', function () {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });
})(jQuery);
