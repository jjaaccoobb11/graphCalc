using System;
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
        //private bool isResizing = false;

        //spannet på x-värdena och y-värdena
        public double xMin {get; set;}
        public double xMax { get; set; }
        public double yMin { get; set; }
        public double yMax { get; set; }


        //Hur många punkter som ska ritas ut på canvasen (exempelvis ger deltaX = 0.1 10 st punkter per x värde, deltaX = 0.01 ger 100st per x värde.)
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

            //för varje x värde skapas 1000 st punkter
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
            //När man har ändrat storleken på fönstret ska grafen ritas ut igen, så den fyller korrekt andel av skärmen
            if (msg == WM_EXITSIZEMOVE)
            {
                DrawOnCanvas(myCanvas, expression.Text);
            }

            return IntPtr.Zero;
        }

        //När man ändrar storlek på fönstret.
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double deltaWidth = Width - initialWidth;
            double deltaHeight = Height - initialHeight;

            if (Math.Abs(deltaWidth) > Math.Abs(deltaHeight))
            {
                //Anpassar höjden för att bibehålla skalan
                Height = Width * (initialHeight / initialWidth);
            }
            else
            {
                //Anpassar bredden för att bibehålla skalan
                Width = Height * (initialWidth / initialHeight);
            }
        }

        //Returnerar absolutbeloppet
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
        
        //Ritar ut x-axlen (den horisontella linjen)
        public void DrawXAxis(Canvas myCanvas)
        {
            if(yMin < 0 && yMax > 0)
            {
                double scale = ABS(yMax) / (ABS(yMax) + ABS(yMin));

                //x linjen
                Line horizontalLine = new Line
                {
                    //anger start och slut punkt för linjen. Även var mitten av canvasen ska vara.
                    X1 = 0,
                    Y1 = myCanvas.ActualHeight * scale,
                    X2 = myCanvas.ActualWidth,
                    Y2 = myCanvas.ActualHeight * scale,

                    //Färg och storlek på linjen som ritas ut
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                myCanvas.Children.Add(horizontalLine);
            }
        }

        //Ritar ut y-axlen (den vertikala linjen)
        public void DrawYAxis(Canvas myCanvas)
        {
            if(xMin < 0 && xMax > 0)
            {
                double scale = ABS(xMin) / (ABS(xMax) + ABS(xMin));

                //y linjen
                Line verticalLine = new Line
                {
                    //anger start och slut punkt för linjen. Även var mitten av canvasen ska vara.
                    X1 = myCanvas.ActualHeight * scale,
                    Y1 = 0,
                    X2 = myCanvas.ActualHeight * scale,
                    Y2 = myCanvas.ActualHeight,

                    //Färg och storlek på linjen som ritas ut
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                myCanvas.Children.Add(verticalLine);
            }
        }

        //Ritar ut både linjerna (x och y/vertikala och horisontella linjerna)
        public void DrawLines(Canvas myCanvas)
        {
            DrawXAxis(myCanvas);
            DrawYAxis(myCanvas);
        }

        //Här sker utritningen av punkterna på canvasen
        public void DrawOnCanvas(Canvas myCanvas, string expression)
        {
            //Ränsar canvasen och ritar ut axlarna
            myCanvas.Children.Clear();
            DrawLines(myCanvas);

            Ellipse point = new Ellipse
            {
                Width = 1,
                Height = 1,
                Fill = Brushes.Red
            };


            //Skalan beroende på y värdena. ymax dividerat med hela y (ymax + ymin) ger vilken % den övre respektive undre halvan ska tas upp av grafen.
            double scale = ABS(yMax) / (ABS(yMax) + ABS(yMin));
            double midpointtop1 = myCanvas.ActualHeight * scale;



            //Får en lista med alla prickar som ska ritas ut
            List<double> points = GetPoints(expression);

            //hur mycket man ska hoppa på x axlen för varje punkt
            double realDeltaX = myCanvas.ActualHeight / points.Count;

            double yMax1 = ABS(yMax);
            double yMin1 = ABS(yMin);

            double scaleY = myCanvas.ActualHeight / (yMax1 + yMin1);

            double xValue = 0;
            //För varje punkt som ska ritas ut
            for (int i = 0; i <= points.Count - 1; i ++)
            {
                //skapar en ny punkt
                point = new Ellipse
                {
                    Width = 1,
                    Height = 1,
                    Fill = Brushes.Red
                };
                //Om punkten ska bli utrtiad eller ej.
                bool shouldBeDrawn = true;

                //x
                Canvas.SetLeft(point, xValue);
               
                double scalingY = midpointtop1 - (points[i] * scaleY);

                //y
                Canvas.SetTop(point, scalingY);


                //Om punkten ligger utanför spannet som ska ritas ut ska punkten inte ritas ut
                if (points[i] > yMax || points[i] < yMin)
                {
                    shouldBeDrawn = false;
                }


                //double f = 0;
                //if (xValue == f)
                //{
                //    DrawXAxis(myCanvas);
                //}
                
                //Om punkten ska bli utritad så ritas den ut
                if (shouldBeDrawn)
                {
                    myCanvas.Children.Add(point);
                }

                xValue += realDeltaX;
            }
        }

        //samma som i MainWindow.xaml.cs med matheval som gör det till ett uttryck
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

        //Räknar ut alla punkter som ska ritas ut
        public List<double> GetPoints(string expressionString)
        {
            org.matheval.Expression expression = new org.matheval.Expression(expressionString);
          
            List<double> points = new List<double>();

            //genom att ersätta x med i och räkna ut värdet för uttrycket kommer man få lika många punkter som det finns i värden, som beror på deltaX värdet.
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

        //När man trycker på knappen DRAW GRAPH
        private void Button_Click_DrawGraph(object sender, RoutedEventArgs e)
        {
            //Ritar ut grafen
            DrawOnCanvas(myCanvas, expression.Text);
        }

        //När man vill uppdatera gränsvärdena
        private void Button_Click_Update(object sender, RoutedEventArgs e)
        {
            //Ändrar x och y min/max värden
            xMin = int.Parse(xMinIn.Text);
            xMax = int.Parse(xMaxIn.Text);
            yMin = int.Parse(yMinIn.Text);
            yMax = int.Parse(yMaxIn.Text);
            //ritar ut den nya grafen
            DrawOnCanvas(myCanvas, expression.Text);
        }
    }


}
