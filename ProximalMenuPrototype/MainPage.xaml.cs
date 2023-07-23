using Microsoft.Toolkit.Uwp.UI.Animations;
using ProximalMenuPrototype.Controls;
using System;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ProximalMenuPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Delegates
        private RadialTaskbarControl radialTaskbar;
        private HorizontalTaskbarLTRControl horizontalTaskbarLTR;
        private HorizontalTaskbarRTLControl horizontalTaskbarRTL;
        private VerticalTaskbarDownControl verticalDownTaskbar;
        private VerticalTaskbarUpControl verticalUpTaskbar;
        private Canvas targetCanvas;
        private Point tappedPoint;
        private double posX;
        private double posY;
        private double slideFrom;
        private double slideTo;
        private double screenWidth;
        private double screenHeight;
        private double leftEdge;
        private double topEdge;
        private double rightEdge;
        private double bottomEdge;
        private double imgWidth;
        private double imgHeight;
        private double tapX;
        private double tapY;
        private bool LeftActionPanelActive = false;
        private bool RightActionPanelActive = false;
        private bool ObjectWasClosed = false;
        private Button targetButton;
        private readonly int rowHeight = 50;
        private readonly int colWidth = 50;
        private readonly int margin = 16;
        private readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Colors.Black);
        private readonly int RedImageWidth = 300;
        private readonly int RedImageHeight = 300;
        private readonly int PurpleImageWidth = 300;
        private readonly int PurpleImageHeight = 150;
        private readonly int OrangeImageWidth = 150;
        private readonly int OrangeImageHeight = 300;
        private readonly int BlueImageWidth = 150;
        private readonly int BlueImageHeight = 200;
        private readonly int GreenImageWidth = 100;
        private readonly int GreenImageHeight = 150;
        #endregion

        public MainPage()
        {
            InitializeComponent();
            HideTitlebar();
            CalculateScreenEdges();
            BackgroundBrush.Opacity = 0.01;
            LayoutRoot.Background = BackgroundBrush;
        }

        #region Shared Methods
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e) => CalculateScreenEdges();

        private void HideTitlebar()
        {
            //ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            //formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;

            //CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            //coreTitleBar.ExtendViewIntoTitleBar = true;
        }

        private void CalculateScreenEdges()
        {
            Rect bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            double scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            Size size = new Size(bounds.Width, bounds.Height);

            screenWidth = size.Width;
            //screenHeight = size.Height + 32; // 32 is title bar height
            screenHeight = size.Height;
            leftEdge = 0;
            topEdge = 0;
            rightEdge = screenWidth;
            bottomEdge = screenHeight;
        }

        private void RemoveObjects()
        {
            foreach (UIElement item in MainCanvas.Children)
            {
                if (item != BottomTaskbar)
                {
                    MainCanvas.Children.Remove(item);
                }
            }

            return;
        }

        private void Placeholder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // I tried to put in a fade animation here, but doing so allows for click-through and it launches another taskbar
            Image placeholder = sender as Image;
            MainCanvas.Children.Remove(placeholder);
            e.Handled = true;
        }

        private void Taskbar_Tapped(object sender, TappedRoutedEventArgs e) => CloseActiveObjects();

        private void LeftActionPanelSlide()
        {
            RemoveObjects();

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Yellow.png"));

            img.Source = bitmapImage;
            img.Height = MainCanvas.ActualHeight - rowHeight;
            img.Width = img.Height / 3.25;
            img.RenderTransform = new CompositeTransform();
            posX = leftEdge - img.Width;
            posY = topEdge + rowHeight;

            if (LeftActionPanelActive != false)
            {
                slideFrom = posX + (img.Width * 2);
                slideTo = posX;
                LeftActionPanelActive = false;
            }
            else
            {
                slideFrom = posX;
                slideTo = posX + (img.Width * 2);
                LeftActionPanelActive = true;
            }

            MainCanvas.Children.Add(img);
            Canvas.SetLeft(img, posX);
            Canvas.SetTop(img, posY);

            img.Tapped += new TappedEventHandler(LeftActionCenter_Tapped);

            Storyboard sb = new Storyboard();

            var slideAnimation = new DoubleAnimation()
            {
                From = slideFrom,
                To = slideTo,
                Duration = TimeSpan.FromMilliseconds(500),
            };
            PowerEase powerEase = new PowerEase
            {
                EasingMode = EasingMode.EaseOut
            };
            slideAnimation.EasingFunction = powerEase;

            Storyboard.SetTarget(slideAnimation, img);
            Storyboard.SetTargetProperty(slideAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
            sb.Children.Add(slideAnimation);
            sb.Begin();
        }

        private void RightActionPanelSlide()
        {
            RemoveObjects();

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Yellow.png"));

            img.Source = bitmapImage;
            img.Height = MainCanvas.ActualHeight - rowHeight;
            img.Width = img.Height / 3.25;
            img.RenderTransform = new CompositeTransform();
            posX = rightEdge / 2;
            posY = topEdge + rowHeight;

            if (RightActionPanelActive != false)
            {
                slideFrom = posX - img.Width;
                slideTo = posX;
                RightActionPanelActive = false;
            }
            else
            {
                slideFrom = posX;
                slideTo = posX - img.Width;
                RightActionPanelActive = true;
            }

            MainCanvas.Children.Add(img);
            Canvas.SetLeft(img, posX);
            Canvas.SetTop(img, posY);

            img.Tapped += new TappedEventHandler(RightActionCenter_Tapped);

            Storyboard sb = new Storyboard();

            var slideAnimation = new DoubleAnimation()
            {
                From = slideFrom,
                To = slideTo,
                Duration = TimeSpan.FromMilliseconds(300),
            };
            PowerEase powerEase = new PowerEase
            {
                EasingMode = EasingMode.EaseOut
            };
            slideAnimation.EasingFunction = powerEase;

            Storyboard.SetTarget(slideAnimation, img);
            Storyboard.SetTargetProperty(slideAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
            sb.Children.Add(slideAnimation);
            sb.Begin();
        }

        private void LeftActionCenter_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LeftActionPanelSlide();
            e.Handled = true;
        }

        private void RightActionCenter_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RightActionPanelSlide();
            e.Handled = true;
        }

        private async void CloseActiveObjects()
        {
            if (horizontalTaskbarLTR != null && ((Storyboard)horizontalTaskbarLTR.Resources["Storyboard_HorizontalTaskbarLTR_Close"] != null))
            {
                ObjectWasClosed = true;
                Storyboard sb = new Storyboard();
                sb = (Storyboard)horizontalTaskbarLTR.Resources["Storyboard_HorizontalTaskbarLTR_Close"];
                await StoryboardAnimations.BeginAsync(sb);
                horizontalTaskbarLTR = null;
            }

            if (horizontalTaskbarRTL != null && ((Storyboard)horizontalTaskbarRTL.Resources["Storyboard_HorizontalTaskbarRTL_Close"] != null))
            {
                ObjectWasClosed = true;
                Storyboard sb = new Storyboard();
                sb = (Storyboard)horizontalTaskbarRTL.Resources["Storyboard_HorizontalTaskbarRTL_Close"];
                await StoryboardAnimations.BeginAsync(sb);
                horizontalTaskbarRTL = null;
            }

            if (verticalDownTaskbar != null && ((Storyboard)verticalDownTaskbar.Resources["Storyboard_VerticalDownTaskbar_Close"] != null))
            {
                ObjectWasClosed = true;
                Storyboard sb = new Storyboard();
                sb = (Storyboard)verticalDownTaskbar.Resources["Storyboard_VerticalDownTaskbar_Close"];
                await StoryboardAnimations.BeginAsync(sb);
                verticalDownTaskbar = null;
            }

            if (verticalUpTaskbar != null && ((Storyboard)verticalUpTaskbar.Resources["Storyboard_VerticalUpTaskbar_Close"] != null))
            {
                ObjectWasClosed = true;
                Storyboard sb = new Storyboard();
                sb = (Storyboard)verticalUpTaskbar.Resources["Storyboard_VerticalUpTaskbar_Close"];
                await StoryboardAnimations.BeginAsync(sb);
                verticalUpTaskbar = null;
            }

            if (radialTaskbar != null && ((Storyboard)radialTaskbar.Resources["Storyboard_RadialTaskbar_Close"] != null))
            {
                ObjectWasClosed = true;
                Storyboard sb = new Storyboard();
                sb = (Storyboard)radialTaskbar.Resources["Storyboard_RadialTaskbar_Close"];
                await StoryboardAnimations.BeginAsync(sb);
                radialTaskbar = null;
            }
        }
        #endregion

        #region Taskbars
        public void Open_Taskbar(object sender, TappedRoutedEventArgs e)
        {
            targetCanvas = sender as Canvas;
            tappedPoint = e.GetPosition(targetCanvas);

            // Do nothing if tapped along bottom edge so as to not interfere with BottomTaskbar buttons
            if (tappedPoint.Y >= bottomEdge - rowHeight)
            {
                return;
            }

            // Horizontal Taskbar
            if (tappedPoint.Y <= topEdge + rowHeight)
            {
                // HorizontalTaskbarLTR
                if (tappedPoint.X < rightEdge - 300)
                {
                    // Get a resource null error if user taps too often and too quickly    
                    try
                    {
                        CloseActiveObjects();

                        if (ObjectWasClosed != true)
                        {
                            RemoveObjects();
                            horizontalTaskbarLTR = new HorizontalTaskbarLTRControl();
                            horizontalTaskbarLTR.Button1Tap += new TappedEventHandler(HorizontalButton1_Tapped);
                            horizontalTaskbarLTR.Button2Tap += new TappedEventHandler(HorizontalButton2_Tapped);
                            horizontalTaskbarLTR.Button3Tap += new TappedEventHandler(HorizontalButton3_Tapped);
                            horizontalTaskbarLTR.Button4Tap += new TappedEventHandler(HorizontalButton4_Tapped);
                            horizontalTaskbarLTR.Button5Tap += new TappedEventHandler(HorizontalButton5_Tapped);
                            horizontalTaskbarLTR.Button6Tap += new TappedEventHandler(horizontalButton6_Tapped);
                            horizontalTaskbarLTR.Tapped += new TappedEventHandler(Taskbar_Tapped);

                            if (tappedPoint.X < 24)
                            {
                                posX = tappedPoint.X;
                            }
                            else
                            {
                                posX = tappedPoint.X - 24;
                            }

                            posY = 1;
                            targetCanvas.Children.Add(horizontalTaskbarLTR);
                            Canvas.SetLeft(horizontalTaskbarLTR, posX);
                            Canvas.SetTop(horizontalTaskbarLTR, posY);

                            Storyboard sb = new Storyboard();
                            sb = (Storyboard)horizontalTaskbarLTR.Resources["Storyboard_HorizontalTaskbarLTR_Open"];
                            sb.Begin();
                        }
                        else
                        {
                            ObjectWasClosed = false;
                        }
                    }
                    catch
                    {
                        e.Handled = true;
                    }
                }
                // HorizontalTaskbarRTL
                else
                {
                    // Get a resource null error if user taps too often and too quickly    
                    try
                    {
                        CloseActiveObjects();

                        if (ObjectWasClosed != true)
                        {
                            RemoveObjects();
                            horizontalTaskbarRTL = new HorizontalTaskbarRTLControl();
                            horizontalTaskbarRTL.Button1Tap += new TappedEventHandler(VerticalButton1_Tapped);
                            horizontalTaskbarRTL.Button2Tap += new TappedEventHandler(VerticalButton2_Tapped);
                            horizontalTaskbarRTL.Button3Tap += new TappedEventHandler(VerticalButton3_Tapped);
                            horizontalTaskbarRTL.Button4Tap += new TappedEventHandler(VerticalButton4_Tapped);
                            horizontalTaskbarRTL.Button5Tap += new TappedEventHandler(VerticalButton5_Tapped);
                            horizontalTaskbarRTL.Button6Tap += new TappedEventHandler(VerticalButton6_Tapped);
                            horizontalTaskbarRTL.Tapped += new TappedEventHandler(Taskbar_Tapped);

                            posX = rightEdge - colWidth;
                            posY = 1;
                            targetCanvas.Children.Add(horizontalTaskbarRTL);
                            Canvas.SetLeft(horizontalTaskbarRTL, posX);
                            Canvas.SetTop(horizontalTaskbarRTL, posY);

                            Storyboard sb = new Storyboard();
                            sb = (Storyboard)horizontalTaskbarRTL.Resources["Storyboard_HorizontalTaskbarRTL_Open"];
                            sb.Begin();
                        }
                        else
                        {
                            ObjectWasClosed = false;
                        }
                    }
                    catch
                    {
                        e.Handled = true;
                    }
                }
            }
            // Vertical Taskbar
            else if (tappedPoint.X <= leftEdge + colWidth || tappedPoint.X >= rightEdge - colWidth)
            {
                // VerticalDownTaskbar
                if (tappedPoint.Y > topEdge + rowHeight && tappedPoint.Y < bottomEdge - 350)
                {
                    // Get a resource null error if user taps too often and too quickly    
                    try
                    {
                        CloseActiveObjects();

                        if (ObjectWasClosed != true)
                        {
                            RemoveObjects();
                            verticalDownTaskbar = new VerticalTaskbarDownControl();
                            verticalDownTaskbar.Button1Tap += new TappedEventHandler(VerticalButton1_Tapped);
                            verticalDownTaskbar.Button2Tap += new TappedEventHandler(VerticalButton2_Tapped);
                            verticalDownTaskbar.Button3Tap += new TappedEventHandler(VerticalButton3_Tapped);
                            verticalDownTaskbar.Button4Tap += new TappedEventHandler(VerticalButton4_Tapped);
                            verticalDownTaskbar.Button5Tap += new TappedEventHandler(VerticalButton5_Tapped);
                            verticalDownTaskbar.Button6Tap += new TappedEventHandler(VerticalButton6_Tapped);
                            verticalDownTaskbar.Tapped += new TappedEventHandler(Taskbar_Tapped);

                            if (tappedPoint.X <= colWidth)
                            {
                                posX = 1;
                            }
                            else
                            {
                                posX = rightEdge - colWidth;
                            }

                            posY = tappedPoint.Y - 24;
                            targetCanvas.Children.Add(verticalDownTaskbar);
                            Canvas.SetLeft(verticalDownTaskbar, posX);
                            Canvas.SetTop(verticalDownTaskbar, posY);

                            Storyboard sb = new Storyboard();
                            sb = (Storyboard)verticalDownTaskbar.Resources["Storyboard_VerticalDownTaskbar_Open"];
                            sb.Begin();
                        }
                        else
                        {
                            ObjectWasClosed = false;
                        }
                    }
                    catch
                    {
                        e.Handled = true;
                    }
                }
                // VerticalUpTaskbar
                else
                {
                    // Get a resource null error if user taps too often and too quickly    
                    try
                    {
                        CloseActiveObjects();

                        if (ObjectWasClosed != true)
                        {
                            RemoveObjects();
                            verticalUpTaskbar = new VerticalTaskbarUpControl();
                            verticalUpTaskbar.Button1Tap += new TappedEventHandler(VerticalButton1_Tapped);
                            verticalUpTaskbar.Button2Tap += new TappedEventHandler(VerticalButton2_Tapped);
                            verticalUpTaskbar.Button3Tap += new TappedEventHandler(VerticalButton3_Tapped);
                            verticalUpTaskbar.Button4Tap += new TappedEventHandler(VerticalButton4_Tapped);
                            verticalUpTaskbar.Button5Tap += new TappedEventHandler(VerticalButton5_Tapped);
                            verticalUpTaskbar.Button6Tap += new TappedEventHandler(VerticalButton6_Tapped);
                            verticalUpTaskbar.Tapped += new TappedEventHandler(Taskbar_Tapped);

                            if (tappedPoint.X <= colWidth)
                            {
                                posX = 1;
                            }
                            else
                            {
                                posX = rightEdge - colWidth;
                            }

                            posY = bottomEdge - 100;
                            targetCanvas.Children.Add(verticalUpTaskbar);
                            Canvas.SetLeft(verticalUpTaskbar, posX);
                            Canvas.SetTop(verticalUpTaskbar, posY);

                            Storyboard sb = new Storyboard();
                            sb = (Storyboard)verticalUpTaskbar.Resources["Storyboard_VerticalUpTaskbar_Open"];
                            sb.Begin();
                        }
                        else
                        {
                            ObjectWasClosed = false;
                        }
                    }
                    catch
                    {
                        e.Handled = true;
                    }
                }
            }
            // Radial Taskbar
            else
            {
                // Get a resource null error if user taps too often and too quickly    
                try
                {
                    CloseActiveObjects();

                    if (ObjectWasClosed != true)
                    {
                        RemoveObjects();
                        radialTaskbar = new RadialTaskbarControl(); // appox 116 x 116
                        radialTaskbar.Button1Tap += new TappedEventHandler(RadialButton1_Tapped);
                        radialTaskbar.Button2Tap += new TappedEventHandler(RadialButton2_Tapped);
                        radialTaskbar.Button3Tap += new TappedEventHandler(RadialButton3_Tapped);
                        radialTaskbar.Button4Tap += new TappedEventHandler(RadialButton4_Tapped);
                        radialTaskbar.Button5Tap += new TappedEventHandler(RadialButton5_Tapped);
                        radialTaskbar.Button6Tap += new TappedEventHandler(RadialButton6_Tapped);
                        radialTaskbar.Tapped += new TappedEventHandler(Taskbar_Tapped);

                        int rtWidth = 116;
                        int rtHeight = 116;

                        if (tappedPoint.X < rtWidth + colWidth)
                        {
                            posX = leftEdge + colWidth + margin + (rtWidth / 2);
                        }
                        else if (tappedPoint.X > rightEdge - rtWidth)
                        {
                            posX = rightEdge - rtWidth - colWidth - margin;
                        }
                        else
                        {
                            posX = tappedPoint.X - 24;
                        }

                        if (tappedPoint.Y < rtHeight + rowHeight)
                        {
                            posY = topEdge + rowHeight + margin + (rtHeight / 2);
                        }
                        else if (tappedPoint.Y > bottomEdge - rtHeight)
                        {
                            posY = bottomEdge - rtHeight - rowHeight - margin;
                        }
                        else
                        {
                            posY = tappedPoint.Y - 24;
                        }

                        targetCanvas.Children.Add(radialTaskbar);
                        Canvas.SetLeft(radialTaskbar, posX);
                        Canvas.SetTop(radialTaskbar, posY);

                        Storyboard sb = new Storyboard();
                        sb = (Storyboard)radialTaskbar.Resources["Storyboard_RadialTaskbar_Open"];
                        sb.Begin();
                    }
                    else
                    {
                        ObjectWasClosed = false;
                    }
                }
                catch
                {
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region Radial Buttons
        private void RadialButton1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Red.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = RedImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = RedImageHeight;
            img.Name = "Red";
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;
            tapY = tappedPoint.Y;

            RadialPlaceholderLocation(img, imgWidth, imgHeight, tapX, tapY);
        }

        private void RadialButton2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Purple.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = PurpleImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = PurpleImageHeight;
            img.Name = "Purple";
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;
            tapY = tappedPoint.Y;

            RadialPlaceholderLocation(img, imgWidth, imgHeight, tapX, tapY);
        }

        private void RadialButton3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Orange.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = OrangeImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = OrangeImageHeight;
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;
            tapY = tappedPoint.Y;

            RadialPlaceholderLocation(img, imgWidth, imgHeight, tapX, tapY);
        }

        private void RadialButton4_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Blue.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = BlueImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = BlueImageHeight;
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;
            tapY = tappedPoint.Y;

            RadialPlaceholderLocation(img, imgWidth, imgHeight, tapX, tapY);
        }

        private void RadialButton5_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            if (tappedPoint.X < screenWidth / 2)
            {
                LeftActionPanelSlide();
            }
            else
            {
                RightActionPanelSlide();
            }
        }

        private void RadialButton6_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Green.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = GreenImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = GreenImageHeight;
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;
            tapY = tappedPoint.Y;

            RadialPlaceholderLocation(img, imgWidth, imgHeight, tapX, tapY);
        }

        private void RadialPlaceholderLocation(Image img, double imgWidth, double imgHeight, double tapX, double tapY)
        {
            if (tapX < imgWidth / 2)
            {
                posX = leftEdge + colWidth + margin;
            }
            else if (tapX > rightEdge - imgWidth)
            {
                posX = rightEdge - imgWidth - colWidth - margin;
            }
            else
            {
                posX = tapX - (imgWidth / 2);
            }

            if (tapY < imgHeight / 2)
            {
                posY = topEdge + rowHeight + margin;
            }
            else if (tapY > bottomEdge - imgHeight)
            {
                posY = bottomEdge - imgHeight - rowHeight - margin;
            }
            else
            {
                posY = tapY - (imgHeight / 2);
            }

            targetCanvas.Children.Add(img);
            Canvas.SetLeft(img, posX);
            Canvas.SetTop(img, posY);
            img.Tapped += new TappedEventHandler(Placeholder_Tapped);

            // Fade
            Storyboard sb = new Storyboard();

            var fadeAnimation = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
            };

            Storyboard.SetTarget(fadeAnimation, img);
            Storyboard.SetTargetProperty(fadeAnimation, "Opacity");
            sb.Children.Add(fadeAnimation);
            sb.Begin();
            CloseActiveObjects();
        }
        #endregion

        #region Horizontal Buttons (Top)
        private void HorizontalButton1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Red.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = RedImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = RedImageHeight;
            img.Name = "Red";
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;

            HorizontalPlaceholderLocation(img, imgWidth, imgHeight, tapX);
        }

        private void HorizontalButton2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Purple.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = PurpleImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = PurpleImageHeight;
            img.Name = "Purple";
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;

            HorizontalPlaceholderLocation(img, imgWidth, imgHeight, tapX);
        }

        private void HorizontalButton3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Orange.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = OrangeImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = OrangeImageHeight;
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;

            HorizontalPlaceholderLocation(img, imgWidth, imgHeight, tapX);
        }

        private void HorizontalButton4_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Blue.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = BlueImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = BlueImageHeight;
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;

            HorizontalPlaceholderLocation(img, imgWidth, imgHeight, tapX);
        }

        private void HorizontalButton5_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RightActionPanelSlide();
            e.Handled = true;
        }

        private void horizontalButton6_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Green.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = GreenImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = GreenImageHeight;
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;

            HorizontalPlaceholderLocation(img, imgWidth, imgHeight, tapX);
        }

        private void HorizontalPlaceholderLocation(Image img, double imgWidth, double imgHeight, double tapX)
        {
            RemoveObjects();

            if (tapX == 0)
            {
                return;
            }
            else
            {
                if (tapX < imgWidth / 2)
                {
                    posX = leftEdge + colWidth + margin;
                }
                else if (tapX > rightEdge - imgWidth)
                {
                    posX = rightEdge - imgWidth - colWidth - margin;
                }
                else
                {
                    posX = tapX - (imgWidth / 2);
                }

                posY = topEdge + rowHeight + margin;

                MainCanvas.Children.Add(img);
                Canvas.SetLeft(img, posX);
                Canvas.SetTop(img, posY);
                img.Tapped += new TappedEventHandler(Placeholder_Tapped);

                // Fade
                Storyboard sb = new Storyboard();

                var fadeAnimation = new DoubleAnimation()
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300),
                };

                Storyboard.SetTarget(fadeAnimation, img);
                Storyboard.SetTargetProperty(fadeAnimation, "Opacity");
                sb.Children.Add(fadeAnimation);
                sb.Begin();
            }
        }
        #endregion

        #region Vertical Buttons
        private void VerticalButton1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Red.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = RedImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = RedImageHeight;
            img.Name = "Red";
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;
            tapY = tappedPoint.Y;

            VerticalPlaceholderLocation(img, imgWidth, imgHeight, tapX, tapY);
        }

        private void VerticalButton2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Purple.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = PurpleImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = PurpleImageHeight;
            img.Name = "Purple";
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;
            tapY = tappedPoint.Y;

            VerticalPlaceholderLocation(img, imgWidth, imgHeight, tapX, tapY);
        }

        private void VerticalButton3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Orange.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = OrangeImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = OrangeImageHeight;
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;
            tapY = tappedPoint.Y;

            VerticalPlaceholderLocation(img, imgWidth, imgHeight, tapX, tapY);
        }

        private void VerticalButton4_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Blue.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = BlueImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = BlueImageHeight;
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;
            tapY = tappedPoint.Y;

            VerticalPlaceholderLocation(img, imgWidth, imgHeight, tapX, tapY);
        }

        private void VerticalButton5_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (tappedPoint.X < screenWidth / 2)
            {
                LeftActionPanelSlide();
            }
            else
            {
                RightActionPanelSlide();
            }
        }

        private void VerticalButton6_Tapped(object sender, TappedRoutedEventArgs e)
        {
            targetButton = sender as Button;
            tappedPoint = e.GetPosition(targetButton);

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Placeholders/Green.png"));
            img.Source = bitmapImage;
            img.Width = bitmapImage.DecodePixelWidth = GreenImageWidth;
            img.Height = bitmapImage.DecodePixelHeight = GreenImageHeight;
            img.Opacity = 0;
            imgWidth = img.Width;
            imgHeight = img.Height;
            tapX = tappedPoint.X;
            tapY = tappedPoint.Y;

            VerticalPlaceholderLocation(img, imgWidth, imgHeight, tapX, tapY);
        }

        private void VerticalPlaceholderLocation(Image img, double imgWidth, double imgHeight, double tapX, double tapY)
        {
            RemoveObjects();

            if (tapX == 0)
            {
                return;
            }
            else
            {
                if (tapX < imgWidth / 2)
                {
                    posX = leftEdge + colWidth + margin;
                }
                else if (tapX > rightEdge - imgWidth)
                {
                    posX = rightEdge - imgWidth - colWidth - margin;
                }
                else
                {
                    posX = tapX - (imgWidth / 2);
                }

                if (tapY < imgHeight / 2)
                {
                    posY = topEdge + rowHeight + margin;
                }
                else if (tapY > bottomEdge - imgHeight)
                {
                    posY = bottomEdge - imgHeight - rowHeight - margin;
                }
                else
                {
                    posY = tapY - (imgHeight / 2);
                }

                targetCanvas.Children.Add(img);
                Canvas.SetLeft(img, posX);
                Canvas.SetTop(img, posY);
                img.Tapped += new TappedEventHandler(Placeholder_Tapped);

                // Fade
                Storyboard sb = new Storyboard();

                var fadeAnimation = new DoubleAnimation()
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300),
                };

                Storyboard.SetTarget(fadeAnimation, img);
                Storyboard.SetTargetProperty(fadeAnimation, "Opacity");
                sb.Children.Add(fadeAnimation);
                sb.Begin();
            }
        }
        #endregion
    }
}
