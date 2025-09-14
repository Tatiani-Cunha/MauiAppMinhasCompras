using MauiAppMinhasCompras.Models;
using SQLite;


namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn; // irá armazenar a conexão com o banco de dados
        public SQLiteDatabaseHelper(string path) // define o caminho do banco de dados
        {
            _conn = new SQLiteAsyncConnection(path); // conexão com arquivo de texto que está no caminho path
            _conn.CreateTableAsync<Produto>().Wait(); //cria a tabela Produto no banco de dados
        }

        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p); // insere o produto p na tabela Produto
        }

        public Task<int> Update(Produto p)
        {
            // Use o método UpdateAsync para atualizar corretamente pelo ID
            return _conn.UpdateAsync(p);
        }

        public Task<int> Delete(int id)
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.ID == id);
        }

        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }

        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * FROM Produto WHERE descricao LIKE '%" + q + "%'";

            return _conn.QueryAsync<Produto>(sql);
        }

        // Adicione este método para obter o total gasto por categoria
        public async Task<List<(string Categoria, double Total)>> GetTotalPorCategoria()
        {
            string sql = "SELECT Categoria, SUM(Quantidade * Preco) as Total FROM Produto GROUP BY Categoria";
            var result = await _conn.QueryAsync<ProdutoTotalCategoria>(sql);
            return result.Select(r => (r.Categoria, r.Total)).ToList();
        }

        // Adicione este método na classe SQLiteDatabaseHelper
        public Task<List<Produto>> GetByCategoria(string categoria)
        {
            // Trate "Sem Categoria" como valor nulo ou vazio no banco
            if (categoria == "Sem Categoria")
                return _conn.Table<Produto>().Where(p => p.Categoria == null || p.Categoria == "").ToListAsync();
            else
                return _conn.Table<Produto>().Where(p => p.Categoria == categoria).ToListAsync();
        }
    }
}
