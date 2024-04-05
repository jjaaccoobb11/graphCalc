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
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Windows.Interop;

namespace graph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string hiddenString;

        //Svaret på uttrycket man skriver in ska inte vara i radianer vid start (tills man bytar till radianer)
        static bool expressionInRadians = false;

        //Skriver man in ett trigonometriskt uttryck ändras den till true
        static bool trigExpression = false;

        //Antalet slutparanteser som behövs
        static int needsEndParenthesis = 0;

        private double initialWidth;
        private double initialHeight;

        public MainWindow()
        {
            InitializeComponent();
            SizeChanged += MainWindow_SizeChanged;
            initialWidth = Width;
            initialHeight = Height;

            this.SizeChanged += GraphWindow_SizeChanged;

            //this.MouseDown += MainWindow_MouseDown;

            this.Loaded += MainWindow_Loaded;
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


        //Länken till sidan med manualen
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "https://jjaaccoobb11.github.io/";
            p.Start();
        }


        //När man trycker på ENTER knappen
        private void Button_Click_Enter(object sender, RoutedEventArgs e)
        {
            //Med hjälp av Matheval så kan man skicka in långa uttryck och få ut ett svar
            answer.Text = EvaluateMathExpression().ToString();
            //Nollställer allt som skall nollställas
            trigExpression = false;
            expression.Text = "";
            expression.Text = "";
            hiddenString = "";
        }

        static string EvaluateMathExpression()
        {
            //Tar och gör en expression av uttrycket
            org.matheval.Expression expression = new org.matheval.Expression(hiddenString);
            try
            {
                //Försöker få ett svar på uttrycket
                Object value = expression.Eval(); // return 0
                return value.ToString();
            }
            catch
            {
                //Går det inte så kommer ett felfönser upp
                ErrorWindow errorWindow = new ErrorWindow();
                errorWindow.Show();
                return "";
            }
            
        }

        //----------------------------------------------------------------------------------------INPUT BUTTONS----------------------------------------------------------------
        //Detta händer när man ska skriva in ett uttryck med hjälp av knapparna.
        private void Button_Input_Text(object sender, RoutedEventArgs e)
        {
            //Använder mig av 2 olika strängar
            //Det gömda uttrycket 
            string hiddenContent = ((Button)sender).Content.ToString();

            //Det uttrycket som syns på skärmen
            string showContent = ((Button)sender).Content.ToString();


            bool pass = false;
            //Om det behövs lägga till en slut parantes så görs det här
            if (needsEndParenthesis != 0)
            {
                //Slut parantesen läggs till (bara i gömda strängen
                hiddenContent += ")";
                needsEndParenthesis--;
                while (needsEndParenthesis != 0)
                {
                    hiddenContent += ")";
                    needsEndParenthesis--;
                }
                pass = true;
            }

            //Kollar vilken knapp som var tryckt på
            switch (hiddenContent)
            {
                case "log":
                    //Det användaren ser
                    showContent = "log(";
                    //Det matheval vill ha som input
                    hiddenContent = "LOG10(";
                    break;
                case "^":

                    hiddenContent = "^";
                    showContent = "^";
                    break;

                //-------------------TRIG UTTRYCK---------------------------------
                //Alla trig uttryck ändrar trigExpressions till true
                case "cos":
                    trigExpression = true;

                    //Kollar om svaret ska komma ut i grader eller radianer
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
                //-----------------------------------SLUT PÅ TRIG------------------------------------------
                case "ln":
                    showContent = "ln(";
                    hiddenContent = "LN(";
                    break;
                case "𝛑":
                    showContent = "𝛑";
                    hiddenContent = "PI";
                    break;

                //(-)
                case "(-)":
                    showContent = "-";
                    hiddenContent = "(-1*";
                    needsEndParenthesis++;
                    break;

                case ")":
                    if (!pass)
                    {
                        if (trigExpression == true && expressionInRadians == false)
                        {
                            hiddenContent = "))";
                        }
                        else
                        {
                            hiddenContent = ")";
                        }

                        showContent = ")";
                    }
                    
                    break;
                

            }

            hiddenString += hiddenContent;
            expression.Text += showContent;
        }

        //Ränsar uttrycket
        private void Button_Click_Clear(object sender, RoutedEventArgs e)
        {
            answer.Text = "";
            expression.Text = "";
            hiddenString = "";
        }

        //Om man kryssar i att man vill ha svaret i radianer istället
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            expressionInRadians = true;
        }

        //Om man vill byta från radianer till grader
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            expressionInRadians = false;
        }

        //Öppnar fönstret med grafen
        private void Button_Open_Graph(object sender, RoutedEventArgs e)
        {
            GraphWindow window2 = new GraphWindow();
            window2.Show();

        }

        private void GraphWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine(Mouse.LeftButton.ToString());

        }
    }
}
