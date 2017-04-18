using Android.App;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using System;

namespace Spot_Market
{
    [Activity(Label = "Map")]
    public class Map : Activity
    {
        static readonly string TAG = typeof(Map).Name;

        private static Canvas canvas;
        private static Bitmap mutableBitmap;
        private static Bitmap workingBitmap;
        private static ImageView map;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);

            //Set BitmapFactory options in order to draw on photo
            BitmapFactory.Options myOptions = new BitmapFactory.Options();
            myOptions.InDither = true;
            myOptions.InScaled = false;
            myOptions.InPreferredConfig = Bitmap.Config.Argb8888;
            myOptions.InPurgeable = true;

            //Created Bitmap with map resource
            Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.map2, myOptions);

            //Red paint with antialias for performance improvement
            //paint.SetARGB(255, 255, 0, 0);
            //paint.AntiAlias = true;

            //Bitmaps used to render the image
            workingBitmap = Bitmap.CreateBitmap(bitmap);
            mutableBitmap = workingBitmap.Copy(Bitmap.Config.Argb8888, true);

            //Canvas to draw over
            canvas = new Canvas(mutableBitmap);

            //ImageView that displays the image 
            map = FindViewById<ImageView>(Resource.Id.imageView1);
            Drawable mapPhoto = GetDrawable(Resource.Drawable.map2);
            map.SetImageDrawable(mapPhoto);
            map.SetAdjustViewBounds(true);
            map.SetImageBitmap(mutableBitmap);

            //Draw point at x=500, y=100, radius=10 
            //drawPointOnMap(413, 379);
    
            FindAllBeacons _findAllBeacons = new FindAllBeacons(this);
            _findAllBeacons.FindBeacons(this);
            //drawPointOnMap(413, 379);

            //Check Data received by Intent in Item Activity
            string data = Intent.GetStringExtra("Data");
            if (data == "bicycle")
            {
                int[] x = new int[] { 185, 225, 252, 325, 325, 185 };
                int[] y = new int[] { 40, 40, 38, 38, 115, 115};
                highlightZone(x, y);
            }
            if (data == "book")
            {
                int[] x = new int[] { 65, 184, 184, 186, 186, 65 };
                int[] y = new int[] { 41, 41, 116, 116, 179, 179 };
                highlightZone(x, y);
            }

            Button item = FindViewById<Button>(Resource.Id.button1); //Declare item button
            item.Click += delegate
            {
                StartActivity(typeof(Item));
            };
        }
        public static void drawPointOnMap(int x, int y)
        {
            Paint paint = new Paint();
            paint.SetARGB(255, 255, 0, 0);
            paint.AntiAlias = true;

            mutableBitmap = workingBitmap.Copy(Bitmap.Config.Argb8888, true);
            canvas = new Canvas(mutableBitmap);
            map.SetImageBitmap(mutableBitmap);

            GC.Collect();

            Log.Debug(TAG, "drawing: {0}, {1}", x, y);
            canvas.DrawCircle(x, y, 10, paint);
        }

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