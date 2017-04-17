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

            Log.Debug(TAG, "drawin: {0}, {1}", x, y);
            canvas.DrawCircle(x, y, 10, paint);
        }
    }
}