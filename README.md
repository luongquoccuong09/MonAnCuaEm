# 🍚 Món Ăn Của Em

Ứng dụng **Blazor WebAssembly (PWA)** giúp quản lý công thức món ăn, lên lịch thực đơn theo tuần
và theo dõi chi phí bếp núc — chạy hoàn toàn phía trình duyệt, **không cần server**, **dùng được offline**
và **cài đặt được như app** trên MacBook (Safari/Chrome) lẫn điện thoại.

Tông màu tím pastel (lavender), bo góc mềm, giao diện tối giản ấm áp.

---

## ✨ Tính năng

### 1. Quản lý công thức món ăn
- Thêm / sửa / xoá món: tên, danh mục (Món chính / Canh / Tráng miệng / Ăn vặt), ghi chú, ảnh.
- Ảnh: **tải lên** (tự thu nhỏ + nén, lưu base64) hoặc **dán URL**.
- Danh sách nguyên liệu cho mỗi món: tên, số lượng, đơn vị, giá tiền → tự tính tổng chi phí món.
- Một dòng **ghi chú kỷ niệm** cho mỗi món (ví dụ: *"món này nấu hôm mình mới quen nhau"* 💜).

### 2. Lịch thực đơn theo tuần
- Xem theo tuần: 7 ngày × 3 buổi (Sáng / Trưa / Tối).
- Gán món đã tạo vào từng ô ngày/buổi; gỡ món chỉ với 1 chạm.
- Chuyển nhanh tuần trước / tuần sau / về tuần này.

### 3. Thống kê chi phí
- Tự động cộng chi phí nguyên liệu của các món trong lịch thực đơn.
- Nhập thêm **chi phí phát sinh** thủ công (ngày + số tiền + ghi chú).
- **Biểu đồ cột** chi phí theo tuần (8 tuần gần nhất) và theo tháng (6 tháng gần nhất).
- Thẻ tổng hợp: chi phí **tuần này / tháng này**, so sánh với kỳ trước (% tăng/giảm).

### 4. Trang chủ
- Thực đơn hôm nay (3 buổi), chi phí tuần này / tháng này, số món đã lưu — dạng thẻ đẹp.

---

## 🧱 Công nghệ

