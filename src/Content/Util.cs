using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using System.Net.Mail;

namespace ERPGEIA.Content
{
    public class Util
    {
        public static bool TemPermissao(string user, string tela, string funcao)
        {
            ERPGeiaEntities db = new ERPGeiaEntities();
            bool tela_permissao = false;

            var usa = ( from u in db.TB_CAD_USUARIO
                        join g in db.TB_CON_GRUPO_USUARIO on u.ID_Grupo_Usuario equals g.ID_Grupo_Usuario
                        join r in db.TB_REL_PERMISSAO on g.ID_Grupo_Usuario equals r.ID_Grupo_Usuario
                        join p in db.TB_CON_PERMISSAO_TELA on r.ID_Permissao_Tela equals p.ID_Permissao_Tela
                        where u.Login == user && p.Nome_Tela == tela && r.Flag_Ativo == 1
                       select new
                        { p.Nome_Tela,
                          p.Permissao_Leitura,
                          p.Permissao_Inclusao,
                          p.Permissao_Edicao,
                          p.Permissao_Exclusao}).FirstOrDefault();

            if (usa != null && funcao != null && usa.Nome_Tela.Equals(tela))
            {
                if (funcao.Equals("Index") && ((Int32.Parse(usa.Permissao_Leitura.ToString()) + Int32.Parse(usa.Permissao_Inclusao.ToString()) + Int32.Parse(usa.Permissao_Edicao.ToString()) + Int32.Parse(usa.Permissao_Exclusao.ToString()))) > 0)
                    tela_permissao = true;
                if (funcao.Equals("Details") && (usa.Permissao_Leitura > 0))
                    tela_permissao = true;
                if (funcao.Equals("Create") && (usa.Permissao_Inclusao > 0))
                    tela_permissao = true;
                if (funcao.Equals("Edit") && (usa.Permissao_Edicao > 0))
                    tela_permissao = true;
                if (funcao.Equals("Delete") && (usa.Permissao_Exclusao > 0))
                    tela_permissao = true;
            }

            return tela_permissao;
        }
        public static bool verificaDuplicidadeNome(string Nome, string Tabela, string Campo)
        {
            ERPGeiaEntities db = new ERPGeiaEntities();
            if (!String.IsNullOrEmpty(Nome) && !String.IsNullOrEmpty(Tabela) & !String.IsNullOrEmpty(Campo))
            {
                string query = "select COUNT(*) as cnt from " + Tabela + " where " + Campo + " like '%" + Nome.Trim() + "%' and Flag_Ativo = 1";
                int result = db.Database.SqlQuery<int>(query).First();
                if (result > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool verificaDuplicidadeInner(string Valor, string Tabela1, string Campo1, int Id, string Tabela2, string Campo2)
        {
            /*Para fazer o inner join os primeiros dados é da tabela Origem: valor, tabela e campo de verificação,
              o segundo dado ref. a tabela de comparação, é obrigatório o ID ser o mesmo nome das duas tabelas.*/
            ERPGeiaEntities db = new ERPGeiaEntities();
            if (!String.IsNullOrEmpty(Valor) || !String.IsNullOrEmpty(Tabela1) || !String.IsNullOrEmpty(Campo1) || Id != 0 || !String.IsNullOrEmpty(Tabela2) || !String.IsNullOrEmpty(Campo2))
            {
                string query = "select COUNT(*) as cnt from " + Tabela1 +" as tb1 "
                            + "INNER JOIN " +Tabela2+" AS tb2 on (tb1."+Campo2+" = tb2."+Campo2+") "
                            + "where tb1."+Campo1+" like '%"+Valor+"%' and tb2."+Campo2+" = "+Id
                            + " and tb1.Flag_Ativo = 1 and tb2.Flag_Ativo = 1";
                 
                int result = db.Database.SqlQuery<int>(query).First();
                if (result > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsCnpj(string cnpj)
       {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);

            throw new NotImplementedException();
       }
        public bool ConfirmaSenha(string senha, string senha1)
       {
           var verifica = false;
           if (senha == senha1)
           {
               verifica = true;
           }
           return verifica;
       }
        public static string toExtenso(decimal valor)
        {
            if (valor <= 0 | valor >= 1000000000000000)
                return "Valor não suportado pelo sistema.";
            else
            {
                string strValor = valor.ToString("000000000000000.00");
                string valor_por_extenso = string.Empty;

                for (int i = 0; i <= 15; i += 3)
                {
                    valor_por_extenso += escreva_parte(Convert.ToDecimal(strValor.Substring(i, 3)));
                    if (i == 0 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(0, 3)) == 1)
                            valor_por_extenso += " TRILHÃO" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(0, 3)) > 1)
                            valor_por_extenso += " TRILHÕES" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 3 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(3, 3)) == 1)
                            valor_por_extenso += " BILHÃO" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(3, 3)) > 1)
                            valor_por_extenso += " BILHÕES" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 6 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(6, 3)) == 1)
                            valor_por_extenso += " MILHÃO" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(6, 3)) > 1)
                            valor_por_extenso += " MILHÕES" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 9 & valor_por_extenso != string.Empty)
                        if (Convert.ToInt32(strValor.Substring(9, 3)) > 0)
                            valor_por_extenso += " MIL" + ((Convert.ToDecimal(strValor.Substring(12, 3)) > 0) ? " E " : string.Empty);

                    if (i == 12)
                    {
                        if (valor_por_extenso.Length > 8)
                            if (valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "BILHÃO" | valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "MILHÃO")
                                valor_por_extenso += " DE";
                            else
                                if (valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "BILHÕES" | valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "MILHÕES" | valor_por_extenso.Substring(valor_por_extenso.Length - 8, 7) == "TRILHÕES")
                                valor_por_extenso += " DE";
                            else
                                    if (valor_por_extenso.Substring(valor_por_extenso.Length - 8, 8) == "TRILHÕES")
                                valor_por_extenso += " DE";

                        if (Convert.ToInt64(strValor.Substring(0, 15)) == 1)
                            valor_por_extenso += " REAL";
                        else if (Convert.ToInt64(strValor.Substring(0, 15)) > 1)
                            valor_por_extenso += " REAIS";

                        if (Convert.ToInt32(strValor.Substring(16, 2)) > 0 && valor_por_extenso != string.Empty)
                            valor_por_extenso += " E ";
                    }

                    if (i == 15)
                        if (Convert.ToInt32(strValor.Substring(16, 2)) == 1)
                            valor_por_extenso += " CENTAVO";
                        else if (Convert.ToInt32(strValor.Substring(16, 2)) > 1)
                            valor_por_extenso += " CENTAVOS";
                }
                return valor_por_extenso;
            }
        }
        public static string escreva_parte(decimal valor)
        {
            if (valor <= 0)
                return string.Empty;
            else
            {
                string montagem = string.Empty;
                if (valor > 0 & valor < 1)
                {
                    valor *= 100;
                }
                string strValor = valor.ToString("000");
                int a = Convert.ToInt32(strValor.Substring(0, 1));
                int b = Convert.ToInt32(strValor.Substring(1, 1));
                int c = Convert.ToInt32(strValor.Substring(2, 1));

                if (a == 1) montagem += (b + c == 0) ? "CEM" : "CENTO";
                else if (a == 2) montagem += "DUZENTOS";
                else if (a == 3) montagem += "TREZENTOS";
                else if (a == 4) montagem += "QUATROCENTOS";
                else if (a == 5) montagem += "QUINHENTOS";
                else if (a == 6) montagem += "SEISCENTOS";
                else if (a == 7) montagem += "SETECENTOS";
                else if (a == 8) montagem += "OITOCENTOS";
                else if (a == 9) montagem += "NOVECENTOS";

                if (b == 1)
                {
                    if (c == 0) montagem += ((a > 0) ? " E " : string.Empty) + "DEZ";
                    else if (c == 1) montagem += ((a > 0) ? " E " : string.Empty) + "ONZE";
                    else if (c == 2) montagem += ((a > 0) ? " E " : string.Empty) + "DOZE";
                    else if (c == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TREZE";
                    else if (c == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUATORZE";
                    else if (c == 5) montagem += ((a > 0) ? " E " : string.Empty) + "QUINZE";
                    else if (c == 6) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSEIS";
                    else if (c == 7) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSETE";
                    else if (c == 8) montagem += ((a > 0) ? " E " : string.Empty) + "DEZOITO";
                    else if (c == 9) montagem += ((a > 0) ? " E " : string.Empty) + "DEZENOVE";
                }
                else if (b == 2) montagem += ((a > 0) ? " E " : string.Empty) + "VINTE";
                else if (b == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TRINTA";
                else if (b == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUARENTA";
                else if (b == 5) montagem += ((a > 0) ? " E " : string.Empty) + "CINQUENTA";
                else if (b == 6) montagem += ((a > 0) ? " E " : string.Empty) + "SESSENTA";
                else if (b == 7) montagem += ((a > 0) ? " E " : string.Empty) + "SETENTA";
                else if (b == 8) montagem += ((a > 0) ? " E " : string.Empty) + "OITENTA";
                else if (b == 9) montagem += ((a > 0) ? " E " : string.Empty) + "NOVENTA";

                if (strValor.Substring(1, 1) != "1" & c != 0 & montagem != string.Empty) montagem += " E ";

                if (strValor.Substring(1, 1) != "1")
                    if (c == 1) montagem += "UM";
                    else if (c == 2) montagem += "DOIS";
                    else if (c == 3) montagem += "TRÊS";
                    else if (c == 4) montagem += "QUATRO";
                    else if (c == 5) montagem += "CINCO";
                    else if (c == 6) montagem += "SEIS";
                    else if (c == 7) montagem += "SETE";
                    else if (c == 8) montagem += "OITO";
                    else if (c == 9) montagem += "NOVE";

                return montagem;
            }
        }
        public static string UploadLogo(HttpPostedFileBase fileinput, string Nome, string Pasta)
        {
            if (fileinput != null)
            {
                string arquivo = Nome + DateTime.Now.ToString().Replace("/", "").Replace("-", "").Replace(":", "").Replace(" ", "") + fileinput.FileName;

                CloudStorageAccount cloudStorageAccount = ConnectionString.GetConnectionString();
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(Pasta);
                cloudBlobContainer.CreateIfNotExists();

                //Setar permissão de acesso para público - para fazer download caso necessário
                cloudBlobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(arquivo);
                blockBlob.UploadFromStream(fileinput.InputStream);

                return "https://armazenamentogeiageral.blob.core.windows.net/" + Pasta + "/" + arquivo;
            }
            return null;
        }
        //Azure Conncetion
        private static class ConnectionString
        {
            static string account = CloudConfigurationManager.GetSetting("StorageAccountName");
            static string key = CloudConfigurationManager.GetSetting("StorageAccountKey");
            public static CloudStorageAccount GetConnectionString()
            {
                string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", account, key);
                return CloudStorageAccount.Parse(connectionString);
            }
        }
        //Se usuário logado é válido
        public static bool IsValid(string usuario, string senha, string tipo)
        {
            ERPGeiaEntities db = new ERPGeiaEntities();

            var crypto = new SimpleCrypto.PBKDF2();
            bool valido = false;
            var user = db.TB_CAD_USUARIO.Where(x => x.Flag_Ativo == 1).FirstOrDefault(u => u.Login == usuario);

            if (user != null)
            {
                if (user.Senha == crypto.Compute(senha, user.SenhaSalt) && user.Tipo.ToString().Trim() == tipo && user.Flag_Ativo == 1)
                {
                    valido = true;
                }
            }
            return valido;
        }
        public static bool envia_email(string from, string to, string subject, string body)
        {
            string senderPass = "Geia$mail15";
            MailMessage mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            SmtpClient sender = new SmtpClient();
            sender.Host = "smtp.office365.com";
            sender.Port = 587;
            sender.Credentials = new System.Net.NetworkCredential(from, senderPass);
            sender.DeliveryMethod = SmtpDeliveryMethod.Network;
            sender.EnableSsl = true;

            try
            {
                sender.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }


    }
}



