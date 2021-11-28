using AutoMapper;
using DogMatch.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public class RandomDogService : IRandomDogService
    {
        #region DI
        private readonly IHttpClientFactory _client;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<RandomDogService> _logger;
        public RandomDogService(IHttpClientFactory client, IMapper mapper, IConfiguration config, ILogger<RandomDogService> logger)
        {
            _client = client;
            _mapper = mapper;
            _config = config;
            _logger = logger;
        }
        #endregion DI

        #region Public Service Methods
        /// <summary>
        /// Gets a random dog by fetching various public APIs to get a random dog image with breed
        /// (using open source public dog.ceo api which utilizes the stanford dogs dataset), a random
        /// name (using a public name generator api), and also generates a random age <see cref="int"/>
        /// </summary>
        /// <returns>Task containing <see cref="RandomDog"/> type object</returns>
        public async Task<RandomDog> FetchRandomDog()
        {
            try
            {
                // get api urls from config and create http request messages
                HttpRequestMessage dogImageRequest = new(HttpMethod.Get, _config.GetValue<string>("API:RandomDogImageApiUrl"));
                HttpRequestMessage dogNameRequest = new(HttpMethod.Get, _config.GetValue<string>("API:RandomDogNameApiUrl"));

                // add http request headers
                dogImageRequest.Headers.Add("Accept", "application/json");
                dogNameRequest.Headers.Add("Accept", "application/json");

                // create http client and prepare request tasks
                HttpClient client = _client.CreateClient();
                Task<HttpResponseMessage> dogImageTask = client.SendAsync(dogImageRequest);
                Task<HttpResponseMessage> dogNameTask = client.SendAsync(dogNameRequest);

                // execute http request tasks
                HttpResponseMessage[] results = await Task.WhenAll(dogImageTask, dogNameTask);

                if (results[0].IsSuccessStatusCode && results[1].IsSuccessStatusCode)
                {
                    // set tasks for reading result contents into json strings
                    Task<string> dogImgJsonStrTask = results[0].Content.ReadAsStringAsync();
                    Task<string> dogNameJsonStrTask = results[1].Content.ReadAsStringAsync();

                    // execute tasks
                    string[] jsonStrings = await Task.WhenAll(dogImgJsonStrTask, dogNameJsonStrTask);

                    // deserialize json strings
                    RandomDogImageResponse randomDogImgRes =
                        JsonConvert.DeserializeObject<RandomDogImageResponse>(jsonStrings[0]);
                    string[] randomDogNameRes = JsonConvert.DeserializeObject<string[]>(jsonStrings[1]);

                    if (randomDogImgRes.status == "success" &&
                        !string.IsNullOrWhiteSpace(randomDogNameRes[0]))
                    {
                        // map results to RandomDog type object and return it
                        RandomDog dog = new();
                        dog = _mapper.Map<RandomDogImageResponse, RandomDog>(randomDogImgRes, opt =>
                            opt.AfterMap((src, dest) => {
                                dest.Age = GenerateRandomDogAge();
                                dest.Name = randomDogNameRes[0];
                            }));

                        return dog;
                    }
                }
            }
            catch (Exception ex)
            {
                // log error
                _logger.LogError(ex, "Error fetching random dog in RandomDogService");
                
                // return dog not found
                return new RandomDog { DogFound = false };
            }

            // dog not found
            return new RandomDog { DogFound = false };
        }
        #endregion Public Service Methods

        #region Internal
        /// <summary>
        /// generates a random <see cref="int"/> to represent a random dog's age
        /// </summary>
        /// <returns>random <see cref="int"/> between 1 and 13</returns>
        private static int GenerateRandomDogAge()
        {
            Random random = new();
            return random.Next(1, 13);
        }
        #endregion Internal
    }

    public class RandomDogImageResponse
    {
        public string message = string.Empty;
        public string status = string.Empty;
    }
}
