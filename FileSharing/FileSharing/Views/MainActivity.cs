using System.Collections.Generic;
using Android.App;
using Android.Media;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace FileSharing.Views
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
        }
    }

	public class ImageAdapterViewHolder : RecyclerView.ViewHolder
	{
		public ImageView Image { get; private set; }
		public TextView TvName { get; private set; }

		public ImageAdapterViewHolder(View itemView) : base(itemView)
		{
			Image = itemView.FindViewById<ImageView>(Resource.Id.image);
			TvName = itemView.FindViewById<TextView>(Resource.Id.tvName);
		}
	}

	public class ImageAdapter : RecyclerView.Adapter
	{
		private readonly List<Image> _images;

		public ImageAdapter(List<Image> images)
		{
			_images = images;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.image_template, parent, false);
			return new ImageAdapterViewHolder(itemView);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			var item = _images[position];
			var holder = viewHolder as ImageAdapterViewHolder;
			
		}

		public override int ItemCount => _images.Count;
	}
}