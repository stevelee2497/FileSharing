using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;

namespace FileSharingApp.View
{
	public class BottomSheetFragment : BottomSheetDialogFragment
	{
		public BottomSheetFragment()
		{
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			return inflater.Inflate(Resource.Layout.bottom_sheet_fragment, container, false);
		}
	}
}