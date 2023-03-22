using Newtonsoft.Json;
using OptimoClientCertificate;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

var (csr, path) = CreateCSR("CN=clientCA");
Console.WriteLine(path);
var signedCertificate = await RegisterOptimo(csr);
Console.WriteLine(signedCertificate);

//var jwt = await MakeHttpsGetRequest();
//Console.WriteLine(jwt);

static (string, string) CreateCSR(string DN)
{

    ECDsa privateKeyECDsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

    var ecdsaPrivateKey = privateKeyECDsa.ExportPkcs8PrivateKeyPem();

    File.WriteAllText("C:\\Dev\\csr-generation-with-ecdsa\\OptimoClientCertificate\\files" + $"\\optimo_{1}.key", ecdsaPrivateKey);

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

    File.WriteAllText("C:\\Dev\\csr-generation-with-ecdsa\\OptimoClientCertificate\\files" + $"\\optimo_{1}.csr", convertedCsr);
    return (convertedCsr, ("C:\\Dev\\csr-generation-with-ecdsa\\OptimoClientCertificate\\files" + $"\\optimo_{1}.csr"));
}

static async Task<string> RegisterOptimo(string csr)
{
    var onboardOptimo = new OnboardOptimoCommand(OptimoType.Dell, "9119020001", "English US", "United States", "1.420.666", "email@domain.com", "Road Type 3", "1.2.5484", "1.2.54874", "2025-05-05", 1000, 2000, 4, 1, "Work222", "Workshop 222", "221B Baker Street", "TECNA", csr);

    var json = JsonConvert.SerializeObject(onboardOptimo);
    var data = new StringContent(json, Encoding.UTF8, "application/json");

    var url = "https://optimo-registration.qa.stoneridgeapps.com/optimo";
    using var client = new HttpClient();
    client.Timeout = new TimeSpan(0, 5, 0);
    client.DefaultRequestHeaders.Add("client_id", "");
    client.DefaultRequestHeaders.Add("client_secret", "");
    var response = await client.PostAsync(url, data);

    var signedCertificate = await response.Content.ReadAsStringAsync();
    var cert = JsonConvert.DeserializeObject<OnboardOptimoResponse>(signedCertificate);
    Console.WriteLine(cert.Certificate);

    File.WriteAllText("C:\\Dev\\csr-generation-with-ecdsa\\OptimoClientCertificate\\files" + $"\\optimo_{1}.pem", cert.Certificate);

    return signedCertificate;
}


static async Task<string> MakeHttpsGetRequest()
{
    var url = "https://optimo-auth.qa.stoneridgeapps.com/authentication?serialNumber=9119020001";
    // need a converted pfx using the private key and PEM cert
    // I did using the openssl manually "openssl pkcs12 -export -out optimo.pfx -inkey optimo_1.key -in optimo_1.pem"
    var certificatePath = "C:\\Dev\\csr-generation-with-ecdsa\\OptimoClientCertificate\\files\\optimo.pfx";
    X509Certificate2 certificate = new X509Certificate2(certificatePath);
    var handler = new HttpClientHandler();
    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
    handler.SslProtocols = SslProtocols.Tls12;
    handler.ClientCertificates.Add(certificate);
    var client = new HttpClient(handler);
    client.Timeout = new TimeSpan(0, 5, 0);

    var response = await client.PostAsync(url, null);

    var jwt = await response.Content.ReadAsStringAsync();

    return jwt;
}
