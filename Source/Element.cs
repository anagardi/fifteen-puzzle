using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fifteen
{
    public class Element : TextBlock
    {
          public Element(int value) {

            TextAlignment = TextAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            Height = 95;
            Width = 95;
            FontSize = 75;
            FontFamily = new FontFamily("Times New Roman");
            Text = value.ToString();

            var list = new List<int> { 1, 3, 6, 8, 9, 11, 14 };
            
            if (list.Contains(value))
            {
                Background = new SolidColorBrush(Colors.Pink);
                Foreground = new SolidColorBrush(Colors.Crimson);
            }
            else
            {
                Background = new SolidColorBrush(Colors.White);
                Foreground = new SolidColorBrush(Colors.Green);
            }

            PreviewMouseLeftButtonDown += MovementHelper.PreviewMouseLeftButtonDown;
            PreviewMouseLeftButtonUp += MovementHelper.PreviewMouseLeftButtonUp;
        }
    }
}
