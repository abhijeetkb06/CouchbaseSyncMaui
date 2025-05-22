using Android.App;
using Android.Runtime;
using Couchbase.Lite.Support;
using System;


namespace AppSync;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership) { }

    public override void OnCreate()
    {
        base.OnCreate();
	
        // set the Android context
        Couchbase.Lite.Support.Droid.Activate(this);
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
