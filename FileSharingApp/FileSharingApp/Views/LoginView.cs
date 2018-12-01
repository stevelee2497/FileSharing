using System;
using System.IO;
using System.Net.Sockets;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FileSharingApp.Models;

namespace FileSharingApp.Views
{
	[Activity(Label = "LoginView", Theme = "@style/AppTheme.Fullscreen", MainLauncher = true)]
	public class LoginView : AppCompatActivity
	{
		public static string HostIp = "Host Ip";
		public static string PortNumber = "Port Number";

		private string _hostIp;
		private int _port;

		private Button _connectBtn;
		private EditText _edtHostIp;
		private EditText _edtPort;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.login_view);

			_connectBtn = FindViewById<Button>(Resource.Id.connectBtn);
			_edtHostIp = FindViewById<EditText>(Resource.Id.edtHostIp);
			_edtPort = FindViewById<EditText>(Resource.Id.edtPort);

			_connectBtn.Click += ConnectToServer;
		}

		private void ConnectToServer(object sender, EventArgs e)
		{
			_hostIp = string.IsNullOrEmpty(_edtHostIp.Text) ? "10.0.143.67" : _edtHostIp.Text;
			_port = string.IsNullOrEmpty(_edtPort.Text) ? 8080 : Convert.ToInt32(_edtPort.Text);

			var result = new FileSharingClient(_hostIp, _port).Login("quoc", "123");

			if ("success".Equals(result))
			{
				var intent = new Intent(this, typeof(HomeView));
				intent.PutExtra(HostIp, _hostIp);
				intent.PutExtra(PortNumber, _port);
				StartActivity(intent);
			}
		}

		protected override void OnDestroy()
		{
			_connectBtn.Click -= ConnectToServer;
			base.OnDestroy();
		}
	}
}