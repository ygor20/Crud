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


namespace cadastrodeclientes
{
    public partial class frmCadastrodeClientes : Form
    {
        public frmCadastrodeClientes()
        {
            InitializeComponent();
        }

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
                    MessageBoxIcon.Warning
               );
                    return; //Impede o prossegumento se o CPF for inváldo
                }

                  }
                  catch (Exception ex)
                  {
                       MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
              
                    
                     }
                }
    }
}
