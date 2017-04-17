using System;
using System.Collections.Generic;

using Android.Content;
using Android.Util;

using Android.Graphics;
using EstimoteSdk;


namespace Spot_Market
{
    class FindAllBeacons : BeaconFinder
    {
        public static readonly EstimoteSdk.Region ALL_ESTIMOTE_BEACONS_REGION = new EstimoteSdk.Region("rid", "B9407F30-F5F8-466E-AFF9-25556B57FE6D");

        static readonly string TAG = typeof(FindAllBeacons).Name;

        public EventHandler<BeaconsFoundEventArgs> BeaconsFound = delegate { };

        public FindAllBeacons(Context context) : base(context)
        {
            BeaconManager.Ranging += HandleRanging;
        }

        public override void OnServiceReady()
        {
            BeaconManager.StartRanging(ALL_ESTIMOTE_BEACONS_REGION);
        }

        private static void swap<T>(ref T a, ref T b)
        {
            T aux = a;
            a = b;
            b = aux;
        }

        private Position intersectThreeCircles(Position p1, int r1, Position p2, int r2, Position p3, int r3)
        {
            int x1 = p1.getX();
            int y1 = p1.getY();
            int x2 = p2.getX();
            int y2 = p2.getY();
            int x3 = p3.getX();
            int y3 = p3.getY();

            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            if (d > r1 + r2)
            { // no intersection
                //Log.Debug(TAG, "\n{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", d, x1, y1, r1, x2, y2, r2, x3, y3, r3);
                //Log.Debug(TAG, "the circles don't intersect each other\n");

                swap(ref x1, ref x3);
                swap(ref y1, ref y3);
                swap(ref r1, ref r3);
                d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
                Log.Debug(TAG, "\n{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", d, x1, y1, r1, x2, y2, r2, x3, y3, r3);
                //Log.Debug(TAG, "the circles don't intersect each other\n");

                if (d > r1 + r2)
                {
                    swap(ref x2, ref x3);
                    swap(ref y2, ref y3);
                    swap(ref r2, ref r3);
                    d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
                    
                    if (d > r1 + r2)
                    {
                        Log.Debug(TAG, "\n{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", d, x1, y1, r1, x2, y2, r2, x3, y3, r3);
                        //Log.Debug(TAG, "the circles don't intersect each other\n");

                        return new Position(0, 0); // empty list of results
                    }
                }
            }
            // intersection(s) should exist

            double l = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
            double h = Math.Sqrt(r1 * r1 - l * l);
            double ix1 = l * (x2 - x1) / d + h * (y2 - y1) / d + x1;
            double ix2 = l * (x2 - x1) / d - h * (y2 - y1) / d + x1;
            double iy1 = l * (y2 - y1) / d - h * (x2 - x1) / d + y1;
            double iy2 = l * (y2 - y1) / d + h * (x2 - x1) / d + y1;

            if (Math.Abs(r3 - Math.Sqrt((x3 - ix1) * (x3 - ix1) + (y3 - iy1) * (y3 - iy1))) < Math.Abs(r3 - Math.Sqrt((x3 - ix2) * (x3 - ix2) + (y3 - iy2) * (y3 - iy2))))
                return new Position((int)ix1, (int)iy1);

            return new Position((int)ix2, (int)iy2);
        }

        // store in mapBeacons the beacons on the map
        private void getMapBeacons(ref BeaconOnMap[] mapBeacons)
        {
            mapBeacons[0] = new BeaconOnMap(23896, 25923, 614, 345);
            mapBeacons[1] = new BeaconOnMap(7354, 47683, 158, 230);
            mapBeacons[2] = new BeaconOnMap(58610, 36050, 324, 257);
            mapBeacons[3] = new BeaconOnMap(18155, 58632, 446, 162);
            mapBeacons[4] = new BeaconOnMap(61265, 34052, 333, 116);
            mapBeacons[5] = new BeaconOnMap(36242, 5515, 306, 493);
            mapBeacons[6] = new BeaconOnMap(31499, 2016, 416, 369);
            mapBeacons[7] = new BeaconOnMap(30745, 50948, 66, 98);
        }

        // store in b3 the closest 3 beacons to the user's position
        // store in d the distances (in pixels) from the user to the closest 3 beacons
        private void getClosest3BeaconsOnMap(ref BeaconOnMap[] b3, ref int[] dist, IList<Beacon> beacons, BeaconOnMap[] mapBeacons, int ratio)
        {
            int i = 0;

            foreach (Beacon b in beacons)
            {
                for (int j = 0; j < 8; j++)
                    if (mapBeacons[j].getMajor() == b.Major && mapBeacons[j].getMinor() == b.Minor)
                    {
                        b3[i] = mapBeacons[j];
                        dist[i] = (int)(ratio * Utils.ComputeAccuracy(b));
                        break;
                    }
                if (++i >= 3)
                    break;
            }
        }

        protected virtual void HandleRanging(object sender, BeaconManager.RangingEventArgs e)
        {     
            // major, minor, xpos, ypos
              
            BeaconOnMap[] mapBeacons = new BeaconOnMap[8];
            getMapBeacons(ref mapBeacons);

            // distanceInMeters * ratio = distanceInPixels
            const int ratio = 43;
            Log.Debug(TAG, "Found {0} beacons.", e.Beacons.Count);
            foreach(Beacon b in e.Beacons)
            {
                Log.Debug(TAG, "major: {0}, minor: {1}, distance: {2}", b.Major, b.Minor, Utils.ComputeAccuracy(b));
            }

            BeaconsFound(this, new BeaconsFoundEventArgs(e.Beacons));

            if (e.Beacons.Count < 3)
                return;

            BeaconOnMap[] b3 = new BeaconOnMap[3];
            int[] dist = new int[3];

            getClosest3BeaconsOnMap(ref b3, ref dist, e.Beacons, mapBeacons, ratio);
            
            Position pos = intersectThreeCircles(b3[0].getPosition(), dist[0], b3[1].getPosition(), dist[1], b3[2].getPosition(), dist[2]);

            //Log.Debug(TAG, "{0}", canvas.ActualHeight);
            if(pos.getX() != 0 && pos.getY() != 0)
                Map.drawPointOnMap(pos.getX(), pos.getY());

            Log.Debug(TAG, pos.ToString());

            //            IEnumerable<Beacon> beacons = from item in e.Beacons
            //                                          let uuid = item.ProximityUUID
            //                                          where uuid.Equals(EstimoteBeacons.EstimoteProximityUuid, StringComparison.OrdinalIgnoreCase) ||
            //                                                uuid.Equals(EstimoteBeacons.EstimoteIosProximityUuid, StringComparison.OrdinalIgnoreCase)
            //                                          select item;
        }

        public void FindBeacons(Context context)
        {
            //TODO: Properly detect BT Enabled
            var btEnabled = true;
            if (!btEnabled)
            {
                throw new Exception("Bluetooth is not enabled.");
            }
            BeaconManager.Connect(this);

        }

        public override void Stop()
        {
            BeaconManager.StopRanging(ALL_ESTIMOTE_BEACONS_REGION);

            base.Stop();
        }
    }
}
