/* =====================================================================
   RentHouse – JavaScript phía khách
   Lỗi nhập liệu dùng class mặc định của jQuery Validation Unobtrusive
   (input-validation-error, field-validation-error) – kiểu nằm trong theme.css.
   ===================================================================== */
(function ($) {
    'use strict';

    // Ô chọn có data-auto-submit: tự gửi form khi đổi lựa chọn (ô "Sắp xếp")
    $(document).on('change', 'select[data-auto-submit]', function () {
        if (this.form) {
            this.form.submit();
        }
    });
})(jQuery);
