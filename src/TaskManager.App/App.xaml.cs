using Microsoft.Maui.Controls;
using System;

namespace TaskManager.App;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        ApplyTheme(RequestedTheme);

        RequestedThemeChanged += (s, a) =>
        {
            ApplyTheme(a.RequestedTheme);
        };
    }

    public static void ApplyTheme(AppTheme theme)
    {
        if (Microsoft.Maui.Controls.Application.Current == null) return;
        var res = Microsoft.Maui.Controls.Application.Current.Resources;
        bool isDark = theme == AppTheme.Dark;

        if (res.TryGetValue("DarkBackground", out var darkBg) && res.TryGetValue("LightBackground", out var lightBg))
        {
            res["BackgroundColor"] = isDark ? darkBg : lightBg;
        }
        if (res.TryGetValue("DarkSurface", out var darkSurf) && res.TryGetValue("LightSurface", out var lightSurf))
        {
            res["SurfaceColor"] = isDark ? darkSurf : lightSurf;
        }
        if (res.TryGetValue("DarkSidebarBackground", out var darkSb) && res.TryGetValue("LightSidebarBackground", out var lightSb))
        {
            res["SidebarBackgroundColor"] = isDark ? darkSb : lightSb;
        }
        if (res.TryGetValue("DarkCardBackground", out var darkCard) && res.TryGetValue("LightCardBackground", out var lightCard))
        {
            res["CardBackgroundColor"] = isDark ? darkCard : lightCard;
        }
        if (res.TryGetValue("DarkCardBorder", out var darkCb) && res.TryGetValue("LightCardBorder", out var lightCb))
        {
            res["CardBorderColor"] = isDark ? darkCb : lightCb;
        }
        if (res.TryGetValue("DarkTextPrimary", out var darkTp) && res.TryGetValue("LightTextPrimary", out var lightTp))
        {
            res["TextPrimaryColor"] = isDark ? darkTp : lightTp;
        }
        if (res.TryGetValue("DarkTextSecondary", out var darkTs) && res.TryGetValue("LightTextSecondary", out var lightTs))
        {
            res["TextSecondaryColor"] = isDark ? darkTs : lightTs;
        }
        if (res.TryGetValue("DarkTextTertiary", out var darkTt) && res.TryGetValue("LightTextTertiary", out var lightTt))
        {
            res["TextTertiaryColor"] = isDark ? darkTt : lightTt;
        }
        if (res.TryGetValue("DarkAccent", out var darkAcc) && res.TryGetValue("LightAccent", out var lightAcc))
        {
            res["AccentColor"] = isDark ? darkAcc : lightAcc;
        }
        if (res.TryGetValue("DarkAccentLight", out var darkAccL) && res.TryGetValue("LightAccentLight", out var lightAccL))
        {
            res["AccentLightColor"] = isDark ? darkAccL : lightAccL;
        }
        if (res.TryGetValue("DarkSuccess", out var darkSuc) && res.TryGetValue("LightSuccess", out var lightSuc))
        {
            res["SuccessColor"] = isDark ? darkSuc : lightSuc;
        }
        if (res.TryGetValue("DarkWarning", out var darkWarn) && res.TryGetValue("LightWarning", out var lightWarn))
        {
            res["WarningColor"] = isDark ? darkWarn : lightWarn;
        }
        if (res.TryGetValue("DarkError", out var darkErr) && res.TryGetValue("LightError", out var lightErr))
        {
            res["ErrorColor"] = isDark ? darkErr : lightErr;
        }
        if (res.TryGetValue("DarkUrgent", out var darkUrg) && res.TryGetValue("LightUrgent", out var lightUrg))
        {
            res["UrgentColor"] = isDark ? darkUrg : lightUrg;
        }
        if (res.TryGetValue("DarkBorder", out var darkBrd) && res.TryGetValue("LightBorder", out var lightBrd))
        {
            res["BorderColor"] = isDark ? darkBrd : lightBrd;
        }
        if (res.TryGetValue("DarkDivider", out var darkDiv) && res.TryGetValue("LightDivider", out var lightDiv))
        {
            res["DividerColor"] = isDark ? darkDiv : lightDiv;
        }
        if (res.TryGetValue("DarkHover", out var darkHov) && res.TryGetValue("LightHover", out var lightHov))
        {
            res["HoverColor"] = isDark ? darkHov : lightHov;
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var mainPage = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<MainPage>(_serviceProvider);
        var window = new Window(new NavigationPage(mainPage))
        {
            Title = "FlowTask - Productivity & Task Management"
        };
        return window;
    }
}