using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class EditarProduto : ContentPage
{
    private Produto produto; // Adiciona o campo produto

    public EditarProduto(Produto produto)
    {
        InitializeComponent();
        this.produto = produto; // Inicializa o campo produto
        // Opcional: preencher os campos da tela com os dados do produto
        txt_descricao.Text = produto.Descricao;
        txt_quantidade.Text = produto.Quantidade.ToString();
        txt_preco.Text = produto.Preco.ToString();
        picker_categoria.SelectedItem = produto.Categoria;
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Exibe o ID do produto antes de atualizar (para depuração)
            //await DisplayAlert("Depuração", $"ID do produto: {produto.ID}", "OK");

            produto.Descricao = txt_descricao.Text;
            produto.Quantidade = Convert.ToDouble(txt_quantidade.Text);
            produto.Preco = Convert.ToDouble(txt_preco.Text);
            produto.Categoria = picker_categoria.SelectedItem?.ToString() ?? string.Empty;

            await App.Db.Update(produto);

            await DisplayAlert("Sucesso", "Produto atualizado com sucesso!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}
