using System;
using Android.App;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FileSharingApp.Helpers;

namespace FileSharingApp.Views
{
	public class FileViewHolder : RecyclerView.ViewHolder
	{
		public string UserName { get; set; }
		public string HostIp { get; set; }
		public int PortNumber { get; set; }
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

		public FileViewHolder(View itemView) : base(itemView)
		{
			Image = itemView.FindViewById<ImageView>(Resource.Id.image);
			TvName = itemView.FindViewById<TextView>(Resource.Id.tvName);

			Image.Click += ImageOnClicked;
		}

		private void ImageOnClicked(object sender, EventArgs e)
		{
			var intent = new Intent(ItemView.Context, typeof(FileDetailView));
			intent.PutExtra(AppConstants.FileName, TvName.Text);
			intent.PutExtra(AppConstants.UserName, UserName);
			intent.PutExtra(AppConstants.HostIp, HostIp);
			intent.PutExtra(AppConstants.PortNumber, PortNumber);
			ItemView.Context.StartActivity(intent);
		}
	}
}