using Newtonsoft.Json;
using OptimoClientCertificate;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

var (csr, path) = CreateCSR("CN=clientCA");
Console.WriteLine(path);
var signedCertificate = await RegisterOptimo(csr);
Console.WriteLine(signedCertificate);

static (string, string) CreateCSR(string DN)
{
    
    //RSA privateKeyRsa = new RSACryptoServiceProvider(2048);
    ECDsa privateKeyECDsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
    //string keygen = Convert.ToBase64String(privateKeyRsa.ExportRSAPrivateKey());

    //var a = privateKeyRsa.ExportRSAPublicKeyPem();    

    var ecdsaPrivateKey = privateKeyECDsa.ExportPkcs8PrivateKeyPem();    
    //var privateKey = privateKeyRsa.ExportRSAPrivateKeyPem();

    File.WriteAllText("C:\\Dev\\OptimoClientCertificate\\OptimoClientCertificate\\files" + $"\\optimo_{1}.key", ecdsaPrivateKey);

    //CertificateRequest certificateRequest = new CertificateRequest(
    //    $"{DN}", privateKeyRsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

    CertificateRequest certificateRequest = new CertificateRequest(
    $"{DN}", privateKeyECDsa, HashAlgorithmName.SHA256);

    certificateRequest.CertificateExtensions.Add(
        new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyCertSign, false));    

    certificateRequest.CertificateExtensions.Add(
    new X509SubjectKeyIdentifierExtension(certificateRequest.PublicKey, false));

    certificateRequest.CertificateExtensions.Add(
        new X509BasicConstraintsExtension(true, false, 0, true));
    
    var csr = certificateRequest.CreateSigningRequest();
    string convertedCsr = new string(PemEncoding.Write("CERTIFICATE REQUEST", csr));

    File.WriteAllText("C:\\Dev\\OptimoClientCertificate\\OptimoClientCertificate\\files" + $"\\optimo_{1}.csr", convertedCsr);
    return (convertedCsr, ("C:\\Dev\\OptimoClientCertificate\\OptimoClientCertificate\\files" + $"\\optimo_{1}.csr"));
}

static async Task<string> RegisterOptimo(string csr)
{    
    var onboardOptimo = new OnboardOptimoCommand(OptimoType.Dell,"SN1", "English US", "United States", "1.420.666", "email@domain.com","Road Type 3", "1.2.5484", "1.2.54874", "2025-05-05", 1000,2000,4,1, "Work222", "Workshop 222", "221B Baker Street", "Example Distributor", csr);

    var json = JsonConvert.SerializeObject(onboardOptimo);
    var data = new StringContent(json, Encoding.UTF8, "application/json");

    var url = "https://localhost:44377/optimo";
    using var client = new HttpClient();
    client.Timeout = new TimeSpan(0, 5, 0);

    var response = await client.PostAsync(url, data);

    var signedCertificate = await response.Content.ReadAsStringAsync();
    var cert = JsonConvert.DeserializeObject<OnboardOptimoResponse>(signedCertificate);
    Console.WriteLine(cert.Certificate);

    File.WriteAllText("C:\\Dev\\OptimoClientCertificate\\OptimoClientCertificate\\files" + $"\\optimo_{1}.pem", cert.Certificate);

    return signedCertificate;
}
