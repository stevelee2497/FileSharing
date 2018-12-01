using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;

namespace FileSharingApp.Views
{
	public class FileAdapter : RecyclerView.Adapter
	{
		private readonly List<string> _images;
		private readonly string _userName;
		private readonly string _ip;
		private readonly int _portNumber;

		public FileAdapter(List<string> images, string ip, int portNumber, string userName)
		{
			_images = images;
			_ip = ip;
			_portNumber = portNumber;
			_userName = userName;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.image_template, parent, false);
			return new FileViewHolder(itemView);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			var holder = ((FileViewHolder)viewHolder);
			holder.HostIp = _ip;
			holder.PortNumber = _portNumber;
			holder.ImageName = _images[position];
			holder.UserName = _userName;
		}

		public override int ItemCount => _images.Count;
	}
}