using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Altcha
{
	// These values need to match to make sure the widget and server are calculating the same hash
	private static readonly string ALTCHA_JS_ALG = "SHA-256";
	private static readonly string ALTCHA_ALG = "SHA256";
	private static readonly string ALTCHA_HMAC_ALG = "HMACSHA256";

	private static readonly int[] ALTCHA_NUM_RANGE = { 1000, 100000 };
	private static readonly string ALTCHA_HMAC_KEY = "$ecret.key-change-me-2024";

	private static (string Challenge, string Signature) CreateAltcha(string salt, int number)
	{
		if (string.IsNullOrWhiteSpace(salt) || salt.Length < 10)
		{
			throw new Exception("Invalid salt value");
		}

		var challenge = GetHashString(salt + number);
		var signature = GetHmacSignature(ALTCHA_HMAC_KEY, challenge);

		return (challenge, signature);
	}

	public static string CreateChallengeJson()
	{
		var salt = GenerateRandomHexString(12);
		var number = GenerateRandomNumber(ALTCHA_NUM_RANGE[0], ALTCHA_NUM_RANGE[1]);
		var altcha = CreateAltcha(salt, number);

		return JsonSerializer.Serialize(new { algorithm = ALTCHA_JS_ALG, challenge = altcha.Challenge, salt, signature = altcha.Signature }); ;
	}

	public static bool VerifyChallengeJson(string payload)
	{
		try
		{
			var challengeResponse = JsonSerializer.Deserialize<AltchaPayload>(Base64Decode(payload));
			if (challengeResponse != null)
			{
				var altcha = CreateAltcha(challengeResponse.Salt, challengeResponse.Number);

				return
					challengeResponse.Algorithm == ALTCHA_JS_ALG &&
					challengeResponse.Challenge == altcha.Challenge &&
					challengeResponse.Signature == altcha.Signature;
			}
		}
		catch
		{
			// invalid payload
		}
		return false;
	}

	private static string GetHashString(string input)
	{
		using (var hashAlgorithm = CryptoConfig.CreateFromName(ALTCHA_ALG) as HashAlgorithm)
		{
			if (hashAlgorithm == null)
			{
				throw new Exception("Invalid hash algorithm.");
			}
			var bytes = Encoding.UTF8.GetBytes(input);
			var hashBytes = hashAlgorithm.ComputeHash(bytes);
			return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
		}
	}

	private static string GetHmacSignature(string key, string input)
	{
		using (var hmac = CryptoConfig.CreateFromName(ALTCHA_HMAC_ALG) as HMAC)
		{
			if (hmac == null)
			{
				throw new Exception("Invalid hash algorithm.");
			}
			hmac.Key = Encoding.UTF8.GetBytes(key);
			var bytes = Encoding.UTF8.GetBytes(input);
			var hashBytes = hmac.ComputeHash(bytes);
			return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
		}
	}

	private static string GenerateRandomHexString(int length)
	{
		var random = new Random();
		var bytes = new byte[length / 2];
		random.NextBytes(bytes);
		return BitConverter.ToString(bytes).Replace("-", "").ToLower();
	}

	private static int GenerateRandomNumber(int min, int max)
	{
		var random = new Random();
		return random.Next(min, max + 1);
	}

	private static string Base64Decode(string input)
	{
		var bytes = Convert.FromBase64String(input);
		return Encoding.UTF8.GetString(bytes);
	}

	private class AltchaPayload
	{
		[JsonPropertyName("algorithm")]
		public string Algorithm { get; set; }
		[JsonPropertyName("challenge")]
		public string Challenge { get; set; }
		[JsonPropertyName("salt")]
		public string Salt { get; set; }
		[JsonPropertyName("signature")]
		public string Signature { get; set; }
		[JsonPropertyName("number")]
		public int Number { get; set; }
	}

}

