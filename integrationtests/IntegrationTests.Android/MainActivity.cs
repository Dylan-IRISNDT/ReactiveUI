﻿using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using IntegrationTests.Shared;
using ReactiveUI;

namespace IntegrationTests.Android
{
    /// <summary>
    /// The main activity for the application.
    /// </summary>
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : ReactiveActivity<LoginViewModel>
    {
        /// <summary>
        /// Gets or sets the user name edit text.
        /// </summary>
        public EditText Username { get; set; }

        /// <summary>
        /// Gets or sets the password edit text.
        /// </summary>
        public EditText Password { get; set; }

        /// <summary>
        /// Gets or sets the login button.
        /// </summary>
        public Button Login { get; set; }

        /// <summary>
        /// Gets or sets the cancel butotn.
        /// </summary>
        public Button Cancel { get; set; }

        /// <inheritdoc />
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        /// <inheritdoc />
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        /// <inheritdoc />
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);

            Username = FindViewById<EditText>(Resource.Id.Username);
            Password = FindViewById<EditText>(Resource.Id.Password);
            Login = FindViewById<Button>(Resource.Id.Login);
            Cancel = FindViewById<Button>(Resource.Id.Cancel);

            ViewModel = new LoginViewModel(RxApp.MainThreadScheduler);

            this
               .WhenActivated(
                   disposables =>
                   {
                       this
                           .Bind(ViewModel, vm => vm.UserName, v => v.Username.Text)
                           .DisposeWith(disposables);
                       this
                           .Bind(ViewModel, vm => vm.Password, v => v.Password.Text)
                           .DisposeWith(disposables);
                       this
                           .BindCommand(ViewModel, vm => vm.Login, v => v.Login)
                           .DisposeWith(disposables);
                       this
                           .BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                           .DisposeWith(disposables);

                       ViewModel
                           .Login
                           .SelectMany(
                               result =>
                               {
                                   if (!result.HasValue)
                                   {
                                       return Observable.Empty<Unit>();
                                   }

                                   if (result.Value)
                                   {
                                       new AlertDialog.Builder(this)
                                           .SetTitle("Login Successful")
                                           .SetMessage("Welcome!")
                                           .Show();
                                   }
                                   else
                                   {
                                       new AlertDialog.Builder(this)
                                           .SetTitle("Login Failed")
                                           .SetMessage("Ah, ah, ah, you didn't say the magic word!")
                                           .Show();
                                   }

                                   return Observable.Return(Unit.Default);
                               })
                           .Subscribe()
                           .DisposeWith(disposables);
                   });
        }
    }
}
