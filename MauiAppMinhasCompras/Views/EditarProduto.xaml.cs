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
            produto.Descricao = txt_descricao.Text;
            produto.Quantidade = Convert.ToDouble(txt_quantidade.Text);
            produto.Preco = Convert.ToDouble(txt_preco.Text);

            // Corrija aqui: pegue o valor selecionado corretamente
            if (picker_categoria.SelectedItem != null && picker_categoria.SelectedItem.ToString() != "Sem Categoria")
                produto.Categoria = picker_categoria.SelectedItem.ToString();
            else
                produto.Categoria = string.Empty;

            await App.Db.Update(produto);

            await DisplayAlert("Sucesso", "Produto atualizado com sucesso!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    // Após salvar a edição do produto, atualize a lista ao voltar para a tela anterior
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        // Se a tela anterior for ListaProduto, atualize a lista
        if (Navigation.NavigationStack.LastOrDefault() is MauiAppMinhasCompras.Views.ListaProduto listaPage)
        {
            listaPage.AtualizarLista();
        }
    }
}
