using System.Windows;

namespace Restaurant_Pos
{
    public class WindowViewModel : BaseViewModel
    {
        private Window mWindow;

        //public ApplicationPage LoginPage { get; set; } = ApplicationPage.Login;
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Login;

        public WindowViewModel(Window window)
        {
            mWindow = window;
        }
    }
}