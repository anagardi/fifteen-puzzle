using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Fifteen
{
    public class MovementHelper
    {
        // Length of each cell on movement target
        public static short Length = 100;

        // The number of moves during one game session
        public static short NumberOfMoves = 0;

        //The target where the elements can move
        private static Canvas _movementTarget;

        //Logical collection of puzzle elements 
        private static Collection<int> _puzzle = new Collection<int>();

        //The width of grid nxn
        public const int GridWidth = 4;


        /// <summary>
        /// Initiates the child elements of movement target 
        /// </summary>
        public static void GenerateElements(RoutedEventArgs e)
        {
            if (_movementTarget == null)
            {
                Visual visual = e.OriginalSource as Visual;
                _movementTarget = FindChild<Canvas>(visual);
            }            

            _puzzle = GeneratePuzzle();

            //Add puzzle element to movement target
            AddElementsToTarget(_puzzle, _movementTarget);
        }

        public static void AddElementsToTarget(Collection<int> collection, Canvas canvas)
        {
            if (canvas != null)
            {
                canvas.Children.Clear();
            }

            for (int k = 0; k < GridWidth * GridWidth; k++)
            {
                if (collection[k] == 0)
                {
                    continue;
                }

                Element element = new Element(collection[k]);

                int i = k % GridWidth; //get the number of column
                int j = k / GridWidth; // get the number of row

                Canvas.SetLeft(element, i * Length);
                Canvas.SetTop(element, j * Length);

               canvas.Children.Add(element);
            }
        }



        /// <summary>
        /// Generate puzzle elements solvable collection
        /// </summary>
        public static Collection<int> GeneratePuzzle()
        {
            Collection<int> collection;

            //Generate solvable sequence of numbers collection
            do
            {
                collection = GenerateCOllection();
            }
            while (!IsSolvable(collection));

            return collection;
        }

        /// <summary>
        /// Generate puzzle elements shuffled collection. Available values from 0 to 15
        /// </summary>
        private static Collection<int> GenerateCOllection()
        {
            Collection<int> collection = new Collection<int>();

            Random random = new Random();

            int bound = GridWidth * GridWidth;

            while (collection.Count < bound)
            {
                int nextValue = random.Next(0, bound);

                if (!collection.Contains(nextValue))
                {
                    collection.Add(nextValue);
                }
            }
            return collection;
        }

        /// <summary>
        /// Checks whether the puzzle is solvable
        /// If a state is such that:
        /// If the width is odd, then the state has an even number of inversions.
        /// If the width is even and the blank is on an odd numbered row counting from the bottom, then the state has an even number of inversions
        /// If the width is even and the blank is on an even numbered row counting from the bottom, then the state has an odd number of inversions
        /// Then the puzzle is solvable from that state.
        /// </summary>
        public static bool IsSolvable(Collection<int> collection)
        {
            int parity = 0;
            int gridWidth = (int)Math.Sqrt(collection.Count);
            int row = 0; // the current row we are on
            int blankRow = 0; // the row with the blank tile

            for (int i = 0; i < collection.Count; i++)
            {
                if (i % gridWidth == 0)
                { // advance to next row
                    row++;
                }
                if (collection[i] == 0)
                { // the blank tile
                    blankRow = row; // save the row on which encountered
                    continue;
                }
                for (int j = i + 1; j < collection.Count; j++)
                {
                    if (collection[i] > collection[j] && collection[j] != 0)
                    {
                        parity++;
                    }
                }
            }

            if (gridWidth % 2 == 0)
            { // even grid
                if (blankRow % 2 == 0)
                { // blank on odd row; counting from bottom
                    return parity % 2 == 0;
                }
                else
                { // blank on even row; counting from bottom
                    return parity % 2 != 0;
                }
            }
            else
            { // odd grid
                return parity % 2 == 0;
            }
        }

        /// <summary>
        /// Finds and returns the empty cell on movement target
        /// </summary>
        public static Rect GetEmptyRectangle(Canvas canvas)
        {
            Rect emptyRectangle = new Rect();           

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    bool isEmpty = true;

                    emptyRectangle = GetRectangleLocation(i, j);

                    foreach (Element element in canvas.Children)
                    {
                        double top = Canvas.GetTop(element);
                        double left = Canvas.GetLeft(element);

                        if (emptyRectangle.Left != left || emptyRectangle.Top != top)
                            continue;

                        isEmpty = false;
                        break;
                    }
                    if (isEmpty)
                    {
                        return emptyRectangle;
                    }
                }
            }
            return emptyRectangle;
        }

        private static Rect GetRectangleLocation(int i, int j)
        {
            return new Rect(i * Length, j * Length, Length, Length);
        }


        /// <summary>
        /// Moves selected element to empty cell 
        /// </summary>
        public static void MoveElementToEmptyRectangle(Rect tempRect, Rect emptyRect)
        {
            foreach (Element element in _movementTarget.Children)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);

                if ((tempRect.Left != left) || tempRect.Top != top) { continue; }

                Canvas.SetLeft(element, emptyRect.Left);
                Canvas.SetTop(element, emptyRect.Top);
                return;
            }
        }

        /// <summary>
        /// Handles keyboard events from left,right, up and down keys to move the cells next to empty one correspondingly 
        /// </summary>
        public static void PreviewKeyDown(KeyEventArgs e)
        {
            if (_movementTarget.IsEnabled)
            {
                Rect emptyRectangle = GetEmptyRectangle(_movementTarget);                

                switch (e.Key)
                {
                    case Key.Left:
                        Rect leftRect = new Rect(emptyRectangle.Left + Length, emptyRectangle.Top, Length, Length);
                        MoveElementToEmptyRectangle(leftRect, emptyRectangle);
                        NumberOfMoves++;
                        break;
                    case Key.Right:
                        Rect rightRect = new Rect(emptyRectangle.Left - Length, emptyRectangle.Top, Length, Length);
                        MoveElementToEmptyRectangle(rightRect, emptyRectangle);
                        NumberOfMoves++;
                        break;
                    case Key.Up:
                        Rect upRect = new Rect(emptyRectangle.Left, emptyRectangle.Top + Length, Length, Length);
                        MoveElementToEmptyRectangle(upRect, emptyRectangle);
                        NumberOfMoves++;
                        break;
                    case Key.Down:
                        Rect downRect = new Rect(emptyRectangle.Left, emptyRectangle.Top - Length, Length, Length);
                        MoveElementToEmptyRectangle(downRect, emptyRectangle);
                        NumberOfMoves++;
                        break;
                }                
            }
        }

        /// <summary>
        /// If the event sender is next to the empty cell, the sender moves to the empty cell
        /// </summary>
        public static void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rect emptyRect = GetEmptyRectangle(_movementTarget);

            Element element = (Element)sender;

            if (element != null)
            {
                element.CaptureMouse();


                if ((Canvas.GetTop(element) == emptyRect.Top) && (Canvas.GetLeft(element) == emptyRect.Right))
                {
                    Canvas.SetLeft(element, emptyRect.Left);
                    NumberOfMoves++;
                } else if ((Canvas.GetTop(element) == emptyRect.Top) && (Canvas.GetLeft(element) + MovementHelper.Length == emptyRect.Left))
                {
                    Canvas.SetLeft(element, emptyRect.Left);
                    NumberOfMoves++;
                } else if ((Canvas.GetTop(element) == emptyRect.Bottom) && (Canvas.GetLeft(element) == emptyRect.Left))
                {
                    Canvas.SetTop(element, emptyRect.Top);
                    NumberOfMoves++;
                } else if ((Canvas.GetTop(element) + MovementHelper.Length == emptyRect.Top) && (Canvas.GetLeft(element) == emptyRect.Left))
                {
                    Canvas.SetTop(element, emptyRect.Top);
                    NumberOfMoves++;
                }
            }
        }

        /// <summary>
        /// If Mouse is captured - release!
        /// </summary>
        public static void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Element element = (Element)sender;

            if (element != null)
            {
                if (element.IsMouseCaptured)
                {
                    element.ReleaseMouseCapture();
                }
            }
        }

        /// <summary>
        /// Checks the order of the child elements of the movement target
        /// </summary>
        /// <returns>Returns true if the elements' values are ordered ascendingly, else - false.</returns>
        public static bool ElementsAreOrdered(Canvas canvas)
        {            

            Collection<Element> elementsCollection = GetElementsCollection(canvas);

            return CheckElementsOrder(elementsCollection, canvas);
            
        }

        private static Collection<Element> GetElementsCollection(Canvas canvas)
        {
            Collection<Element> elements = new Collection<Element>();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Rect rect = GetRectangleLocation(j,i);

                    foreach (Element element in canvas.Children)
                    {
                        double left = Canvas.GetLeft(element);
                        double top = Canvas.GetTop(element);

                        if ((left == rect.Left) && (top == rect.Top))
                        {
                            elements.Add(element);
                        }
                    }
                }
            }

            return elements;
        }

        private static bool CheckElementsOrder(Collection<Element> collection, Canvas canvas)
        {
            foreach (Element element in collection)
            {
                string tempString = (collection.IndexOf(element) + 1).ToString();
                if (tempString != element.Text)
                {
                    return false;
                }
            }

            Rect emptyRect = GetEmptyRectangle(canvas);

            return (emptyRect.Left == Length * 3) && (emptyRect.Top == Length * 3);
        }
                

        #region Utilities
        /// <summary>
        /// Looks for a ancestor control over a visual by ancestorType
        /// </summary>
        public static FrameworkElement FindAncestor(Type ancestorType, Visual visual)
        {
            while (visual != null && !ancestorType.IsInstanceOfType(visual))
            {
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }
            return visual as FrameworkElement;
        }

        /// <summary>
        /// Looks for a child control within a parent by type
        /// </summary>
        public static T FindChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            // confirm parent is valid.
            if (parent == null)
                return null;
            if (parent is T)
                return parent as T;

            DependencyObject foundChild = null;

            if (parent is FrameworkElement)
                (parent as FrameworkElement).ApplyTemplate();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                foundChild = FindChild<T>(child);
                if (foundChild != null)
                    break;
            }

            return foundChild as T;
        }
        #endregion
    }
}
