namespace Spot_Market
{
    class BeaconOnMap
    {
        Position pos;
        private int major;
        private int minor;

        public BeaconOnMap(int major, int minor, int xPos, int yPos)
        {
            this.major = major;
            this.minor = minor;
            pos = new Position(xPos, yPos);
        }
        public BeaconOnMap(int major, int minor, Position position)
        {
            this.major = major;
            this.minor = minor;
            this.pos = position;
        }

        public int getMajor()
        {
            return major;
        }
        public int getMinor()
        {
            return minor;
        }
        public Position getPosition()
        {
            return pos;
        }

        public override string ToString()
        {
            return "major: " + major.ToString() + ", minor:" + minor.ToString() + ", position: " + pos.getX().ToString() + ", " + pos.getY().ToString();
        }
    }
}