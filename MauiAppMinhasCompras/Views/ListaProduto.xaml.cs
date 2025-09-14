using MauiAppMinhasCompras.Models;	
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
		CarregarCategorias();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		CarregarProdutos(); // Certifique-se de que este método recarrega a lista do banco
	}

	private async void CarregarProdutos()
	{
		try
		{
			lista.Clear();

			List<Produto> tmp = await App.Db.GetAll();

			tmp.ForEach(i => lista.Add(i));
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ops", ex.Message, "OK");
		}
	}

	private async void CarregarCategorias()
	{
		var produtos = await App.Db.GetAll();
		var categorias = produtos
			.Select(p => string.IsNullOrWhiteSpace(p.Categoria) ? "Sem Categoria" : p.Categoria)
			.Distinct()
			.OrderBy(c => c)
			.ToList();
		categorias.Insert(0, "Todas");
		categoriaPicker.ItemsSource = categorias;
	}

	private void ToolbarItem_Clicked(object sender, EventArgs e)
	{
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());

		}
		catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}

	}

	private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			string q = e.NewTextValue;

            lst_produtos.IsRefreshing = true;

            lista.Clear();

			List<Produto> tmp = await App.Db.Search(q);

			tmp.ForEach(i => lista.Add(i));
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ops", ex.Message, "OK");
		}
		finally
		{
			lst_produtos.IsRefreshing = false;
        }

	}

	private void ToolbarItem_Clicked_1(object sender, EventArgs e)
	{
		double soma = lista.Sum(i => i.Total);

		string msg = $"O total é {soma:C}";

		DisplayAlert("Total dos Produtos", msg, "OK");
	}

	private async void MenuItem_Clicked(object sender, EventArgs e)
	{
		try
		{
			MenuItem selecionado = sender as MenuItem;

			Produto p = selecionado.BindingContext as Produto;

			bool confirm = await DisplayAlert(
				"Tem certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

			if (confirm)
			{
				await App.Db.Delete(p.ID);
				lista.Remove(p);
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ops", ex.Message, "OK");
		}
	}

	private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		try
		{
			Produto p = e.SelectedItem as Produto;

			// Verifica se o produto está sem categoria
			if (string.IsNullOrWhiteSpace(p.Categoria))
			{
				DisplayAlert("Atenção", "Este produto está sem categoria. Edite para adicionar uma categoria.", "OK");
			}

			Navigation.PushAsync(new Views.EditarProduto(p)
			{
				BindingContext = p,
			});
		}
		catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}
	}

	private async void lst_produtos_Refreshing(object sender, EventArgs e)
	{
		try
		{
			lista.Clear();

			List<Produto> tmp = await App.Db.GetAll();

			tmp.ForEach(i => lista.Add(i));
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ops", ex.Message, "OK");
		}
		finally
		{
			lst_produtos.IsRefreshing = false;
		}
	}

	public void AtualizarLista()
	{
		// Supondo que você já tenha um método para atualizar a lista, como OnAppearing
		// Você pode chamar OnAppearing diretamente ou recarregar a lista conforme sua lógica
		OnAppearing();
	}

	// Adicione este método no code-behind para exibir o relatório ao clicar no botão
	private async void ToolbarItemRelatorio_Clicked(object sender, EventArgs e)
	{
		try
		{
			var totais = await App.Db.GetTotalPorCategoria();
			if (totais.Count == 0)
			{
				await DisplayAlert("Relatório", "Nenhum dado encontrado.", "OK");
				return;
			}

			string relatorio = string.Join(
				Environment.NewLine,
				totais.Select(t => $"{(string.IsNullOrWhiteSpace(t.Categoria) ? "Sem Categoria" : t.Categoria)}: {t.Total:C}")
			);

			await DisplayAlert("Total por Categoria", relatorio, "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ops", ex.Message, "OK");
		}
	}

	// Adicione este método na classe ListaProduto
	private async void CategoriaPicker_SelectedIndexChanged(object sender, EventArgs e)
	{
		var picker = sender as Picker;
		string categoriaSelecionada = picker.SelectedItem as string;

		lista.Clear();

		if (string.IsNullOrWhiteSpace(categoriaSelecionada) || categoriaSelecionada == "Todas")
		{
			List<Produto> tmp = await App.Db.GetAll();
			tmp.ForEach(i => lista.Add(i));
		}
		else
		{
			List<Produto> tmp = await App.Db.GetByCategoria(categoriaSelecionada);
			tmp.ForEach(i => lista.Add(i));
		}
	}
}