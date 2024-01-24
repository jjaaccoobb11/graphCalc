using System;
using System.Collections.Generic;
using System.Linq;
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
        public GraphWindow()
        {
            InitializeComponent();
        }



        static void DrawOnCanvas(Canvas mycanvas)
        {
            Ellipse point = new Ellipse
            {
                Width = 2,
                Height = 2,
                Fill = Brushes.Red
            };

            // set canvas.left and canvas.top to position the point in the middle

            double midpointleft = (mycanvas.ActualWidth - point.Width) / 2;
            double midpointtop = (mycanvas.ActualHeight - point.Width) / 2;



            for (int i = 0; i != 100; i++)
            {
                point = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = Brushes.Red
                };

                Canvas.SetLeft(point, midpointleft + i);
                Canvas.SetTop(point, midpointtop + i);

                mycanvas.Children.Add(point);
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

        static List<double> GetPoints()
        {
            string expression = "x^2";
            List<double> points = new List<double>();

            for (double i = 0; i != 5; i += 0.1)
            {
                expression = expression.Replace("x^2", $"{i}^2");

                points.Add(double.Parse(EvaluateMathExpression(expression)));

            }
            return points;
        }





        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetPoints();
            DrawOnCanvas(myCanvas);
        }
    }


}
