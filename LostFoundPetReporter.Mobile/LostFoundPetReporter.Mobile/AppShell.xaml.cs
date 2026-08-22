using LostFoundPetReporter.Mobile.Views;

namespace LostFoundPetReporter.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            nameof(CreateLostReportPage),
            typeof(CreateLostReportPage));

        Routing.RegisterRoute(
            nameof(CreateFoundReportPage),
            typeof(CreateFoundReportPage));

        Routing.RegisterRoute("specificlostreport", typeof(SpecificLostReportPage));
    }
}