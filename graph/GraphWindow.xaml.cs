﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D.Converters;
using System.Windows.Shapes;
using org.matheval;


namespace graph
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        private bool isResizing = false;
        //spannet på x-värdena
        public double xMin {get; set;}
        public double xMax { get; set; }

        //spannet på y-värdena
        public double yMin { get; set; }
        public double yMax { get; set; }

        //
        public double deltaX { get; set; }

        private double initialWidth;
        private double initialHeight;


        public GraphWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;

            //spannet på x-värdena
            xMin = -5;
            xMax = 5;

            //spannet på y-värdena
            yMin = -5;
            yMax = 5;

            deltaX = 0.001;

            SizeChanged += MainWindow_SizeChanged;
            initialWidth = Width;
            initialHeight = Height;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource src = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            src.AddHook(new HwndSourceHook(WndProc));
        }
        const int WM_EXITSIZEMOVE = 0x232;
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == WM_EXITSIZEMOVE)
            {
                Debug.WriteLine("SIZED FINIOSHED");
                DrawOnCanvas(myCanvas, expression.Text);
            }

            return IntPtr.Zero;
        }


        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double deltaWidth = Width - initialWidth;
            double deltaHeight = Height - initialHeight;

            if (Math.Abs(deltaWidth) > Math.Abs(deltaHeight))
            {
                // Adjust height to maintain aspect ratio
                Height = Width * (initialHeight / initialWidth);
            }
            else
            {
                // Adjust width to maintain aspect ratio
                Width = Height * (initialWidth / initialHeight);
            }
        }

        static double ABS(double doubleIn)
        {
            if(doubleIn < 0)
            {
                return doubleIn * -1;
            }
            else
            {
                return doubleIn;
            }
        }
        
        //Ritar ut x-axlen
        public void DrawXAxis(Canvas myCanvas)
        {
            if(yMin < 0 && yMax > 0)
            {
                double scale = ABS(yMax) / (ABS(yMax) + ABS(yMin));

                // x line
                Line horizontalLine = new Line
                {
                    X1 = 0,
                    Y1 = myCanvas.ActualHeight * scale,
                    X2 = myCanvas.ActualWidth,
                    Y2 = myCanvas.ActualHeight * scale,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                myCanvas.Children.Add(horizontalLine);
            }
        }

        //Ritar ut y-axlen
        public void DrawYAxis(Canvas myCanvas)
        {
            if(xMin < 0 && xMax > 0)
            {
                double scale = ABS(xMin) / (ABS(xMax) + ABS(xMin));

                Line verticalLine = new Line
                {
                    X1 = myCanvas.ActualHeight * scale,
                    Y1 = 0,
                    X2 = myCanvas.ActualHeight * scale,
                    Y2 = myCanvas.ActualHeight,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                myCanvas.Children.Add(verticalLine);
            }
        }

        public void DrawLines(Canvas myCanvas)
        {
            DrawXAxis(myCanvas);
            DrawYAxis(myCanvas);
        }


        public void DrawOnCanvas(Canvas myCanvas, string expression)
        {
            myCanvas.Children.Clear();
            DrawLines(myCanvas);

            Ellipse point = new Ellipse
            {
                Width = 1,
                Height = 1,
                Fill = Brushes.Red
            };

            ////sätter x axlen till mitten
            //double midpointleft = (myCanvas.ActualWidth) / 2;
            ////sätter y axlen till mitten
            //double midpointtop = (myCanvas.ActualHeight) / 2;

            //TEST
            double scale = ABS(yMax) / (ABS(yMax) + ABS(yMin));
            double midpointtop1 = myCanvas.ActualHeight * scale;



            //Får en lista med olika prickar som ska ritas ut
            List<double> points = GetPoints(expression);

            double realDeltaX = myCanvas.ActualHeight / points.Count;

            double yMax1 = ABS(yMax);
            double yMin1 = ABS(yMin);

            //double scaleY = myCanvas.ActualHeight / (yMax * 2);
            double scaleY = myCanvas.ActualHeight / (yMax1 + yMin1);

            double xValue = 0;
            for (int i = 0; i <= points.Count - 1; i ++)
            {
                point = new Ellipse
                {
                    Width = 1,
                    Height = 1,
                    Fill = Brushes.Red
                };

                bool shouldBeDrawn = true;

                //x
                Canvas.SetLeft(point, xValue);
                //y
                double scalingY = midpointtop1 - (points[i] * scaleY);

                
                Canvas.SetTop(point, scalingY);



                if (points[i] > yMax || points[i] < yMin)
                {
                    shouldBeDrawn = false;
                }

                double f = 0;
                if (xValue == f)
                {
                    DrawXAxis(myCanvas);
                }
                

                if (shouldBeDrawn)
                {
                    myCanvas.Children.Add(point);
                }

                xValue += realDeltaX;
            }
        }

        static string EvaluateMathExpression(org.matheval.Expression expression)
        {
            try
            {
                Object value = expression.Eval(); // return 0
                return value.ToString();
            }
            catch
            {
                ErrorWindow errorWindow = new ErrorWindow();
                errorWindow.Show();
                return "";
            }

        }

        public List<double> GetPoints(string expressionString)
        {

            org.matheval.Expression expression = new org.matheval.Expression(expressionString);
            
            List<double> points = new List<double>();

            //bool negative = true;
            for (double i = xMin; i <= xMax; i += deltaX)
            {
                try
                {
                    expression.Bind("x", i);
                    points.Add(double.Parse(EvaluateMathExpression(expression)));
                }
                catch {
                    return null;
                }

            }
            return points;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DrawOnCanvas(myCanvas, expression.Text);
            Debug.WriteLine(Mouse.LeftButton.ToString());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            xMin = int.Parse(xMinIn.Text);
            xMax = int.Parse(xMaxIn.Text);
            yMin = int.Parse(yMinIn.Text);
            yMax = int.Parse(yMaxIn.Text);
            DrawOnCanvas(myCanvas, expression.Text);
        }
    }


}
