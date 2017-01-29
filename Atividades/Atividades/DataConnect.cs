﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;


namespace Atividades
{
    class DataConnect

    {
        private static string connectBase = "Data Source=Banco.db";
        private static string bancoName = "Banco.db";

        public void conectar()
        {
            if (!File.Exists(bancoName))
            {
                SQLiteConnection.CreateFile(bancoName);
                SQLiteConnection conn = new SQLiteConnection(connectBase);
                conn.Open();

                StringBuilder sql = new StringBuilder();

                //SE NÃO EXISTIR CRIA A TABELA UF
                sql.AppendLine("CREATE TABLE IF NOT EXISTS [tbUF] ([codUf] VARCHAR(2) PRIMARY KEY NOT NULL, [nomeEstado] VARCHAR(60));");

                //SE NÃO EXISTIR CRIA A TABELA PESSOAS
                sql.AppendLine(@"CREATE TABLE IF NOT EXISTS [tbPessoas] ([codPessoa] INTEGER PRIMARY KEY AUTOINCREMENT,
                [NomePessoa] VARCHAR(100) , [Gerente] BOOL, [colaborador] BOOL);");

                //SE NÃO EXISTIR CRIA A TABELA CIDADES
                sql.AppendLine(@"CREATE TABLE IF NOT EXISTS [tbCidades] ([codCidade] INTEGER PRIMARY KEY AUTOINCREMENT, 
                [codUf] VARCHAR(2) NOT NULL, [nomeCidade] VARCHAR(60), FOREIGN KEY (codUf) REFERENCES tbUF (codUf));");


                //SE NÃO EXISTIR CRIA A TABELA PROJEITOS
                sql.AppendLine(@"CREATE TABLE IF NOT EXISTS [tbProjetos] ( [codProjeto] INT PRIMARY KEY,
                [nomeProjeto] VARCHAR(60), [codGerente] INT, [codUf] VARCHAR(2), [codCidade] INT,
                FOREIGN KEY (codGerente) REFERENCES tbPessoas (codPessoa), 
                FOREIGN KEY (codUf) REFERENCES tbUF (codUf),
                FOREIGN KEY (codCidade) REFERENCES tbCidades (codCidade));");

                //SE NÃO EXISTIR CRIA A TABELA ATIVIDADES
                sql.AppendLine(@"CREATE TABLE IF NOT EXISTS [tbAtividades] ([codAtividade] INTEGER PRIMARY KEY AUTOINCREMENT,
                [codProjeto] INT, [dataAtividade] DATETIME, [entrada1] VARCHAR(5), [entrada2] VARCHAR(5),
                [entrada3] VARCHAR(5), [saida1] VARCHAR(5), [saida2] VARCHAR(5), [saida3] VARCHAR(5),
                [codColaborador] INT, [codGerente] INT,  [obsAtividade] VARCHAR(1000), [totalMinutos] INT, [minutosExtras] INT,
                FOREIGN KEY (codProjeto) REFERENCES tbProjetos (codProjeto), 
                FOREIGN KEY (codColaborador) REFERENCES tbPessoas (codPessoa),
                FOREIGN KEY (codGerente) REFERENCES tbProjetos (codPessoa));");



                //INSERT NA TABELA DE ESTADOS
                sql.AppendLine(@"INSERT INTO tbUF (codUf, nomeEstado) VALUES ('AC', 'ACRE');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('AL', 'ALAGOAS');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('AP', 'AMAPÁ');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('AM', 'AMAZONAS');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('BA', 'BAHIA');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('CE', 'CEARÁ');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('EX', 'EXTERIOR');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('DF', 'DISTRITO FEDERAL');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('ES', 'ESPÍRITO SANTO');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('GO', 'GOIÁS');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('MA', 'MARANHÃO');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('MT', 'MATO GROSSO');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('MS', 'MATO GROSSO DO SUL');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('MG', 'MINAS GERAIS');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('PA', 'PARÁ');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('PB', 'PARAÍBA');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('PR', 'PARANÁ');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('PE', 'PERNAMBUCO');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('PI', 'PIAUÍ');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('RJ', 'RIO DE JANEIRO');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('RN', 'RIO GRANDE DO NORTE');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('RS', 'RIO GRANDE DO SUL');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('RO', 'RONDÔNIA');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('RR', 'RORAIMA');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('SC', 'SANTA CATARINA');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('SP', 'SÃO PAULO');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('SE', 'SERGIPE');
                INSERT INTO tbUF (codUf, nomeEstado) VALUES ('TO', 'TOCANTINS');");



                SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao Criar Banco de Dados" + ex.Message);

                }
            }
        }
    }
}
