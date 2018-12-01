using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FileSharingApp.Models;
using System;
using Android.Views;

namespace FileSharingApp.Views
{
	[Activity(Label = "@string/app_name", Icon = "@mipmap/ic_launcher", Theme = "@style/AppTheme.Fullscreen", MainLauncher = true)]
	public class LoginView : AppCompatActivity
	{
		private string _userName;
		private string _password;
		private string _passwordRetype;
		private string _hostIp;
		private int _port;
		private Button _connectBtn;
		private Button _signUpBtn;
		private EditText _edtHostIp;
		private EditText _edtPort;
		private EditText _edtUserName;
		private EditText _edtPassword;
		private EditText _edtPasswordRetype;
		private TextView _tvSignUp;
		private TextView _tvLogin;
		private LinearLayout _passwordRetypeView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.login_view);

			_connectBtn = FindViewById<Button>(Resource.Id.connectBtn);
			_signUpBtn = FindViewById<Button>(Resource.Id.signUpBtn);
			_edtHostIp = FindViewById<EditText>(Resource.Id.edtHostIp);
			_edtPort = FindViewById<EditText>(Resource.Id.edtPort);
			_edtUserName = FindViewById<EditText>(Resource.Id.edtUserName);
			_edtPassword = FindViewById<EditText>(Resource.Id.edtPassword);
			_edtPasswordRetype = FindViewById<EditText>(Resource.Id.edtPasswordRetype);
			_tvSignUp = FindViewById<TextView>(Resource.Id.tvSignUp);
			_tvLogin = FindViewById<TextView>(Resource.Id.tvLogin);
			_passwordRetypeView = FindViewById<LinearLayout>(Resource.Id.passwordRetypeView);

			_connectBtn.Click += Login;
			_signUpBtn.Click += SignUp;
			_tvSignUp.Click += ShowSignUpForm;
			_tvLogin.Click += ShowLoginForm;
		}

		private void SignUp(object sender, EventArgs e)
		{
			_userName = string.IsNullOrEmpty(_edtUserName.Text) ? "quoc" : _edtUserName.Text;
			_password = string.IsNullOrEmpty(_edtPassword.Text) ? "123" : _edtPassword.Text;
			_hostIp = string.IsNullOrEmpty(_edtHostIp.Text) ? "10.0.143.67" : _edtHostIp.Text;
			_port = string.IsNullOrEmpty(_edtPort.Text) ? 8080 : Convert.ToInt32(_edtPort.Text);

			var result = new FileSharingClient(_hostIp, _port).SignUp(_userName, _password);

			if ("success".Equals(result))
			{
				var intent = new Intent(this, typeof(HomeView));
				intent.PutExtra(AppConstants.UserName, _userName);
				intent.PutExtra(AppConstants.HostIp, _hostIp);
				intent.PutExtra(AppConstants.PortNumber, _port);
				StartActivity(intent);
			}
		}

		private void Login(object sender, EventArgs e)
		{
			_userName = string.IsNullOrEmpty(_edtUserName.Text) ? "quoc" : _edtUserName.Text;
			_password = string.IsNullOrEmpty(_edtPassword.Text) ? "123" : _edtPassword.Text;
			_hostIp = string.IsNullOrEmpty(_edtHostIp.Text) ? "10.0.143.67" : _edtHostIp.Text;
			_port = string.IsNullOrEmpty(_edtPort.Text) ? 8080 : Convert.ToInt32(_edtPort.Text);

			var result = new FileSharingClient(_hostIp, _port).Login(_userName, _password);

			if ("success".Equals(result))
			{
				var intent = new Intent(this, typeof(HomeView));
				intent.PutExtra(AppConstants.UserName, _userName);
				intent.PutExtra(AppConstants.HostIp, _hostIp);
				intent.PutExtra(AppConstants.PortNumber, _port);
				StartActivity(intent);
			}
		}

		private void ShowLoginForm(object sender, EventArgs e)
		{
			SwitchForm(ViewStates.Gone, ViewStates.Visible);
		}

		private void ShowSignUpForm(object sender, EventArgs e)
		{
			SwitchForm(ViewStates.Visible, ViewStates.Gone);
		}

		private void SwitchForm(ViewStates signUpFormViewState, ViewStates loginFormViewState)
		{
			_signUpBtn.Visibility = signUpFormViewState;
			_passwordRetypeView.Visibility = signUpFormViewState;
			_tvLogin.Visibility = signUpFormViewState;
			_connectBtn.Visibility = loginFormViewState;
			_tvSignUp.Visibility = loginFormViewState;
		}

		protected override void OnDestroy()
		{
			_connectBtn.Click -= Login;
			base.OnDestroy();
		}
	}
}