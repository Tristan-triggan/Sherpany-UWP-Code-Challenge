using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Sherpany_UWP_Code_Challenge.Messages;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Sherpany_UWP_Code_Challenge
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPageView : Page
    {
        public MainPageView()
        {
            this.InitializeComponent();
            Messenger.Default.Register<StartAnimationMessage>(this, m => CloseAppAnimation.Begin());
        }

        private void OnDragableGridManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            DragableGridTransform.X += e.Delta.Translation.X;
            DragableGridTransform.Y += e.Delta.Translation.Y;
        }

        private void OnDragableGridManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            Button.IsEnabled = false;
        }

        private void OnDragableGridManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Button.IsEnabled = true;
        }

        private void TextBoxBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }
    }
}
