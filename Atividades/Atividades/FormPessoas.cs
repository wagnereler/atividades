﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Atividades
{

    public partial class FormPessoas : Form
    {
        private static string connectBase = "Data Source=Banco.db";
        private static string bancoName = "Banco.db";
        public FormPessoas()
        {
            InitializeComponent();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void butInserirPessoa_Click(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(connectBase);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }

            if (radioUsuarioPadrao.Checked == false & radioGerente.Checked == false)
            {
                MessageBox.Show("Selecione o tipo de pessoa!");
            }else
            {
                SQLiteCommand cmd = new SQLiteCommand(@"INSERT INTO tbPessoas (NomePessoa, Gerente, colaboradorPadrao)
            VALUES(@nomePessoa, @Gerente, @colaboradorPadrao)", conn);

                //captrua os valores para os parametros do sql
                cmd.Parameters.AddWithValue("NomePessoa", textNomePessoa.Text.Trim());
                //convert verifica se é falso e converte para zero





                if (radioGerente.Checked == true)
                {
                    cmd.Parameters.AddWithValue("Gerente", "1");
                }
                else
                {
                    cmd.Parameters.AddWithValue("Gerente", "0");
                }
                if (radioUsuarioPadrao.Checked == true)
                {
                    cmd.Parameters.AddWithValue("colaboradorPadrao", "1");
                }
                else
                {
                    cmd.Parameters.AddWithValue("colaboradorPadrao", "0");
                }


                try
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    textNomePessoa.Clear();
                    radioGerente.Checked = false;
                    radioUsuarioPadrao.Checked = false;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao Criar Banco de Dados" + ex.Message);
                }
            }
        }

            

    }
}
