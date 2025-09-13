using MauiAppMinhasCompras.Models;
using System.Threading.Tasks;
using System;
using Microsoft.Maui.Controls;

namespace MauiAppMinhasCompras.Views;

public partial class NovoProduto : ContentPage
{
	public NovoProduto()
	{
		this.InitializeComponent();
	}

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Produto p = new Produto
			{
				Descricao = txt_descricao.Text,
				Quantidade = Convert.ToDouble(txt_quantidade.Text),
				Preco = Convert.ToDouble(txt_preco.Text),
				// Corrigido: Usar SelectedItem do Picker e tratar possível nulo
				Categoria = picker_categoria.SelectedItem != null ? picker_categoria.SelectedItem.ToString() : string.Empty
			};

			await App.Db.Insert(p);
			await DisplayAlert("Sucesso", "Produto inserido com sucesso!", "OK");
			await Navigation.PopAsync();

        } catch(Exception ex)
		{
			await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}