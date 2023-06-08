using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using Amazon;

namespace MvcConciertos.Helpers
{
    public class SecretsManager
    {





        public static async Task<string> GetSecretAsync()
        {
            string secretName = "comics-secret";
            string region = "us-east-1";

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified.
            };

            GetSecretValueResponse response;

            try
            {
                response = await client.GetSecretValueAsync(request);
            }
            catch (Exception e)
            {
                // For a list of the exceptions thrown, see
                // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
                throw e;
            }

            string secret = response.SecretString;
            return secret;
            // Your code goes here
        }

    }

}