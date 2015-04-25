namespace Kelner.Model
{
    using System.Windows.Media;

    /// <summary>
    /// Klasa bazowa dla sekcji w programie czyli jednego sektora na planszy restauracji
    /// </summary>
    public abstract class Section
    {
        public int X { get; set; }
        public int Y { get; set; }

        public virtual Brush Brush
        {
            get
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public abstract bool CanEnter();
    }

    public class FreeField : Section
    {
        public override bool CanEnter()
        {
            return true;
        }
    }

    public class Obstacle : Section
    {
        public override bool CanEnter()
        {
            return false;
        }
    }

    public class TablePart : Obstacle
    {
        public override Brush Brush
        {
            get
            {
                return new SolidColorBrush(Colors.Brown);
            }
        }
    }

    public class Floor : FreeField
    {
        public override Brush Brush
        {
            get
            {
                return new SolidColorBrush(Colors.CadetBlue);
            }
        }
    }

    public class Chair : Obstacle
    {
        public bool IsFree { get; set; }

        public override Brush Brush
        {
            get
            {
                return new SolidColorBrush(Colors.RosyBrown);
            }
        }
    }

    public class Kitchen : Obstacle
    {
        public override Brush Brush
        {
            get
            {
                return new SolidColorBrush(Colors.Blue);
            }
        }
    }

    public class Waiter : Obstacle
    {
        
    }

    public class ClientSection : Obstacle
    {
        
    }
}
