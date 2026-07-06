// Tiện ích trình duyệt cho MonAnCuaEm.
window.appInterop = {
    // Thu nhỏ + nén ảnh về JPEG để LocalStorage không bị tràn.
    // Trả về data URL; nếu lỗi thì trả lại ảnh gốc.
    resizeImage: function (dataUrl, maxSize, quality) {
        return new Promise(function (resolve) {
            try {
                var img = new Image();
                img.onload = function () {
                    var width = img.width;
                    var height = img.height;
                    if (width > maxSize || height > maxSize) {
                        if (width >= height) {
                            height = Math.round(height * maxSize / width);
                            width = maxSize;
                        } else {
                            width = Math.round(width * maxSize / height);
                            height = maxSize;
                        }
                    }
                    var canvas = document.createElement('canvas');
                    canvas.width = width;
                    canvas.height = height;
                    var ctx = canvas.getContext('2d');
                    ctx.drawImage(img, 0, 0, width, height);
                    resolve(canvas.toDataURL('image/jpeg', quality));
                };
                img.onerror = function () { resolve(dataUrl); };
                img.src = dataUrl;
            } catch (e) {
                resolve(dataUrl);
            }
        });
    }
};
