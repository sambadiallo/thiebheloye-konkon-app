using DryIoc;
using Prism.DryIoc;
using KonKon.Mobile.Views;
using Xamarin.Forms;

namespace KonKon.Mobile
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.NavigateAsync("NavigationPage/MainPage?title=Hello from Xamarin.Forms");
        }

        protected override void RegisterTypes()
        {
            NavigationRegistation();
        }

        private void NavigationRegistation()
        {
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MainPage>();
        }
    }
}
