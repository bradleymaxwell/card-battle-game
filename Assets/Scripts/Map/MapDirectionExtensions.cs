namespace Map
{
    public static class MapDirectionExtensions
    {
        public static (int q, int r) GetOffset(this MapDirection direction)
        {
            return direction switch
            {
                MapDirection.Left => (-1, 0),
                MapDirection.Right => (1, 0),
                MapDirection.LeftUp => (-1, 1),
                MapDirection.RightUp => (0, 1),
                MapDirection.LeftDown => (0, -1),
                MapDirection.RightDown => (1, -1),
                _ => (0, 0)
            };
        }
        
        public static MapDirection GetLeft(this MapDirection direction)
        {
            return direction switch
            {
                MapDirection.Left => MapDirection.LeftDown,
                MapDirection.LeftDown => MapDirection.RightDown,
                MapDirection.RightDown => MapDirection.Right,
                MapDirection.Right => MapDirection.RightUp,
                MapDirection.RightUp => MapDirection.LeftUp,
                MapDirection.LeftUp => MapDirection.Left,
                _ => direction
            };
        }
        
        public static MapDirection GetRight(this MapDirection direction)
        {
            return direction switch
            {
                MapDirection.Left => MapDirection.LeftUp,
                MapDirection.LeftDown => MapDirection.RightDown,
                MapDirection.RightDown => MapDirection.Right,
                MapDirection.Right => MapDirection.RightUp,
                MapDirection.RightUp => MapDirection.LeftUp,
                MapDirection.LeftUp => MapDirection.Left,
                _ => direction
            };
        }
    }
}