| Thành phần | Lựa chọn |
|---|---|
| Framework | .NET 8 · Blazor WebAssembly (Standalone, PWA) |
| UI | [MudBlazor](https://mudblazor.com/) |
| Biểu đồ | [Blazor-ApexCharts](https://github.com/apexcharts/Blazor-ApexCharts) |
| Lưu trữ | [Blazored.LocalStorage](https://github.com/Blazored/LocalStorage) (không cần backend) |

Dữ liệu được lưu trong **LocalStorage của trình duyệt** dưới các khoá `monan.recipes`,
`monan.mealplan`, `monan.expenses`. Xoá dữ liệu trình duyệt = xoá toàn bộ dữ liệu app.

---

## 📁 Cấu trúc thư mục

```
MonAnCuaEm/
├── Models/            # Lớp dữ liệu: Recipe, Ingredient, MealPlanEntry, ExtraExpense, enum...
├── Services/          # Nghiệp vụ + lưu trữ: RecipeService, MealPlanService,
│                      #   ExpenseService, StatisticsService, ImageService
├── Helpers/           # DisplayNames (nhãn tiếng Việt), DateHelper (tuần/tháng), Format (tiền tệ)
├── Components/        # Dialog dùng chung: RecipeDialog, AssignMealDialog, ExpenseDialog
├── Pages/             # Home, Recipes, MealPlan, Statistics
├── Layout/            # MainLayout (AppBar + Drawer), NavMenu
├── AppTheme.cs        # Bộ màu tím pastel + bo góc
├── Program.cs         # Đăng ký dịch vụ (DI)
└── wwwroot/           # index.html, css/app.css, js/app.js, manifest, service worker, icon
```

> Quy ước: **tên biến/class bằng tiếng Anh**, **nội dung hiển thị (label/text) bằng tiếng Việt**.

---

## ▶️ Chạy dự án (local)

### Yêu cầu
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) trở lên.

### Các bước
```bash
# 1. Vào thư mục dự án
cd MonAnCuaEm

# 2. Khôi phục gói (lần đầu)
dotnet restore

# 3. Chạy ở chế độ phát triển
dotnet run
```
Mở trình duyệt tới địa chỉ hiển thị trên terminal (ví dụ `http://localhost:5099`).

> Muốn tự động nạp lại khi sửa code: `dotnet watch run`.

---

## 📦 Build bản phát hành

```bash
dotnet publish -c Release -o release
```
Toàn bộ file tĩnh nằm trong `release/wwwroot/` — có thể đưa lên bất kỳ static host nào
(GitHub Pages, Netlify, Azure Static Web Apps, Cloudflare Pages...).

---

## 🚀 Deploy lên GitHub Pages

Kho đã kèm sẵn workflow **`.github/workflows/deploy.yml`** để tự động build và deploy.

### Cách 1 — Tự động bằng GitHub Actions (khuyến nghị)
1. Tạo repo trên GitHub và đẩy code lên nhánh `main`:
   ```bash
   git init
   git add .
   git commit -m "Khởi tạo Món Ăn Của Em"
   git branch -M main
   git remote add origin https://github.com/<user>/<repo>.git
   git push -u origin main
   ```
2. Trên GitHub: **Settings → Pages → Build and deployment → Source = GitHub Actions**.
3. Mỗi lần push lên `main`, workflow sẽ:
   - `dotnet publish` bản Release,
   - đổi `<base href="/" />` thành `<base href="/<repo>/" />` (cho *project site*),
   - thêm `.nojekyll` (để GitHub không bỏ qua thư mục `_framework`),
   - tạo `404.html` (để điều hướng SPA hoạt động khi tải thẳng vào URL con),
   - đẩy lên GitHub Pages.
4. App sẽ chạy tại `https://<user>.github.io/<repo>/`.

> **Lưu ý về `base href`:**
> - Dùng *project site* (`https://<user>.github.io/<repo>/`) → giữ nguyên workflow.
> - Dùng *user/organization site* (repo tên `<user>.github.io`, chạy ở gốc `/`) →
>   **xoá bước “Rewrite base href”** trong workflow.

### Cách 2 — Thủ công
```bash
dotnet publish -c Release -o release
# Sửa release/wwwroot/index.html: <base href="/<repo>/" />
# Thêm file rỗng release/wwwroot/.nojekyll
# Copy index.html thành 404.html
# Đẩy nội dung release/wwwroot lên nhánh gh-pages
```
Sau đó chọn **Settings → Pages → Source = Deploy from a branch → gh-pages / (root)**.

---

## 📱 Cài đặt như ứng dụng (PWA)

- **Chrome / Edge (MacBook, Windows):** nhấn biểu tượng *Install* trên thanh địa chỉ,
  hoặc menu → *Cài đặt Món Ăn Của Em*.
- **Safari (macOS):** menu *File → Add to Dock*.
- **iPhone/iPad (Safari):** nút *Chia sẻ* → *Thêm vào MH chính*.
- **Android (Chrome):** menu → *Thêm vào MH chính*.

Sau khi cài, app chạy được **offline** nhờ service worker.
Cập nhật mới sẽ tự tải sau khi bạn đóng/mở lại app.

---

## ⚠️ Ghi chú kỹ thuật
- Ảnh được **thu nhỏ tối đa 900px + nén JPEG** (trong `wwwroot/js/app.js`) trước khi lưu,
  giúp LocalStorage (giới hạn ~5MB) không bị tràn. Nếu bạn cần lưu **rất nhiều ảnh lớn**,
  nên chuyển sang IndexedDB — kiến trúc dịch vụ (lớp `*Service`) đã tách sẵn nên việc đổi chỗ lưu trữ khá dễ.
- Dữ liệu chỉ nằm trên **trình duyệt/máy hiện tại**, không đồng bộ giữa các thiết bị.

---

Made with 💜 for the person who cooks for me.
