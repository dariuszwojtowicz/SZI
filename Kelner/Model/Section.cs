namespace Kelner.Model
{
    /// <summary>
    /// Klasa bazowa dla sekcji w programie czyli jednego sektora na planszy restauracji
    /// </summary>
    public abstract class Section
    {
        public int X { get; set; }
        public int Y { get; set; }

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
        
    }

    public class Floor : FreeField
    {
        
    }

    public class Chair : Obstacle
    {
        public bool IsFree { get; set; }
    }

    public class Kitchen : Obstacle
    {
        
    }

    public class Waiter : Obstacle
    {
        
    }

    public class ClientSection : Obstacle
    {
        
    }
}
