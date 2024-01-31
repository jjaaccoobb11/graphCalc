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

        

        static void DrawOnCanvas(Canvas myCanvas)
        {
            //spannet på x-värdena
            double xMin = -5;
            double xMax = 5;

            //spannet på y-värdena
            double yMin = -5;
            double yMax = 5;

            //
            double deltaX = 0.01;

            Ellipse point = new Ellipse
            {
                Width = 2,
                Height = 2,
                Fill = Brushes.Red
            };

            // set canvas.left and canvas.top to position the point in the middle
            double midpointleft = (myCanvas.ActualWidth - point.Width) / 2;
            double midpointtop = (myCanvas.ActualHeight - point.Width) / 2;


            //Får en lista med olika prickar som ska ritas ut
            List<double> points = GetPoints(xMin, xMax, deltaX);


            for (int i = 0; i <= points.Count - 1; i++)
            {
                point = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = Brushes.Red
                };

                //
                //Canvas.SetLeft(point, midpointleft + (xMin + i));
                Canvas.SetLeft(point, i);

                Canvas.SetTop(point, midpointtop - points[i]);

                myCanvas.Children.Add(point);
            }
        }

        static string EvaluateMathExpression(string expressionIn)
        {

            org.matheval.Expression expression = new org.matheval.Expression(expressionIn);
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

        static List<double> GetPoints(double xMin, double xMax, double deltaX)
        {


            string expression = "x^2";
            string newEx = expression.Replace("x", "0.5");
            string expression1 = expression;
            List<double> points = new List<double>();


            for (double i = xMin; i <= xMax; i += deltaX)
            {
                string c = i.ToString();
                c = c.Replace(',', '.');

                points.Add(double.Parse(EvaluateMathExpression(expression.Replace("x", c))));

            }
            return points;
        }





        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetPoints(-5, 5, 0.01);
            DrawOnCanvas(myCanvas);
        }
    }


}
