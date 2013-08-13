using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ExemploSingleton
{
    /// <summary>
    /// Desenvolvedor.....: Julio Cesar Goldschmidt <julio.gold@gmail.com>
    /// Data..............: 31/07/2013
    /// Exemplo da utilização de classes que implementam o design patterns singleton para conexão com banco de dados.
    /// Referências:
    /// http://msdn.microsoft.com/en-us/library/ff650316.aspx
    /// http://msdn.microsoft.com/en-us/library/x13ttww7(v=vs.71).aspx
    /// http://msdn.microsoft.com/pt-br/library/system.collections.arraylist.syncroot.aspx
    /// http://msdn.microsoft.com/en-us/library/system.collections.icollection.syncroot.aspx
    /// </summary>
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                #region Trecho de conexões normais sem a utilização do padrão Singleton

                string strConexao = @"Data Source=SERVIDOR;Initial Catalog=BaseInicial;Persist Security Info=True;User ID=USUARIO;Password=SENHA;Connect Timeout=1000;Application Name=Singleton Normal";

                SqlConnection conNormal = new SqlConnection(strConexao);
                conNormal.Open();
                SqlCommand cmdNormal = new SqlCommand("SELECT GETDATE()", conNormal);
                SqlDataReader drNormal = cmdNormal.ExecuteReader();

                SqlConnection conNormal1 = new SqlConnection(strConexao);
                conNormal1.Open();
                SqlCommand cmdNormal1 = new SqlCommand("SELECT GETDATE()", conNormal1);
                SqlDataReader drNormal1 = cmdNormal1.ExecuteReader();

                SqlConnection conNormal2 = new SqlConnection(strConexao);
                conNormal2.Open();
                SqlCommand cmdNormal2 = new SqlCommand("SELECT GETDATE()", conNormal2);
                SqlDataReader drNormal2 = cmdNormal2.ExecuteReader();

                #endregion

                #region Trecho de conexões com a utilização do padrão Singleton Not Thread Safe

                SqlConnection conNTS = NotThreadSafe.Singleton.Instance.Conexao;
                SqlCommand cmdN = new SqlCommand("SELECT GETDATE()", conNTS);
                SqlDataReader drN = cmdN.ExecuteReader();

                SqlConnection conNTS1 = ThreadSafe.Singleton.Instance.Conexao;
                SqlCommand cmdN1 = new SqlCommand("SELECT GETDATE()", conNTS1);
                SqlDataReader drN1 = cmdN1.ExecuteReader();

                SqlConnection conNTS2 = ThreadSafe.Singleton.Instance.Conexao;
                SqlCommand cmdN2 = new SqlCommand("SELECT GETDATE()", conNTS2);
                SqlDataReader drN2 = cmdN2.ExecuteReader();

                #endregion

                #region Trecho de conexões com a utilização do padrão Singleton Thread Safe

                SqlConnection conTS = ThreadSafe.Singleton.Instance.Conexao;
                SqlCommand cmd = new SqlCommand("SELECT GETDATE()", conTS);
                SqlDataReader dr = cmd.ExecuteReader();

                SqlConnection conTS1 = ThreadSafe.Singleton.Instance.Conexao;
                SqlCommand cmd1 = new SqlCommand("SELECT GETDATE()", conTS1);
                SqlDataReader dr1 = cmd1.ExecuteReader();

                SqlConnection conTS2 = ThreadSafe.Singleton.Instance.Conexao;
                SqlCommand cmd2 = new SqlCommand("SELECT GETDATE()", conTS2);
                SqlDataReader dr2 = cmd2.ExecuteReader();

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string blabla = "";
        }
    }
}

namespace ThreadSafe
{
    /// <summary>
    /// Desenvolvedor.....: Julio Cesar Goldschmidt <julio.gold@gmail.com>
    /// Data..............: 31/07/2013
    /// Classe que implementa o design patern singleton e é responsável por estabelecer uma 
    /// única conexão com o banco de dados para a aplicação e enquanto a mesma estiver em execução.
    /// Esta classe é singleton thread safe
    /// </summary>
    public sealed class Singleton
    {
        /// <summary>
        /// Guarda a instância da classe.
        /// </summary>
        private static volatile Singleton instance;

        //Objeto para fazer lock da parte do código
        private static object syncRoot = new Object();

        /// <summary>
        /// Guarda a conexão com o banco.
        /// </summary>
        private SqlConnection conexao;

