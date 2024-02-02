using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using org.matheval;


namespace graph
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {

        //spannet på x-värdena
        public double xMin = -5;
        public double xMax = 5;

        //spannet på y-värdena
        public double yMin = -5;
        public double yMax = 5;

        //
        public double deltaX = 0.01;


        public GraphWindow()
        {
            InitializeComponent();
            //spannet på x-värdena
            double xMin = -5;
            double xMax = 5;

            //spannet på y-värdena
            double yMin = -5;
            double yMax = 5;

            //
            double deltaX = 0.01;

    }
        static void DrawLines(Canvas myCanvas)
        {
            // Horizontal line
            Line horizontalLine = new Line
            {
                X1 = 0,
                Y1 = 150,
                X2 = 300,
                Y2 = 150,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            // Vertical line
            Line verticalLine = new Line
            {
                X1 = 150,
                Y1 = 0,
                X2 = 150,
                Y2 = 300,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            // Add lines to the WPF Window (Assuming you have a Canvas named "myCanvas" in your XAML)
            myCanvas.Children.Add(horizontalLine);
            myCanvas.Children.Add(verticalLine);
        }


        static void DrawOnCanvas(Canvas myCanvas, string expression)
        {
            //spannet på x-värdena
            double xMin = -5;
            double xMax = 5;

            //spannet på y-värdena
            double yMin = -5;
            double yMax = 5;

            //
            double deltaX = 0.01;


            myCanvas.Children.Clear();
            DrawLines(myCanvas);


            Ellipse point = new Ellipse
            {
                Width = 2,
                Height = 2,
                Fill = Brushes.Red
            };

            //sätter x axlen till mitten
            double midpointleft = (myCanvas.ActualWidth - point.Width) / 2;
            //sätter y axlen till mitten
            double midpointtop = (myCanvas.ActualHeight - point.Width) / 2;


            //Får en lista med olika prickar som ska ritas ut
            List<double> points = GetPoints(xMin, xMax, deltaX, expression);

            double realDeltaX = myCanvas.ActualWidth / points.Count;



            double xValue = 0;
            for (int i = 0; i <= points.Count - 1; i ++)
            {
                point = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = Brushes.Red
                };


                //x
                Canvas.SetLeft(point, xValue);
                //y
                Canvas.SetTop(point, midpointtop - points[i]);
                xValue += realDeltaX;
                myCanvas.Children.Add(point);
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

        static List<double> GetPoints(double xMin, double xMax, double deltaX, string expressionString)
        {

            org.matheval.Expression expression = new org.matheval.Expression(expressionString);
            
            List<double> points = new List<double>();

            bool negative = true;
            for (double i = xMin; i <= xMax; i += deltaX)
            {

                expression.Bind("x", i);
                points.Add(double.Parse(EvaluateMathExpression(expression)));

            }
            return points;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //GetPoints(-5, 5, 0.01, expression.Text);
            DrawOnCanvas(myCanvas, expression.Text);
        }
    }


}
