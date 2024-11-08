using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FI.AtividadeEntrevista.DAL
{
    internal class AcessoDados
    {
        private string stringDeConexao
        {
            get
            {
                var conn = ConfigurationManager.ConnectionStrings["BancoDeDados"];
                if (conn != null)
                {
                    return conn.ConnectionString;
                }
                else
                {
                    throw new ConfigurationErrorsException("A conexão 'BancoDeDados' não foi encontrada no arquivo de configuração.");
                }
            }
        }

        internal void Executar(string NomeProcedure, List<SqlParameter> parametros)
        {
            using (var conexao = new SqlConnection(stringDeConexao))
            using (var comando = new SqlCommand(NomeProcedure, conexao) { CommandType = CommandType.StoredProcedure })
            {
                comando.Parameters.AddRange(parametros.ToArray());
                conexao.Open();
                comando.ExecuteNonQuery();
            }
        }

        internal DataSet Consultar(string NomeProcedure, List<SqlParameter> parametros)
        {
            using (var conexao = new SqlConnection(stringDeConexao))
            using (var comando = new SqlCommand(NomeProcedure, conexao) { CommandType = CommandType.StoredProcedure })
            {
                comando.Parameters.AddRange(parametros.ToArray());
                var adapter = new SqlDataAdapter(comando);
                var ds = new DataSet();
                conexao.Open();
                adapter.Fill(ds);
                return ds;
            }
        }
    }
}