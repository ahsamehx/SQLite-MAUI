using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuaiSQL.Data;
using MuaiSQL.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace MuaiSQL.ViewModels
{
    public partial class ProductsViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;
        public ProductsViewModel(DatabaseContext context)
        {
            _context = context;
        }
        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private Product _operatingProduct = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _isBusyText;

        
        public async Task LoadProductsAsync()
        {
            await ExecuteAysnc(async () =>
            {
                var products = await _context.GetAllAsync<Product>();
                if (products is not null && products.Any())
                {
                    Products ??= new ObservableCollection<Product>();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                }

            }, "Loading products...");
        }
        [RelayCommand]
        private void SetOperatingProduct(Product? product) => OperatingProduct = product ?? new();

        [RelayCommand]
        private async Task SaveProductAsync()
        {
            if (OperatingProduct is null)
                return;
            var (isValid, errorMessage) = OperatingProduct.validate();
            if (!isValid)
            {
                await Shell.Current.DisplayAlert("Validation Error", errorMessage, "Ok");
                return;
            }
            var busyText = OperatingProduct.Id == 0 ? "Creating Product..." : "Updating product...";

            await ExecuteAysnc(async () =>
            {
                if (OperatingProduct.Id == 0)
                {
                    //Create product 
                    await _context.AddItemAsync<Product>(OperatingProduct);
                    Products.Add(_operatingProduct);
                }
                else
                {
                    //update product 
                    await _context.UpdateItemAsync<Product>(OperatingProduct);
                    var productCopy = OperatingProduct.Clone();
                    var index = Products.IndexOf(OperatingProduct);
                    Products.RemoveAt(index);
                    Products.Insert(index, productCopy);
                }
                OperatingProduct = new();
                SetOperatingProductCommand.Execute(new());

            }, IsBusyText);
        }

        [RelayCommand]
        private async Task DeleteProductAsync(int id)
        {

            await Shell.Current.DisplayAlert("Debug", $"Deleting {id}", "OK");
           
            await ExecuteAysnc(async()=>
            {
                if (await _context.DeleteItemByKeyAsync<Product>(id))
                {
                    var product = Products.FirstOrDefault(p => p.Id == id);
                    Products.Remove(product);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Product was not deleted", "Ok");
                }
            }, "Deleting product..." );
        }

        private async Task ExecuteAysnc (Func<Task> operaion, string? BusyText = null)
        {
            IsBusy = true;
            IsBusyText = BusyText;
            try
            {
                await operaion?.Invoke();
            }
            finally
            {
                IsBusy = false;
                IsBusyText = "Processing...";
            }

        }

    }
}
