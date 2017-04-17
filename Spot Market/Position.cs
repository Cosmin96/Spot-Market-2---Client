namespace Spot_Market
{
    class Position
    {
        private int xPos;
        private int yPos;

        public Position(int xPos, int yPos)
        {
            this.xPos = xPos;
            this.yPos = yPos;
        }

        public void setCoordinates(int xPos, int yPos)
        {
            this.xPos = xPos;
            this.yPos = yPos;
        }
        
        public void setX(int xPos)
        {
            this.xPos = xPos;
        }
        public void setY(int yPos)
        {
            this.yPos = yPos;
        }

        public int getX()
        {
            return xPos;
        }
        public int getY()
        {
            return yPos;
        }

        override public string ToString()
        {
            return "x: " + xPos.ToString() + ", y: " + yPos.ToString();
        }
    }
}