        /// <summary>
        /// Esta variável está aqui apenas por questões de teste, o correto é que a mesma fique armazenada em algum local
        /// onde seja possível alterar a mesma sem precisar alerar a aplicação.
        /// </summary>
        private string strConexao = @"Data Source=SERVIDOR;Initial Catalog=BaseInicial;Persist Security Info=True;User ID=USUARIO;Password=SENHA;Connect Timeout=1000;MultipleActiveResultSets=True;Application Name=Singleton Thread Safe";

        /// <summary>
        /// O consrutor da classe é privado, não posso deixar que seja feito um new desta classe, a instância deve ser pega
        /// pela propriedade Instance.
        /// </summary>
        private Singleton()
        {
            //Cria o objeto de conexão com a string de conexão mas ainda não abre a conexão.
            this.conexao = new SqlConnection(strConexao);
        }

        /// <summary>
        /// Retorna a instância da classe, caso ainda não tenha sido instanciada, faz na primeira vez que for chamada.
        /// </summary>
        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Singleton();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Conexão com o banco de dados.
        /// </summary>
        /// <returns>Conexão já aberta com o banco de dados, pronta para ser usada.</returns>
        public SqlConnection Conexao
        {
            get
            {
                try
                {
                    //Se o estado da conexão estiver diferente de aberto vou abrí-la.
                    if (this.conexao.State != System.Data.ConnectionState.Open)
                    {
                        //Atualiza a string de conexão
                        this.conexao.ConnectionString = strConexao;
                        //Abre a conexão
                        this.conexao.Open();
                    }
                    return this.conexao;
                }
                catch (Exception ex)
                {
                    //Se a conexão estiver aberta como ocorreu algum erro, fecho a mesma.
                    if (this.conexao.State == ConnectionState.Open)
                    {
                        this.conexao.Close();
                    }
                    throw ex;
                }
            }
        }
    }
}

namespace NotThreadSafe
{
    /// <summary>
    /// Desenvolvedor.....: Julio Cesar Goldschmidt <julio.gold@gmail.com>
    /// Data..............: 31/07/2013
    /// Classe que implementa o design patern singleton e é responsável por estabelecer uma 
    /// única conexão com o banco de dados para a aplicação e enquanto a mesma estiver em execução.
    /// Esta classe é not thread safe
    /// </summary>
    public sealed class Singleton
    {
        /// <summary>
        /// Guarda a instância da classe.
        /// </summary>
        private static Singleton instance;

        /// <summary>
        /// Guarda a conexão com o banco.
        /// </summary>
        private SqlConnection conexao;

        /// <summary>
        /// Esta variável está aqui apenas por questões de teste, o correto é que a mesma fique armazenada em algum local
        /// onde seja possível alterar a mesma sem precisar alerar a aplicação.
        /// </summary>
        private string strConexao = @"Data Source=SERVIDOR;Initial Catalog=BaseInicial;Persist Security Info=True;User ID=USUARIO;Password=SENHA;Connect Timeout=1000;MultipleActiveResultSets=True;Application Name=Singleton Not Thread Safe";

        /// <summary>
        /// O consrutor da classe é privado, não posso deixar que seja feito um new desta classe, a instância deve ser pega
        /// pela propriedade Instance.
        /// </summary>
        private Singleton()
        {
            //Cria o objeto de conexão com a string de conexão mas ainda não abre a conexão.
            this.conexao = new SqlConnection(strConexao);
        }

        /// <summary>
        /// Retorna a instância da classe, caso ainda não tenha sido instanciada, faz na primeira vez que for chamada.
        /// </summary>
        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }

        /// <summary>
        /// Conexão com o banco de dados.
        /// </summary>
        /// <returns>Conexão já aberta com o banco de dados, pronta para ser usada.</returns>
        public SqlConnection Conexao
        {
            get
            {
                try
                {
                    //Se o estado da conexão estiver diferente de aberto vou abrí-la.
                    if (this.conexao.State != System.Data.ConnectionState.Open)
                    {
                        //Atualiza a string de conexão
                        this.conexao.ConnectionString = strConexao;
                        //Abre a conexão
                        this.conexao.Open();
                    }
                    return this.conexao;
                }
                catch (Exception ex)
                {
                    //Se a conexão estiver aberta como ocorreu algum erro, fecho a mesma.
                    if (this.conexao.State == ConnectionState.Open)
                    {
                        this.conexao.Close();
                    }
                    throw ex;
                }
            }
        }
    }
}
