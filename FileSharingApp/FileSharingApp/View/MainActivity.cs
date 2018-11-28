using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FileSharingApp.Helpers;
using System;
using System.Collections.Generic;

namespace FileSharingApp.View
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		private readonly List<string> _files = new List<string>
		{
			"image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip",
			"image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip",
			"image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip",
			"image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip",
			"image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg",
			"image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg",
			"image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg",
			"image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg",
		};

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			var rvFiles = FindViewById<RecyclerView>(Resource.Id.recyclerView);
			var fileAdapter = new FileAdapter(_files);
			rvFiles.SetLayoutManager(new GridLayoutManager(this, 4));
			rvFiles.SetAdapter(fileAdapter);

			var btnNewFile = FindViewById<ImageButton>(Resource.Id.btnNewFile);
			btnNewFile.Click += ShowBottomSheet;
		}

		private void ShowBottomSheet(object sender, EventArgs e)
		{
			var bottomSheet = new BottomSheetFragment();
			bottomSheet.Show(SupportFragmentManager, bottomSheet.Tag);
		}
	}

	public class FileAdapter : RecyclerView.Adapter
	{
		private readonly List<string> _images;

		public FileAdapter(List<string> images)
		{
			_images = images;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.image_template, parent, false);
			return new FileViewHolder(itemView);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			((FileViewHolder)viewHolder).ImageName = _images[position];
		}

		public override int ItemCount => _images.Count;
	}

	public class FileViewHolder : RecyclerView.ViewHolder
	{
		public string ImageName
		{
			set
			{
				TvName.Text = value;
				Image.SetImageDrawable(value.HasImageExtension()
					? ContextCompat.GetDrawable(Application.Context, Resource.Drawable.default_image)
					: ContextCompat.GetDrawable(Application.Context, Resource.Drawable.default_file));
			}
		}

		public ImageView Image { get; private set; }
		public TextView TvName { get; private set; }

		public FileViewHolder(Android.Views.View itemView) : base(itemView)
		{
			Image = itemView.FindViewById<ImageView>(Resource.Id.image);
			TvName = itemView.FindViewById<TextView>(Resource.Id.tvName);
		}
	}


}