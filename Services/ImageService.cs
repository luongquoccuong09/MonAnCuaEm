using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace MonAnCuaEm.Services;

/// <summary>
/// Đọc ảnh người dùng chọn, thu nhỏ + nén bằng canvas (JS interop) rồi trả về data URL base64.
/// Việc thu nhỏ giúp LocalStorage không bị tràn dung lượng.
/// </summary>
public class ImageService
{
    private const long MaxUploadBytes = 8 * 1024 * 1024; // 8MB ảnh gốc tối đa

    private readonly IJSRuntime _js;

    public ImageService(IJSRuntime js) => _js = js;

    public async Task<string> ToCompressedDataUrlAsync(IBrowserFile file, int maxSize = 900, double quality = 0.8)
    {
        using var stream = file.OpenReadStream(MaxUploadBytes);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var base64 = Convert.ToBase64String(ms.ToArray());
        var sourceDataUrl = $"data:{file.ContentType};base64,{base64}";

        // Thu nhỏ trong trình duyệt; nếu lỗi thì dùng ảnh gốc.
        return await _js.InvokeAsync<string>("appInterop.resizeImage", sourceDataUrl, maxSize, quality);
    }
}
