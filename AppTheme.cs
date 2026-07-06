using MudBlazor;

namespace MonAnCuaEm;

/// <summary>Bộ màu chủ đạo tím pastel (lavender) và bo góc mềm cho toàn app.</summary>
public static class AppTheme
{
    public static readonly MudTheme Lavender = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#8A6FD4",          // tím lilac
            Secondary = "#B79CE0",        // tím nhạt
            Tertiary = "#F2A9C4",         // hồng pastel nhấn
            Background = "#FAF8FE",       // nền tím rất nhạt
            Surface = "#FFFFFF",
            AppbarBackground = "#8A6FD4",
            AppbarText = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#4A4458",
            DrawerIcon = "#8A6FD4",
            TextPrimary = "#332F3D",
            TextSecondary = "#6E6980",
            ActionDefault = "#8A6FD4",
            Success = "#6FB894",
            Info = "#8FB7E8",
            Warning = "#E0A94E",
            Error = "#E37B93",
            Divider = "#ECE6F6",
            LinesDefault = "#ECE6F6",
            TableLines = "#ECE6F6"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "16px",
            DrawerWidthLeft = "260px",
            AppbarHeight = "64px"
        }
    };
}
