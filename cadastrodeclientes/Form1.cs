using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;


namespace cadastrodeclientes
{
    public partial class frmCadastrodeClientes : Form
    {
        //Conexão com o banco de dados MySQL
        MySqlConnection conexao;
        string data_source = "datasource=localhost; username=root; password=; database=db_cadastro";

        public frmCadastrodeClientes()
        {
            InitializeComponent();

            //Configuração inicial da ListView para exibicão dos dados dos clientes
            lstCliente.View = View.Details; //Define a visualização como "Detalhes"
            lstCliente.LabelEdit = true; //Permite editar o titulos das colunas
            lstCliente.AllowColumnReorder = true; //Permite reordenar as colunas
            lstCliente.FullRowSelect = true; //Seleciona a linha inteira ao clicar
            lstCliente.GridLines = true; //Exibe as linhas de grade no ListView

            //Definindo as colunas do ListView
            lstCliente.Columns.Add("Codigo", 100, HorizontalAlignment.Left); //Coluna de código
            lstCliente.Columns.Add("Nome Completo", 200, HorizontalAlignment.Left); //Coluna de Nome completo
            lstCliente.Columns.Add("Nome Social", 200, HorizontalAlignment.Left); //Coluna de Nome Social
            lstCliente.Columns.Add("E-mail", 200, HorizontalAlignment.Left); //Coluna de E-mail
            lstCliente.Columns.Add("CPF", 200, HorizontalAlignment.Left); //Coluna de CPF

            //Carrega os dados dos clientes na interface
            carregar_clientes();

        }

        private void carregar_clientes_com_query(string query)
        {
            try
            {
                //cria a conexão com o banco de dados
                conexao = new MySqlConnection(data_source);
                conexao.Open();

                //executa a consulta SQL fornecida
                MySqlCommand cmd = new MySqlCommand(query, conexao);

                //se a consulta contém o parâmetro @q, adiciona o valor da caixa de pesquisa
                if (query.Contains("@q"))
                {
                    cmd.Parameters.AddWithValue("@q", "%" + txtBuscar.Text + "%");
                }

                //executa o comando e obtém os resultados
                MySqlDataReader reader = cmd.ExecuteReader();

                //limpa os itens existentes no ListView antes de adicionar novos
                lstCliente.Items.Clear();

                //Preenche o ListView com os dados dos clientes
                while (reader.Read())
                {
                    //Cria uma linha para cada cliente com os dados retornados da consulta
                    string[] row =

                    {
                        Convert.ToString(reader.GetInt32(0)), //Codigo
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4)

                     };

                    //Adiciona a linha ao ListView
                    lstCliente.Items.Add(new ListViewItem(row));
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro" + ex.Number + "ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

            }

            catch (Exception ex)
            {
                //Trate erros relacionados ao MySql
                MessageBox.Show("Ocorreu " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro
                if (conexao != null && conexao.State == ConnectionState.Open)
                {
                    conexao.Close();
                 
                }

            }
        }

        //Método para carregar todos os clientes no ListView (usando uma consulta sem parêmetros)
        private void carregar_clientes()
        {
            string query = "SELECT * FROM dadosdecliente ORDER BY codigo DESC";
            carregar_clientes_com_query(query);
        }

        //validação de regex
        private bool isValidEmail (string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        //Função para validar se o CPF tem exatamente 11 dígitos numéricos
        private bool isValidCPFLegth (string cpf)
        {
            //Remover quaisquer caracteres não numéricos (como pontos e traços)
            cpf = cpf.Replace(".", "").Replace("-", "");

            //Verificar se o CPF tem exatamente 11 caracteres numéricos
            if (cpf.Length !=11 || !cpf.All(char.IsDigit))

            {
                return false;
            }
            return true;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                //Validação de campos obrigatórios
                if (string.IsNullOrEmpty(txtNomeCompleto.Text.Trim()) ||
                        string.IsNullOrEmpty(txtEmail.Text.Trim()) ||
                        string.IsNullOrEmpty(txtCPF.Text.Trim()))
                {
                    MessageBox.Show("Todos os campos devem ser preenchidos.", "Validação",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return; //Impede o prosseguimento se algum campo estiver vazio
                }

                //Validação do e-mail
                string email = txtEmail.Text.Trim();
                if (!isValidEmail(email))
                {
                    MessageBox.Show("E-mail inválido. Certifique-se de que o e-mail está no formato correto.",
                        "Validação",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return; //Impede o prosseguimento se o e-mail for inválido
                }


                //Validação CPF
                string cpf = txtCPF.Text.Trim();
                if (!isValidCPFLegth(cpf))
                {
                    MessageBox.Show("CPF inválido. Certifique-se de que o CPF tenha 11 digitos numéricos.",
                    "Validação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                    return; //Impede o prossegumento se o CPF for inváldo
                }
            
                //Cria a conexão com o banco de dados
                conexao = new MySqlConnection(data_source);
                conexao.Open();

                //Teste de abertura de banco
                //MessageBox.Show("Conexão aberta com sucesso");

                //Comando SQL para inserir um novo cliente no banco de dados
                MySqlCommand cmd = new MySqlCommand

                {
                    Connection = conexao
                };
                cmd.Prepare();

                cmd.CommandText = "INSERT INTO dadosdecliente(nomecompleto, nomesocial,email,cpf) " +
                    "VALUES (@nomecompleto, @nomesocial, @email, @cpf)";

                //Adiciona os parâmetros com os dados do formulário
                cmd.Parameters.AddWithValue("@nomecompleto", txtNomeCompleto.Text.Trim());
                cmd.Parameters.AddWithValue("@nomesocial", txtNomeSocial.Text.Trim());
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@cpf", cpf);

                //Executa o comando de inserção no banco 
                cmd.ExecuteNonQuery();


                //Mensagem de sucesso 
                MessageBox.Show("Contato inserido com Sucesso:",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                //Limpa os campos após o sucesso
                txtNomeCompleto.Text = string.Empty;
                txtNomeSocial.Text = " ";
                txtEmail.Text = " ";
                txtCPF.Text = " ";

                //Recarregar os clientes na ListView
                carregar_clientes();

                //Muda para a aba de pesquisa
                tabControl1.SelectedIndex = 1;
            }

            catch(MySqlException ex)
            {
                //Trata erros relacionados ao MySQL
                MessageBox.Show("Erro" + ex.Number + "ocorreu:" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

                  catch (Exception ex)
                  {//Trata outros tipos de erro
                    MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);                                   
            }

            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro
                if(conexao!=null && conexao.State == ConnectionState.Open)
                {
                    conexao.Close();
                    //Teste de fechamento de banco
                   // MessageBox.Show("Conexão fechada com sucesso");
                }
            }           
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM dadosdecliente WHERE nomecompleto LIKE @q OR nomesocial LIKE @q ORDER by codigo DESC";
            carregar_clientes_com_query(query);
        }
    }
}
