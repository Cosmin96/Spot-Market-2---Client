using Android.App;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using System;

namespace Spot_Market
{
    /// <summary>
    /// Activity for drawing the store's map and the user's location on the map
    /// </summary>
    [Activity(Label = "Map")]
    public class Map : Activity
    {
        static readonly string TAG = typeof(Map).Name;

        /// <summary>
        /// Canvas on which the map of the store and the user's location are drawn
        /// </summary>
        private static Canvas canvas;
        
        // Bitmaps used to render the image, used to restore the canvas to the initial state with the store's map on it
        private static Bitmap mutableBitmap;
        private static Bitmap workingBitmap;
        
        // ImageView with the store's map
        private static ImageView map;
        
        // The radius of the points drawn on canvas
        private static int pointRadius = 10;

        private LocateUser _locateUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);

            //Set BitmapFactory options in order to draw on the store's map
            BitmapFactory.Options myOptions = new BitmapFactory.Options();
            myOptions.InDither = true;
            myOptions.InScaled = false;
            myOptions.InPreferredConfig = Bitmap.Config.Argb8888;
            myOptions.InPurgeable = true;

            //Created Bitmap with map resource
            Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.map6, myOptions);

            //Bitmap used to render the image
            workingBitmap = Bitmap.CreateBitmap(bitmap);

            //ImageView that displays the image 
            map = FindViewById<ImageView>(Resource.Id.imageView1);
            Drawable mapPhoto = GetDrawable(Resource.Drawable.map6);
            map.SetImageDrawable(mapPhoto);
            map.SetAdjustViewBounds(true);
            
            _locateUser = new LocateUser(this);
            // Start the thread that is searching for beacons and is doing the positioning algorithm
            _locateUser.FindBeacons(this);

            // Check Data received by Intent in Item Activity, if returning from the Items screen
            string data = Intent.GetStringExtra("Data");

            switch (data)
            {
                case "bicycle":
                    int[] x = new int[] { 133, 350, 350, 348, 348, 133 };
                    int[] y = new int[] { 57, 57, 148, 148, 209, 209 };
                    highlightZone(x, y);
                    break;
                case "book":
                    x = new int[] { 351, 455, 455, 351 };
                    y = new int[] { 59, 59, 148, 148 };
                    highlightZone(x, y);
                    break;
            }

            //Declare item button
            Button item = FindViewById<Button>(Resource.Id.button1); 
            item.Click += delegate
            {
                StartActivity(typeof(Item));
                _locateUser.Stop();
            };
        }

        /// <summary>
        /// Restores the canvas back to the initial state when it only had the map of the store drawn on it (on the store's map)
        /// </summary> 
        public static void refreshCanvas()
        {
            mutableBitmap = workingBitmap.Copy(Bitmap.Config.Argb8888, true);
            canvas = new Canvas(mutableBitmap);
            map.SetImageBitmap(mutableBitmap);

            GC.Collect();
        }

        /// <summary>
        /// Draws a point on the canvas (on the store's map)
        /// </summary> 
        /// <param name="x">X coordinate of the point you want to draw</param>
        /// <param name="y">Y coordinate of the point you want to draw</param>
        /// <param name="color">The color you want the point to be drawn with</param>
        public static void drawPointOnMap(int x, int y, string color)
        {
            Paint paint = new Paint();
            switch(color)
            {
                case "red":
                    paint.SetARGB(255, 255, 0, 0);
                    break;
                case "green":
                    paint.SetARGB(255, 0, 255, 0);
                    break;
                case "blue":
                    paint.SetARGB(255, 0, 0, 255);
                    break;
                case "cyan":
                    paint.SetARGB(255, 0, 255, 255);
                    break;
                case "yellow":
                    paint.SetARGB(255, 255, 255, 0);
                    break;
                case "purple":
                    paint.SetARGB(255, 255, 0, 255);
                    break;
                default:
                    paint.SetARGB(255, 0, 0, 0);
                    break;
            }
            paint.AntiAlias = true;

            canvas.DrawCircle(x, y, pointRadius, paint);
        }

        /// <summary>
        /// Highlight a specific zone given x and y coordinates of the zone's vertices
        /// </summary> 
        /// <param name="x">The ordered list with the X coordinates of the vertices</param>
        /// <param name="y">The ordered list with the Y coordinates of the vertices</param>
        public static void highlightZone(int[] x, int[] y)
        {
            Paint zonePaint = new Paint();
            zonePaint.SetARGB(255, 0, 156, 222);
            zonePaint.SetStyle(Paint.Style.FillAndStroke);
            zonePaint.AntiAlias = true;

            Paint contourPaint = new Paint();
            contourPaint.SetARGB(255, 255, 0, 0);
            contourPaint.SetStyle(Paint.Style.Stroke);
            contourPaint.AntiAlias = true;

            Path zone = new Path();
            Path contour = new Path();

            zone.MoveTo(x[0], y[0]);
            contour.MoveTo(x[0], y[0]);

            for (int i = 1; i < x.Length; i++)
            {
                zone.LineTo(x[i], y[i]);
                contour.LineTo(x[i], y[i]);
            }
            zone.LineTo(x[0], y[0]);
            contour.LineTo(x[0], y[0]);
            zone.Close();
            canvas.DrawPath(zone, zonePaint);
            canvas.DrawPath(contour, contourPaint);
        }
    }
}