﻿using MuaiSQL.ViewModels;

namespace MuaiSQL
{
    public partial class MainPage : ContentPage
    {
        private readonly ProductsViewModel _viewModel;
        public MainPage(ProductsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();
           await _viewModel.LoadProductsAsync();
        }


    }

}
