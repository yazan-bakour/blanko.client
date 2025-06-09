using System.Collections.Generic;
using MudBlazor;
using MudBlazor.Utilities;

namespace Banko.Client.Helper
{
  public static class ThemeCustomColors
  {

    // light palette
    public const string LightGradientGlow = "radial-gradient(50% 50% at 50% 50%, #e0e0e0 0%, rgba(0, 38, 77, 0) 100%),radial-gradient(50% 50% at 50% 50%, #e0e0e0 0%, rgba(0, 38, 77, 0) 100%), radial-gradient(50% 50% at 50% 50%, #e0e0e0 0%, rgba(0, 38, 77, 0) 100%)";
    public const string LightGradientImage = "linear-gradient(271deg,rgb(148, 173, 255) 20%,rgb(145, 137, 255) 50%,rgb(202, 201, 241) 70%)";
    public static class Light
    {
      public static readonly MudColor Primery = new("#00468d");
      public static readonly MudColor ActionDisabledBackground = new MudColor("#00468d").SetAlpha(0.2).ToString(MudColorOutputFormats.RGBA);
      public static readonly MudColor ActionDefault = new("#00468d"); //--mud-palette-action-default-hover
      public static readonly MudColor LightAppbarBackground = new("#f0f0f0");
    }

    // // dark palette
    public const string DarkGradientGlow = "radial-gradient(50% 50% at 50% 50%, #00264d 0%, rgba(0, 38, 77, 0) 100%), radial-gradient(50% 50% at 50% 50%, #00264d 0%, rgba(0, 38, 77, 0) 100%), radial-gradient(50% 50% at 50% 50%, #00264d 0%, rgba(0, 38, 77, 0) 100%)";
    public const string DarkGradientImage = "linear-gradient(271deg, #001a33 20%, #00264d 50%, #00468d 70%)";
    public static class Dark
    {
      public static readonly MudColor Primary = new("#092f52");
      public static readonly MudColor DarkAppbarBackground = new("#001a33");
    }


    public static MudTheme AppTheme { get; } = new MudTheme()
    {
      PaletteLight = new PaletteLight()
      {
        Primary = Light.Primery,
        Secondary = Colors.Indigo.Darken2,
        Surface = Colors.Blue.Lighten5,
        TextPrimary = Light.Primery,
        TextDisabled = Light.Primery,
        DrawerText = Light.Primery,
        DrawerIcon = Light.Primery,
        LinesInputs = Colors.Gray.Darken2,
        Divider = Colors.Gray.Darken2,
        TableLines = Colors.Gray.Darken1,
        ActionDefault = Light.ActionDefault,
        ActionDisabled = Light.ActionDefault,
        ActionDisabledBackground = Light.ActionDisabledBackground,
        HoverOpacity = 0.10,
        LinesDefault = Colors.Gray.Darken2,
        Dark = Light.Primery
      },
      PaletteDark = new PaletteDark()
      {
        Primary = Colors.LightBlue.Lighten5,
        Secondary = Colors.Indigo.Darken3,
        Dark = Colors.LightBlue.Darken4,
        Info = Colors.Blue.Accent1,
        TextDisabled = Colors.Gray.Lighten4,
        Surface = Dark.Primary,
        TextPrimary = Colors.Gray.Lighten4,
        ActionDefault = Dark.Primary,
        // RippleOpacity = 0.5,
        HoverOpacity = 1,
      },

      LayoutProperties = new LayoutProperties()
      {
        DefaultBorderRadius = "5px",
        // DrawerWidthRight = "260px",
        // DrawerWidthLeft = "200px"
      }
    };

  }
}