using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ProximalMenuPrototype.Controls
{
    public sealed partial class VerticalTaskbarDownControl : UserControl
    {
        public VerticalTaskbarDownControl() => InitializeComponent();

        public event TappedEventHandler Button1Tap;
        public event TappedEventHandler Button2Tap;
        public event TappedEventHandler Button3Tap;
        public event TappedEventHandler Button4Tap;
        public event TappedEventHandler Button5Tap;
        public event TappedEventHandler Button6Tap;

        private void Btn_1_Tapped(object sender, TappedRoutedEventArgs e) => Button1Tap?.Invoke(this, e);

        private void Btn_2_Tapped(object sender, TappedRoutedEventArgs e) => Button2Tap?.Invoke(this, e);

        private void Btn_3_Tapped(object sender, TappedRoutedEventArgs e) => Button3Tap?.Invoke(this, e);

        private void Btn_4_Tapped(object sender, TappedRoutedEventArgs e) => Button4Tap?.Invoke(this, e);

        private void Btn_5_Tapped(object sender, TappedRoutedEventArgs e) => Button5Tap?.Invoke(this, e);

        private void Btn_6_Tapped(object sender, TappedRoutedEventArgs e) => Button6Tap?.Invoke(this, e);
    }
}