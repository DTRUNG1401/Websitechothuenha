/* =====================================================================
   RentHouse – JavaScript trang quản trị
   Lỗi nhập liệu dùng class mặc định của jQuery Validation Unobtrusive
   (input-validation-error, field-validation-error) – kiểu nằm trong theme.css.
   ===================================================================== */
(function ($) {
    'use strict';

    // Form có data-confirm: hỏi lại trước khi gửi (xóa, hủy, đổi trạng thái...)
    $(document).on('submit', 'form[data-confirm]', function (e) {
        if (!window.confirm($(this).attr('data-confirm'))) {
            e.preventDefault();
        }
    });

    // Kiểm tra ảnh ngay ở trình duyệt: đúng định dạng, dưới 2MB, xem trước ảnh
    $(document).on('change', 'input[type=file][data-max-size]', function () {
        var input = this;
        var maxSize = parseInt($(input).attr('data-max-size'), 10);
        var $error = $('#' + input.id + '-client-error');
        var preview = $(input).attr('data-preview');
        $error.text('');

        if (!input.files || input.files.length === 0) {
            return;
        }

        var file = input.files[0];
        if (!/\.(jpe?g|png|webp)$/i.test(file.name)) {
            $error.text('Chỉ nhận ảnh JPG, PNG hoặc WEBP.');
            input.value = '';
            return;
        }
        if (file.size > maxSize) {
            $error.text('Ảnh phải nhỏ hơn 2MB (ảnh đã chọn: ' + (file.size / 1024 / 1024).toFixed(2) + 'MB).');
            input.value = '';
            return;
        }
        if (preview && window.URL && window.URL.createObjectURL) {
            $(preview).attr('src', window.URL.createObjectURL(file));
        }
    });

    // Hiển thị giá đã định dạng dưới ô nhập: 6500000 => 6.500.000 đ/tháng
    var $price = $('#Price');
    var $pricePreview = $('#pricePreview');
    function updatePricePreview() {
        var value = parseInt($price.val(), 10);
        if (isNaN(value) || value <= 0) {
            $pricePreview.text('');
            return;
        }
        $pricePreview.text(value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.') + ' đ/tháng');
    }
    if ($price.length && $pricePreview.length) {
        $price.on('input', updatePricePreview);
        updatePricePreview();
    }
})(jQuery);
