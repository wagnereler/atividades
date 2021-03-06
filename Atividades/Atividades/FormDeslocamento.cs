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


    public partial class FormDeslocamento : Form
    {
        private static string connectBase = "Data Source=Banco.db";
        private static string bancoName = "Banco.db";
        public string codCidadeOrigem;
        public string codCidadeDestino;
        public string codProjetoVinculado;
        public int totalMinutosDeslocamento;
        public int totalMinutosExtras;
        public string codColaborador;



        public FormDeslocamento()
        {
            InitializeComponent();
        }

        private void FormDeslocamento_Load(object sender, EventArgs e)
        {
            totalMinutosDeslocamento = 0;
            totalMinutosExtras = 0;
            carregarComboProjetos();
            carregarComboColaborador();
            carregarUf();
        }

        public int calculaMinutosExtras()
        {
            string diaSemana = Convert.ToString(dateTimePicker1.Value.DayOfWeek);

            if (diaSemana == "Saturday")
            {
                totalMinutosExtras = totalMinutosDeslocamento;
            }
            else if (diaSemana == "Sunday")
            {
                totalMinutosExtras = (totalMinutosDeslocamento * 2);
            }
            else
            {
                //Converte em mintos o valor do primeiro campo
                int valor1a = (Convert.ToInt32(textHoraInicio.Text.Substring(0, 2)) * 60);
                int valor1b = (Convert.ToInt32(textHoraInicio.Text.Substring(3, 2)));
                int minInicio = (valor1a + valor1b);

                //Converte em mintos o valor do primeiro campo
                int valor2a = (Convert.ToInt32(textHoraTermino.Text.Substring(0, 2)) * 60);
                int valor2b = (Convert.ToInt32(textHoraTermino.Text.Substring(3, 2)));
                int minFinal = (valor2a + valor2b);

                if (minInicio < 480)
                {
                    totalMinutosExtras += (minInicio - 480);
                }
                if (minFinal > 1020)
                {
                    totalMinutosExtras += (1020 - minFinal);
                }
            }

            return totalMinutosExtras;

        }

        public void atualizaDados()
        {
            SQLiteConnection conn = new SQLiteConnection(connectBase);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }

            //Converte em mintos o valor do primeiro campo
            int valor1a = (Convert.ToInt32(textHoraInicio.Text.Substring(0, 2)) * 60);
            int valor1b = (Convert.ToInt32(textHoraInicio.Text.Substring(3, 2)));
            //Converte em mintos o valor do primeiro campo
            int valor2a = (Convert.ToInt32(textHoraTermino.Text.Substring(0, 2)) * 60);
            int valor2b = (Convert.ToInt32(textHoraTermino.Text.Substring(3, 2)));

            //calcula a quantidade em mintuso e grava em uma variável
            //SE o valor final for no nesmo dia calcula-se (valor final) - (valor inicial)
            if ((valor1a + valor1b) < (valor2a + valor2b))
            {
                totalMinutosDeslocamento = (valor2a + valor2b) - (valor1a + valor1b);

            }
            if ((valor1a + valor1b) == (valor2a + valor2b))
            {
                totalMinutosDeslocamento = 1440;
                MessageBox.Show("Este deslocamento será registrado com 24h. \n Deseja prosseguir com a operação?",
                    "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            //Mas se o valor final for no dia segui calcula-se  (1440 - (valor inicial)) + valor final
            else
            {
                totalMinutosDeslocamento = (1440 - (valor1a + valor1b)) + (valor2a + valor2b);

            }

            SQLiteCommand cmd = new SQLiteCommand(
                @" UPDATE tbDeslocamento SET(
                                ,codColaborador = SELECT codPessoa FROM tbPessoa WHERE nomePessoa = @nomePessoa
                                ,codUfOrigem = @codUfOrigem
                                ,codCidadeOrigem = SELECT codCidade FROM tbCidades WHERE nomeCidade = @nomeCidadeOrigem
                                ,obsOrigem = @obsOrigem
                                ,codUfDestino = @codUfDestino
                                ,codCidadeDestino = SELECT codCidade FROM tbCidades WHERE nomeCidade = @nomeCidadeDestino
                                ,obsDestino = @obsDestino
                                ,codProjeto = SELECT codProjeto FROM tbProjeto WHERE nomeProjeto = @nomeProjeto
                                ,dataDeslocamento = @dataDeslocamento", conn);

            cmd.Parameters.AddWithValue("nomePessoa", comboColaborador.Text.Trim());
            cmd.Parameters.AddWithValue("codUfOrigem", comboUfOrigem.Text.Trim());
            cmd.Parameters.AddWithValue("nomeCidadeOrigem", comboCidadeOrigem.Text.Trim());
            cmd.Parameters.AddWithValue("obsOrigem", textObsInicio.Text.Trim());
            cmd.Parameters.AddWithValue("codUfDestino", comboUfDestino.Text.Trim());
            cmd.Parameters.AddWithValue("nomeCidadeDestino", comboCidadeDestino.Text.Trim());
            cmd.Parameters.AddWithValue("obsDestino", textObsTermino.Text.Trim());
            cmd.Parameters.AddWithValue("nomeProjeto", comboProjetoVinculado.Text.Trim());
            cmd.Parameters.AddWithValue("dataDeslocamento", dateTimePicker1.Text.TrimStart());
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Registro Salvo com Sucesso!", "Operação Realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                comboProjetoVinculado.Text = String.Empty;
                comboCidadeDestino.Text = String.Empty;
                comboCidadeOrigem.Text = String.Empty;
                comboColaborador.Text = String.Empty;
                comboUfDestino.Text = String.Empty;
                comboUfOrigem.Text = String.Empty;
                textHoraInicio.Text = String.Empty;
                textHoraTermino.Text = String.Empty;
                textObsInicio.Text = String.Empty;
                textObsTermino.Text = String.Empty;
                conn.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar arquivo: " + ex.Message,
                    "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void registraDados()
        {
            SQLiteConnection conn = new SQLiteConnection(connectBase);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }

            //Converte em mintos o valor do primeiro campo
            int valor1a = (Convert.ToInt32(textHoraInicio.Text.Substring(0, 2)) * 60);
            int valor1b = (Convert.ToInt32(textHoraInicio.Text.Substring(3, 2)));
            //Converte em mintos o valor do primeiro campo
            int valor2a = (Convert.ToInt32(textHoraTermino.Text.Substring(0, 2)) * 60);
            int valor2b = (Convert.ToInt32(textHoraTermino.Text.Substring(3, 2)));

            //calcula a quantidade em mintuso e grava em uma variável
            //SE o valor final for no nesmo dia calcula-se (valor final) - (valor inicial)
            if ((valor1a + valor1b) < (valor2a + valor2b))
            {
                totalMinutosDeslocamento = (valor2a + valor2b) - (valor1a + valor1b);
            }
            if ((valor1a + valor1b) == (valor2a + valor2b))
            {
                totalMinutosDeslocamento = 1440;
                MessageBox.Show("Este deslocamento será registrado com 24h. \n Deseja prosseguir com a operação?",
                    "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            //Mas se o valor final for no dia segui calcula-se  (1440 - (valor inicial)) + valor final
            else
            {
                totalMinutosDeslocamento = (1440 - (valor1a + valor1b)) + (valor2a + valor2b);
            }


            SQLiteCommand cmd = new SQLiteCommand(
                @"
INSERT INTO tbDeslocamento(
    codColaborador
    ,codUfOrigem
    ,codCidadeOrigem
    ,obsOrigem
    ,codUfDestino
    ,codCidadeDestino
    ,obsDestino
    ,codProjeto
    ,dataDeslocamento
    ,horaInicioDeslocamento
    ,horaFimDeslocamento
    ,totalMinutosDeslocamento
    ,minutosExtrasDeslocamento)
VALUES(
    (SELECT codPessoa FROM tbPessoas WHERE nomePessoa = @nomePessoa)
    ,@codUfOrigem
    ,(SELECT codCidade FROM tbCidades WHERE nomeCidade = @nomeCidadeOrigem)
    ,@obsOrigem
    ,@codUfDestino
    ,(SELECT codCidade FROM tbCidades WHERE nomeCidade = @nomeCidadeDestino)
    ,@obsDestino
    ,(SELECT codProjeto FROM tbProjetos WHERE nomeProjeto = @nomeProjeto)
    ,@dataDeslocamento
    ,@horaInicioDeslocamento
    ,@horaFimDeslocamento
    ,@totalMinutosDeslocamento
    ,@minutosExtrasDeslocamento);", conn);


            cmd.Parameters.AddWithValue("nomePessoa", comboColaborador.Text.Trim());
            cmd.Parameters.AddWithValue("codUfOrigem", comboUfOrigem.Text.Trim());
            cmd.Parameters.AddWithValue("nomeCidadeOrigem", comboCidadeOrigem.Text.Trim());
            cmd.Parameters.AddWithValue("obsOrigem", textObsInicio.Text.Trim());
            cmd.Parameters.AddWithValue("codUfDestino", comboUfDestino.Text.Trim());
            cmd.Parameters.AddWithValue("nomeCidadeDestino", comboCidadeDestino.Text.Trim());
            cmd.Parameters.AddWithValue("obsDestino", textObsTermino.Text.Trim());
            cmd.Parameters.AddWithValue("nomeProjeto", comboProjetoVinculado.Text.Trim());
            cmd.Parameters.AddWithValue("dataDeslocamento", dateTimePicker1.Text.TrimStart());
            cmd.Parameters.AddWithValue("horaInicioDeslocamento", textHoraInicio.Text.Trim());
            cmd.Parameters.AddWithValue("horaFimDeslocamento", textHoraTermino.Text.Trim());
            cmd.Parameters.AddWithValue("totalMinutosDeslocamento", totalMinutosDeslocamento);
            cmd.Parameters.AddWithValue("minutosExtrasDeslocamento", calculaMinutosExtras());
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Registro Salvo com Sucesso!", "Operação Realizada", MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
                comboProjetoVinculado.Text = String.Empty;
                comboCidadeDestino.Text = String.Empty;
                comboCidadeOrigem.Text = String.Empty;
                comboColaborador.Text = String.Empty;
                comboUfDestino.Text = String.Empty;
                comboUfOrigem.Text = String.Empty;
                textHoraInicio.Text = String.Empty;
                textHoraTermino.Text = String.Empty;
                textObsInicio.Text = String.Empty;
                textObsTermino.Text = String.Empty;
                totalMinutosExtras = 0;
                totalMinutosDeslocamento = 0;
                conn.Close();
                

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar arquivo: " + ex.Message,
                    "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }



    


        public void carregarComboColaborador()
        {
            SQLiteConnection conn = new SQLiteConnection(connectBase);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT nomePessoa FROM tbPessoas WHERE colaborador = 1", conn);
            //SQLiteDataReader drComboProjeto = cmd.ExecuteReader();
            SQLiteDataAdapter daCombo = new SQLiteDataAdapter(cmd);
            DataTable dtCombo = new DataTable();
            daCombo.Fill(dtCombo);
            foreach (DataRow drCombo in dtCombo.Rows)

            {
                comboColaborador.Items.Add(drCombo["nomePessoa"]);

            }
            conn.Close();

        }
        public void carregarCodigoColaborador()
        {



            try
            {
                {
                    SQLiteConnection conn = new SQLiteConnection(connectBase);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT codPessoa FROM tbPessoas WHERE nomePessoa = '" + comboColaborador.Text.Trim() + "'", conn);
                    SQLiteDataReader ler = cmd.ExecuteReader();
                    ler.Read();
                    codColaborador = ler["codPessoa"].ToString();
                    conn.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao acessar dados " + ex.Message, "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void carregarUf()
        {
            try
            {
                {
                    SQLiteConnection conn = new SQLiteConnection(connectBase);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT CodUf FROM tbUf", conn);
                    SQLiteDataAdapter daComboUF = new SQLiteDataAdapter(cmd);
                    DataTable dtComboUF = new DataTable();
                    daComboUF.Fill(dtComboUF);
                    foreach (DataRow drComboUF in dtComboUF.Rows)

                    {
                        comboUfDestino.Items.Add(drComboUF["codUf"]);
                        comboUfOrigem.Items.Add(drComboUF["codUf"]);

                    }
                    conn.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex);
            }


        }
        public void carregarCidadeOrigem()
        {
            try
            {
                {
                    SQLiteConnection conn = new SQLiteConnection(connectBase);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT nomeCidade FROM tbCidades WHERE codUf = '" + comboUfOrigem.Text.Trim() + "'", conn);
                    SQLiteDataAdapter daComboCidade = new SQLiteDataAdapter(cmd);
                    DataTable dtComboCidade = new DataTable();
                    daComboCidade.Fill(dtComboCidade);
                    foreach (DataRow drComboCidade in dtComboCidade.Rows)

                    {
                        comboCidadeOrigem.Items.Add(drComboCidade["nomeCidade"]);

                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex);
            }

        }
        public void carregarCidadeDestino()
        {
            try
            {
                {
                    SQLiteConnection conn = new SQLiteConnection(connectBase);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT nomeCidade FROM tbCidades WHERE codUf = '" + comboUfDestino.Text.Trim() + "'", conn);
                    SQLiteDataAdapter daComboCidade = new SQLiteDataAdapter(cmd);
                    DataTable dtComboCidade = new DataTable();
                    daComboCidade.Fill(dtComboCidade);
                    foreach (DataRow drComboCidade in dtComboCidade.Rows)

                    {
                        comboCidadeDestino.Items.Add(drComboCidade["nomeCidade"]);

                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex);
            }

        }



        public void carregarCodigoCidadeOrigem()
        {
            try
            {
                {
                    //consulta código da cidade origem
                    SQLiteConnection conn = new SQLiteConnection(connectBase);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    SQLiteCommand cmdOrigem = new SQLiteCommand("SELECT codCidade FROM tbCidades WHERE nomeCidade = '" + comboCidadeOrigem.Text.Trim() + "'", conn);
                    SQLiteDataReader lerOrigem = cmdOrigem.ExecuteReader();
                    lerOrigem.Read();
                    codCidadeOrigem = lerOrigem["codCidade"].ToString();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        public void carregarCodigoCidadeDestino()
        {
            try
            {
                {
                    //consulta código da cidade origem
                    SQLiteConnection conn = new SQLiteConnection(connectBase);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    SQLiteCommand cmdOrigem = new SQLiteCommand("SELECT codCidade FROM tbCidades WHERE nomeCidade = '" + comboCidadeDestino.Text.Trim() + "'", conn);
                    SQLiteDataReader lerOrigem = cmdOrigem.ExecuteReader();
                    lerOrigem.Read();
                    codCidadeDestino = lerOrigem["codCidade"].ToString();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
        public void carregarComboProjetos()
        {
            SQLiteConnection conn = new SQLiteConnection(connectBase);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT NomeProjeto FROM tbProjetos", conn);
            SQLiteDataAdapter daComboProjeto = new SQLiteDataAdapter(cmd);
            DataTable dtComboProjetos = new DataTable();
            daComboProjeto.Fill(dtComboProjetos);
            foreach (DataRow drComboProjeto in dtComboProjetos.Rows)

            {
                comboProjetoVinculado.Items.Add(drComboProjeto["NomeProjeto"]);

            }
            conn.Close();

        }
          public void carregarCodigoProjeto()
        {
            try
            {
                {
                    SQLiteConnection conn = new SQLiteConnection(connectBase);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT codProjeto FROM tbProjetos WHERE nomeProjeto = '" + comboProjetoVinculado.Text.Trim() + "'", conn);
                    SQLiteDataReader ler = cmd.ExecuteReader();
                    ler.Read();
                    codProjetoVinculado = ler["codProjeto"].ToString();
                    conn.Close();

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Selecione o gerente!", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

        }


        //Formata campo hora início para o formato adequado
        private void validaHoraOrigem(object sender, EventArgs e)
        {
            string Entrada = textHoraInicio.Text.Trim();
            int z = Entrada.Length;
            if (z != 0)
            {
                try
                {
                    Convert.ToInt32(Entrada);
                    if (z == 1)
                    {
                        textHoraInicio.Text = "0" + Entrada + ":00";
                    }
                    if (z == 2)
                    {

                        if (Entrada.Substring(0, 1) != "0" & Entrada.Substring(0, 1) != "1" & Entrada.Substring(0, 1) != "2")
                        {
                            textHoraInicio.Text = "0" + Entrada.Substring(0, 1) + ":" + Entrada.Substring(1, 1) + "0";
                        }
                        else
                        {
                            textHoraInicio.Text = Entrada + ":00";
                        }

                    }
                    if (z == 3)
                    {
                        if (Entrada.Substring(0, 1) != "0" & Entrada.Substring(0, 1) != "1" & Entrada.Substring(0, 1) != "2")
                        {
                            textHoraInicio.Text = "0" + Entrada.Substring(0, 1) + ":" + Entrada.Substring(1, 2);
                        }
                        else
                        {
                            textHoraInicio.Text = Entrada.Substring(0, 2) + ":" + Entrada.Substring(2, 1) + "0";
                        }

                    }
                    if (z == 4)
                    {
                        textHoraInicio.Text = Entrada.Substring(0, 2) + ":" + Entrada.Substring(2, 2);
                    }

                    string a = textHoraInicio.Text.Substring(0, 2);
                    string b = textHoraInicio.Text.Substring(3, 2);

                    if (Convert.ToInt32(a) > 23)
                    {
                        MessageBox.Show("O  formato da hora está incorreto.", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textHoraInicio.Focus();
                    }
                    if (Convert.ToInt32(b) > 59)
                    {
                        MessageBox.Show("O  formato da hora está incorreto.", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textHoraInicio.Focus();
                    }
                    //calculando valor em minutos
                    int minutosEntrada1 = ((Convert.ToInt32(a) * 60) + Convert.ToInt32(b));

                }
                catch
                {
                    MessageBox.Show("Só é permitido caracteres numéricos. Não utilize ':' ", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textHoraInicio.Text = String.Empty;
                    textHoraInicio.Focus();
                }
            }

        }
        //Formata campo hora termino para o formato adequado
        private void validaHoraDestino(object sender, EventArgs e)
        {
            string Entrada = textHoraTermino.Text.Trim();
            int z = Entrada.Length;
            if (z != 0)
            {
                try
                {
                    Convert.ToInt32(Entrada);
                    if (z == 1)
                    {
                        textHoraTermino.Text = "0" + Entrada + ":00";
                    }
                    if (z == 2)
                    {

                        if (Entrada.Substring(0, 1) != "0" & Entrada.Substring(0, 1) != "1" & Entrada.Substring(0, 1) != "2")
                        {
                            textHoraTermino.Text = "0" + Entrada.Substring(0, 1) + ":" + Entrada.Substring(1, 1) + "0";
                        }
                        else
                        {
                            textHoraTermino.Text = Entrada + ":00";
                        }

                    }
                    if (z == 3)
                    {
                        if (Entrada.Substring(0, 1) != "0" & Entrada.Substring(0, 1) != "1" & Entrada.Substring(0, 1) != "2")
                        {
                            textHoraTermino.Text = "0" + Entrada.Substring(0, 1) + ":" + Entrada.Substring(1, 2);
                        }
                        else
                        {
                            textHoraTermino.Text = Entrada.Substring(0, 2) + ":" + Entrada.Substring(2, 1) + "0";
                        }

                    }
                    if (z == 4)
                    {
                        textHoraTermino.Text = Entrada.Substring(0, 2) + ":" + Entrada.Substring(2, 2);
                    }

                    string a = textHoraTermino.Text.Substring(0, 2);
                    string b = textHoraTermino.Text.Substring(3, 2);

                    if (Convert.ToInt32(a) > 23)
                    {
                        MessageBox.Show("O  formato da hora está incorreto.", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textHoraTermino.Focus();
                    }
                    if (Convert.ToInt32(b) > 59)
                    {
                        MessageBox.Show("O  formato da hora está incorreto.", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textHoraTermino.Focus();
                    }
                    //calculando valor em minutos
                    int minutosEntrada1 = ((Convert.ToInt32(a) * 60) + Convert.ToInt32(b));

                }
                catch
                {
                    MessageBox.Show("Só é permitido caracteres numéricos. Não utilize ':' ", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textHoraTermino.Text = String.Empty;
                    textHoraTermino.Focus();
                }
            }

        }


        private void buttonInsert_Click(object sender, EventArgs e)
        {
            if (codCidadeOrigem == null)
            {
                MessageBox.Show("Informe a cidade de destino", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (codCidadeDestino == null)
            {
                MessageBox.Show("Informe a cidade de destino", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (codCidadeDestino == codCidadeOrigem)
            {
                MessageBox.Show("As cidades de origem e destino não devem ser a mesma", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (textHoraInicio.Text.Length < 5)
            {
                MessageBox.Show("Informe em INÍCIO a hora inicial de deslocamento", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textHoraInicio.Focus();
            }
            if (textHoraTermino.Text.Length < 5)
            {
                MessageBox.Show("Informe em TÉRMINO a hora final de deslocamento", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textHoraTermino.Focus();
            }
            else
            {
                try
                {

                    //Converte em mintos o valor do primeiro campo
                    int valor1a = (Convert.ToInt32(textHoraInicio.Text.Substring(0, 2)) * 60);
                    int valor1b = (Convert.ToInt32(textHoraInicio.Text.Substring(3, 2)));
                    //Converte em mintos o valor do primeiro campo
                    int valor2a = (Convert.ToInt32(textHoraTermino.Text.Substring(0, 2)) * 60);
                    int valor2b = (Convert.ToInt32(textHoraTermino.Text.Substring(3, 2)));

                    //calcula a quantidade em mintuso e grava em uma variável
                    //SE o valor final for no nesmo dia calcula-se (valor final) - (valor inicial)
                    if ((valor1a + valor1b) < (valor2a + valor2b))
                    {
                        totalMinutosDeslocamento = (valor2a + valor2b) - (valor1a + valor1b);
                    }
                    if ((valor1a + valor1b) == (valor2a + valor2b))
                    {
                        totalMinutosDeslocamento = 1440;
                        MessageBox.Show("Este deslocamento será registrado com 24h. \n Deseja prosseguir com a operação?",
                            "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    //Mas se o valor final for no dia segui calcula-se  (1440 - (valor inicial)) + valor final
                    else
                    {
                        totalMinutosDeslocamento = (1440 - (valor1a + valor1b)) + (valor2a + valor2b);
                    }

                    registraDados();
                    
                }
                catch (Exception ex)
                {
                   
                    MessageBox.Show("Nos campos INÍCIO e TÉRMINO só é permitido caracteres numéricos", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            
        }

        private void comboUfOrigem_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Carrega combo cidade origem
            codCidadeOrigem = null;
            comboCidadeOrigem.Items.Clear();
            comboCidadeOrigem.Text = String.Empty;
            carregarCidadeOrigem();

        }

        private void comboUfDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Carrega combo cidade destino
            codCidadeDestino = null;
            comboCidadeDestino.Items.Clear();
            comboCidadeDestino.Text = String.Empty;
            carregarCidadeDestino();
        }

        private void comboCidadeOrigem_SelectedIndexChanged(object sender, EventArgs e)
        {
            carregarCodigoCidadeOrigem();
        }

        private void comboCidadeDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            carregarCodigoCidadeDestino();
        }

        private void comboProjetoVinculado_SelectedIndexChanged(object sender, EventArgs e)
        {
            carregarCodigoProjeto();
        }

        private void comboColaborador_SelectedIndexChanged(object sender, EventArgs e)
        {
            carregarCodigoColaborador();
        }
    }
}
