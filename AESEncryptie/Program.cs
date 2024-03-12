using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

public class GebruikerContext : DbContext
{
    public DbSet<Gebruiker> Gebruikers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=(local);Initial Catalog=AESEncryptie;Integrated Security=true;TrustServerCertificate=true");
    }
}

public class Gebruiker
{
    [Key]
    public int Id { get; set; }
    public string Voornaam { get; set; }
    public string Achternaam { get; set; }
    public string Straat { get; set; }
    public int Huisnummer { get; set; }
    public string Postcode { get; set; }
    public string Woonplaats { get; set; }
    public string Creditcardnummer { get; set; }
}

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

        // Voeg gebruiker toe aan de database
        using (var context = new GebruikerContext())
        {
            var gebruiker = new Gebruiker
            {
                Voornaam = voornaam,
                Achternaam = achternaam,
                Straat = straat,
                Huisnummer = huisnummer,
                Postcode = postcode,
                Woonplaats = woonplaats,
                Creditcardnummer = encryptedCreditcardnummer
            };

            context.Gebruikers.Add(gebruiker);
            context.SaveChanges();
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