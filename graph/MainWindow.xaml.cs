using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using org.matheval;

namespace graph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string hiddenString;

        static bool expressionInRadians = false;
        static bool trigExpression = false;

        public MainWindow()
        {
            InitializeComponent();
            
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //answer.Text = EvaluateMathExpression(expression.Text).ToString();

            answer.Text = EvaluateMathExpression(hiddenString).ToString();
            trigExpression = false;


            Ellipse point = new Ellipse
            {
                Width = 2,
                Height = 2,
                Fill = Brushes.Red
            };

            // Set Canvas.Left and Canvas.Top to position the point in the middle
            Canvas.SetLeft(point, (myCanvas.ActualWidth - point.Width) / 2);
            Canvas.SetTop(point, (myCanvas.ActualHeight - point.Height) / 2);

            // Add the point to the Canvas
            myCanvas.Children.Add(point);



            //EvaluateExpression(expression.Text);
        }

        static string EvaluateMathExpression(string expressionIn)
        {

            org.matheval.Expression expression = new org.matheval.Expression(hiddenString);
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

        //----------------------------------------------------------------------------------------INPUT BUTTONS----------------------------------------------------------------
        private void Button_Click_Text(object sender, RoutedEventArgs e)
        {
            //expression.Text += ((Button)sender).Content.ToString();

            string hiddenContent = ((Button)sender).Content.ToString();
            string showContent = ((Button)sender).Content.ToString();
            

            switch (hiddenContent)
            {
                case "log":
                    showContent = "log(";
                    hiddenContent = "LOG10(";
                    break;

                //-------------------TRIG EXPRESSIONS---------------------------------
                case "cos":
                    //
                    trigExpression = true;


                    if (expressionInRadians == false)
                    {
                        hiddenContent = "COS(RADIANS(";
                    }
                    else
                    {
                        hiddenContent = "COS(";
                    }

                    showContent = "cos(";
                    break;
                case "sin":
                    trigExpression = true;

                    if (expressionInRadians == false)
                    {
                        hiddenContent = "SIN(RADIANS(";
                    }
                    else
                    {
                        hiddenContent = "SIN(";
                    }

                    showContent = "sin(";
                    break;

                case "tan":
                    trigExpression = true;

                    if(expressionInRadians == false)
                    {
                        hiddenContent = "TAN(RADIANS(";
                    }
                    else
                    {
                        hiddenContent = "TAN(";
                    }

                    showContent = "tan(";
                    break;
                //----------------------------------------------------------------------------------------------
                case "ln":
                    showContent = "ln(";
                    hiddenContent = "LN(";
                    break;
                case "𝛑":
                    showContent = "𝛑";
                    hiddenContent = "PI";
                    break;

                case ")":

                    if(trigExpression == true && expressionInRadians == false)
                    {
                        hiddenContent = "))";
                    }
                    else
                    {
                        hiddenContent = ")";
                    }

                    showContent = ")";
                    break;
                

            }

            hiddenString += hiddenContent;
            expression.Text += showContent;
        }

        private void Button_Click_Clear(object sender, RoutedEventArgs e)
        {
            
            expression.Text = "";
            hiddenString = "";
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            expressionInRadians = true;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            expressionInRadians = false;
        }
    }
}
