using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        Console.WriteLine("Voer gegevens in om een nieuwe gebruiker toe te voegen:");

        Console.Write("Voornaam: ");
        string voornaam = Console.ReadLine();

        Console.Write("Achternaam: ");
        string achternaam = Console.ReadLine();

        Console.Write("Straat: ");
        string straat = Console.ReadLine();

        Console.Write("Huisnummer: ");
        int huisnummer = int.Parse(Console.ReadLine());

        Console.Write("Postcode: ");
        string postcode = Console.ReadLine();

        Console.Write("Woonplaats: ");
        string woonplaats = Console.ReadLine();

        Console.Write("Creditcardnummer: ");
        string creditcardnummer = Console.ReadLine();

        Console.WriteLine("Geef de versleutelingssleutel in:");
        string encryptionKey = Console.ReadLine();

        // Versleutel het creditcardnummer
        string encryptedCreditcardnummer = EncryptCreditCard(creditcardnummer, encryptionKey);


        using(SqlConnection connection = new SqlConnection("Data Source=(local);Initial Catalog=AESEncryptie;Integrated Security=true;TrustServerCertificate=true"))
        {
            string query = "INSERT INTO dbo.Gebruikers (Voornaam,Achternaam,Straat,Huisnummer,Postcode,Woonplaats,Creditcardnummer) VALUES (@voornaam,@achternaam,@straat,@huisnummer,@postcode,@woonplaats,@creditcardnummer)";

            using(SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@voornaam", voornaam);
                command.Parameters.AddWithValue("@achternaam", achternaam);
                command.Parameters.AddWithValue("@straat", straat);
                command.Parameters.AddWithValue("@huisnummer", huisnummer);
                command.Parameters.AddWithValue("@postcode", postcode);
                command.Parameters.AddWithValue("@woonplaats", woonplaats);
                command.Parameters.AddWithValue("@creditcardnummer", encryptedCreditcardnummer);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        Console.WriteLine("Gebruiker is toegevoegd aan de database.");

        Console.ReadLine();
    }

    static string EncryptCreditCard(string creditCardNumber, string encryptionKey)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
            aesAlg.Mode = CipherMode.ECB;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(creditCardNumber);
                    }
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
}