using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine("Geef het creditcardnummer in:");
        string creditCardNumber = Console.ReadLine();

        Console.WriteLine("Geef de versleutelingssleutel in:");
        string encryptionKey = Console.ReadLine();

        string encryptedCreditCard = EncryptCreditCard(creditCardNumber, encryptionKey);

        Console.WriteLine("\nOrigineel creditcardnummer: " + creditCardNumber);
        Console.WriteLine("Versleuteld creditcardnummer: " + encryptedCreditCard);

        string decryptedCreditCard = DecryptCreditCard(encryptedCreditCard, encryptionKey);
        Console.WriteLine("Oorspronkelijk creditcardnummer na ontsleuteling: " + decryptedCreditCard);

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

    static string DecryptCreditCard(string encryptedCreditCard, string encryptionKey)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
            aesAlg.Mode = CipherMode.ECB;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedCreditCard)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}