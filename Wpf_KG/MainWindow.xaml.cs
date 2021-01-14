using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Wpf_KG
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObjParser pars;
        SceneProcessor proc;
        public MainWindow()
        {
            InitializeComponent();
            pars = new ObjParser();
            proc = new SceneProcessor();
            proc.mw = this;
            sliderX.Maximum = 2 * Math.PI;
            sliderY.Maximum = 2 * Math.PI;
            sliderZ.Maximum = 2 * Math.PI;

            proc.DisplayLine("Red", 0, 0, 0, 200, 1);
            proc.DisplayLine("Red", 0, 0, 200, 0, 1);
        }

        public bool flag = true; 
        private void functionAnimation()
        {
            proc.setRotation(0.05);
            
            Dispatcher.Invoke(() => {
                proc.Command("Вращение по X");
                proc.Command("Перемещение");
                
                if (flag == true)
                {
                    proc.internalPoint.X += 1;
                    proc.movingX += 1;
                }
                else
                {
                    proc.internalPoint.X -= 1;
                    proc.movingX -= 1;
                }

                if(proc.movingX < -20)
                {
                    flag = true;
                }
                if (proc.movingX > 20)
                {
                    flag = false;
                }
            });
        }

        private bool botFlag = false;
        System.Timers.Timer t1;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (botFlag == false)
            {
                proc.movingX = 0;
                proc.movingY = 0;
                proc.movingZ = 0;
                t1 = new System.Timers.Timer();
                t1.Interval = 50;
                t1.Elapsed += (s, a) =>
                {
                    functionAnimation();

                };

                t1.Start();
                proc.movingX = 0;
                botFlag = true;
            }
            else
            {
                t1.Stop();
                t1.Dispose();
                botFlag = false;
            }
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {/*
            double add = 3;
            if(e.Key == Key.Right)
            {
                if(sliderX.Value + add > 100)
                {
                    sliderX.Value = add - (100 - sliderX.Value);
                }
                else
                {
                    sliderX.Value += add;
                }
            } else if(e.Key == Key.Left)
            {
                if (sliderX.Value - add < 0)
                {
                    sliderX.Value = 100 - (add - sliderX.Value);
                }
                else
                {
                    sliderX.Value -= add;
                }
            }else if(e.Key == Key.Down)
            {
                if (sliderY.Value + add > 100)
                {
                    sliderY.Value = add - (100 - sliderY.Value);
                }
                else
                {
                    sliderY.Value += add;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (sliderY.Value - add < 0)
                {
                    sliderY.Value = 100 - (add - sliderY.Value);
                }
                else
                {
                    sliderY.Value -= add;
                }
            }*/
        }


        //Загрузка и выбор объекта
        bool init = false;
        private void listObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (init == true)
            {
                sliderScal.Value = 1;
                listAction.SelectedIndex = 0;
                RedrawScene();
                proc.listEdge = pars.WriteObject((listObject.SelectedItem as TextBlock).Text);
                proc.DrawAllEdge(proc.listEdge);
            }
        }

        public void RedrawScene()
        {
            CanvasArea.Children.Clear();
            proc.DisplayLine("Red", 0, 0, 0, 200, 1);
            proc.DisplayLine("Red", 0, 0, 200, 0, 1);
        }

        public double prevValueScal = 0;

        private void sliderScal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init == true)
            {
                proc.setScale(sliderScal.Value - prevValueScal);
                proc.Command("Масштабирование");
            }
            prevValueScal = sliderScal.Value;
        }

        //Выбор действия над объектом
        private void listAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (init == true)
            {
                RedrawScene();
                string commandCurrent = (listAction.SelectedItem as TextBlock).Text;
                if(commandCurrent == "Перемещение")
                {
                    proc.movingX = 100;
                    proc.movingY = 100;
                    proc.movingZ = 0;
                }
                proc.Command(commandCurrent);
            }
            else
            {
                init = true;
            }
        }

        private double prevRotationSliderX = 0;
        private void sliderX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init == true)
            {
                proc.setRotation(sliderX.Value - prevRotationSliderX);
                proc.Command("Вращение по X");
            }
            prevRotationSliderX = sliderX.Value;
        }

        private double prevRotationSliderY = 0;
        private void sliderY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init == true)
            {
                proc.setRotation(sliderY.Value - prevRotationSliderY);
                proc.Command("Вращение по Y");
            }
            prevRotationSliderY = sliderY.Value;
        }

        private double prevRotationSliderZ = 0;
        private void sliderZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init == true)
            {
                proc.setRotation(sliderZ.Value - prevRotationSliderZ);
                proc.Command("Вращение по Z");
            }
            prevRotationSliderZ = sliderZ.Value;
        }
    }
}
