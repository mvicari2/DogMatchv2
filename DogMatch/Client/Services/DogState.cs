using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DogMatch.Client.Services
{
    public class DogState
    {
        // declarations
        public Dog dog { get; set; }
        public IEnumerable<Dog> doggos { get; set; }
        public DateTime today = DateTime.Now;      

        // styles
        public string ColStyle = "display: block; margin-left: auto; margin-right: auto;";
        public string CardStyle = "display: block; margin-left: auto; margin-right: auto; width: 60%; margin-bottom: 20px; height:200px;";

        // DI
        private readonly HttpClient _http;
        private readonly NavigationManager _nav;        

        public DogState(HttpClient httpInstance, NavigationManager navigation)
        {
            _http = httpInstance;
            _nav = navigation;
        }
        

        // actions / webapi calls
        public async Task GetAllDoggos()
        {
            doggos = await _http.GetFromJsonAsync<IEnumerable<Dog>>("/api/Doggo");
        }

        public async Task GetDoggo(int id)
        {
            NewDoggo();
            dog = await _http.GetFromJsonAsync<Dog>("api/Doggo/" + id);           
        }

        public async Task CreateDoggo()
        {
            var response = await _http.PostAsJsonAsync("api/Doggo/", dog);
            var newDog = await response.Content.ReadFromJsonAsync<Dog>();            
           
            dog.Id = newDog.Id;            
            _nav.NavigateTo("/DoggoDetails/" + dog.Id);
        }

        public async Task UpdateDoggo()
        {
            var response = await _http.PutAsJsonAsync("api/Doggo/" + dog.Id, dog);

            if (response.IsSuccessStatusCode)
            {
                _nav.NavigateTo("/DoggoProfile/" + dog.Id);
            }
        }

        // Navigation
        public void ViewProfile(int id)
        {
            _nav.NavigateTo("/DoggoProfile/" + id);
        }

        public void UpdateProfile(int id)
        {
            _nav.NavigateTo("/DoggoDetails/" + id);
        }


        // data
        public void NewDoggo()
        {
            dog = new Dog();
        }

        public string GetBirthday(DateTime? bday)
        {
            return bday.Value.ToShortDateString();
        }
        
        public Task GetAge(DateTime? date)
        {
            dog.Birthday = date;

            var today = DateTime.Today;
            var age = 0;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (date.Value.Year * 100 + date.Value.Month) * 100 + date.Value.Day;

            age = (a - b) / 10000;

            // if less than 1 year old then determine age in months
            if (age < 1)
            {
                int m1 = (today.Month - date.Value.Month);
                int m2 = (today.Year - date.Value.Year) * 12;
                int months = m1 + m2;

                dog.Age = "Age: " + months + " months old";
            }
            else if (age == 1)
            {
                dog.Age = "Age: " + age + " year old";
            }
            else
            {
                dog.Age = "Age: " + age + " years old";
            }

            return Task.CompletedTask;
        }             
    }
}
