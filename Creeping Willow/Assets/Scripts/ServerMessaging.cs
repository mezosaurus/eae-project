using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class ServerMessaging : MonoBehaviour
{
    private const string ServerPublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC5ysS01fjb5Oqc8mzeDMAZQgAKXp4yuB6H/f48aD/IVTd9/sS8uBHIhCYLQ6QvY2289nPsKP3l7E/1Dy4UOZtd22Q+J7PeP2Aijx5HwA0VG46G3VBABCb4EbtcKoYWUp2n76G6Z592JbzAskIron/3n50uZ8rnhEcZEhM5XaV98wIDAQAB";


    private static readonly Dictionary<string, ScoreLevel> levelLookup = new Dictionary<string, ScoreLevel>()
    {
        {"Tutorial", ScoreLevel.Tutorial},
        {"Evan_Level1", ScoreLevel.BloodyBeginnings},
        {"Quadrants", ScoreLevel.LakesideLullaby},
        {"Bridge_Level", ScoreLevel.OverTroubledWaters},
        {"Maze_Level", ScoreLevel.HallowedLabyrinth}
    };
    

    
    public static void UploadScoreToServer(string name, UInt32 score, ScoreGameType gameType, string levelName)
    {
        if (name.Length != 3) return;
        try
        {
            ScoreLevel level = levelLookup[levelName];
            
            byte[] buffer = new byte[9];

            new ASCIIEncoding().GetBytes(name).CopyTo(buffer, 0);

            // Convert score to bytes
            buffer[3] = (byte)((score & 0xFF000000) >> 24);
            buffer[4] = (byte)((score & 0x00FF0000) >> 16);
            buffer[5] = (byte)((score & 0x0000FF00) >>  8);
            buffer[6] = (byte)((score & 0x000000FF) >>  0);

            buffer[7] = (byte)gameType;
            buffer[8] = (byte)level;

            Coroutiner.StartCoroutine(SendData(GetEncryptedBytes(buffer)));
        }
        catch (Exception) { Debug.Log("Error uploading score to server"); }
    }

    private static IEnumerator SendData(byte[] data)
    {
        // Create POST form for data
        WWWForm form = new WWWForm();

        form.AddBinaryData("data", data);

        // Upload the form to the web server
        WWW www = new WWW("http://creepingwillow.com/scores.php?submit", form);

        yield return www;

        if (www.text == "Success") Debug.Log("Successfully uploaded score to the server.");
        else Debug.Log("Error uploading score to the server: " + www.text);
    }

    private static byte[] GetEncryptedBytes(byte[] message)
    {       
        // Compute hash of message
        SHA256 sha256 = SHA256Managed.Create();
        byte[] hash = sha256.ComputeHash(message);

        // Combine hash and message into one buffer
        byte[] buffer = new byte[hash.Length + message.Length];

        hash.CopyTo(buffer, 0);
        message.CopyTo(buffer, hash.Length);

        // Encrypt the combined hash and message using the server's public key
        using(RSACryptoServiceProvider rsa = GetPublicKey(ServerPublicKey))
        {
            if (rsa == null)
            {
                Debug.Log("Couldn't load server's public key");

                throw new ApplicationException();
            }

            return rsa.Encrypt(buffer, false);
        }
    }

    /*******************************************************************
     * 
     * Code for reading RSA keys taken from:
     * https://langcongnghe.com/2014/11/10/tony/cach-thuc-doc-rsa-private-key-va-public-key-trong-c.html
     * 
     *******************************************************************/

    private static RSACryptoServiceProvider GetPublicKey(string publicKeyString)
    {
        // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
        byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        byte[] x509key;
        byte[] seq = new byte[15];
        int x509size;

        x509key = Convert.FromBase64String(publicKeyString);
        x509size = x509key.Length;

        // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
        MemoryStream mem = new MemoryStream(x509key);
        BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
        byte bt = 0;
        ushort twobytes = 0;

        try
        {

            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                binr.ReadByte();	//advance 1 byte
            else if (twobytes == 0x8230)
                binr.ReadInt16();	//advance 2 bytes
            else
                return null;

            seq = binr.ReadBytes(15);		//read the Sequence OID
            if (!CompareBytearrays(seq, SeqOID))	//make sure Sequence for OID is correct
                return null;

            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8103)	//data read as little endian order (actual data order for Bit String is 03 81)
                binr.ReadByte();	//advance 1 byte
            else if (twobytes == 0x8203)
                binr.ReadInt16();	//advance 2 bytes
            else
                return null;

            bt = binr.ReadByte();
            if (bt != 0x00)		//expect null byte next
                return null;

            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                binr.ReadByte();	//advance 1 byte
            else if (twobytes == 0x8230)
                binr.ReadInt16();	//advance 2 bytes
            else
                return null;

            twobytes = binr.ReadUInt16();
            byte lowbyte = 0x00;
            byte highbyte = 0x00;

            if (twobytes == 0x8102)	//data read as little endian order (actual data order for Integer is 02 81)
                lowbyte = binr.ReadByte();	// read next bytes which is bytes in modulus
            else if (twobytes == 0x8202)
            {
                highbyte = binr.ReadByte();	//advance 2 bytes
                lowbyte = binr.ReadByte();
            }
            else
                return null;
            byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
            int modsize = BitConverter.ToInt32(modint, 0);

            int firstbyte = binr.PeekChar();
            if (firstbyte == 0x00)
            {	//if first byte (highest order) of modulus is zero, don't include it
                binr.ReadByte();	//skip this null byte
                modsize -= 1;	//reduce modulus buffer size by 1
            }

            byte[] modulus = binr.ReadBytes(modsize);	//read the modulus bytes

            if (binr.ReadByte() != 0x02)			//expect an Integer for the exponent data
                return null;
            int expbytes = (int)binr.ReadByte();		// should only need one byte for actual exponent data (for all useful values)
            byte[] exponent = binr.ReadBytes(expbytes);

            // ------- create RSACryptoServiceProvider instance and initialize with public key -----
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSAParameters RSAKeyInfo = new RSAParameters();
            RSAKeyInfo.Modulus = modulus;
            RSAKeyInfo.Exponent = exponent;
            RSA.ImportParameters(RSAKeyInfo);

            return RSA;
        }

        finally
        {
            binr.Close();
        }
    }

    private static int GetIntegerSize(BinaryReader binr)
    {
        byte bt = 0;
        byte lowbyte = 0x00;
        byte highbyte = 0x00;
        int count = 0;
        bt = binr.ReadByte();
        if (bt != 0x02)		//expect integer
            return 0;
        bt = binr.ReadByte();

        if (bt == 0x81)
            count = binr.ReadByte();	// data size in next byte
        else
            if (bt == 0x82)
            {
                highbyte = binr.ReadByte();	// data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;		// we already have the data size
            }

        while (binr.ReadByte() == 0x00)
        {	//remove high order zeros in data
            count -= 1;
        }
        binr.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
        return count;
    }

    private static bool CompareBytearrays(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;
        int i = 0;
        foreach (byte c in a)
        {
            if (c != b[i])
                return false;
            i++;
        }
        return true;
    }
}
