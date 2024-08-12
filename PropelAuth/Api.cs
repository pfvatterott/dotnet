using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PropelAuth
{
    public class Api
    {
        private readonly string _apiKey;
        private readonly string _issuer;
        private readonly HttpClient _httpClient;

        public Api(string apiKey, string issuer)
        {
            _apiKey = apiKey;
            _issuer = issuer;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_issuer}/api/backend/v1/user/", request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateUserResponse>(content);
        }
    }

    public class CreateUserRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_confirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonProperty("send_email_to_confirm_email_address")]
        public bool SendEmailToConfirmEmailAddress { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("ask_user_to_update_password_on_login")]
        public bool AskUserToUpdatePasswordOnLogin { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, object> Properties { get; set; }
    }

    public class CreateUserResponse
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}